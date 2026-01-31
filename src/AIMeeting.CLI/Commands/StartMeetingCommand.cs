using System.CommandLine;
using AIMeeting.Core.Agents;
using AIMeeting.Core.Configuration;
using AIMeeting.Core.Events;
using AIMeeting.Core.FileSystem;
using AIMeeting.Core.Models;
using AIMeeting.Core.Orchestration;
using AIMeeting.Core.Prompts;
using AIMeeting.Copilot;
using AIMeeting.CLI.Display;
using AIMeeting.CLI.Errors;

namespace AIMeeting.CLI.Commands
{
    /// <summary>
    /// Command to start a meeting.
    /// </summary>
    public static class StartMeetingCommandBuilder
    {
        public static Command BuildCommand()
        {
            var command = new Command("start-meeting", "Start a multi-agent meeting");

            var topicOption = new Option<string>("--topic")
            {
                Description = "Meeting topic"
            };

            var agentsOption = new Option<string[]>("--agents")
            {
                Description = "Paths to agent configuration files",
                Arity = ArgumentArity.OneOrMore,
                AllowMultipleArgumentsPerToken = true
            };

            var maxDurationOption = new Option<int?>("--max-duration")
            {
                Description = "Maximum meeting duration in minutes"
            };

            var maxMessagesOption = new Option<int?>("--max-messages")
            {
                Description = "Maximum total messages across all agents"
            };

            command.Options.Add(topicOption);
            command.Options.Add(agentsOption);
            command.Options.Add(maxDurationOption);
            command.Options.Add(maxMessagesOption);

            command.SetAction(parseResult =>
            {
                var topic = parseResult.GetValue(topicOption) ?? string.Empty;
                var agents = parseResult.GetValue(agentsOption) ?? Array.Empty<string>();
                var maxDuration = parseResult.GetValue(maxDurationOption);
                var maxMessages = parseResult.GetValue(maxMessagesOption);

                return ExecuteAsync(topic, agents, maxDuration, maxMessages).Result;
            });

            return command;
        }

        private static async Task<int> ExecuteAsync(
            string topic,
            IReadOnlyList<string> agents,
            int? maxDuration,
            int? maxMessages)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                Console.Error.WriteLine("Error: --topic is required");
                return 1;
            }

            if (agents.Count == 0)
            {
                Console.Error.WriteLine("Error: --agents requires at least one configuration file");
                return 1;
            }

            if (maxDuration.HasValue && maxDuration.Value <= 0)
            {
                Console.Error.WriteLine("Error: --max-duration must be a positive integer");
                return 1;
            }

            if (maxMessages.HasValue && maxMessages.Value <= 0)
            {
                Console.Error.WriteLine("Error: --max-messages must be a positive integer");
                return 1;
            }

            foreach (var agentPath in agents)
            {
                if (!File.Exists(agentPath))
                {
                    Console.Error.WriteLine($"Error: Agent configuration not found: {agentPath}");
                    return 1;
                }
            }

            var meetingId = GenerateMeetingId();
            var configuration = new MeetingConfiguration
            {
                MeetingId = meetingId,
                MeetingTopic = topic,
                AgentConfigPaths = agents.ToList(),
                HardLimits = new MeetingLimits
                {
                    MaxDurationMinutes = maxDuration,
                    MaxTotalMessages = maxMessages
                }
            };

            var eventBus = new InMemoryEventBus();
            var fileLocker = new FileLocker(eventBus);
            var meetingRoomFactory = new MeetingRoomFactory(fileLocker, eventBus);
            var transcriptManager = new TranscriptManager();
            var errorLogManager = new ErrorLogManager();
            var progressDisplay = new MeetingProgressDisplay();

            IMeetingRoom meetingRoom;
            try
            {
                meetingRoom = await meetingRoomFactory.CreateMeetingRoomAsync(
                    meetingId,
                    configuration,
                    configuration.OutputDirectory);
                await transcriptManager.SubscribeToEventsAsync(meetingRoom, eventBus);
                await errorLogManager.SubscribeToEventsAsync(meetingRoom, eventBus);
                progressDisplay.Attach(eventBus, configuration);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: Failed to create meeting room: {ex.Message}");
                return 1;
            }

            var promptBuilder = new PromptBuilder();
            var parser = new AgentConfigurationParser();
            var validator = new AgentConfigurationValidator();

            ICopilotClient? copilotClient = null;
            var agentMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE")?.ToLowerInvariant();
            if (agentMode != "stub")
            {
                copilotClient = new CopilotClient();
                try
                {
                    await copilotClient.StartAsync();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error: Failed to initialize Copilot CLI: {ex.Message}");
                    return 1;
                }
            }

            var agentFactory = new AgentFactory(parser, validator, promptBuilder, copilotClient);
            var turnManager = new FifoTurnManager();
            var orchestrator = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

            using var cancellationSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationSource.Cancel();
                Console.WriteLine("Cancellation requested. Finishing current work...");
            };

            try
            {
                var result = await orchestrator.RunMeetingAsync(configuration, cancellationSource.Token);
                Console.WriteLine($"Meeting artifacts: {meetingRoom.MeetingRoomPath}");
                Console.WriteLine($"Messages: {result.MessageCount}");
                return result.State == MeetingState.Completed ? 0 : 1;
            }
            catch (Exception ex)
            {
                return CliErrorHandler.HandleException(ex);
            }
            finally
            {
                if (copilotClient != null)
                {
                    await copilotClient.StopAsync();
                    await copilotClient.DisposeAsync();
                }
            }
        }

        private static string GenerateMeetingId()
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyyMMdd_HHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];
            return $"{timestamp}_{guid}";
        }
    }
}
