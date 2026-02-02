namespace AIMeeting.Core.Validation
{
    using AIMeeting.Core.Models;
    
    /// <summary>
    /// Result of validation operation.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Whether validation succeeded.
        /// </summary>
        public bool IsValid { get; init; }
        
        /// <summary>
        /// Error message if validation failed.
        /// </summary>
        public string? ErrorMessage { get; init; }
        
        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        
        /// <summary>
        /// Creates a failed validation result with error message.
        /// </summary>
        public static ValidationResult Fail(string errorMessage) => new ValidationResult 
        { 
            IsValid = false, 
            ErrorMessage = errorMessage 
        };
    }
    
    /// <summary>
    /// Validates orchestrator responses for correctness and completeness.
    /// </summary>
    public class OrchestratorResponseValidator
    {
        private static readonly HashSet<string> ValidDecisions = new(StringComparer.OrdinalIgnoreCase)
        {
            "continue",
            "change_phase",
            "end_meeting"
        };
        
        private static readonly HashSet<string> ValidPhases = new(StringComparer.OrdinalIgnoreCase)
        {
            "Initialization",
            "ProblemClarification",
            "OptionGeneration",
            "Evaluation",
            "Decision",
            "ExecutionPlanning",
            "Conclusion"
        };
        
        /// <summary>
        /// Validates an orchestrator response against meeting context.
        /// </summary>
        /// <param name="response">The response to validate</param>
        /// <param name="context">Current meeting context</param>
        /// <returns>Validation result</returns>
        public ValidationResult Validate(OrchestratorResponse response, MeetingContext context)
        {
            if (response == null)
            {
                return ValidationResult.Fail("Response cannot be null");
            }
            
            // 1. Check decision is valid
            if (string.IsNullOrWhiteSpace(response.Decision))
            {
                return ValidationResult.Fail("Decision field is required");
            }
            
            if (!ValidDecisions.Contains(response.Decision))
            {
                return ValidationResult.Fail($"Invalid decision: {response.Decision}. Must be one of: continue, change_phase, end_meeting");
            }
            
            // 2. Check rationale is present
            if (string.IsNullOrWhiteSpace(response.Rationale))
            {
                return ValidationResult.Fail("Rationale field is required for all decisions");
            }
            
            // 3. Validate decision-specific fields
            if (response.Decision.Equals("continue", StringComparison.OrdinalIgnoreCase))
            {
                return ValidateContinueDecision(response, context);
            }
            else if (response.Decision.Equals("change_phase", StringComparison.OrdinalIgnoreCase))
            {
                return ValidateChangePhaseDecision(response);
            }
            else if (response.Decision.Equals("end_meeting", StringComparison.OrdinalIgnoreCase))
            {
                return ValidateEndMeetingDecision(response);
            }
            
            return ValidationResult.Success();
        }
        
        private ValidationResult ValidateContinueDecision(OrchestratorResponse response, MeetingContext context)
        {
            // Check next_agent_id is present
            if (string.IsNullOrWhiteSpace(response.NextAgentId))
            {
                return ValidationResult.Fail("next_agent_id is required for 'continue' decision");
            }
            
            // Check agent exists in context
            if (!context.Agents.ContainsKey(response.NextAgentId))
            {
                return ValidationResult.Fail($"Agent '{response.NextAgentId}' not found in meeting context");
            }
            
            return ValidationResult.Success();
        }
        
        private ValidationResult ValidateChangePhaseDecision(OrchestratorResponse response)
        {
            // Check new_phase is present
            if (string.IsNullOrWhiteSpace(response.NewPhase))
            {
                return ValidationResult.Fail("new_phase is required for 'change_phase' decision");
            }
            
            // Check phase is valid
            if (!ValidPhases.Contains(response.NewPhase))
            {
                return ValidationResult.Fail($"Invalid phase: {response.NewPhase}. Must be one of: {string.Join(", ", ValidPhases)}");
            }
            
            return ValidationResult.Success();
        }
        
        private ValidationResult ValidateEndMeetingDecision(OrchestratorResponse response)
        {
            // Check end_reason is present
            if (string.IsNullOrWhiteSpace(response.EndReason))
            {
                return ValidationResult.Fail("end_reason is required for 'end_meeting' decision");
            }
            
            return ValidationResult.Success();
        }
    }
}
