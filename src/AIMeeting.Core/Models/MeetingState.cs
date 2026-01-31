namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Enumeration of possible meeting states.
    /// </summary>
    public enum MeetingState
    {
        /// <summary>
        /// Meeting not yet started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Meeting is initializing agents and validating configuration.
        /// </summary>
        Initializing,

        /// <summary>
        /// Meeting is actively running.
        /// </summary>
        InProgress,

        /// <summary>
        /// Meeting is wrapping up and generating artifacts.
        /// </summary>
        EndingGracefully,

        /// <summary>
        /// Meeting completed successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// Meeting failed with an error.
        /// </summary>
        Failed,

        /// <summary>
        /// Meeting was cancelled by user.
        /// </summary>
        Cancelled
    }
}
