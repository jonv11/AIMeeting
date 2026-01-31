using System.IO;
using AIMeeting.Core.Agents;
using AIMeeting.Core.Configuration;
using AIMeeting.Core.Prompts;
using Xunit;

namespace AIMeeting.Core.Tests.Agents
{
    public class AgentFactoryTests
    {
        [Fact]
        public void CreateModeratorAgent_NullConfig_Throws()
        {
            var factory = CreateFactory();

            Assert.Throws<ArgumentNullException>(() => factory.CreateModeratorAgent(null!));
        }

        [Fact]
        public void CreateModeratorAgent_ReturnsModeratorAgent()
        {
            var factory = CreateFactory();
            var config = new AgentConfiguration
            {
                Role = "Moderator",
                Description = "Guides the discussion",
                Instructions = ["Summarize"]
            };

            var agent = factory.CreateModeratorAgent(config);

            Assert.IsType<ModeratorAgent>(agent);
            Assert.Equal("moderator", agent.AgentId);
        }

        [Fact]
        public async Task CreateAgentAsync_ModeratorRole_ReturnsModeratorAgent()
        {
            var factory = CreateFactory();
            var configPath = WriteTempConfig("Moderator");

            try
            {
                var agent = await factory.CreateAgentAsync(configPath);

                Assert.IsType<ModeratorAgent>(agent);
                Assert.Equal("moderator", agent.AgentId);
            }
            finally
            {
                File.Delete(configPath);
            }
        }

        [Fact]
        public async Task CreateAgentAsync_StandardRole_ReturnsStandardAgent()
        {
            var factory = CreateFactory();
            var configPath = WriteTempConfig("QA Lead");

            try
            {
                var agent = await factory.CreateAgentAsync(configPath);

                Assert.IsType<StandardAgent>(agent);
                Assert.Equal("qa-lead", agent.AgentId);
            }
            finally
            {
                File.Delete(configPath);
            }
        }

        [Fact]
        public async Task CreateAgentsAsync_ReturnsAllAgents()
        {
            var factory = CreateFactory();
            var moderatorPath = WriteTempConfig("Moderator");
            var developerPath = WriteTempConfig("Developer");

            try
            {
                var agents = await factory.CreateAgentsAsync(new[] { moderatorPath, developerPath });

                Assert.Equal(2, agents.Count);
                Assert.Contains("moderator", agents.Keys);
                Assert.Contains("developer", agents.Keys);
            }
            finally
            {
                File.Delete(moderatorPath);
                File.Delete(developerPath);
            }
        }

        private static AgentFactory CreateFactory()
        {
            return new AgentFactory(
                new AgentConfigurationParser(),
                new AgentConfigurationValidator(),
                new PromptBuilder(),
                null);
        }

        private static string WriteTempConfig(string role)
        {
            var content = $@"ROLE: {role}
DESCRIPTION: Supports the meeting
INSTRUCTIONS:
- Provide concise input
";

            var path = Path.Combine(Path.GetTempPath(), $"agent-{Guid.NewGuid():N}.txt");
            File.WriteAllText(path, content);
            return path;
        }
    }
}
