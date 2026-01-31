using AIMeeting.CLI.Commands;
using AIMeeting.CLI.Logging;
using AIMeeting.CLI.Display;
using AIMeeting.CLI.Errors;
using AIMeeting.Core.Events;
using AIMeeting.Core.Models;
using AIMeeting.Core.FileSystem;
using AIMeeting.Core.Agents;
using AIMeeting.Core.Configuration;
using AIMeeting.Core.Orchestration;
using AIMeeting.Core.Prompts;
using Serilog;
using Xunit;

namespace AIMeeting.Integration.Tests
{
    public class StartMeetingCommandTests
    {
        [Fact]
        public async Task MeetingProgressDisplay_ReportsRemainingLimits()
        {
            var eventBus = new InMemoryEventBus();
            var configuration = new MeetingConfiguration
            {
                MeetingTopic = "Progress test",
                AgentConfigPaths = ["agent-a"],
                HardLimits = new MeetingLimits
                {
                    MaxDurationMinutes = 1,
                    MaxTotalMessages = 5
                }
            };

            var display = new MeetingProgressDisplay();
            display.Attach(eventBus, configuration);

            await eventBus.PublishAsync(new MeetingStartedEvent { Configuration = configuration });
            await eventBus.PublishAsync(new TurnStartedEvent { AgentId = "agent-a", TurnNumber = 1 });

            Assert.NotNull(display.LastStatus);
            Assert.Contains("Time remaining", display.LastStatus);
            Assert.Contains("Messages remaining", display.LastStatus);

            await eventBus.PublishAsync(new TurnCompletedEvent { AgentId = "agent-a", Message = "Hello" });

            Assert.Contains("Messages remaining: 4", display.LastStatus);
        }

        [Fact]
        public void CliErrorHandler_MapsKnownExceptions()
        {
            var configException = new AIMeeting.Core.Exceptions.MeetingConfigurationException("Bad config");
            var copilotException = new AIMeeting.Core.Exceptions.CopilotApiException("Copilot down");
            var limitException = new AIMeeting.Core.Exceptions.MeetingLimitExceededException("Messages", 1);

            Assert.Equal(1, CliErrorHandler.HandleException(configException));
            Assert.Equal(1, CliErrorHandler.HandleException(copilotException));
            Assert.Equal(0, CliErrorHandler.HandleException(limitException));
        }

