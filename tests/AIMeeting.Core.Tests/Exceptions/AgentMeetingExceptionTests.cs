using AIMeeting.Core.Exceptions;
using Xunit;

namespace AIMeeting.Core.Tests.Exceptions
{
    public class AgentMeetingExceptionTests
    {
        [Fact]
        public void MeetingConfigurationException_SetsErrorCodeAndMessage()
        {
            var ex = new MeetingConfigurationException("bad config");

            Assert.Equal("bad config", ex.Message);
            Assert.Equal("CONFIG_ERROR", ex.ErrorCode);
        }

        [Fact]
        public void AgentInitializationException_AddsContext()
        {
            var ex = new AgentInitializationException("agent-1", "init failed");

            Assert.Equal("AGENT_INIT_ERROR", ex.ErrorCode);
            Assert.Equal("agent-1", ex.AgentId);
            Assert.Equal("agent-1", ex.Context["agent_id"]);
        }

        [Fact]
        public void TurnTimeoutException_CapturesTimeout()
        {
            var timeout = TimeSpan.FromSeconds(5);
            var ex = new TurnTimeoutException("agent-1", timeout);

            Assert.Equal("TURN_TIMEOUT", ex.ErrorCode);
            Assert.Equal("agent-1", ex.AgentId);
            Assert.Equal(timeout, ex.Timeout);
            Assert.Equal(timeout.TotalSeconds, ex.Context["timeout_seconds"]);
        }

        [Fact]
        public void FileOperationException_CapturesFilePath()
        {
            var ex = new FileOperationException("/tmp/file.txt", "failed");

            Assert.Equal("FILE_OPERATION_ERROR", ex.ErrorCode);
            Assert.Equal("/tmp/file.txt", ex.FilePath);
            Assert.Equal("/tmp/file.txt", ex.Context["file_path"]);
        }

        [Fact]
        public void FileLockTimeoutException_CapturesAgentAndPath()
        {
            var ex = new FileLockTimeoutException("/tmp/file.txt", "agent-1");

            Assert.Equal("FILE_LOCK_TIMEOUT", ex.ErrorCode);
            Assert.Equal("/tmp/file.txt", ex.FilePath);
            Assert.Equal("agent-1", ex.RequestedByAgentId);
            Assert.Equal("agent-1", ex.Context["agent_id"]);
        }

        [Fact]
        public void CopilotApiException_SetsErrorCode()
        {
            var ex = new CopilotApiException("copilot error");

            Assert.Equal("COPILOT_API_ERROR", ex.ErrorCode);
        }

        [Fact]
        public void MeetingLimitExceededException_SetsLimitInfo()
        {
            var ex = new MeetingLimitExceededException("Messages", 10);

            Assert.Equal("MEETING_LIMIT_EXCEEDED", ex.ErrorCode);
            Assert.Equal("Messages", ex.LimitType);
            Assert.Equal(10, ex.LimitValue);
            Assert.Equal("Messages", ex.Context["limit_type"]);
            Assert.Equal(10, ex.Context["limit_value"]);
        }

        [Fact]
        public void AgentMeetingException_AddContext_StoresValue()
        {
            var ex = new MeetingConfigurationException("bad config");

            ex.AddContext("key", 123);

            Assert.Equal(123, ex.Context["key"]);
        }
    }
}
