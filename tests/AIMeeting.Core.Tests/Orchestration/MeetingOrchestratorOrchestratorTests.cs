namespace AIMeeting.Core.Tests.Orchestration
{
    using AIMeeting.Core.Agents;
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Orchestration;
    using AIMeeting.Core.Prompts;
    using Xunit;

    public class MeetingOrchestratorOrchestratorTests
    {
        [Fact]
        public async Task RunMeetingAsync_WithOrchestrator_EndMeetingDecisionStopsMeeting()
        {
            var eventBus = new InMemoryEventBus();
            var turnManager = new FifoTurnManager();
            var orchestrator = new TestOrchestrator("orchestrator-1", endAfterTurns: 2);
            var agentFactory = new TestAgentFactory(orchestrator);

            var orchestratorRunner = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

            var config = new MeetingConfiguration
            {
                MeetingId = "meeting-001",
                MeetingTopic = "Test topic",
                AgentConfigPaths = new List<string>
                {
                    "config/agents/orchestrator.txt",
                    "config/agents/agent-a.txt",
                    "config/agents/agent-b.txt"
                },
                HardLimits = new MeetingLimits
                {
                    MaxTotalMessages = 10
                }
            };

            var result = await orchestratorRunner.RunMeetingAsync(config);

            Assert.Equal(MeetingState.Completed, result.State);
            Assert.Equal("Orchestrator requested end", result.EndReason);
            Assert.Equal(2, result.MessageCount);
            Assert.True(orchestrator.WasStarted);
            Assert.True(orchestrator.WasStopped);
        }

        [Fact]
        public async Task RunMeetingAsync_WithOrchestrator_ChangePhasePublishesEvents()
        {
            var eventBus = new InMemoryEventBus();
            var turnManager = new FifoTurnManager();
            var orchestrator = new TestOrchestrator(
                "orchestrator-1",
                endAfterTurns: 2,
                changePhaseAtTurn: 1,
                changePhaseTo: MeetingPhase.Evaluation);
            var agentFactory = new TestAgentFactory(orchestrator);

            MeetingPhase? observedPhase = null;
            eventBus.Subscribe<PhaseChangedEvent>(evt =>
            {
                observedPhase = evt.NewPhase;
                return Task.CompletedTask;
            });

            var orchestratorRunner = new MeetingOrchestrator(agentFactory, eventBus, turnManager);

            var config = new MeetingConfiguration
            {
                MeetingId = "meeting-002",
                MeetingTopic = "Test topic",
                AgentConfigPaths = new List<string>
                {
                    "config/agents/orchestrator.txt",
                    "config/agents/agent-a.txt",
                    "config/agents/agent-b.txt"
                },
                HardLimits = new MeetingLimits
                {
                    MaxTotalMessages = 10
                }
            };

            var result = await orchestratorRunner.RunMeetingAsync(config);

            Assert.Equal(MeetingState.Completed, result.State);
            Assert.Equal(MeetingPhase.Evaluation, observedPhase);
        }

        private sealed class TestAgentFactory : IAgentFactory
        {
            private readonly IOrchestratorDecisionMaker _orchestrator;

            public TestAgentFactory(IOrchestratorDecisionMaker orchestrator)
            {
                _orchestrator = orchestrator;
            }

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
                    if (path.Contains("orchestrator", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var agentId = $"agent-{index}";
                    agents[agentId] = new TestAgent(agentId);
                    index++;
                }

                return Task.FromResult(agents);
            }

            public Task<IOrchestratorDecisionMaker?> DetectOrchestratorAsync(
                IEnumerable<string> configPaths,
                CancellationToken cancellationToken = default)
            {
                foreach (var path in configPaths)
                {
                    if (path.Contains("orchestrator", StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult<IOrchestratorDecisionMaker?>(_orchestrator);
                    }
                }

                return Task.FromResult<IOrchestratorDecisionMaker?>(null);
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

        private sealed class TestOrchestrator : IOrchestratorDecisionMaker
        {
            private readonly int _endAfterTurns;
            private readonly int? _changePhaseAtTurn;
            private readonly MeetingPhase _changePhaseTo;
            private int _turnCount;
            private IEventBus? _eventBus;
            private IDisposable? _subscription;

            public TestOrchestrator(
                string orchestratorId,
                int endAfterTurns,
                int? changePhaseAtTurn = null,
                MeetingPhase changePhaseTo = MeetingPhase.Evaluation)
            {
                OrchestratorId = orchestratorId;
                _endAfterTurns = endAfterTurns;
                _changePhaseAtTurn = changePhaseAtTurn;
                _changePhaseTo = changePhaseTo;
            }

            public string OrchestratorId { get; }

            public bool WasStarted { get; private set; }

            public bool WasStopped { get; private set; }

            public Task InitializeAsync(MeetingContext context, IEventBus eventBus, CancellationToken cancellationToken = default)
            {
                _eventBus = eventBus;
                return Task.CompletedTask;
            }

            public Task StartAsync(CancellationToken cancellationToken = default)
            {
                WasStarted = true;
                _subscription = _eventBus!.Subscribe<OrchestratorTurnRequestEvent>(OnTurnRequested);
                return Task.CompletedTask;
            }

            public Task StopAsync()
            {
                WasStopped = true;
                _subscription?.Dispose();
                return Task.CompletedTask;
            }

            private async Task OnTurnRequested(OrchestratorTurnRequestEvent request)
            {
                _turnCount++;

                if (_changePhaseAtTurn.HasValue && _turnCount == _changePhaseAtTurn.Value)
                {
                    await _eventBus!.PublishAsync(new OrchestratorDecisionEvent
                    {
                        MeetingId = request.MeetingId,
                        Type = DecisionType.ChangePhase,
                        NewPhase = _changePhaseTo,
                        Rationale = "Phase change for testing"
                    });
                    return;
                }

                if (_turnCount > _endAfterTurns)
                {
                    await _eventBus!.PublishAsync(new OrchestratorDecisionEvent
                    {
                        MeetingId = request.MeetingId,
                        Type = DecisionType.EndMeeting,
                        EndReason = "Orchestrator requested end",
                        Rationale = "Test complete"
                    });
                    return;
                }

                var nextAgentId = _turnCount % 2 == 1 ? "agent-0" : "agent-1";
                await _eventBus!.PublishAsync(new OrchestratorDecisionEvent
                {
                    MeetingId = request.MeetingId,
                    Type = DecisionType.ContinueMeeting,
                    NextAgentId = nextAgentId,
                    Rationale = "Test round-robin"
                });
            }
        }
    }
}
