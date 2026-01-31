namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Results and artifacts from a completed meeting.
    /// </summary>
    public class MeetingResult
    {
        /// <summary>
        /// Unique identifier for the meeting.
        /// </summary>
        public string MeetingId { get; set; } = null!;

        /// <summary>
        /// Final state of the meeting.
        /// </summary>
        public MeetingState State { get; set; }

        /// <summary>
        /// Reason the meeting ended.
        /// </summary>
        public string EndReason { get; set; } = null!;

        /// <summary>
        /// How long the meeting lasted.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Total messages exchanged.
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Path to the transcript file.
        /// </summary>
        public string TranscriptPath { get; set; } = null!;

        /// <summary>
        /// Path to the meeting metadata file.
        /// </summary>
        public string MeetingMetadataPath { get; set; } = null!;

        /// <summary>
        /// Path to the error log file (if any).
        /// </summary>
        public string? ErrorLogPath { get; set; }
    }
}
