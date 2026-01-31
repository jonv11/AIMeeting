namespace AIMeeting.Core.FileSystem
{
    /// <summary>
    /// Represents an acquired lock on a file.
    /// </summary>
    public interface IFileLock : IDisposable
    {
        /// <summary>
        /// Path to the locked file.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Agent ID that acquired the lock.
        /// </summary>
        string LockedByAgentId { get; }

        /// <summary>
        /// When the lock was acquired.
        /// </summary>
        DateTime AcquiredAt { get; }
    }

    /// <summary>
    /// Interface for managing file locks.
    /// </summary>
    public interface IFileLocker
    {
        /// <summary>
        /// Acquires an exclusive lock on a file.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="agentId">ID of the agent acquiring the lock</param>
        /// <param name="timeout">Maximum time to wait for lock acquisition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The acquired lock (dispose to release)</returns>
        Task<IFileLock> AcquireLockAsync(
            string filePath,
            string agentId,
            TimeSpan timeout,
            CancellationToken cancellationToken);
    }
}
