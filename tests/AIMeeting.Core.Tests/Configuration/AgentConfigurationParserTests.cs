using Xunit;
using AIMeeting.Core.Configuration;

namespace AIMeeting.Core.Tests.Configuration
{
    public class AgentConfigurationParserTests
    {
        private readonly AgentConfigurationParser _parser = new();

        [Fact]
        public void Parse_ValidMinimalConfig_ReturnsSuccessfulResult()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider implementation complexity
                - Identify technical debt
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal("Senior Developer", result.Configuration.Role);
            Assert.Equal("Evaluates technical feasibility", result.Configuration.Description);
            Assert.NotEmpty(result.Configuration.Instructions);
        }

        [Fact]
        public void Parse_MissingRole_ReturnsFailure()
        {
            var content = """
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                """;

            var result = _parser.Parse(content);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Parse_MissingDescription_ReturnsFailure()
        {
            var content = """
                ROLE: Senior Developer
                INSTRUCTIONS:
                - Consider complexity
                """;

            var result = _parser.Parse(content);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Parse_MissingInstructions_ReturnsFailure()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                """;

            var result = _parser.Parse(content);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Parse_PersonaSection_ParsesCorrectly()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                PERSONA:
                - Pragmatic and detail-oriented
                - Focuses on implementation challenges
                - Advocates for code quality
                INSTRUCTIONS:
                - Consider complexity
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Configuration.PersonaTraits.Count);
            Assert.Contains("Pragmatic and detail-oriented", result.Configuration.PersonaTraits);
        }

        [Fact]
        public void Parse_WithBlankLines_IgnoresThemCorrectly()
        {
            var content = """
                ROLE: Senior Developer

                DESCRIPTION: Evaluates technical feasibility

                INSTRUCTIONS:
                - Consider complexity
                - Identify debt
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Configuration.Instructions.Count);
        }

