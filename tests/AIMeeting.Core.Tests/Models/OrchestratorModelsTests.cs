namespace AIMeeting.Core.Tests.Models
{
    using AIMeeting.Core.Models;
    using Xunit;

    /// <summary>
    /// Tests for OrchestratorResponse model.
    /// </summary>
    public class OrchestratorResponseTests
    {
        [Fact]
        public void OrchestratorResponse_CanBeCreated()
        {
            // Arrange & Act
            var response = new OrchestratorResponse
            {
                Decision = "continue",
                Rationale = "Test rationale",
                NextAgentId = "test-agent"
            };

            // Assert
            Assert.Equal("continue", response.Decision);
            Assert.Equal("Test rationale", response.Rationale);
            Assert.Equal("test-agent", response.NextAgentId);
        }

        [Fact]
        public void OrchestratorResponse_OptionalFields_CanBeNull()
        {
            // Arrange & Act
            var response = new OrchestratorResponse
            {
                Decision = "continue",
                Rationale = "Test"
            };

            // Assert
            Assert.Null(response.NextAgentId);
            Assert.Null(response.NewPhase);
            Assert.Null(response.EndReason);
        }
    }

    /// <summary>
    /// Tests for MeetingPhase enum.
    /// </summary>
    public class MeetingPhaseTests
    {
        [Fact]
        public void MeetingPhase_HasExpectedValues()
        {
            // Assert all expected phases exist
            Assert.Equal(0, (int)MeetingPhase.Initialization);
            Assert.Equal(1, (int)MeetingPhase.ProblemClarification);
            Assert.Equal(2, (int)MeetingPhase.OptionGeneration);
            Assert.Equal(3, (int)MeetingPhase.Evaluation);
            Assert.Equal(4, (int)MeetingPhase.Decision);
            Assert.Equal(5, (int)MeetingPhase.ExecutionPlanning);
            Assert.Equal(6, (int)MeetingPhase.Conclusion);
        }
    }

    /// <summary>
    /// Tests for OrchestratorState model.
    /// </summary>
    public class OrchestratorStateTests
    {
        [Fact]
        public void OrchestratorState_InitializesEmptyCollections()
        {
            // Act
            var state = new OrchestratorState();

            // Assert
            Assert.NotNull(state.Hypotheses);
            Assert.Empty(state.Hypotheses);
            Assert.NotNull(state.Decisions);
            Assert.Empty(state.Decisions);
            Assert.NotNull(state.OpenQuestions);
            Assert.Empty(state.OpenQuestions);
            Assert.NotNull(state.Disagreements);
            Assert.Empty(state.Disagreements);
            Assert.NotNull(state.ActionItems);
            Assert.Empty(state.ActionItems);
        }

        [Fact]
        public void Decision_CanBeCreated()
        {
            // Arrange & Act
            var decision = new Decision
            {
                DecisionText = "Use option A",
                Rationale = "Best performance",
                Tradeoffs = new List<string> { "Higher cost" },
                Timestamp = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("Use option A", decision.DecisionText);
            Assert.Equal("Best performance", decision.Rationale);
            Assert.Single(decision.Tradeoffs);
        }

        [Fact]
        public void ActionItem_CanBeCreated()
        {
            // Arrange & Act
            var item = new ActionItem
            {
                Description = "Implement feature X",
                Owner = "engineer-001",
                SuccessCriteria = "Feature passes all tests"
            };

            // Assert
            Assert.Equal("Implement feature X", item.Description);
            Assert.Equal("engineer-001", item.Owner);
            Assert.Equal("Feature passes all tests", item.SuccessCriteria);
        }
    }

    /// <summary>
    /// Tests for MeetingContext extensions.
    /// </summary>
    public class MeetingContextExtensionTests
    {
        [Fact]
        public void MeetingContext_HasCurrentPhase_DefaultsToInitialization()
        {
            // Act
            var context = new MeetingContext
            {
                MeetingId = "test",
                Topic = "Test"
            };

            // Assert
            Assert.Equal(MeetingPhase.Initialization, context.CurrentPhase);
        }

        [Fact]
        public void MeetingContext_OrchestratorState_CanBeSet()
        {
            // Arrange
            var context = new MeetingContext
            {
                MeetingId = "test",
                Topic = "Test"
            };
            var state = new OrchestratorState();

            // Act
            context.OrchestratorState = state;

            // Assert
            Assert.NotNull(context.OrchestratorState);
            Assert.Same(state, context.OrchestratorState);
        }

        [Fact]
        public void MeetingContext_CurrentPhase_CanBeChanged()
        {
            // Arrange
            var context = new MeetingContext
            {
                MeetingId = "test",
                Topic = "Test"
            };

            // Act
            context.CurrentPhase = MeetingPhase.Evaluation;

            // Assert
            Assert.Equal(MeetingPhase.Evaluation, context.CurrentPhase);
        }
    }
}
