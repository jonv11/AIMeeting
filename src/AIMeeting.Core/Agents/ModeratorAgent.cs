namespace AIMeeting.Core.Agents
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Prompts;

    /// <summary>
    /// Special agent that manages meeting flow and ensures productive discussion.
    /// The moderator speaks last in each round to summarize and redirect.
    /// </summary>
    public class ModeratorAgent : AgentBase
    {
        private readonly IPromptBuilder _promptBuilder;
        private dynamic? _copilotClient;

        /// <summary>
        /// Initializes a new instance of the ModeratorAgent class.
        /// </summary>
        /// <param name="agentId">Unique identifier for the agent (typically "moderator")</param>
        /// <param name="configuration">Agent configuration for the moderator</param>
        /// <param name="promptBuilder">Prompt builder for generating prompts</param>
        /// <param name="copilotClient">Optional Copilot client for real responses</param>
        public ModeratorAgent(
            string agentId,
            AgentConfiguration configuration,
            IPromptBuilder promptBuilder,
            dynamic? copilotClient = null)
        {
            AgentId = agentId;
            Configuration = configuration;
            _promptBuilder = promptBuilder ?? throw new ArgumentNullException(nameof(promptBuilder));
            _copilotClient = copilotClient;
        }

        /// <summary>
        /// Determines if the moderator should speak in this turn.
        /// Always participates after other agents have spoken.
        /// </summary>
        public override async Task<bool> ShouldParticipateAsync(MeetingContext context)
        {
            // For now, moderator always participates
            // Future enhancement: Skip if very few messages or no new contributions
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Generates a response that summarizes and guides the discussion.
        /// </summary>
        protected override async Task<string> GenerateResponseAsync(
            MeetingContext context,
            CancellationToken cancellationToken)
        {
            // Check if in stub mode (for testing)
            var agentMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE")?.ToLowerInvariant();
            if (agentMode == "stub")
            {
                return GenerateStubResponse(context);
            }

            // If no Copilot client, use stub mode
            if (_copilotClient == null)
            {
                return GenerateStubResponse(context);
            }

            try
            {
                // Build prompt for moderator
                var prompt = _promptBuilder.BuildModeratorPrompt(Configuration, context);

                // Get response from Copilot
                var response = await _copilotClient.GenerateAsync(
                    prompt,
                    null,
                    cancellationToken);

                // Respect max message length if configured
                if (Configuration.MaxMessageLength > 0 && response.Length > Configuration.MaxMessageLength)
                {
                    response = response[..Configuration.MaxMessageLength].TrimEnd();
                }

                return response;
            }
            catch (Exception)
            {
                // Fall back to stub on error
                return GenerateStubResponse(context);
            }
        }

        /// <summary>
        /// Generates a deterministic stub response for testing.
        /// </summary>
        private string GenerateStubResponse(MeetingContext context)
        {
            var agentCount = context.Agents.Count;
            var messageCount = context.Messages.Count;

            return $"[Moderator Summary] We've heard from {agentCount} perspectives over {messageCount} messages. " +
                   $"Key themes include collaboration, careful consideration, and teamwork. " +
                   $"Let's continue to build on these insights.";
        }
    }
}
