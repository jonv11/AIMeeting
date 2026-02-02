namespace AIMeeting.Core.Tests.Orchestration
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Orchestration;
    using AIMeeting.Core.Prompts;
    using Moq;
    using Xunit;

    /// <summary>
    /// Tests for AIOrchestrator in stub mode.
    /// </summary>
    public class AIOrchestratorStubTests
    {
        private readonly Mock<IPromptBuilder> _mockPromptBuilder;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly MeetingContext _context;
        private readonly AIOrchestrator _orchestrator;

        public AIOrchestratorStubTests()
        {
            _mockPromptBuilder = new Mock<IPromptBuilder>();
            _mockEventBus = new Mock<IEventBus>();
            
            _context = new MeetingContext
            {
                MeetingId = "test-meeting",
                Topic = "Test topic",
                Agents = new Dictionary<string, object>
                {
                    { "orchestrator-001", new object() },
                    { "engineer-001", new object() },
                    { "pm-001", new object() }
                }
            };
            
            _orchestrator = new AIOrchestrator(
                "orchestrator-001",
                _mockPromptBuilder.Object,
                copilotClient: null); // Null means stub mode
        }

        [Fact]
        public async Task InitializeAsync_SetsContextAndEventBus()
        {
            // Act
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);

            // Assert - no exception thrown
            Assert.True(true);
        }

        [Fact]
        public async Task InitializeAsync_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _orchestrator.InitializeAsync(null!, _mockEventBus.Object));
        }

        [Fact]
        public async Task InitializeAsync_WithNullEventBus_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _orchestrator.InitializeAsync(_context, null!));
        }

        [Fact]
        public async Task StartAsync_SubscribesToEvents()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);

            // Act
            await _orchestrator.StartAsync();

            // Assert - Should subscribe to turn request and turn completed events
            _mockEventBus.Verify(
                x => x.Subscribe<OrchestratorTurnRequestEvent>(It.IsAny<Func<OrchestratorTurnRequestEvent, Task>>()),
                Times.Once);
            _mockEventBus.Verify(
                x => x.Subscribe<TurnCompletedEvent>(It.IsAny<Func<TurnCompletedEvent, Task>>()),
                Times.Once);
        }

        [Fact]
        public async Task StartAsync_WithoutInitialize_ThrowsInvalidOperationException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _orchestrator.StartAsync());
        }

        [Fact]
        public async Task StubMode_CyclesThroughAgentsInRoundRobin()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            
            Func<OrchestratorTurnRequestEvent, Task>? capturedHandler = null;
            _mockEventBus
                .Setup(x => x.Subscribe<OrchestratorTurnRequestEvent>(It.IsAny<Func<OrchestratorTurnRequestEvent, Task>>()))
                .Callback<Func<OrchestratorTurnRequestEvent, Task>>(handler => capturedHandler = handler)
                .Returns(Mock.Of<IDisposable>());

            var publishedDecisions = new List<OrchestratorDecisionEvent>();
            _mockEventBus
                .Setup(x => x.PublishAsync(It.IsAny<OrchestratorDecisionEvent>()))
                .Callback<OrchestratorDecisionEvent>(evt => publishedDecisions.Add(evt))
                .Returns(Task.CompletedTask);

            await _orchestrator.StartAsync();
            
            // Ensure handler was captured
            Assert.NotNull(capturedHandler);

            // Act - Request 3 turns
            await capturedHandler!(new OrchestratorTurnRequestEvent
            {
                MeetingId = "test-meeting",
                CurrentTurnNumber = 1,
                CurrentPhase = MeetingPhase.Initialization
            });
            
            await capturedHandler(new OrchestratorTurnRequestEvent
            {
                MeetingId = "test-meeting",
                CurrentTurnNumber = 2,
                CurrentPhase = MeetingPhase.Initialization
            });
            
            await capturedHandler(new OrchestratorTurnRequestEvent
            {
                MeetingId = "test-meeting",
                CurrentTurnNumber = 3,
                CurrentPhase = MeetingPhase.Initialization
            });

            // Assert - Should cycle through non-orchestrator agents
            Assert.Equal(3, publishedDecisions.Count);
            
            // All should be continue decisions
            Assert.All(publishedDecisions, d => Assert.Equal(DecisionType.ContinueMeeting, d.Type));
            
            // Should cycle: engineer-001, pm-001, engineer-001 (wraps around)
            Assert.Equal("engineer-001", publishedDecisions[0].NextAgentId);
            Assert.Equal("pm-001", publishedDecisions[1].NextAgentId);
            Assert.Equal("engineer-001", publishedDecisions[2].NextAgentId);
        }

        [Fact]
        public async Task StubMode_WithNoAgents_DecidesToEndMeeting()
        {
            // Arrange - Context with only orchestrator (no other agents)
            var emptyContext = new MeetingContext
            {
                MeetingId = "test-meeting",
                Topic = "Test",
                Agents = new Dictionary<string, object>
                {
                    { "orchestrator-001", new object() }
                }
            };

            await _orchestrator.InitializeAsync(emptyContext, _mockEventBus.Object);
            
            Func<OrchestratorTurnRequestEvent, Task>? capturedHandler = null;
            _mockEventBus
                .Setup(x => x.Subscribe<OrchestratorTurnRequestEvent>(It.IsAny<Func<OrchestratorTurnRequestEvent, Task>>()))
                .Callback<Func<OrchestratorTurnRequestEvent, Task>>(handler => capturedHandler = handler)
                .Returns(Mock.Of<IDisposable>());

            OrchestratorDecisionEvent? publishedDecision = null;
            _mockEventBus
                .Setup(x => x.PublishAsync(It.IsAny<OrchestratorDecisionEvent>()))
                .Callback<OrchestratorDecisionEvent>(evt => publishedDecision = evt)
                .Returns(Task.CompletedTask);

            await _orchestrator.StartAsync();
            Assert.NotNull(capturedHandler);

            // Act
            await capturedHandler!(new OrchestratorTurnRequestEvent
            {
                MeetingId = "test-meeting",
                CurrentTurnNumber = 1,
                CurrentPhase = MeetingPhase.Initialization
            });

            // Assert
            Assert.NotNull(publishedDecision);
            Assert.Equal(DecisionType.EndMeeting, publishedDecision.Type);
            Assert.Equal("No agents available", publishedDecision.EndReason);
        }

        [Fact]
        public async Task StubMode_IncludesRationaleInDecisions()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            
            Func<OrchestratorTurnRequestEvent, Task>? capturedHandler = null;
            _mockEventBus
                .Setup(x => x.Subscribe<OrchestratorTurnRequestEvent>(It.IsAny<Func<OrchestratorTurnRequestEvent, Task>>()))
                .Callback<Func<OrchestratorTurnRequestEvent, Task>>(handler => capturedHandler = handler)
                .Returns(Mock.Of<IDisposable>());

            OrchestratorDecisionEvent? publishedDecision = null;
            _mockEventBus
                .Setup(x => x.PublishAsync(It.IsAny<OrchestratorDecisionEvent>()))
                .Callback<OrchestratorDecisionEvent>(evt => publishedDecision = evt)
                .Returns(Task.CompletedTask);

            await _orchestrator.StartAsync();
            Assert.NotNull(capturedHandler);

            // Act
            await capturedHandler!(new OrchestratorTurnRequestEvent
            {
                MeetingId = "test-meeting",
                CurrentTurnNumber = 5,
                CurrentPhase = MeetingPhase.Initialization
            });

            // Assert
            Assert.NotNull(publishedDecision);
            Assert.NotNull(publishedDecision.Rationale);
            Assert.Contains("stub mode", publishedDecision.Rationale, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("turn 5", publishedDecision.Rationale);
        }

        [Fact]
        public async Task StopAsync_DisposesSubscriptions()
        {
            // Arrange
            var disposableMock1 = new Mock<IDisposable>();
            var disposableMock2 = new Mock<IDisposable>();
            
            _mockEventBus
                .Setup(x => x.Subscribe<OrchestratorTurnRequestEvent>(It.IsAny<Func<OrchestratorTurnRequestEvent, Task>>()))
                .Returns(disposableMock1.Object);
            _mockEventBus
                .Setup(x => x.Subscribe<TurnCompletedEvent>(It.IsAny<Func<TurnCompletedEvent, Task>>()))
                .Returns(disposableMock2.Object);

            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            await _orchestrator.StartAsync();

            // Act
            await _orchestrator.StopAsync();

            // Assert
            disposableMock1.Verify(x => x.Dispose(), Times.Once);
            disposableMock2.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void OrchestratorId_IsSetCorrectly()
        {
            // Assert
            Assert.Equal("orchestrator-001", _orchestrator.OrchestratorId);
        }
    }
}
