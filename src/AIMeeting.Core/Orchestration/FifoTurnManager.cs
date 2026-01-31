namespace AIMeeting.Core.Orchestration
{
    /// <summary>
    /// FIFO (First-In-First-Out) turn manager for agents.
    /// Agents take turns in a fixed order, cycling back to the beginning.
    /// </summary>
    public class FifoTurnManager : ITurnManager
    {
        private readonly Queue<string> _agents = new();
        private readonly object _lock = new();

        /// <summary>
        /// Registers an agent with the turn manager.
        /// </summary>
        public void RegisterAgent(string agentId)
        {
            if (string.IsNullOrWhiteSpace(agentId))
                throw new ArgumentException("Agent ID cannot be null or empty", nameof(agentId));

            lock (_lock)
            {
                // Avoid duplicate registrations
                if (!_agents.Contains(agentId))
                {
                    _agents.Enqueue(agentId);
                }
            }
        }

        /// <summary>
        /// Removes an agent from the turn queue.
        /// </summary>
        public void UnregisterAgent(string agentId)
        {
            if (string.IsNullOrWhiteSpace(agentId))
                return;

            lock (_lock)
            {
                // Remove the agent by creating a new queue without it
                var remaining = _agents.Where(a => a != agentId).ToList();
                _agents.Clear();
                foreach (var agent in remaining)
                {
                    _agents.Enqueue(agent);
                }
            }
        }

        /// <summary>
        /// Gets the next agent to speak.
        /// Cycles back to the beginning after reaching the end.
        /// </summary>
        public string GetNextAgent()
        {
            lock (_lock)
            {
                if (_agents.Count == 0)
                    throw new InvalidOperationException("No agents available");

                // Dequeue the next agent and re-queue it for the next cycle
                var agent = _agents.Dequeue();
                _agents.Enqueue(agent);
                return agent;
            }
        }

        /// <summary>
        /// Gets whether there are agents remaining.
        /// </summary>
        public bool HasAgents
        {
            get
            {
                lock (_lock)
                {
                    return _agents.Count > 0;
                }
            }
        }
    }
}
