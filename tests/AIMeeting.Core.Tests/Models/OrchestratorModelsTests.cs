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

    /// <summary>
    /// Tests for OrchestratorMetrics model.
    /// </summary>
    public class OrchestratorMetricsTests
    {
        [Fact]
        public void OrchestratorMetrics_InitializesWithDefaults()
        {
            // Act
            var metrics = new OrchestratorMetrics();

            // Assert
            Assert.Equal(0, metrics.TotalDecisions);
            Assert.Equal(0, metrics.SuccessfulFirstAttempts);
            Assert.Equal(0, metrics.RetriedDecisions);
            Assert.Equal(0, metrics.StubFallbacks);
            Assert.Equal(0, metrics.TotalRetryAttempts);
            Assert.Equal(0, metrics.JsonParseErrors);
            Assert.Equal(0, metrics.ApiCallFailures);
            Assert.Equal(0, metrics.TotalDecisionTimeMs);
            Assert.Equal(long.MaxValue, metrics.MinDecisionTimeMs);
            Assert.Equal(0, metrics.MaxDecisionTimeMs);
            Assert.Equal(0, metrics.ContinueDecisions);
            Assert.Equal(0, metrics.PhaseChangeDecisions);
            Assert.Equal(0, metrics.EndMeetingDecisions);
        }

        [Fact]
        public void AverageDecisionTimeMs_ReturnsZero_WhenNoDecisions()
        {
            // Arrange
            var metrics = new OrchestratorMetrics();

            // Act & Assert
            Assert.Equal(0, metrics.AverageDecisionTimeMs);
        }

        [Fact]
        public void AverageDecisionTimeMs_CalculatesCorrectly()
        {
            // Arrange
            var metrics = new OrchestratorMetrics
            {
                TotalDecisions = 4,
                TotalDecisionTimeMs = 1000
            };

            // Act & Assert
            Assert.Equal(250, metrics.AverageDecisionTimeMs);
        }

        [Fact]
        public void StubFallbackRate_ReturnsZero_WhenNoDecisions()
        {
            // Arrange
            var metrics = new OrchestratorMetrics();

            // Act & Assert
            Assert.Equal(0, metrics.StubFallbackRate);
        }

        [Fact]
        public void StubFallbackRate_CalculatesPercentageCorrectly()
        {
            // Arrange
            var metrics = new OrchestratorMetrics
            {
                TotalDecisions = 10,
                StubFallbacks = 3
            };

            // Act & Assert
            Assert.Equal(30.0, metrics.StubFallbackRate);
        }

        [Fact]
        public void RetryRate_ReturnsZero_WhenNoDecisions()
        {
            // Arrange
            var metrics = new OrchestratorMetrics();

            // Act & Assert
            Assert.Equal(0, metrics.RetryRate);
        }

        [Fact]
        public void RetryRate_CalculatesPercentageCorrectly()
        {
            // Arrange
            var metrics = new OrchestratorMetrics
            {
                TotalDecisions = 20,
                RetriedDecisions = 5
            };

            // Act & Assert
            Assert.Equal(25.0, metrics.RetryRate);
        }

        [Fact]
        public void AverageRetriesPerDecision_ReturnsZero_WhenNoDecisions()
        {
            // Arrange
            var metrics = new OrchestratorMetrics();

            // Act & Assert
            Assert.Equal(0, metrics.AverageRetriesPerDecision);
        }

        [Fact]
        public void AverageRetriesPerDecision_CalculatesCorrectly()
        {
            // Arrange
            var metrics = new OrchestratorMetrics
            {
                TotalDecisions = 10,
                TotalRetryAttempts = 15
            };

            // Act & Assert
            Assert.Equal(1.5, metrics.AverageRetriesPerDecision);
        }

        [Fact]
        public void GetSummary_ReturnsFormattedString()
        {
            // Arrange
            var metrics = new OrchestratorMetrics
            {
                TotalDecisions = 10,
                ContinueDecisions = 7,
                PhaseChangeDecisions = 2,
                EndMeetingDecisions = 1,
                SuccessfulFirstAttempts = 8,
                RetriedDecisions = 2,
                StubFallbacks = 0,
                JsonParseErrors = 1,
                ApiCallFailures = 1,
                TotalRetryAttempts = 3,
                TotalDecisionTimeMs = 5000,
                MinDecisionTimeMs = 100,
                MaxDecisionTimeMs = 1000
            };

            // Act
            var summary = metrics.GetSummary();

            // Assert
            Assert.Contains("Total:                 10", summary);
            Assert.Contains("Continue:              7", summary);
            Assert.Contains("Phase Changes:         2", summary);
            Assert.Contains("End Meeting:           1", summary);
            Assert.Contains("First Attempt Success: 8", summary);
            Assert.Contains("Required Retries:      2", summary);
            Assert.Contains("Stub Fallbacks:        0", summary);
            Assert.Contains("JSON Parse Errors:     1", summary);
            Assert.Contains("API Call Failures:     1", summary);
            Assert.Contains("Total Retry Attempts:  3", summary);
            Assert.Contains("Avg Decision Time:     500ms", summary);
            Assert.Contains("Min Decision Time:     100ms", summary);
            Assert.Contains("Max Decision Time:     1000ms", summary);
        }

        [Fact]
        public void GetSummary_HandlesZeroDecisions()
        {
            // Arrange
            var metrics = new OrchestratorMetrics();

            // Act
            var summary = metrics.GetSummary();

            // Assert
            Assert.Contains("Total:                 0", summary);
            Assert.Contains("Avg Decision Time:     0ms", summary);
            Assert.Contains("Min Decision Time:     0ms", summary); // long.MaxValue should display as 0
        }

        [Fact]
        public void MetricsProperties_CanBeSetAndRead()
        {
            // Arrange
            var metrics = new OrchestratorMetrics();

            // Act
            metrics.TotalDecisions = 100;
            metrics.SuccessfulFirstAttempts = 85;
            metrics.RetriedDecisions = 10;
            metrics.StubFallbacks = 5;
            metrics.TotalRetryAttempts = 15;
            metrics.JsonParseErrors = 3;
            metrics.ApiCallFailures = 7;
            metrics.TotalDecisionTimeMs = 50000;
            metrics.MinDecisionTimeMs = 50;
            metrics.MaxDecisionTimeMs = 2000;
            metrics.ContinueDecisions = 90;
            metrics.PhaseChangeDecisions = 8;
            metrics.EndMeetingDecisions = 2;

            // Assert
            Assert.Equal(100, metrics.TotalDecisions);
            Assert.Equal(85, metrics.SuccessfulFirstAttempts);
            Assert.Equal(10, metrics.RetriedDecisions);
            Assert.Equal(5, metrics.StubFallbacks);
            Assert.Equal(15, metrics.TotalRetryAttempts);
            Assert.Equal(3, metrics.JsonParseErrors);
            Assert.Equal(7, metrics.ApiCallFailures);
            Assert.Equal(50000, metrics.TotalDecisionTimeMs);
            Assert.Equal(50, metrics.MinDecisionTimeMs);
            Assert.Equal(2000, metrics.MaxDecisionTimeMs);
            Assert.Equal(90, metrics.ContinueDecisions);
            Assert.Equal(8, metrics.PhaseChangeDecisions);
            Assert.Equal(2, metrics.EndMeetingDecisions);
        }
    }
}
