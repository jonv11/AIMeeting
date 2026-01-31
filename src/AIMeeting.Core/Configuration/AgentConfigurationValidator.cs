namespace AIMeeting.Core.Configuration
{
    /// <summary>
    /// Validates agent configurations after parsing.
    /// </summary>
    public interface IAgentConfigurationValidator
    {
        /// <summary>
        /// Validates a parsed configuration and returns validation errors.
        /// </summary>
        /// <param name="configuration">The configuration to validate</param>
        /// <param name="parseErrors">Existing parse errors that may have occurred during parsing</param>
        /// <returns>Validation result with any errors found</returns>
        AgentConfigurationValidationResult Validate(AgentConfiguration configuration, List<ParseError> parseErrors);
    }

    /// <summary>
    /// Result of validating an agent configuration.
    /// </summary>
    public class AgentConfigurationValidationResult
    {
        public bool IsValid { get; set; }
        public List<ValidationError> Errors { get; set; } = new();

        public static AgentConfigurationValidationResult Success() => new() { IsValid = true };
        public static AgentConfigurationValidationResult Failure(params ValidationError[] errors) => new()
        {
            IsValid = false,
            Errors = [..errors]
        };
    }

    /// <summary>
    /// A validation error with context.
    /// </summary>
    public class ValidationError
    {
        public string Message { get; }
        public string? FieldName { get; }
        public int? LineNumber { get; }

        public ValidationError(string message, string? fieldName = null, int? lineNumber = null)
        {
            Message = message;
            FieldName = fieldName;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            var location = LineNumber.HasValue ? $" (line {LineNumber})" : "";
            var field = !string.IsNullOrEmpty(FieldName) ? $" [{FieldName}]" : "";
            return $"Validation Error{location}{field}: {Message}";
        }
    }

    /// <summary>
    /// Default implementation of agent configuration validator.
    /// </summary>
    public class AgentConfigurationValidator : IAgentConfigurationValidator
    {
        /// <summary>
        /// Validates a configuration after parsing.
        /// Note: The parser already enforces most validation rules.
        /// This validator can be used for additional runtime validation if needed.
        /// </summary>
        public AgentConfigurationValidationResult Validate(AgentConfiguration configuration, List<ParseError> parseErrors)
        {
            var errors = new List<ValidationError>();

            // If there are any parse errors that are not warnings, validation fails
            var fatalErrors = parseErrors.Where(e => !e.IsWarning).ToList();
            foreach (var error in fatalErrors)
            {
                errors.Add(new ValidationError(error.Message, lineNumber: error.LineNumber));
            }

            if (errors.Count > 0)
            {
                return AgentConfigurationValidationResult.Failure([..errors]);
            }

            // Additional runtime validations can be added here
            // For now, the parser already validates:
            // - Required fields (ROLE, DESCRIPTION, INSTRUCTIONS)
            // - MAX_MESSAGE_LENGTH is numeric
            // - Format correctness

            return AgentConfigurationValidationResult.Success();
        }
    }
}
