using System.Text.Json;
using AIMeeting.Core.Events;
using AIMeeting.Core.FileSystem;
using AIMeeting.Core.Models;
using Xunit;

namespace AIMeeting.Core.Tests.FileSystem
{
    public class MeetingRoomFactoryTests
    {
        [Fact]
        public async Task CreateMeetingRoom_WritesMeetingJson()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var eventBus = new InMemoryEventBus();
                var factory = new MeetingRoomFactory(new FileLocker(eventBus), eventBus);

                var config = new MeetingConfiguration
                {
                    MeetingId = "meeting-123",
                    MeetingTopic = "Sample Topic!",
                    AgentConfigPaths = ["a", "b"],
                    HardLimits = new MeetingLimits
                    {
                        MaxDurationMinutes = 5,
                        MaxTotalMessages = 10
                    }
                };

                var meetingRoom = await factory.CreateMeetingRoomAsync(config.MeetingId, config, tempDirectory);

                var metadataPath = Path.Combine(meetingRoom.MeetingRoomPath, "meeting.json");
                Assert.True(File.Exists(metadataPath));

                var json = await File.ReadAllTextAsync(metadataPath);
                using var document = JsonDocument.Parse(json);

                Assert.Equal("meeting-123", document.RootElement.GetProperty("MeetingId").GetString());
                Assert.Equal("Sample Topic!", document.RootElement.GetProperty("Topic").GetString());
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
