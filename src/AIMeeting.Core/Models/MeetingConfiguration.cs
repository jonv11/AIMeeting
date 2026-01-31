namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Defines the parameters for a meeting.
    /// </summary>
    public class MeetingConfiguration
    {
        /// <summary>
        /// Optional meeting ID to use instead of auto-generation.
        /// </summary>
        public string? MeetingId { get; set; }

        /// <summary>
        /// The main topic or question for the meeting.
        /// </summary>
        public string MeetingTopic { get; set; } = null!;

        /// <summary>
        /// Paths to agent configuration files.
        /// </summary>
        public List<string> AgentConfigPaths { get; set; } = new();

        /// <summary>
        /// Hard limits for the meeting.
        /// </summary>
        public MeetingLimits HardLimits { get; set; } = new();

        /// <summary>
        /// Base directory for meeting artifacts (optional).
        /// </summary>
        public string? OutputDirectory { get; set; }
    }
}
