namespace AIMeeting.Core.Orchestration
{
    /// <summary>
    /// Interface for coordinating which agent speaks next.
    /// </summary>
    public interface ITurnManager
    {
        /// <summary>
        /// Registers an agent with the turn manager.
        /// </summary>
        /// <param name="agentId">The agent ID to register</param>
        void RegisterAgent(string agentId);

        /// <summary>
        /// Removes an agent from the turn queue.
        /// </summary>
        /// <param name="agentId">The agent ID to unregister</param>
        void UnregisterAgent(string agentId);

        /// <summary>
        /// Gets the next agent to speak.
        /// </summary>
        /// <returns>The agent ID of the next agent to speak</returns>
        string GetNextAgent();

        /// <summary>
        /// Gets whether there are agents remaining.
        /// </summary>
        bool HasAgents { get; }
    }
}
