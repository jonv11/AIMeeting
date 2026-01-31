namespace AIMeeting.Core.Agents
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Prompts;

    /// <summary>
    /// Standard agent that generates responses using configured behavior.
    /// Uses Copilot CLI or stub mode based on configuration.
    /// </summary>
    public class StandardAgent : AgentBase
    {
        private readonly IPromptBuilder _promptBuilder;
        private dynamic? _copilotClient; // Use dynamic to avoid hard dependency

        /// <summary>
        /// Initializes a new instance of the StandardAgent class.
        /// </summary>
        /// <param name="agentId">Unique identifier for the agent</param>
        /// <param name="configuration">Agent configuration</param>
        /// <param name="promptBuilder">Prompt builder for generating prompts</param>
        /// <param name="copilotClient">Optional Copilot client for real responses</param>
        public StandardAgent(
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
        /// Generates a response for this agent's turn.
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
                // Build prompt for this agent
                var prompt = _promptBuilder.BuildAgentPrompt(Configuration, context);

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
            var turnCount = context.Messages.Count + 1;
            var perspective = Configuration.PersonaTraits.FirstOrDefault() ?? "practical perspective";

            return $"[{RoleName} - Turn {turnCount}] " +
                   $"From my {perspective}, regarding '{context.Topic}': " +
                   $"I think we should consider the implications carefully and collaborate with the team.";
        }
    }
}
