namespace AIMeeting.Core.Events
{
    using AIMeeting.Core.Models;

    /// <summary>
    /// Base class for all meeting events.
    /// </summary>
    public abstract class MeetingEvent
    {
        /// <summary>
        /// When the event occurred.
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Agent lifecycle events.
    /// </summary>
    public class AgentJoinedEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that joined the meeting.
        /// </summary>
        public required string AgentId { get; init; }
    }

    public class AgentReadyEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that is ready.
        /// </summary>
        public required string AgentId { get; init; }
    }

    public class AgentLeftEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that left the meeting.
        /// </summary>
        public required string AgentId { get; init; }
    }

    /// <summary>
    /// Meeting lifecycle events.
    /// </summary>
    public class MeetingStartedEvent : MeetingEvent
    {
        /// <summary>
        /// The meeting configuration that was started.
        /// </summary>
        public required MeetingConfiguration Configuration { get; init; }
    }

    public class MeetingEndingEvent : MeetingEvent
    {
        /// <summary>
        /// Reason the meeting is ending.
        /// </summary>
        public required string Reason { get; init; }
    }

    public class MeetingEndedEvent : MeetingEvent
    {
        /// <summary>
        /// Final reason for meeting end.
        /// </summary>
        public required string EndReason { get; init; }
    }

    /// <summary>
    /// Turn management events.
    /// </summary>
    public class TurnStartedEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that is speaking in this turn.
        /// </summary>
        public required string AgentId { get; init; }

        /// <summary>
        /// The turn number (starts at 1).
        /// </summary>
        public required int TurnNumber { get; init; }
    }

    public class TurnCompletedEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that completed the turn.
        /// </summary>
        public required string AgentId { get; init; }

        /// <summary>
        /// The message content the agent produced.
        /// </summary>
        public required string Message { get; init; }
    }

    public class TurnSkippedEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID whose turn was skipped.
        /// </summary>
        public required string AgentId { get; init; }

        /// <summary>
        /// Reason the turn was skipped.
        /// </summary>
        public required string Reason { get; init; }
    }

    /// <summary>
    /// File operation events.
    /// </summary>
    public class FileCreatedEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that created the file (or "system").
        /// </summary>
        public required string AgentId { get; init; }

        /// <summary>
        /// Path to the file that was created.
        /// </summary>
        public required string FilePath { get; init; }
    }

    public class FileModifiedEvent : MeetingEvent
    {
        /// <summary>
        /// Agent ID that modified the file (or "system").
        /// </summary>
        public required string AgentId { get; init; }

        /// <summary>
        /// Path to the file that was modified.
        /// </summary>
        public required string FilePath { get; init; }
    }

    public class FileLockedEvent : MeetingEvent
    {
        /// <summary>
        /// Path to the file that was locked.
        /// </summary>
        public required string FilePath { get; init; }

        /// <summary>
        /// Agent ID that acquired the lock.
        /// </summary>
        public required string AgentId { get; init; }
    }

    public class FileUnlockedEvent : MeetingEvent
    {
        /// <summary>
        /// Path to the file that was unlocked.
        /// </summary>
        public required string FilePath { get; init; }
    }
}
