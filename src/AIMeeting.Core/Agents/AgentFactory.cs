namespace AIMeeting.Core.Agents
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Prompts;
    using AIMeeting.Core.Orchestration;

    /// <summary>
    /// Default implementation of IAgentFactory.
    /// Creates StandardAgent and ModeratorAgent instances from configurations.
    /// </summary>
    public class AgentFactory : IAgentFactory
    {
        private readonly AgentConfigurationParser _configParser;
        private readonly AgentConfigurationValidator _configValidator;
        private readonly IPromptBuilder _promptBuilder;
        private readonly dynamic? _copilotClient;

        /// <summary>
        /// Initializes a new instance of the AgentFactory class.
        /// </summary>
        /// <param name="configParser">Configuration parser</param>
        /// <param name="configValidator">Configuration validator</param>
        /// <param name="promptBuilder">Prompt builder for agents</param>
        /// <param name="copilotClient">Optional Copilot client (uses stub mode if not provided)</param>
        public AgentFactory(
            AgentConfigurationParser configParser,
            AgentConfigurationValidator configValidator,
            IPromptBuilder promptBuilder,
            dynamic? copilotClient = null)
        {
            _configParser = configParser ?? throw new ArgumentNullException(nameof(configParser));
            _configValidator = configValidator ?? throw new ArgumentNullException(nameof(configValidator));
            _promptBuilder = promptBuilder ?? throw new ArgumentNullException(nameof(promptBuilder));
            _copilotClient = copilotClient;
        }

        /// <summary>
        /// Creates an agent from a configuration path.
        /// </summary>
        public async Task<IAgent> CreateAgentAsync(
            string configPath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException("Config path cannot be null or empty", nameof(configPath));

            // Parse the configuration
            var parseResult = await _configParser.ParseAsync(configPath, cancellationToken);

            if (!parseResult.IsSuccess)
            {
                var fatalErrors = parseResult.Errors.Where(e => !e.IsWarning);
                var errors = string.Join("; ", fatalErrors.Select(e => e.ToString()));
                throw new InvalidOperationException(
                    $"Agent configuration parsing failed: {errors}");
            }

            var config = parseResult.Configuration;

            // Validate the configuration
            var validationResult = _configValidator.Validate(config, parseResult.Errors);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors);
                throw new InvalidOperationException(
                    $"Agent configuration validation failed: {errors}");
            }

            // Generate agent ID from config (role-based)
            var agentId = GenerateAgentId(config);

            // Create the appropriate agent type
            // Check if this is a moderator agent by role name
            if (config.Role.Equals("Moderator", StringComparison.OrdinalIgnoreCase))
            {
                return new ModeratorAgent(agentId, config, _promptBuilder, _copilotClient);
            }

            return new StandardAgent(agentId, config, _promptBuilder, _copilotClient);
        }

        /// <summary>
        /// Creates multiple agents from configuration paths.
        /// </summary>
        public async Task<Dictionary<string, IAgent>> CreateAgentsAsync(
            IEnumerable<string> configPaths,
            CancellationToken cancellationToken = default)
        {
            var agents = new Dictionary<string, IAgent>();
            var paths = configPaths?.ToList() ?? new List<string>();

            foreach (var configPath in paths)
            {
                var agent = await CreateAgentAsync(configPath, cancellationToken);
                agents[agent.AgentId] = agent;
            }

            return agents;
        }

        /// <summary>
        /// Detects and creates an orchestrator from the list of configuration paths.
        /// Returns null if no orchestrator configuration is found.
        /// </summary>
        /// <param name="configPaths">List of configuration file paths</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Orchestrator instance or null</returns>
        public async Task<IOrchestratorDecisionMaker?> DetectOrchestratorAsync(
            IEnumerable<string> configPaths,
            CancellationToken cancellationToken = default)
        {
            var paths = configPaths?.ToList() ?? new List<string>();
            
            foreach (var configPath in paths)
            {
                // Parse the configuration
                var parseResult = await _configParser.ParseAsync(configPath, cancellationToken);
                
                if (!parseResult.IsSuccess)
                {
                    continue; // Skip invalid configs
                }
                
                var config = parseResult.Configuration;
                
                // Check if this is an orchestrator by role
                if (config.Role.Equals("Orchestrator", StringComparison.OrdinalIgnoreCase))
                {
                    // Validate the configuration
                    var validationResult = _configValidator.Validate(config, parseResult.Errors);
                    if (!validationResult.IsValid)
                    {
                        var errors = string.Join("; ", validationResult.Errors);
                        throw new InvalidOperationException(
                            $"Orchestrator configuration validation failed: {errors}");
                    }
                    
                    // Generate orchestrator ID
                    var orchestratorId = GenerateAgentId(config);
                    
                    // Create and return AIOrchestrator
                    return new AIOrchestrator(orchestratorId, _promptBuilder, _copilotClient);
                }
            }

            // No orchestrator found
            return null;
        }

        /// <summary>
        /// Creates a moderator agent with the given configuration.
        /// </summary>
        public IAgent CreateModeratorAgent(AgentConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var agentId = "moderator";
            return new ModeratorAgent(agentId, configuration, _promptBuilder, _copilotClient);
        }

        /// <summary>
        /// Generates a unique agent ID from a configuration.
        /// </summary>
        private string GenerateAgentId(AgentConfiguration config)
        {
            // Generate ID from role, making it lowercase and replacing spaces with hyphens
            var baseId = config.Role
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-");

            // Remove any special characters
            var id = new string(baseId
                .Where(c => char.IsLetterOrDigit(c) || c == '-')
                .ToArray());

            return id;
        }
    }
}
