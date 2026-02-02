namespace AIMeeting.Core.Tests.Orchestration
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Orchestration;
    using AIMeeting.Core.Prompts;
    using Moq;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Tests for AIOrchestrator prompt building and decision parsing.
    /// </summary>
    public class AIOrchestratorPromptTests
    {
        private readonly Mock<IPromptBuilder> _mockPromptBuilder;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly AIOrchestrator _orchestrator;
        private readonly MeetingContext _context;

        public AIOrchestratorPromptTests()
        {
            _mockPromptBuilder = new Mock<IPromptBuilder>();
            _mockEventBus = new Mock<IEventBus>();
            _orchestrator = new AIOrchestrator("orchestrator-1", _mockPromptBuilder.Object);
            
            // Setup meeting context
            _context = new MeetingContext
            {
                MeetingId = "meeting-123",
                Topic = "Test topic",
                StartedAt = DateTime.UtcNow,
                CurrentPhase = MeetingPhase.ProblemClarification,
                Agents = new()
                {
                    { "orchestrator-1", _orchestrator },
                    { "agent-1", new object() },
                    { "agent-2", new object() }
                },
                Messages = new()
                {
                    new Message { AgentId = "agent-1", Content = "First message" },
                    new Message { AgentId = "agent-2", Content = "Second message" }
                }
            };
        }

        [Fact]
        public async Task BuildOrchestratorPrompt_IncludesMeetingContext()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var request = new OrchestratorTurnRequestEvent
            {
                MeetingId = "meeting-123",
                CurrentTurnNumber = 5,
                CurrentPhase = MeetingPhase.ProblemClarification
            };

            // Act
            var prompt = InvokeBuildPrompt(request);

            // Assert
            Assert.Contains("MEETING CONTEXT", prompt);
            Assert.Contains("Test topic", prompt);
            Assert.Contains("ProblemClarification", prompt);
            Assert.Contains("Turn Number: 5", prompt);
        }

        [Fact]
        public async Task BuildOrchestratorPrompt_ListsAvailableAgents()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var request = new OrchestratorTurnRequestEvent
            {
                MeetingId = "meeting-123",
                CurrentTurnNumber = 1,
                CurrentPhase = MeetingPhase.ProblemClarification
            };

            // Act
            var prompt = InvokeBuildPrompt(request);

            // Assert
            Assert.Contains("AVAILABLE AGENTS", prompt);
            Assert.Contains("agent-1", prompt);
            Assert.Contains("agent-2", prompt);
            Assert.DoesNotContain("orchestrator-1", prompt); // Orchestrator should be excluded
        }

        [Fact]
        public async Task BuildOrchestratorPrompt_IncludesRecentMessages()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var request = new OrchestratorTurnRequestEvent
            {
                MeetingId = "meeting-123",
                CurrentTurnNumber = 3,
                CurrentPhase = MeetingPhase.ProblemClarification
            };

            // Act
            var prompt = InvokeBuildPrompt(request);

            // Assert
            Assert.Contains("RECENT MESSAGES", prompt);
            Assert.Contains("[agent-1]: First message", prompt);
            Assert.Contains("[agent-2]: Second message", prompt);
        }

        [Fact]
        public async Task BuildOrchestratorPrompt_IncludesResponseFormat()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var request = new OrchestratorTurnRequestEvent
            {
                MeetingId = "meeting-123",
                CurrentTurnNumber = 1,
                CurrentPhase = MeetingPhase.ProblemClarification
            };

            // Act
            var prompt = InvokeBuildPrompt(request);

            // Assert
            Assert.Contains("RESPONSE FORMAT", prompt);
            Assert.Contains("valid JSON", prompt);
            Assert.Contains("decision", prompt);
            Assert.Contains("next_agent_id", prompt);
            Assert.Contains("rationale", prompt);
        }

        [Fact]
        public async Task BuildOrchestratorPrompt_IncludesDecisionGuidance()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var request = new OrchestratorTurnRequestEvent
            {
                MeetingId = "meeting-123",
                CurrentTurnNumber = 1,
                CurrentPhase = MeetingPhase.ProblemClarification
            };

            // Act
            var prompt = InvokeBuildPrompt(request);

            // Assert
            Assert.Contains("DECISION GUIDANCE", prompt);
            Assert.Contains("relevant expertise", prompt);
            Assert.Contains("End meeting when", prompt);
            Assert.Contains("Change phase when", prompt);
        }

        [Fact]
        public async Task ParseDecision_ValidContinueDecision_ReturnsEvent()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "continue",
                  "next_agent_id": "agent-1",
                  "rationale": "Agent 1 has relevant expertise"
                }
                """;

            // Act
            var decision = InvokeParseDecision(json, "meeting-123");

            // Assert
            Assert.Equal(DecisionType.ContinueMeeting, decision.Type);
            Assert.Equal("agent-1", decision.NextAgentId);
            Assert.Equal("Agent 1 has relevant expertise", decision.Rationale);
            Assert.Equal("meeting-123", decision.MeetingId);
        }

        [Fact]
        public async Task ParseDecision_ValidChangePhaseDecision_ReturnsEvent()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "change_phase",
                  "new_phase": "Evaluation",
                  "rationale": "Problem is well understood, time to evaluate options"
                }
                """;

            // Act
            var decision = InvokeParseDecision(json, "meeting-123");

            // Assert
            Assert.Equal(DecisionType.ChangePhase, decision.Type);
            Assert.Equal(MeetingPhase.Evaluation, decision.NewPhase);
            Assert.Equal("Problem is well understood, time to evaluate options", decision.Rationale);
        }

        [Fact]
        public async Task ParseDecision_ValidEndMeetingDecision_ReturnsEvent()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "end_meeting",
                  "end_reason": "Consensus reached on all points",
                  "rationale": "All agents agree, no further discussion needed"
                }
                """;

            // Act
            var decision = InvokeParseDecision(json, "meeting-123");

            // Assert
            Assert.Equal(DecisionType.EndMeeting, decision.Type);
            Assert.Equal("Consensus reached on all points", decision.EndReason);
            Assert.Equal("All agents agree, no further discussion needed", decision.Rationale);
        }

        [Fact]
        public async Task ParseDecision_JsonInMarkdownCodeBlock_Extracts()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var response = """
                ```json
                {
                  "decision": "continue",
                  "next_agent_id": "agent-2",
                  "rationale": "Need agent 2's perspective"
                }
                ```
                """;

            // Act
            var decision = InvokeParseDecision(response, "meeting-123");

            // Assert
            Assert.Equal(DecisionType.ContinueMeeting, decision.Type);
            Assert.Equal("agent-2", decision.NextAgentId);
        }

        [Fact]
        public async Task ParseDecision_InvalidJson_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var invalidJson = "{ this is not valid json }";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(invalidJson, "meeting-123"));
            Assert.Contains("Failed to parse orchestrator JSON", ex.Message);
        }

        [Fact]
        public async Task ParseDecision_MissingNextAgentId_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "continue",
                  "rationale": "Need someone to speak"
                }
                """;

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(json, "meeting-123"));
            Assert.Contains("requires next_agent_id", ex.Message);
        }

        [Fact]
        public async Task ParseDecision_MissingNewPhase_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "change_phase",
                  "rationale": "Time to move on"
                }
                """;

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(json, "meeting-123"));
            Assert.Contains("requires new_phase", ex.Message);
        }

        [Fact]
        public async Task ParseDecision_InvalidPhaseName_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "change_phase",
                  "new_phase": "InvalidPhase",
                  "rationale": "Moving to invalid phase"
                }
                """;

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(json, "meeting-123"));
            Assert.Contains("Invalid phase name", ex.Message);
        }

        [Fact]
        public async Task ParseDecision_MissingEndReason_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "end_meeting",
                  "rationale": "Meeting is over"
                }
                """;

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(json, "meeting-123"));
            Assert.Contains("requires end_reason", ex.Message);
        }

        [Fact]
        public async Task ParseDecision_MissingRationale_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "continue",
                  "next_agent_id": "agent-1"
                }
                """;

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(json, "meeting-123"));
            Assert.Contains("rationale", ex.Message);
        }

        [Fact]
        public async Task ParseDecision_UnknownDecisionType_ThrowsException()
        {
            // Arrange
            await _orchestrator.InitializeAsync(_context, _mockEventBus.Object);
            var json = """
                {
                  "decision": "unknown_decision",
                  "rationale": "Trying something new"
                }
                """;

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(
                () => InvokeParseDecision(json, "meeting-123"));
            Assert.Contains("Invalid decision type", ex.Message);
        }

        // Helper methods to invoke private methods using reflection
        private string InvokeBuildPrompt(OrchestratorTurnRequestEvent request)
        {
            var method = typeof(AIOrchestrator).GetMethod(
                "BuildOrchestratorPrompt",
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            return (string)method!.Invoke(_orchestrator, new object[] { request })!;
        }

        private OrchestratorDecisionEvent InvokeParseDecision(string json, string meetingId)
        {
            var method = typeof(AIOrchestrator).GetMethod(
                "ParseDecision",
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            try
            {
                return (OrchestratorDecisionEvent)method!.Invoke(
                    _orchestrator, new object[] { json, meetingId })!;
            }
            catch (TargetInvocationException ex)
            {
                // Unwrap the inner exception for proper test assertions
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }
        }
    }
}
