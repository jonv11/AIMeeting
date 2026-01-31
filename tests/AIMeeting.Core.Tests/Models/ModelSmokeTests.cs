using AIMeeting.Core.Models;
using Xunit;

namespace AIMeeting.Core.Tests.Models
{
    public class ModelSmokeTests
    {
        [Fact]
        public void MeetingConfiguration_StoresValues()
        {
            var config = new MeetingConfiguration
            {
                MeetingId = "meeting-1",
                MeetingTopic = "Topic",
                OutputDirectory = "out"
            };

            config.AgentConfigPaths.Add("agent.txt");

            Assert.Equal("meeting-1", config.MeetingId);
            Assert.Equal("Topic", config.MeetingTopic);
            Assert.Equal("out", config.OutputDirectory);
            Assert.Single(config.AgentConfigPaths);
        }

        [Fact]
        public void MeetingResult_StoresErrorLogPath()
        {
            var result = new MeetingResult
            {
                MeetingId = "meeting-1",
                State = MeetingState.Completed,
                EndReason = "done",
                ErrorLogPath = "errors.log"
            };

            Assert.Equal("errors.log", result.ErrorLogPath);
        }

        [Fact]
        public void Message_StoresReplyToMessageId()
        {
            var message = new Message
            {
                MessageId = "msg-1",
                ReplyToMessageId = "msg-0",
                AgentId = "agent-1",
                Content = "hello",
                Timestamp = DateTime.UtcNow,
                Type = MessageType.Statement
            };

            Assert.Equal("msg-0", message.ReplyToMessageId);
        }
    }
}
