using AIMeeting.Core.Configuration;
using AIMeeting.Core.Models;
using AIMeeting.Core.Prompts;
using Xunit;

namespace AIMeeting.Core.Tests.Prompts
{
    public class PromptBuilderTests
    {
        [Fact]
        public void BuildAgentPrompt_IncludesKeySections()
        {
            var builder = new PromptBuilder();
            var config = new AgentConfiguration
            {
                Role = "Developer",
                Description = "Writes code",
                PersonaTraits = ["Pragmatic"],
                Instructions = ["Be concise"],
                MaxMessageLength = 200
            };

            var context = new MeetingContext
            {
                MeetingId = "id",
                Topic = "Prompt test",
                Messages =
                [
                    new Message
                    {
                        AgentId = "agent-1",
                        Content = "Hello",
                        Timestamp = DateTime.UtcNow,
                        Type = MessageType.Statement
                    }
                ],
                Agents = new Dictionary<string, object>
                {
                    { "agent-1", new StubAgentInfo { RoleName = "Developer" } }
                }
            };

            var prompt = builder.BuildAgentPrompt(config, context);

            Assert.Contains("Prompt test", prompt);
            Assert.Contains("Your Role", prompt);
            Assert.Contains("Your Instructions", prompt);
            Assert.Contains("Hello", prompt);
        }

        [Fact]
        public void BuildModeratorPrompt_IncludesSummaryAndGuidance()
        {
            var builder = new PromptBuilder();
            var config = new AgentConfiguration
            {
                Role = "Moderator",
                Description = "Guides the discussion",
                Instructions = ["Summarize"],
                MaxMessageLength = 150
            };

            var context = new MeetingContext
            {
                MeetingId = "id",
                Topic = "Moderator prompt test",
                Messages =
                [
                    new Message
                    {
                        AgentId = "agent-1",
                        Content = "Key point one",
                        Timestamp = DateTime.UtcNow,
                        Type = MessageType.Statement
                    }
                ],
                Agents = new Dictionary<string, object>
                {
                    { "agent-1", new StubAgentInfo { RoleName = "Developer" } }
                }
            };

            var prompt = builder.BuildModeratorPrompt(config, context);

            Assert.Contains("Moderator Summary", prompt);
            Assert.Contains("Messages so far", prompt);
            Assert.Contains("Key point one", prompt);
            Assert.Contains("Limit:", prompt);
        }

        public sealed class StubAgentInfo
        {
            public string RoleName { get; set; } = string.Empty;
        }
    }
}
