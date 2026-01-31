namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Current state and history of a meeting.
    /// </summary>
    public class MeetingContext
    {
        /// <summary>
        /// Unique identifier for the meeting.
        /// </summary>
        public string MeetingId { get; set; } = null!;

        /// <summary>
        /// The meeting topic.
        /// </summary>
        public string Topic { get; set; } = null!;

        /// <summary>
        /// All messages in the meeting.
        /// </summary>
        public List<Message> Messages { get; set; } = new();

        /// <summary>
        /// Available agents (key: AgentId, value: IAgent).
        /// </summary>
        public Dictionary<string, object> Agents { get; set; } = new();

        /// <summary>
        /// Current state of the meeting.
        /// </summary>
        public MeetingState State { get; set; }

        /// <summary>
        /// When the meeting started.
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Which agent is currently speaking (if any).
        /// </summary>
        public string? CurrentSpeakingAgentId { get; set; }
    }
}
