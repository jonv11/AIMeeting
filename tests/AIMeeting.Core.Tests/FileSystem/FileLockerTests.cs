using AIMeeting.Core.FileSystem;
using Xunit;

namespace AIMeeting.Core.Tests.FileSystem
{
    public class FileLockerTests
    {
        [Fact]
        public async Task AcquireLock_Twice_ThrowsTimeout()
        {
            var locker = new FileLocker();
            var filePath = Path.Combine(Path.GetTempPath(), "lock-test.txt");

            using var firstLock = await locker.AcquireLockAsync(
                filePath,
                "agent-1",
                TimeSpan.FromMilliseconds(50),
                CancellationToken.None);

            await Assert.ThrowsAsync<FileLockException>(async () =>
                await locker.AcquireLockAsync(
                    filePath,
                    "agent-2",
                    TimeSpan.FromMilliseconds(50),
                    CancellationToken.None));
        }

        [Fact]
        public async Task AcquireLock_AfterDispose_Succeeds()
        {
            var locker = new FileLocker();
            var filePath = Path.Combine(Path.GetTempPath(), "lock-test-2.txt");

            using (await locker.AcquireLockAsync(
                filePath,
                "agent-1",
                TimeSpan.FromMilliseconds(50),
                CancellationToken.None))
            {
            }

            using var secondLock = await locker.AcquireLockAsync(
                filePath,
                "agent-2",
                TimeSpan.FromMilliseconds(50),
                CancellationToken.None);

            Assert.Equal(filePath, secondLock.FilePath);
        }
    }
}
