namespace AIMeeting.Core.Orchestration
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;

    /// <summary>
    /// Interface for orchestrator decision-making.
    /// Implementations can be AI-driven, human-driven, or rule-based.
    /// </summary>
    public interface IOrchestratorDecisionMaker
    {
        /// <summary>
        /// Unique identifier for this orchestrator.
        /// </summary>
        string OrchestratorId { get; }
        
        /// <summary>
        /// Initializes the orchestrator for a meeting.
        /// </summary>
        /// <param name="context">Meeting context</param>
        /// <param name="eventBus">Event bus for pub/sub</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task InitializeAsync(
            MeetingContext context,
            IEventBus eventBus,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Starts the orchestrator (begins listening for events).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        Task StartAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Stops the orchestrator gracefully.
        /// </summary>
        Task StopAsync();
    }
}
