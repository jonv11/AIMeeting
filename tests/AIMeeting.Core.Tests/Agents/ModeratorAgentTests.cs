using AIMeeting.Core.Agents;
using AIMeeting.Core.Configuration;
using AIMeeting.Core.Models;
using AIMeeting.Core.Prompts;
using Xunit;

namespace AIMeeting.Core.Tests.Agents
{
    public class ModeratorAgentTests
    {
        [Fact]
        public async Task RespondAsync_UsesStubMode_WhenEnabled()
        {
            var originalMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE");
            Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", "stub");

            try
            {
                var config = new AgentConfiguration
                {
                    Role = "Moderator",
                    Description = "Guides the discussion",
                    Instructions = ["Summarize"]
                };

                var agent = new ModeratorAgent("moderator", config, new PromptBuilder(), null);
                var context = new MeetingContext
                {
                    MeetingId = "id",
                    Topic = "Test topic",
                    Messages = new List<Message>(),
                    Agents = new Dictionary<string, object>()
                };

                var response = await agent.RespondAsync(context, CancellationToken.None);

                Assert.Contains("Moderator Summary", response.Content);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AIMEETING_AGENT_MODE", originalMode);
            }
        }

        [Fact]
        public async Task ShouldParticipateAsync_AlwaysReturnsTrue()
        {
            var config = new AgentConfiguration
            {
                Role = "Moderator",
                Description = "Guides the discussion",
                Instructions = ["Summarize"]
            };

            var agent = new ModeratorAgent("moderator", config, new PromptBuilder(), null);
            var context = new MeetingContext
            {
                MeetingId = "id",
                Topic = "Test topic",
                Messages = new List<Message>(),
                Agents = new Dictionary<string, object>()
            };

            var shouldParticipate = await agent.ShouldParticipateAsync(context);

            Assert.True(shouldParticipate);
        }
    }
}
