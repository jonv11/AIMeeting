namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Represents the current phase of a meeting.
    /// Used by orchestrator agents to track meeting progression.
    /// </summary>
    public enum MeetingPhase
    {
        /// <summary>
        /// Meeting initialization phase.
        /// </summary>
        Initialization,
        
        /// <summary>
        /// Clarifying the problem or topic being discussed.
        /// </summary>
        ProblemClarification,
        
        /// <summary>
        /// Generating potential solutions or options.
        /// </summary>
        OptionGeneration,
        
        /// <summary>
        /// Evaluating the proposed options.
        /// </summary>
        Evaluation,
        
        /// <summary>
        /// Making a final decision.
        /// </summary>
        Decision,
        
        /// <summary>
        /// Planning execution of the decided option.
        /// </summary>
        ExecutionPlanning,
        
        /// <summary>
        /// Concluding the meeting.
        /// </summary>
        Conclusion
    }
}