        [Fact]
        public void Parse_WithComments_IgnoresThem()
        {
            var content = """
                # This is a comment
                ROLE: Senior Developer
                # Another comment
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal("Senior Developer", result.Configuration.Role);
        }

        [Fact]
        public void Parse_MaxMessageLength_ParsesAsInteger()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                MAX_MESSAGE_LENGTH: 500
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal(500, result.Configuration.MaxMessageLength);
        }

        [Fact]
        public void Parse_InvalidMaxMessageLength_ReturnsError()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                MAX_MESSAGE_LENGTH: not_a_number
                """;

            var result = _parser.Parse(content);

            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Message.Contains("MAX_MESSAGE_LENGTH"));
        }

        [Fact]
        public void Parse_UnknownHeader_GeneratesWarning()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                CUSTOM_FIELD: custom value
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.IsWarning && e.Message.Contains("Unknown header"));
            Assert.True(result.Configuration.CustomFields.ContainsKey("CUSTOM_FIELD"));
        }

        [Fact]
        public void Parse_ExpertiseAreas_ParsesCommaSeparatedList()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                EXPERTISE_AREAS: Backend Architecture, Performance, Code Quality
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Configuration.ExpertiseAreas.Count);
            Assert.Contains("Backend Architecture", result.Configuration.ExpertiseAreas);
        }

        [Fact]
        public void Parse_ResponseStyle_SetCorrectly()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                RESPONSE_STYLE: Technical, code-focused
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal("Technical, code-focused", result.Configuration.ResponseStyle);
        }

        [Fact]
        public void Parse_InitialMessageTemplate_SetCorrectly()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                INITIAL_MESSAGE_TEMPLATE: Let me review {topic} from a technical perspective.
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Contains("{topic}", result.Configuration.InitialMessageTemplate);
        }

        [Fact]
        public void Parse_InvalidHeaderFormat_GeneratesError()
        {
            var content = """
                This is not a valid header format
                ROLE: Senior Developer
                """;

            var result = _parser.Parse(content);

            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Message.Contains("Invalid line format"));
        }

        [Fact]
        public void Parse_InvalidHeaderName_GeneratesError()
        {
            var content = """
                ROLE!: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                """;

            var result = _parser.Parse(content);

            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Message.Contains("Invalid header name"));
        }

        [Fact]
        public void Parse_EmptyContent_ReturnsFailure()
        {
            var result = _parser.Parse("");

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Parse_PersonaInlineValue_ParsesSingleItem()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                PERSONA: Pragmatic and detail-oriented
                INSTRUCTIONS:
                - Consider complexity
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Configuration.PersonaTraits);
            Assert.Equal("Pragmatic and detail-oriented", result.Configuration.PersonaTraits[0]);
        }

        [Fact]
        public void Parse_InstructionsSection_StopsAtNextHeader()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                RESPONSE_STYLE: Technical
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Configuration.Instructions);
            Assert.Equal("Technical", result.Configuration.ResponseStyle);
        }

        [Fact]
        public void Parse_MultipleInstructions_ParsesAll()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider implementation complexity
                - Identify potential technical debt
                - Suggest practical solutions
                - Ask questions about scalability
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal(4, result.Configuration.Instructions.Count);
        }

        [Fact]
        public void Parse_LineNumberTracking_IncludedInErrors()
        {
            var content = """
                ROLE: Senior Developer
                DESCRIPTION: Evaluates technical feasibility
                INSTRUCTIONS:
                - Consider complexity
                MAX_MESSAGE_LENGTH: invalid
                """;

            var result = _parser.Parse(content);

            Assert.Contains(result.Errors, e => e.LineNumber > 0);
        }

        [Fact]
        public void Parse_WindowsLineEndings_NormalizedCorrectly()
        {
            var content = "ROLE: Senior Developer\r\nDESCRIPTION: Evaluates technical feasibility\r\nINSTRUCTIONS:\r\n- Consider complexity\r\n";

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal("Senior Developer", result.Configuration.Role);
        }

        [Fact]
        public void Parse_MacLineEndings_NormalizedCorrectly()
        {
            var content = "ROLE: Senior Developer\rDESCRIPTION: Evaluates technical feasibility\rINSTRUCTIONS:\r- Consider complexity\r";

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal("Senior Developer", result.Configuration.Role);
        }

        [Fact]
        public void Parse_TrailingWhitespace_Trimmed()
        {
            var content = """
                ROLE: Senior Developer   
                DESCRIPTION: Evaluates technical feasibility   
                INSTRUCTIONS:
                - Consider complexity   
                """;

            var result = _parser.Parse(content);

            Assert.True(result.IsSuccess);
            Assert.Equal("Senior Developer", result.Configuration.Role);
        }

        [Fact]
        public async Task ParseAsync_ValidFile_ReturnsSuccessfulResult()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var content = """
                    ROLE: Senior Developer
                    DESCRIPTION: Evaluates technical feasibility
                    INSTRUCTIONS:
                    - Consider complexity
                    """;
                
                await File.WriteAllTextAsync(tempFile, content, System.Text.Encoding.UTF8);

                var result = await _parser.ParseAsync(tempFile);

                Assert.True(result.IsSuccess);
                Assert.Equal("Senior Developer", result.Configuration.Role);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ParseAsync_NonexistentFile_ReturnsFailure()
        {
            var result = await _parser.ParseAsync("/nonexistent/path/config.txt");

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task ParseAsync_FileTooLarge_ReturnsFailure()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var content = new string('A', 65 * 1024);
                await File.WriteAllTextAsync(tempFile, content, System.Text.Encoding.UTF8);

                var result = await _parser.ParseAsync(tempFile);

                Assert.False(result.IsSuccess);
                Assert.Contains(result.Errors, e => e.Message.Contains("File size exceeds maximum"));
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ParseAsync_InvalidUtf8_ReturnsFailure()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var invalidBytes = new byte[] { 0xFF, 0xFE, 0xFD };
                await File.WriteAllBytesAsync(tempFile, invalidBytes);

                var result = await _parser.ParseAsync(tempFile);

                Assert.False(result.IsSuccess);
                Assert.NotEmpty(result.Errors);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
