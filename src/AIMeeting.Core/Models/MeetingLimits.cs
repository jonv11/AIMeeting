namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Defines hard boundaries for meeting execution.
    /// </summary>
    public class MeetingLimits
    {
        /// <summary>
        /// Maximum meeting duration in minutes.
        /// </summary>
        public int? MaxDurationMinutes { get; set; }

        /// <summary>
        /// Maximum total messages across all agents.
        /// </summary>
        public int? MaxTotalMessages { get; set; }
    }
}
