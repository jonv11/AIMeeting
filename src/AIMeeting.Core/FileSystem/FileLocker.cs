namespace AIMeeting.Core.FileSystem
{
    using AIMeeting.Core.Events;

    /// <summary>
    /// Exception thrown when file lock acquisition fails.
    /// </summary>
    public class FileLockException : Exception
    {
        /// <summary>
        /// Path to the file that couldn't be locked.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Agent ID that requested the lock.
        /// </summary>
        public string RequestedByAgentId { get; }

        /// <summary>
        /// Initializes a new instance of the FileLockException class.
        /// </summary>
        public FileLockException(string message, string filePath, string requestedByAgentId)
            : base(message)
        {
            FilePath = filePath;
            RequestedByAgentId = requestedByAgentId;
        }
    }

    /// <summary>
    /// Internal implementation of a file lock.
    /// </summary>
    internal class FileLock : IFileLock
    {
        private readonly FileLocker _fileLocker;
        private bool _disposed;

        public string FilePath { get; }
        public string LockedByAgentId { get; }
        public DateTime AcquiredAt { get; }

        public FileLock(FileLocker fileLocker, string filePath, string agentId)
        {
            _fileLocker = fileLocker;
            FilePath = filePath;
            LockedByAgentId = agentId;
            AcquiredAt = DateTime.UtcNow;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _fileLocker.ReleaseLock(FilePath);
            _disposed = true;
        }
    }

    /// <summary>
    /// Default implementation of file locking with timeout support.
    /// Uses in-memory lock table for single-process scenarios.
    /// </summary>
    public class FileLocker : IFileLocker
    {
        private readonly Dictionary<string, FileLockInfo> _locks = new();
        private readonly SemaphoreSlim _lockTable = new(1, 1);
        private readonly IEventBus? _eventBus;

        private class FileLockInfo
        {
            public string FilePath { get; set; } = null!;
            public string LockedByAgentId { get; set; } = null!;
            public DateTime AcquiredAt { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the FileLocker class.
        /// </summary>
        /// <param name="eventBus">Optional event bus for publishing lock events</param>
        public FileLocker(IEventBus? eventBus = null)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// Acquires an exclusive lock on a file.
        /// </summary>
        public async Task<IFileLock> AcquireLockAsync(
            string filePath,
            string agentId,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (string.IsNullOrWhiteSpace(agentId))
                throw new ArgumentException("Agent ID cannot be null or empty", nameof(agentId));

            var deadline = DateTime.UtcNow.Add(timeout);

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    await _lockTable.WaitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw new FileLockException(
                        $"Lock acquisition for {filePath} was cancelled",
                        filePath,
                        agentId);
                }

                try
                {
                    if (!_locks.ContainsKey(filePath))
                    {
                        var lockInfo = new FileLockInfo
                        {
                            FilePath = filePath,
                            LockedByAgentId = agentId,
                            AcquiredAt = DateTime.UtcNow
                        };
                        _locks[filePath] = lockInfo;

                        // Publish lock acquired event
                        if (_eventBus != null)
                        {
                            await _eventBus.PublishAsync(new FileLockedEvent
                            {
                                FilePath = filePath,
                                AgentId = agentId
                            });
                        }

                        return new FileLock(this, filePath, agentId);
                    }
                }
                finally
                {
                    _lockTable.Release();
                }

                // Wait before retrying (50ms backoff)
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw new FileLockException(
                        $"Lock acquisition for {filePath} was cancelled",
                        filePath,
                        agentId);
                }
            }

            throw new FileLockException(
                $"Could not acquire lock on {filePath} within {timeout.TotalSeconds:F1} seconds",
                filePath,
                agentId);
        }

        /// <summary>
        /// Internal method to release a lock.
        /// </summary>
        internal void ReleaseLock(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            try
            {
                _lockTable.Wait();
                try
                {
                    if (_locks.Remove(filePath))
                    {
                        // Lock was successfully released
                        // Note: We could publish FileUnlockedEvent here if _eventBus was async
                    }
                }
                finally
                {
                    _lockTable.Release();
                }
            }
            catch
            {
                // Suppress exceptions during cleanup
            }
        }
    }
}
