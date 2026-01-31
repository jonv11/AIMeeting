namespace AIMeeting.Core.FileSystem
{
    /// <summary>
    /// Interface for accessing and modifying meeting artifacts and shared files.
    /// </summary>
    public interface IMeetingRoom
    {
        /// <summary>
        /// Gets the absolute path to the meeting room directory.
        /// </summary>
        string MeetingRoomPath { get; }

        /// <summary>
        /// Reads content from a file in the meeting room.
        /// </summary>
        /// <param name="relativePath">Relative path within the meeting room</param>
        /// <returns>File content as string</returns>
        Task<string> ReadFileAsync(string relativePath);

        /// <summary>
        /// Writes content to a file in the meeting room.
        /// </summary>
        /// <param name="relativePath">Relative path within the meeting room</param>
        /// <param name="content">Content to write</param>
        Task WriteFileAsync(string relativePath, string content);

        /// <summary>
        /// Appends content to a file in the meeting room.
        /// </summary>
        /// <param name="relativePath">Relative path within the meeting room</param>
        /// <param name="content">Content to append</param>
        Task AppendToFileAsync(string relativePath, string content);

        /// <summary>
        /// Checks if a file exists in the meeting room.
        /// </summary>
        /// <param name="relativePath">Relative path within the meeting room</param>
        /// <returns>True if file exists, false otherwise</returns>
        Task<bool> FileExistsAsync(string relativePath);

        /// <summary>
        /// Lists all files in a directory within the meeting room.
        /// </summary>
        /// <param name="relativeDirectory">Relative directory path (empty string for root)</param>
        /// <returns>List of relative file paths</returns>
        Task<IEnumerable<string>> ListFilesAsync(string relativeDirectory = "");

        /// <summary>
        /// Acquires an exclusive lock on a file for safe modification.
        /// </summary>
        /// <param name="relativePath">Relative path within the meeting room</param>
        /// <param name="agentId">ID of the agent acquiring the lock</param>
        /// <param name="timeout">Maximum time to wait for lock acquisition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The acquired lock (dispose to release)</returns>
        Task<IFileLock> AcquireLockAsync(
            string relativePath,
            string agentId,
            TimeSpan timeout,
            CancellationToken cancellationToken);
    }
}
