namespace AIMeeting.Core.FileSystem
{
    using AIMeeting.Core.Events;

    /// <summary>
    /// Service for managing meeting transcripts.
    /// </summary>
    public interface ITranscriptManager
    {
        /// <summary>
        /// Subscribes to meeting events and updates transcript file.
        /// </summary>
        /// <param name="meetingRoom">The meeting room to write transcripts to</param>
        /// <param name="eventBus">The event bus to subscribe to</param>
        Task SubscribeToEventsAsync(IMeetingRoom meetingRoom, IEventBus eventBus);
    }

    /// <summary>
    /// Default implementation of ITranscriptManager.
    /// Listens to TurnCompletedEvent and appends to transcript file.
    /// </summary>
    public class TranscriptManager : ITranscriptManager
    {
        private const string TranscriptPath = "transcript.md";

        /// <summary>
        /// Subscribes to meeting events and updates transcript file.
        /// </summary>
        public async Task SubscribeToEventsAsync(IMeetingRoom meetingRoom, IEventBus eventBus)
        {
            if (meetingRoom == null)
                throw new ArgumentNullException(nameof(meetingRoom));

            if (eventBus == null)
                throw new ArgumentNullException(nameof(eventBus));

            // Initialize transcript file with header
            var header = $"# Meeting Transcript\n\n**Started:** {DateTime.UtcNow:O}\n\n";
            if (!await meetingRoom.FileExistsAsync(TranscriptPath))
            {
                await meetingRoom.WriteFileAsync(TranscriptPath, header);
            }

            // Subscribe to meeting started event for header
            eventBus.Subscribe<MeetingStartedEvent>(async (evt) =>
            {
                var topicLine = $"**Topic:** {evt.Configuration.MeetingTopic}\n\n";
                await meetingRoom.AppendToFileAsync(TranscriptPath, topicLine);
            });

            // Subscribe to turn completed events
            eventBus.Subscribe<TurnCompletedEvent>(async (evt) =>
            {
                var timestamp = DateTime.UtcNow;
                var entry = $"## Turn - {timestamp:HH:mm:ss}\n\n**Agent:** {evt.AgentId}\n\n{evt.Message}\n\n";
                await meetingRoom.AppendToFileAsync(TranscriptPath, entry);
            });

            // Subscribe to meeting ended event for footer
            eventBus.Subscribe<MeetingEndedEvent>(async (evt) =>
            {
                var footer = $"\n---\n\n**Ended:** {DateTime.UtcNow:O}\n\n**Reason:** {evt.EndReason}\n";
                await meetingRoom.AppendToFileAsync(TranscriptPath, footer);
            });

            await Task.CompletedTask;
        }
    }
}