        [Fact]
        public async Task StubMeeting_Workflow_WritesTranscript()
        {
            var originalMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE");
            Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", "stub");

            var tempRoot = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            try
            {
                var eventBus = new InMemoryEventBus();
                var fileLocker = new FileLocker(eventBus);
                var meetingRoomFactory = new MeetingRoomFactory(fileLocker, eventBus);
                var transcriptManager = new TranscriptManager();

                var config = new MeetingConfiguration
                {
                    MeetingId = "stub-meeting",
                    MeetingTopic = "Stub Meeting",
                    OutputDirectory = tempRoot,
                    AgentConfigPaths =
                    [
                        GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-one.txt"),
                        GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-two.txt")
                    ],
                    HardLimits = new MeetingLimits { MaxTotalMessages = 2 }
                };

                var meetingRoom = await meetingRoomFactory.CreateMeetingRoomAsync(
                    config.MeetingId!,
                    config,
                    tempRoot);
                await transcriptManager.SubscribeToEventsAsync(meetingRoom, eventBus);

                var parser = new AgentConfigurationParser();
                var validator = new AgentConfigurationValidator();
                var promptBuilder = new PromptBuilder();
                var agentFactory = new AgentFactory(parser, validator, promptBuilder, null);
                var turnManager = new FifoTurnManager();
                var orchestrator = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

                var result = await orchestrator.RunMeetingAsync(config);

                Assert.Equal(MeetingState.Completed, result.State);

                var transcriptPath = Path.Combine(meetingRoom.MeetingRoomPath, "transcript.md");
                Assert.True(File.Exists(transcriptPath));

                var transcript = await File.ReadAllTextAsync(transcriptPath);
                Assert.Contains("test-agent-one", transcript);
                Assert.Contains("test-agent-two", transcript);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", originalMode);
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, recursive: true);
                }
            }
        }

        [Fact]
        public void LoggingBootstrapper_CreatesLogFile()
        {
            var logDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));

            try
            {
                LoggingBootstrapper.Configure(logDirectory);
                Log.CloseAndFlush();

                var files = Directory.GetFiles(logDirectory, "meeting-*.log");
                Assert.NotEmpty(files);
            }
            finally
            {
                if (Directory.Exists(logDirectory))
                {
                    Directory.Delete(logDirectory, recursive: true);
                }
            }
        }

        [Fact]
        public async Task ErrorLogManager_WritesErrorsLogOnFailure()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var eventBus = new InMemoryEventBus();
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker(eventBus), eventBus);
                var errorLogManager = new ErrorLogManager();

                await errorLogManager.SubscribeToEventsAsync(meetingRoom, eventBus);

                await eventBus.PublishAsync(new MeetingEndedEvent
                {
                    EndReason = "Meeting failed: simulated error"
                });

                var errorLogPath = Path.Combine(tempDirectory, "errors.log");
                Assert.True(File.Exists(errorLogPath));

                var content = await File.ReadAllTextAsync(errorLogPath);
                Assert.Contains("Meeting failed: simulated error", content);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
            }
        }

        [Fact]
        public void StartMeeting_MissingTopic_ReturnsError()
        {
            var testAgentPath = GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-one.txt");
            var command = StartMeetingCommandBuilder.BuildCommand();
            var exitCode = command.Parse(new[] { "--agents", testAgentPath }).Invoke();
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void StartMeeting_MissingAgents_ReturnsError()
        {
            var command = StartMeetingCommandBuilder.BuildCommand();
            var exitCode = command.Parse(new[] { "--topic", "Test meeting" }).Invoke();
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void StartMeeting_InvalidMaxMessages_ReturnsError()
        {
            var testAgentPath = GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-one.txt");
            var command = StartMeetingCommandBuilder.BuildCommand();
            var exitCode = command.Parse(new[]
            {
                "--topic", "Test meeting",
                "--agents", testAgentPath,
                "--max-messages", "0"
            }).Invoke();
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void StartMeeting_WithStubAgents_Completes()
        {
            var originalMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE");
            Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", "stub");

            try
            {
                var testAgentPath = GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-one.txt");
                var testEngineerPath = GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-two.txt");
                var command = StartMeetingCommandBuilder.BuildCommand();
                var exitCode = command.Parse(new[]
                {
                    "--topic", "Test meeting",
                    "--agents", testAgentPath, testEngineerPath,
                    "--max-messages", "2"
                }).Invoke();

                Assert.Equal(0, exitCode);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", originalMode);
            }
        }

        [Fact]
        public void ValidateConfig_MissingRole_ReturnsError()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var content = """
                    DESCRIPTION: Missing role
                    INSTRUCTIONS:
                    - Consider complexity
                    """;
                File.WriteAllText(tempFile, content);

                var command = ValidateConfigCommandBuilder.BuildCommand();
                var exitCode = command.Parse(new[] { tempFile }).Invoke();

                Assert.Equal(1, exitCode);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public void StartMeeting_ValidConfig_ReturnsSuccess()
        {
            var originalMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE");
            Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", "stub");

            try
            {
                var testAgentPath = GetRepoRelativePath("tests", "AIMeeting.Integration.Tests", "TestData", "agent-one.txt");
                var command = StartMeetingCommandBuilder.BuildCommand();
                var exitCode = command.Parse(new[]
                {
                    "--topic", "E2E meeting",
                    "--agents", testAgentPath,
                    "--max-messages", "1"
                }).Invoke();

                Assert.Equal(0, exitCode);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", originalMode);
            }
        }

        private static string GetRepoRelativePath(params string[] segments)
        {
            var root = GetRepoRoot();
            return Path.Combine(new[] { root }.Concat(segments).ToArray());
        }

        private static string GetRepoRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                var slnPath = Path.Combine(current.FullName, "AIMeeting.sln");
                if (File.Exists(slnPath))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Repository root not found for tests");
        }
    }
}
