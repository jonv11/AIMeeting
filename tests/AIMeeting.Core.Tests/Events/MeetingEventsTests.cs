using AIMeeting.Core.Events;
using AIMeeting.Core.Models;
using Xunit;

namespace AIMeeting.Core.Tests.Events
{
    public class MeetingEventsTests
    {
        [Fact]
        public void EventProperties_AreSettable()
        {
            var started = new MeetingStartedEvent
            {
                Configuration = new MeetingConfiguration { MeetingTopic = "Topic" }
            };
            var ending = new MeetingEndingEvent { Reason = "Done" };
            var ended = new MeetingEndedEvent { EndReason = "Completed" };
            var turnStarted = new TurnStartedEvent { AgentId = "agent-1", TurnNumber = 1 };
            var turnCompleted = new TurnCompletedEvent { AgentId = "agent-1", Message = "Hi" };
            var turnSkipped = new TurnSkippedEvent { AgentId = "agent-2", Reason = "Skipped" };
            var agentJoined = new AgentJoinedEvent { AgentId = "agent-1" };
            var agentReady = new AgentReadyEvent { AgentId = "agent-1" };
            var agentLeft = new AgentLeftEvent { AgentId = "agent-1" };
            var fileLocked = new FileLockedEvent { AgentId = "agent-1", FilePath = "file.txt" };
            var fileUnlocked = new FileUnlockedEvent { FilePath = "file.txt" };
            var fileCreated = new FileCreatedEvent { AgentId = "system", FilePath = "file.txt" };
            var fileModified = new FileModifiedEvent { AgentId = "system", FilePath = "file.txt" };

            Assert.Equal("Topic", started.Configuration.MeetingTopic);
            Assert.Equal("Done", ending.Reason);
            Assert.Equal("Completed", ended.EndReason);
            Assert.Equal("agent-1", turnStarted.AgentId);
            Assert.Equal(1, turnStarted.TurnNumber);
            Assert.Equal("Hi", turnCompleted.Message);
            Assert.Equal("Skipped", turnSkipped.Reason);
            Assert.Equal("agent-1", agentJoined.AgentId);
            Assert.Equal("agent-1", agentReady.AgentId);
            Assert.Equal("agent-1", agentLeft.AgentId);
            Assert.Equal("file.txt", fileLocked.FilePath);
            Assert.Equal("file.txt", fileUnlocked.FilePath);
            Assert.Equal("file.txt", fileCreated.FilePath);
            Assert.Equal("file.txt", fileModified.FilePath);

            Assert.True(started.Timestamp <= DateTime.UtcNow);
        }
    }
}
