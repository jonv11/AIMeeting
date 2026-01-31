using AIMeeting.Core.Agents;
using AIMeeting.Core.Events;
using AIMeeting.Core.Models;
using AIMeeting.Core.Orchestration;
using Xunit;

namespace AIMeeting.Core.Tests.Orchestration
{
    public class MeetingOrchestratorLimitTests
    {
        [Fact]
        public async Task RunMeetingAsync_MaxTotalMessages_StopsAtLimit()
        {
            var eventBus = new InMemoryEventBus();
            var turnManager = new FifoTurnManager();
            var agentFactory = new TestAgentFactory();
            var orchestrator = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

            var configuration = new MeetingConfiguration
            {
                MeetingTopic = "Limit test",
                AgentConfigPaths = ["agent-a"],
                HardLimits = new MeetingLimits
                {
                    MaxTotalMessages = 1
                }
            };

            var result = await orchestrator.RunMeetingAsync(configuration);

            Assert.Equal(1, result.MessageCount);
            Assert.Equal(MeetingState.Completed, result.State);
        }

        [Fact]
        public async Task RunMeetingAsync_MaxDurationMinutes_StopsImmediately()
        {
            var eventBus = new InMemoryEventBus();
            var turnManager = new FifoTurnManager();
            var agentFactory = new TestAgentFactory();
            var orchestrator = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

            var configuration = new MeetingConfiguration
            {
                MeetingTopic = "Duration test",
                AgentConfigPaths = ["agent-a"],
                HardLimits = new MeetingLimits
                {
                    MaxDurationMinutes = 0
                }
            };

            var result = await orchestrator.RunMeetingAsync(configuration);

            Assert.Equal(0, result.MessageCount);
            Assert.Equal(MeetingState.Completed, result.State);
        }

        [Fact]
        public async Task RunMeetingAsync_CancelledToken_ReturnsCancelled()
        {
            var eventBus = new InMemoryEventBus();
            var turnManager = new FifoTurnManager();
            var agentFactory = new TestAgentFactory();
            var orchestrator = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

            var configuration = new MeetingConfiguration
            {
                MeetingTopic = "Cancellation test",
                AgentConfigPaths = ["agent-a"]
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await orchestrator.RunMeetingAsync(configuration, cts.Token);

            Assert.Equal(MeetingState.Cancelled, result.State);
            Assert.Contains("cancelled", result.EndReason, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class TestAgentFactory : IAgentFactory
        {
            public Task<IAgent> CreateAgentAsync(string configPath, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IAgent>(new TestAgent(configPath));
            }

            public Task<Dictionary<string, IAgent>> CreateAgentsAsync(IEnumerable<string> configPaths, CancellationToken cancellationToken = default)
            {
                var agents = new Dictionary<string, IAgent>();
                var index = 0;
                foreach (var path in configPaths)
                {
                    var agentId = $"test-agent-{index}";
                    agents[agentId] = new TestAgent(agentId);
                    index++;
                }

                return Task.FromResult(agents);
            }

            public IAgent CreateModeratorAgent(AIMeeting.Core.Configuration.AgentConfiguration configuration)
            {
                return new TestAgent("moderator");
            }
        }

        private sealed class TestAgent : IAgent
        {
            public TestAgent(string agentId)
            {
                AgentId = agentId;
            }

            public string AgentId { get; }

            public string RoleName => "Test";

            public Task InitializeAsync(MeetingContext context, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task<AgentResponse> RespondAsync(MeetingContext context, CancellationToken cancellationToken)
            {
                return Task.FromResult(new AgentResponse
                {
                    Content = "Test response",
                    Type = MessageType.Statement
                });
            }

            public Task<bool> ShouldParticipateAsync(MeetingContext context)
            {
                return Task.FromResult(true);
            }
        }
    }
}
