using AIMeeting.Core.Events;
using AIMeeting.Core.FileSystem;
using AIMeeting.Core.Models;
using Xunit;

namespace AIMeeting.Core.Tests.FileSystem
{
    public class TranscriptManagerTests
    {
        [Fact]
        public async Task SubscribeToEvents_AppendsTranscriptEntries()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var eventBus = new InMemoryEventBus();
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker(eventBus), eventBus);
                var transcriptManager = new TranscriptManager();

                await transcriptManager.SubscribeToEventsAsync(meetingRoom, eventBus);

                var config = new MeetingConfiguration
                {
                    MeetingTopic = "Transcript test",
                    AgentConfigPaths = ["agent-a"]
                };

                await eventBus.PublishAsync(new MeetingStartedEvent { Configuration = config });
                await eventBus.PublishAsync(new TurnCompletedEvent { AgentId = "agent-a", Message = "Hello" });
                await eventBus.PublishAsync(new MeetingEndedEvent { EndReason = "Done" });

                var transcriptPath = Path.Combine(tempDirectory, "transcript.md");
                var content = await File.ReadAllTextAsync(transcriptPath);

                Assert.Contains("Transcript test", content);
                Assert.Contains("agent-a", content);
                Assert.Contains("Hello", content);
                Assert.Contains("Ended", content);
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
