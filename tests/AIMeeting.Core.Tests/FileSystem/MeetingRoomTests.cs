using System.Security;
using AIMeeting.Core.Events;
using AIMeeting.Core.FileSystem;
using Xunit;

namespace AIMeeting.Core.Tests.FileSystem
{
    public class MeetingRoomTests
    {
        [Fact]
        public async Task WriteReadAppend_Works()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var eventBus = new InMemoryEventBus();
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker(eventBus), eventBus);

                await meetingRoom.WriteFileAsync("notes.txt", "first");
                await meetingRoom.AppendToFileAsync("notes.txt", " second");

                var content = await meetingRoom.ReadFileAsync("notes.txt");

                Assert.Equal("first second", content);
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
        public async Task ValidatePath_PreventsTraversal()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker());

                await Assert.ThrowsAsync<SecurityException>(() => meetingRoom.ReadFileAsync("../secret.txt"));
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
        public async Task ListFiles_ReturnsRelativePaths()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "AIMeeting.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var meetingRoom = new MeetingRoom(tempDirectory, new FileLocker());

                await meetingRoom.WriteFileAsync("folder/a.txt", "content");

                var files = await meetingRoom.ListFilesAsync();

                Assert.Contains("folder/a.txt", files);
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
