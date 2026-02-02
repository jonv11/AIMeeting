namespace AIMeeting.Core.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Structured response from orchestrator agent.
    /// MUST be valid JSON to be parseable.
    /// </summary>
    public class OrchestratorResponse
    {
        /// <summary>
        /// Decision type: "continue", "change_phase", or "end_meeting"
        /// </summary>
        [JsonPropertyName("decision")]
        public required string Decision { get; set; }
        
        /// <summary>
        /// Agent ID to speak next (required if decision == "continue")
        /// </summary>
        [JsonPropertyName("next_agent_id")]
        public string? NextAgentId { get; set; }
        
        /// <summary>
        /// Rationale for the decision (required)
        /// </summary>
        [JsonPropertyName("rationale")]
        public required string Rationale { get; set; }
        
        /// <summary>
        /// New phase to transition to (required if decision == "change_phase")
        /// </summary>
        [JsonPropertyName("new_phase")]
        public string? NewPhase { get; set; }
        
        /// <summary>
        /// Reason for ending meeting (required if decision == "end_meeting")
        /// </summary>
        [JsonPropertyName("end_reason")]
        public string? EndReason { get; set; }
    }
    
    /// <summary>
    /// Decision type enumeration for type-safe orchestrator decisions.
    /// </summary>
    public enum DecisionType
    {
        /// <summary>
        /// Continue meeting with next agent.
        /// </summary>
        ContinueMeeting,
        
        /// <summary>
        /// Advance to next meeting phase.
        /// </summary>
        ChangePhase,
        
        /// <summary>
        /// Terminate meeting.
        /// </summary>
        EndMeeting
    }
}
