namespace AIMeeting.Core.FileSystem
{
    using System.Security;
    using AIMeeting.Core.Events;

    /// <summary>
    /// Default implementation of IMeetingRoom with isolated file access.
    /// Provides path traversal protection and file locking.
    /// </summary>
    public class MeetingRoom : IMeetingRoom
    {
        private readonly IFileLocker _fileLocker;
        private readonly IEventBus? _eventBus;
        private readonly string _meetingRoomPath;

        /// <summary>
        /// Gets the absolute path to the meeting room directory.
        /// </summary>
        public string MeetingRoomPath => _meetingRoomPath;

        /// <summary>
        /// Initializes a new instance of the MeetingRoom class.
        /// </summary>
        /// <param name="meetingRoomPath">Absolute path to the meeting room directory</param>
        /// <param name="fileLocker">File locker for managing access</param>
        /// <param name="eventBus">Optional event bus for publishing events</param>
        public MeetingRoom(
            string meetingRoomPath,
            IFileLocker fileLocker,
            IEventBus? eventBus = null)
        {
            if (string.IsNullOrWhiteSpace(meetingRoomPath))
                throw new ArgumentException("Meeting room path cannot be null or empty", nameof(meetingRoomPath));

            _meetingRoomPath = Path.GetFullPath(meetingRoomPath);
            _fileLocker = fileLocker ?? throw new ArgumentNullException(nameof(fileLocker));
            _eventBus = eventBus;

            // Create directory if it doesn't exist
            Directory.CreateDirectory(_meetingRoomPath);
        }

        /// <summary>
        /// Reads content from a file in the meeting room.
        /// </summary>
        public async Task<string> ReadFileAsync(string relativePath)
        {
            var fullPath = ValidateAndGetFullPath(relativePath);

            try
            {
                return await File.ReadAllTextAsync(fullPath, System.Text.Encoding.UTF8);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"File not found: {relativePath}");
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to read file {relativePath}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Writes content to a file in the meeting room.
        /// </summary>
        public async Task WriteFileAsync(string relativePath, string content)
        {
            var fullPath = ValidateAndGetFullPath(relativePath);

            try
            {
                // Create parent directory if needed
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write to temporary file first (atomic write)
                var tempFile = fullPath + ".tmp";
                await File.WriteAllTextAsync(tempFile, content, System.Text.Encoding.UTF8);

                // Atomically move temp file to target
                File.Move(tempFile, fullPath, overwrite: true);

                // Publish file created event
                if (_eventBus != null)
                {
                    await _eventBus.PublishAsync(new FileCreatedEvent
                    {
                        AgentId = "system",
                        FilePath = relativePath
                    });
                }
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to write file {relativePath}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Appends content to a file in the meeting room.
        /// </summary>
        public async Task AppendToFileAsync(string relativePath, string content)
        {
            var fullPath = ValidateAndGetFullPath(relativePath);

            try
            {
                // Create parent directory if needed
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.AppendAllTextAsync(fullPath, content, System.Text.Encoding.UTF8);

                // Publish file modified event
                if (_eventBus != null)
                {
                    await _eventBus.PublishAsync(new FileModifiedEvent
                    {
                        AgentId = "system",
                        FilePath = relativePath
                    });
                }
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to append to file {relativePath}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if a file exists in the meeting room.
        /// </summary>
        public async Task<bool> FileExistsAsync(string relativePath)
        {
            try
            {
                var fullPath = ValidateAndGetFullPath(relativePath);
                return await Task.FromResult(File.Exists(fullPath));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lists all files in a directory within the meeting room.
        /// </summary>
        public async Task<IEnumerable<string>> ListFilesAsync(string relativeDirectory = "")
        {
            try
            {
                var fullPath = string.IsNullOrEmpty(relativeDirectory)
                    ? _meetingRoomPath
                    : ValidateAndGetFullPath(relativeDirectory);

                if (!Directory.Exists(fullPath))
                {
                    return Enumerable.Empty<string>();
                }

                var files = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
                var relativePaths = files.Select(f =>
                    Path.GetRelativePath(_meetingRoomPath, f).Replace("\\", "/"));

                return await Task.FromResult(relativePaths.ToList());
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to list files in {relativeDirectory}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Acquires an exclusive lock on a file for safe modification.
        /// </summary>
        public async Task<IFileLock> AcquireLockAsync(
            string relativePath,
            string agentId,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var fullPath = ValidateAndGetFullPath(relativePath);
            return await _fileLocker.AcquireLockAsync(
                fullPath,
                agentId,
                timeout,
                cancellationToken);
        }

        /// <summary>
        /// Validates a relative path and returns the full path.
        /// Throws SecurityException if path traversal is attempted.
        /// </summary>
        private string ValidateAndGetFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Path cannot be null or empty", nameof(relativePath));

            // Prevent directory traversal
            if (relativePath.Contains("..") || relativePath.StartsWith("/"))
                throw new SecurityException($"Invalid path: {relativePath}");

            var fullPath = Path.Combine(_meetingRoomPath, relativePath);
            var resolvedPath = Path.GetFullPath(fullPath);
            var resolvedBase = Path.GetFullPath(_meetingRoomPath);

            // Ensure resolved path is within meeting room
            if (!resolvedPath.StartsWith(resolvedBase + Path.DirectorySeparatorChar) &&
                resolvedPath != resolvedBase)
            {
                throw new SecurityException(
                    $"Path traversal detected: {relativePath} resolves outside meeting room");
            }

            return resolvedPath;
        }
    }
}
