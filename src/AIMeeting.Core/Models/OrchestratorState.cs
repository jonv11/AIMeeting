namespace AIMeeting.Core.Models
{
    /// <summary>
    /// State maintained by orchestrator agent.
    /// Tracks meeting progression, decisions, and open items.
    /// </summary>
    public class OrchestratorState
    {
        /// <summary>
        /// Hypotheses or theories proposed during the meeting.
        /// </summary>
        public List<string> Hypotheses { get; set; } = new();
        
        /// <summary>
        /// Decisions made during the meeting.
        /// </summary>
        public List<Decision> Decisions { get; set; } = new();
        
        /// <summary>
        /// Open questions that need answers.
        /// </summary>
        public List<string> OpenQuestions { get; set; } = new();
        
        /// <summary>
        /// Tracked disagreements between agents.
        /// Key: topic, Value: description of disagreement.
        /// </summary>
        public Dictionary<string, string> Disagreements { get; set; } = new();
        
        /// <summary>
        /// Action items identified during the meeting.
        /// </summary>
        public List<ActionItem> ActionItems { get; set; } = new();
    }
    
    /// <summary>
    /// Represents a decision made during the meeting.
    /// </summary>
    public class Decision
    {
        /// <summary>
        /// The decision text.
        /// </summary>
        public required string DecisionText { get; set; }
        
        /// <summary>
        /// Rationale for the decision.
        /// </summary>
        public required string Rationale { get; set; }
        
        /// <summary>
        /// Tradeoffs considered.
        /// </summary>
        public List<string> Tradeoffs { get; set; } = new();
        
        /// <summary>
        /// When the decision was made.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
    
    /// <summary>
    /// Represents an action item from the meeting.
    /// </summary>
    public class ActionItem
    {
        /// <summary>
        /// Description of what needs to be done.
        /// </summary>
        public required string Description { get; set; }
        
        /// <summary>
        /// Who is responsible (agent ID or person).
        /// </summary>
        public string? Owner { get; set; }
        
        /// <summary>
        /// Criteria for successful completion.
        /// </summary>
        public required string SuccessCriteria { get; set; }
    }
}
