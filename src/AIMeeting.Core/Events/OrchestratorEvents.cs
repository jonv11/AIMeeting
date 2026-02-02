namespace AIMeeting.Core.Events
{
    using AIMeeting.Core.Models;

    /// <summary>
    /// Orchestrator agent requests a turn decision.
    /// Published by MeetingOrchestrator when it needs to know who speaks next.
    /// </summary>
    public class OrchestratorTurnRequestEvent : MeetingEvent
    {
        /// <summary>
        /// Meeting identifier.
        /// </summary>
        public required string MeetingId { get; init; }
        
        /// <summary>
        /// Current turn number.
        /// </summary>
        public required int CurrentTurnNumber { get; init; }
        
        /// <summary>
        /// Current meeting phase.
        /// </summary>
        public required MeetingPhase CurrentPhase { get; init; }
    }

    /// <summary>
    /// Orchestrator agent publishes a turn decision.
    /// </summary>
    public class OrchestratorDecisionEvent : MeetingEvent
    {
        /// <summary>
        /// Meeting identifier.
        /// </summary>
        public required string MeetingId { get; init; }
        
        /// <summary>
        /// Type of decision made.
        /// </summary>
        public required DecisionType Type { get; init; }
        
        /// <summary>
        /// Populated when Type == ContinueMeeting.
        /// </summary>
        public string? NextAgentId { get; init; }
        
        /// <summary>
        /// Rationale for the decision.
        /// </summary>
        public string? Rationale { get; init; }
        
        /// <summary>
        /// Populated when Type == ChangePhase.
        /// </summary>
        public MeetingPhase? NewPhase { get; init; }
        
        /// <summary>
        /// Populated when Type == EndMeeting.
        /// </summary>
        public string? EndReason { get; init; }
    }

    /// <summary>
    /// Orchestrator agent requests a phase change.
    /// </summary>
    public class PhaseChangeRequestedEvent : MeetingEvent
    {
        /// <summary>
        /// Previous phase.
        /// </summary>
        public required MeetingPhase OldPhase { get; init; }
        
        /// <summary>
        /// New phase being requested.
        /// </summary>
        public required MeetingPhase NewPhase { get; init; }
        
        /// <summary>
        /// Reason for the phase change.
        /// </summary>
        public required string Reason { get; init; }
    }

    /// <summary>
    /// Phase has been changed.
    /// </summary>
    public class PhaseChangedEvent : MeetingEvent
    {
        /// <summary>
        /// Previous phase.
        /// </summary>
        public required MeetingPhase OldPhase { get; init; }
        
        /// <summary>
        /// New current phase.
        /// </summary>
        public required MeetingPhase NewPhase { get; init; }
    }
}
