using Xunit;
using AIMeeting.Core.Configuration;

namespace AIMeeting.Core.Tests.Configuration
{
    public class AgentConfigurationValidatorTests
    {
        private readonly IAgentConfigurationValidator _validator = new AgentConfigurationValidator();

        [Fact]
        public void Validate_WithNoErrors_ReturnsSuccess()
        {
            var config = new AgentConfiguration
            {
                Role = "Developer",
                Description = "Evaluates technical aspects",
                Instructions = ["Consider complexity", "Suggest solutions"]
            };

            var result = _validator.Validate(config, new List<ParseError>());

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithParseErrors_ReturnsFails()
        {
            var config = new AgentConfiguration();
            var parseErrors = new List<ParseError>
            {
                new ParseError("Missing required field: ROLE", 1)
            };

            var result = _validator.Validate(config, parseErrors);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void Validate_WithWarnings_Succeeds()
        {
            var config = new AgentConfiguration
            {
                Role = "Developer",
                Description = "Evaluates technical aspects",
                Instructions = ["Consider complexity"]
            };
            var parseErrors = new List<ParseError>
            {
                new ParseError("Unknown header 'CUSTOM_FIELD'", 5, isWarning: true)
            };

            var result = _validator.Validate(config, parseErrors);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WithMultipleParseErrors_ReturnsAll()
        {
            var config = new AgentConfiguration();
            var parseErrors = new List<ParseError>
            {
                new ParseError("Missing required field: ROLE", 1),
                new ParseError("Missing required field: DESCRIPTION", 2),
                new ParseError("Missing required field: INSTRUCTIONS", 3)
            };

            var result = _validator.Validate(config, parseErrors);

            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
        }
    }
}
