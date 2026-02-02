namespace AIMeeting.Core.Tests.Validation
{
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Validation;
    using Xunit;

    /// <summary>
    /// Tests for OrchestratorResponseValidator.
    /// </summary>
    public class OrchestratorResponseValidatorTests
    {
        private readonly OrchestratorResponseValidator _validator;
        private readonly MeetingContext _context;

        public OrchestratorResponseValidatorTests()
        {
            _validator = new OrchestratorResponseValidator();
            _context = new MeetingContext
            {
                MeetingId = "test-meeting",
                Topic = "Test topic",
                Agents = new Dictionary<string, object>
                {
                    { "engineer-001", new object() },
                    { "pm-001", new object() }
                }
            };
        }

        [Fact]
        public void Validate_NullResponse_FailsValidation()
        {
            // Act
            var result = _validator.Validate(null!, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("cannot be null", result.ErrorMessage);
        }

        [Fact]
        public void Validate_MissingDecision_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "",
                Rationale = "Some rationale"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Decision field is required", result.ErrorMessage);
        }

        [Fact]
        public void Validate_InvalidDecisionType_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "invalid_decision",
                Rationale = "Some rationale"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Invalid decision", result.ErrorMessage);
        }

        [Fact]
        public void Validate_MissingRationale_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "continue",
                Rationale = "",
                NextAgentId = "engineer-001"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Rationale field is required", result.ErrorMessage);
        }

        [Fact]
        public void Validate_ContinueWithoutAgentId_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "continue",
                Rationale = "Need more input"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("next_agent_id is required", result.ErrorMessage);
        }

        [Fact]
        public void Validate_ContinueWithNonexistentAgent_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "continue",
                Rationale = "Need more input",
                NextAgentId = "nonexistent-agent"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("not found in meeting context", result.ErrorMessage);
        }

        [Fact]
        public void Validate_ValidContinueDecision_PassesValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "continue",
                Rationale = "Engineer has relevant expertise",
                NextAgentId = "engineer-001"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void Validate_ChangePhaseWithoutNewPhase_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "change_phase",
                Rationale = "Time to evaluate"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("new_phase is required", result.ErrorMessage);
        }

        [Fact]
        public void Validate_ChangePhaseWithInvalidPhase_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "change_phase",
                Rationale = "Time to evaluate",
                NewPhase = "InvalidPhase"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Invalid phase", result.ErrorMessage);
        }

        [Fact]
        public void Validate_ValidChangePhaseDecision_PassesValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "change_phase",
                Rationale = "Sufficient options have been generated",
                NewPhase = "Evaluation"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void Validate_EndMeetingWithoutReason_FailsValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "end_meeting",
                Rationale = "Meeting complete"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("end_reason is required", result.ErrorMessage);
        }

        [Fact]
        public void Validate_ValidEndMeetingDecision_PassesValidation()
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "end_meeting",
                Rationale = "Clear consensus achieved",
                EndReason = "Consensus reached on option B with action items defined"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorMessage);
        }

        [Theory]
        [InlineData("continue")]
        [InlineData("CONTINUE")]
        [InlineData("Continue")]
        public void Validate_DecisionIsCaseInsensitive(string decision)
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = decision,
                Rationale = "Test",
                NextAgentId = "engineer-001"
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Evaluation")]
        [InlineData("evaluation")]
        [InlineData("EVALUATION")]
        public void Validate_PhaseIsCaseInsensitive(string phase)
        {
            // Arrange
            var response = new OrchestratorResponse
            {
                Decision = "change_phase",
                Rationale = "Test",
                NewPhase = phase
            };

            // Act
            var result = _validator.Validate(response, _context);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
