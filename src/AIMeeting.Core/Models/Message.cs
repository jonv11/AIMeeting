namespace AIMeeting.Core.Models
{
    /// <summary>
    /// Type of message in the meeting transcript.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// A standalone statement or contribution.
        /// </summary>
        Statement,

        /// <summary>
        /// A question posed to other agents.
        /// </summary>
        Question,

        /// <summary>
        /// A direct response to another message.
        /// </summary>
        Response
    }

    /// <summary>
    /// A single message in the meeting transcript.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Unique message identifier.
        /// </summary>
        public string MessageId { get; set; } = null!;

        /// <summary>
        /// ID of the agent who sent this message.
        /// </summary>
        public string AgentId { get; set; } = null!;

        /// <summary>
        /// The message content.
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// When the message was sent.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ID of the message this is a reply to (if any).
        /// </summary>
        public string? ReplyToMessageId { get; set; }

        /// <summary>
        /// Type of message (Statement, Question, Response).
        /// </summary>
        public MessageType Type { get; set; }
    }
}
