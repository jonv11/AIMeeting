namespace AIMeeting.Core.Agents
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Orchestration;

    /// <summary>
    /// Factory for creating agent instances from configuration.
    /// </summary>
    public interface IAgentFactory
    {
        /// <summary>
        /// Creates an agent from a configuration path.
        /// </summary>
        /// <param name="configPath">Path to the agent configuration file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created agent instance</returns>
        Task<IAgent> CreateAgentAsync(
            string configPath,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates multiple agents from configuration paths.
        /// </summary>
        /// <param name="configPaths">Paths to agent configuration files</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of AgentId to IAgent</returns>
        Task<Dictionary<string, IAgent>> CreateAgentsAsync(
            IEnumerable<string> configPaths,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Detects and creates an orchestrator from the list of configuration paths.
        /// Returns null if no orchestrator configuration is found.
        /// </summary>
        /// <param name="configPaths">Paths to agent configuration files</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Orchestrator instance or null</returns>
        Task<IOrchestratorDecisionMaker?> DetectOrchestratorAsync(
            IEnumerable<string> configPaths,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a moderator agent with the given configuration.
        /// </summary>
        /// <param name="configuration">Moderator configuration</param>
        /// <returns>The moderator agent</returns>
        IAgent CreateModeratorAgent(AgentConfiguration configuration);
    }
}
