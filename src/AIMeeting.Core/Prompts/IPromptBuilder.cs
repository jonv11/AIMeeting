namespace AIMeeting.Core.Prompts
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;
    using System.Text;

    /// <summary>
    /// Interface for building context-aware prompts for agents.
    /// </summary>
    public interface IPromptBuilder
    {
        /// <summary>
        /// Builds a prompt for a standard agent's turn.
        /// </summary>
        /// <param name="agentConfig">The agent's configuration</param>
        /// <param name="context">The current meeting context</param>
        /// <returns>The formatted prompt</returns>
        string BuildAgentPrompt(
            AgentConfiguration agentConfig,
            MeetingContext context);

        /// <summary>
        /// Builds a prompt for the moderator agent.
        /// </summary>
        /// <param name="moderatorConfig">The moderator's configuration</param>
        /// <param name="context">The current meeting context</param>
        /// <returns>The formatted prompt for the moderator</returns>
        string BuildModeratorPrompt(
            AgentConfiguration moderatorConfig,
            MeetingContext context);
    }
}
