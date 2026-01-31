namespace AIMeeting.Core.Orchestration
{
    using AIMeeting.Core.Models;

    /// <summary>
    /// Central interface for running meetings and managing their lifecycle.
    /// </summary>
    public interface IMeetingOrchestrator
    {
        /// <summary>
        /// Runs a meeting to completion or until a limit is exceeded.
        /// </summary>
        /// <param name="configuration">The meeting configuration</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The meeting result</returns>
        Task<MeetingResult> RunMeetingAsync(
            MeetingConfiguration configuration,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current state of a running meeting.
        /// </summary>
        /// <param name="meetingId">The meeting ID</param>
        /// <returns>The current meeting state</returns>
        Task<MeetingState> GetMeetingStateAsync(string meetingId);

        /// <summary>
        /// Stops a meeting gracefully and generates final artifacts.
        /// </summary>
        /// <param name="meetingId">The meeting ID</param>
        /// <param name="reason">Reason for stopping</param>
        Task StopMeetingAsync(string meetingId, string reason);
    }
}
