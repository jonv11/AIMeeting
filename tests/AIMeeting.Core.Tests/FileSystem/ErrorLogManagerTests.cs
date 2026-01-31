using AIMeeting.Core.Events;
using AIMeeting.Core.FileSystem;
using Xunit;

namespace AIMeeting.Core.Tests.FileSystem
{
    public class ErrorLogManagerTests
    {
        [Fact]
        public async Task SubscribeToEventsAsync_WritesErrorLog_OnFailure()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var eventBus = new InMemoryEventBus();
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker(eventBus), eventBus);
                var errorLogManager = new ErrorLogManager();

                await errorLogManager.SubscribeToEventsAsync(meetingRoom, eventBus);

                await eventBus.PublishAsync(new MeetingEndedEvent
                {
                    EndReason = "Meeting failed: simulated error"
                });

                var errorLogPath = Path.Combine(tempDirectory, "errors.log");
                Assert.True(File.Exists(errorLogPath));

                var content = await File.ReadAllTextAsync(errorLogPath);
                Assert.Contains("Meeting failed", content);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
            }
        }

        [Fact]
        public async Task SubscribeToEventsAsync_IgnoresNonFailureReasons()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var eventBus = new InMemoryEventBus();
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker(eventBus), eventBus);
                var errorLogManager = new ErrorLogManager();

                await errorLogManager.SubscribeToEventsAsync(meetingRoom, eventBus);

                await eventBus.PublishAsync(new MeetingEndedEvent
                {
                    EndReason = "Meeting completed normally"
                });

                var errorLogPath = Path.Combine(tempDirectory, "errors.log");
                Assert.False(File.Exists(errorLogPath));
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
            }
        }
    }
}
