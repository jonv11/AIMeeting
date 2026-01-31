namespace AIMeeting.Core.Exceptions
{
    /// <summary>
    /// Base exception for all AIMeeting errors.
    /// </summary>
    public abstract class AgentMeetingException : Exception
    {
        /// <summary>
        /// Error code for categorizing the error.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Context information about the error.
        /// </summary>
        public Dictionary<string, object> Context { get; }

        /// <summary>
        /// Initializes a new instance of the AgentMeetingException class.
        /// </summary>
        protected AgentMeetingException(
            string message,
            string errorCode,
            Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Context = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds context information to this exception.
        /// </summary>
        public void AddContext(string key, object value)
        {
            Context[key] = value;
        }
    }

    /// <summary>
    /// Exception for invalid meeting configuration.
    /// </summary>
    public class MeetingConfigurationException : AgentMeetingException
    {
        /// <summary>
        /// Initializes a new instance of the MeetingConfigurationException class.
        /// </summary>
        public MeetingConfigurationException(
            string message,
            Exception? innerException = null)
            : base(message, "CONFIG_ERROR", innerException)
        {
        }
    }

    /// <summary>
    /// Exception for agent initialization failures.
    /// </summary>
    public class AgentInitializationException : AgentMeetingException
    {
        /// <summary>
        /// ID of the agent that failed to initialize.
        /// </summary>
        public string AgentId { get; }

        /// <summary>
        /// Initializes a new instance of the AgentInitializationException class.
        /// </summary>
        public AgentInitializationException(
            string agentId,
            string message,
            Exception? innerException = null)
            : base(message, "AGENT_INIT_ERROR", innerException)
        {
            AgentId = agentId;
            AddContext("agent_id", agentId);
        }
    }

    /// <summary>
    /// Exception for when an agent exceeds response time limit.
    /// </summary>
    public class TurnTimeoutException : AgentMeetingException
    {
        /// <summary>
        /// ID of the agent that timed out.
        /// </summary>
        public string AgentId { get; }

        /// <summary>
        /// Timeout duration that was exceeded.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Initializes a new instance of the TurnTimeoutException class.
        /// </summary>
        public TurnTimeoutException(
            string agentId,
            TimeSpan timeout,
            Exception? innerException = null)
            : base(
                $"Agent {agentId} exceeded response timeout of {timeout.TotalSeconds:F1} seconds",
                "TURN_TIMEOUT",
                innerException)
        {
            AgentId = agentId;
            Timeout = timeout;
            AddContext("agent_id", agentId);
            AddContext("timeout_seconds", timeout.TotalSeconds);
        }
    }

    /// <summary>
    /// Exception for file operation failures.
    /// </summary>
    public class FileOperationException : AgentMeetingException
    {
        /// <summary>
        /// Path to the file that caused the error.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Initializes a new instance of the FileOperationException class.
        /// </summary>
        public FileOperationException(
            string filePath,
            string message,
            Exception? innerException = null)
            : base(message, "FILE_OPERATION_ERROR", innerException)
        {
            FilePath = filePath;
            AddContext("file_path", filePath);
        }
    }

    /// <summary>
    /// Exception for file lock acquisition failures.
    /// </summary>
    public class FileLockTimeoutException : AgentMeetingException
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
        /// Initializes a new instance of the FileLockTimeoutException class.
        /// </summary>
        public FileLockTimeoutException(
            string filePath,
            string requestedByAgentId,
            Exception? innerException = null)
            : base(
                $"Failed to acquire lock on {filePath} for agent {requestedByAgentId}",
                "FILE_LOCK_TIMEOUT",
                innerException)
        {
            FilePath = filePath;
            RequestedByAgentId = requestedByAgentId;
            AddContext("file_path", filePath);
            AddContext("agent_id", requestedByAgentId);
        }
    }

    /// <summary>
    /// Exception for Copilot API errors.
    /// </summary>
    public class CopilotApiException : AgentMeetingException
    {
        /// <summary>
        /// Initializes a new instance of the CopilotApiException class.
        /// </summary>
        public CopilotApiException(
            string message,
            Exception? innerException = null)
            : base(message, "COPILOT_API_ERROR", innerException)
        {
        }
    }

    /// <summary>
    /// Exception for when a hard limit is exceeded.
    /// </summary>
    public class MeetingLimitExceededException : AgentMeetingException
    {
        /// <summary>
        /// Type of limit that was exceeded (e.g., "Time", "Messages").
        /// </summary>
        public string LimitType { get; }

        /// <summary>
        /// The configured limit value.
        /// </summary>
        public object? LimitValue { get; }

        /// <summary>
        /// Initializes a new instance of the MeetingLimitExceededException class.
        /// </summary>
        public MeetingLimitExceededException(
            string limitType,
            object? limitValue = null,
            Exception? innerException = null)
            : base(
                $"Meeting limit exceeded: {limitType}",
                "MEETING_LIMIT_EXCEEDED",
                innerException)
        {
            LimitType = limitType;
            LimitValue = limitValue;
            AddContext("limit_type", limitType);
            if (limitValue != null)
            {
                AddContext("limit_value", limitValue);
            }
        }
    }
}
