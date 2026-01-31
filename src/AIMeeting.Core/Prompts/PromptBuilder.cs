namespace AIMeeting.Core.Prompts
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;
    using System.Text;

    /// <summary>
    /// Default implementation of IPromptBuilder.
    /// Builds context-aware prompts including role, persona, instructions, and recent discussion.
    /// </summary>
    public class PromptBuilder : IPromptBuilder
    {
        private const int MaxRecentMessages = 10;

        /// <summary>
        /// Builds a prompt for a standard agent's turn.
        /// </summary>
        public string BuildAgentPrompt(
            AgentConfiguration agentConfig,
            MeetingContext context)
        {
            if (agentConfig == null)
                throw new ArgumentNullException(nameof(agentConfig));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sb = new StringBuilder();

            // Header with meeting context
            sb.AppendLine("# Meeting Discussion");
            sb.AppendLine($"**Topic:** {context.Topic}");
            sb.AppendLine($"**Participants:** {string.Join(", ", context.Agents.Keys)}");
            sb.AppendLine();

            // Role definition
            sb.AppendLine($"## Your Role: {agentConfig.Role}");
            sb.AppendLine(agentConfig.Description);
            sb.AppendLine();

            // Persona traits
            if (agentConfig.PersonaTraits.Count > 0)
            {
                sb.AppendLine("## Your Persona:");
                foreach (var trait in agentConfig.PersonaTraits)
                {
                    sb.AppendLine($"- {trait}");
                }
                sb.AppendLine();
            }

            // Instructions
            if (agentConfig.Instructions.Count > 0)
            {
                sb.AppendLine("## Your Instructions:");
                foreach (var instruction in agentConfig.Instructions)
                {
                    sb.AppendLine($"- {instruction}");
                }
                sb.AppendLine();
            }

            // Expertise areas
            if (agentConfig.ExpertiseAreas.Count > 0)
            {
                sb.AppendLine("## Your Expertise:");
                sb.AppendLine(string.Join(", ", agentConfig.ExpertiseAreas));
                sb.AppendLine();
            }

            // Recent discussion (sliding window)
            sb.AppendLine("## Recent Discussion:");
            var recentMessages = context.Messages.TakeLast(MaxRecentMessages).ToList();

            if (recentMessages.Count == 0)
            {
                sb.AppendLine("(No messages yet - you are starting the discussion)");
            }
            else
            {
                foreach (var msg in recentMessages)
                {
                    var agentName = context.Agents.TryGetValue(msg.AgentId, out var agent)
                        ? (agent as dynamic)?.RoleName ?? msg.AgentId
                        : msg.AgentId;

                    sb.AppendLine($"**{agentName}:** {msg.Content}");
                }
            }

            sb.AppendLine();

            // Response guidance
            sb.AppendLine("## Your Response");
            sb.AppendLine($"Provide your next contribution to the discussion.");
            if (agentConfig.MaxMessageLength > 0)
            {
                sb.AppendLine($"**Limit: {agentConfig.MaxMessageLength} characters**");
            }

            // Communication style if specified
            if (!string.IsNullOrWhiteSpace(agentConfig.ResponseStyle))
            {
                sb.AppendLine($"**Style:** {agentConfig.ResponseStyle}");
            }

            sb.AppendLine();
            sb.AppendLine("Respond directly with your contribution:");

            return sb.ToString();
        }

        /// <summary>
        /// Builds a prompt for the moderator agent.
        /// </summary>
        public string BuildModeratorPrompt(
            AgentConfiguration moderatorConfig,
            MeetingContext context)
        {
            if (moderatorConfig == null)
                throw new ArgumentNullException(nameof(moderatorConfig));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sb = new StringBuilder();

            sb.AppendLine("# Moderator Summary");
            sb.AppendLine($"**Topic:** {context.Topic}");
            sb.AppendLine($"**Participants:** {string.Join(", ", context.Agents.Keys)}");
            sb.AppendLine($"**Messages so far:** {context.Messages.Count}");
            sb.AppendLine();

            sb.AppendLine("## Your Role: Moderator");
            sb.AppendLine(moderatorConfig.Description);
            sb.AppendLine();

            // Recent discussion
            sb.AppendLine("## Discussion Summary:");
            var recentMessages = context.Messages.TakeLast(MaxRecentMessages).ToList();

            if (recentMessages.Count == 0)
            {
                sb.AppendLine("(No messages yet)");
            }
            else
            {
                foreach (var msg in recentMessages)
                {
                    var agentName = context.Agents.TryGetValue(msg.AgentId, out var agent)
                        ? (agent as dynamic)?.RoleName ?? msg.AgentId
                        : msg.AgentId;

                    sb.AppendLine($"**{agentName}:** {msg.Content}");
                }
            }

            sb.AppendLine();

            // Moderator guidance
            sb.AppendLine("## As Moderator, Consider:");
            sb.AppendLine("- Summarize key points discussed");
            sb.AppendLine("- Identify areas of agreement and disagreement");
            sb.AppendLine("- Suggest next steps or clarifying questions");
            sb.AppendLine("- Keep the discussion focused on the topic");
            sb.AppendLine("- Ensure all perspectives have been heard");
            sb.AppendLine();

            sb.AppendLine("## Your Summary");
            if (moderatorConfig.MaxMessageLength > 0)
            {
                sb.AppendLine($"**Limit: {moderatorConfig.MaxMessageLength} characters**");
            }

            sb.AppendLine();
            sb.AppendLine("Provide your moderator summary:");

            return sb.ToString();
        }
    }
}
