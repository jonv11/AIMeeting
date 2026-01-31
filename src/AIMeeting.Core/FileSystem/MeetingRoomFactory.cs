namespace AIMeeting.Core.FileSystem
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using System.Text.Json;

    /// <summary>
    /// Factory for creating and managing meeting rooms.
    /// </summary>
    public interface IMeetingRoomFactory
    {
        /// <summary>
        /// Creates a new meeting room for a given meeting.
        /// </summary>
        /// <param name="meetingId">The meeting ID</param>
        /// <param name="configuration">Meeting configuration</param>
        /// <param name="baseDirectory">Base directory for meeting artifacts (defaults to "meetings")</param>
        /// <returns>The created meeting room</returns>
        Task<IMeetingRoom> CreateMeetingRoomAsync(
            string meetingId,
            MeetingConfiguration configuration,
            string? baseDirectory = null);
    }

    /// <summary>
    /// Default implementation of IMeetingRoomFactory.
    /// </summary>
    public class MeetingRoomFactory : IMeetingRoomFactory
    {
        private readonly IFileLocker _fileLocker;
        private readonly IEventBus? _eventBus;

        /// <summary>
        /// Initializes a new instance of the MeetingRoomFactory class.
        /// </summary>
        public MeetingRoomFactory(IFileLocker fileLocker, IEventBus? eventBus = null)
        {
            _fileLocker = fileLocker ?? throw new ArgumentNullException(nameof(fileLocker));
            _eventBus = eventBus;
        }

        /// <summary>
        /// Creates a new meeting room for a given meeting.
        /// </summary>
        public async Task<IMeetingRoom> CreateMeetingRoomAsync(
            string meetingId,
            MeetingConfiguration configuration,
            string? baseDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting ID cannot be null or empty", nameof(meetingId));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Use provided base directory or default to "meetings"
            baseDirectory ??= "meetings";

            // Create meeting directory with timestamp and sanitized topic
            var sanitizedTopic = SanitizeTopicForPath(configuration.MeetingTopic);
            var meetingDir = Path.Combine(baseDirectory, $"{meetingId}_{sanitizedTopic}");
            var fullPath = Path.GetFullPath(meetingDir);

            // Create the directory
            Directory.CreateDirectory(fullPath);

            // Write meeting metadata
            var metadata = new
            {
                MeetingId = meetingId,
                Topic = configuration.MeetingTopic,
                AgentCount = configuration.AgentConfigPaths.Count,
                CreatedAt = DateTime.UtcNow,
                Limits = new
                {
                    MaxDurationMinutes = configuration.HardLimits?.MaxDurationMinutes,
                    MaxTotalMessages = configuration.HardLimits?.MaxTotalMessages
                }
            };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var metadataJson = JsonSerializer.Serialize(metadata, jsonOptions);
            var metadataPath = Path.Combine(fullPath, "meeting.json");
            await File.WriteAllTextAsync(metadataPath, metadataJson);

            // Publish file created event
            if (_eventBus != null)
            {
                await _eventBus.PublishAsync(new FileCreatedEvent
                {
                    AgentId = "system",
                    FilePath = Path.GetRelativePath(baseDirectory, metadataPath).Replace("\\", "/")
                });
            }

            // Create and return the meeting room
            return new MeetingRoom(fullPath, _fileLocker, _eventBus);
        }

        /// <summary>
        /// Sanitizes a topic string for use in file paths.
        /// </summary>
        private string SanitizeTopicForPath(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return "meeting";

            // Convert to lowercase and replace spaces/punctuation with underscores
            var invalid = new string(Path.GetInvalidFileNameChars());
            var sanitized = topic
                .ToLowerInvariant()
                .Take(50) // Limit length
                .Aggregate(
                    new System.Text.StringBuilder(),
                    (sb, c) => invalid.Contains(c) ? sb.Append('_') : sb.Append(c),
                    sb => sb.ToString());

            // Remove consecutive underscores
            while (sanitized.Contains("__"))
                sanitized = sanitized.Replace("__", "_");

            // Trim underscores from ends
            return sanitized.Trim('_');
        }
    }
}
