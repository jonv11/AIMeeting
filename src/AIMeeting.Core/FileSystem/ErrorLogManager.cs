namespace AIMeeting.Core.FileSystem
{
    using AIMeeting.Core.Events;

    /// <summary>
    /// Service for persisting meeting error details.
    /// </summary>
    public interface IErrorLogManager
    {
        /// <summary>
        /// Subscribes to meeting events and writes errors.log when failures occur.
        /// </summary>
        Task SubscribeToEventsAsync(IMeetingRoom meetingRoom, IEventBus eventBus);
    }

    /// <summary>
    /// Default implementation of IErrorLogManager.
    /// </summary>
    public class ErrorLogManager : IErrorLogManager
    {
        private const string ErrorLogPath = "errors.log";

        public async Task SubscribeToEventsAsync(IMeetingRoom meetingRoom, IEventBus eventBus)
        {
            if (meetingRoom == null)
                throw new ArgumentNullException(nameof(meetingRoom));
            if (eventBus == null)
                throw new ArgumentNullException(nameof(eventBus));

            eventBus.Subscribe<MeetingEndedEvent>(async evt =>
            {
                if (!IsFailure(evt.EndReason))
                {
                    return;
                }

                var entry = $"[{DateTime.UtcNow:O}] {evt.EndReason}\n";
                await meetingRoom.AppendToFileAsync(ErrorLogPath, entry);
            });

            await Task.CompletedTask;
        }

        private static bool IsFailure(string reason)
        {
            return reason.StartsWith("Meeting failed", StringComparison.OrdinalIgnoreCase);
        }
    }
}
