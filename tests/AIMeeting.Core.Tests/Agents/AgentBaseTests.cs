using AIMeeting.Core.Agents;
using AIMeeting.Core.Configuration;
using AIMeeting.Core.Models;
using Xunit;

namespace AIMeeting.Core.Tests.Agents
{
    public class AgentBaseTests
    {
        [Fact]
        public async Task InitializeAsync_DefaultCompletes()
        {
            var agent = new TestAgent("agent-1", new AgentConfiguration
            {
                Role = "Developer",
                Description = "Writes code",
                Instructions = ["Be concise"]
            });

            await agent.InitializeAsync(new MeetingContext
            {
                MeetingId = "id",
                Topic = "topic",
                Messages = new List<Message>(),
                Agents = new Dictionary<string, object>()
            }, CancellationToken.None);
        }

        [Fact]
        public async Task ShouldParticipateAsync_DefaultReturnsTrue()
        {
            var agent = new TestAgent("agent-1", new AgentConfiguration
            {
                Role = "Developer",
                Description = "Writes code",
                Instructions = ["Be concise"]
            });

            var shouldParticipate = await agent.ShouldParticipateAsync(new MeetingContext
            {
                MeetingId = "id",
                Topic = "topic",
                Messages = new List<Message>(),
                Agents = new Dictionary<string, object>()
            });

            Assert.True(shouldParticipate);
        }

        private sealed class TestAgent : AgentBase
        {
            public TestAgent(string agentId, AgentConfiguration configuration)
            {
                AgentId = agentId;
                Configuration = configuration;
            }

            protected override Task<string> GenerateResponseAsync(MeetingContext context, CancellationToken cancellationToken)
            {
                return Task.FromResult("ok");
            }
        }
    }
}
