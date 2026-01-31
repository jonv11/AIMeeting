namespace AIMeeting.Core.Agents
{
    using AIMeeting.Core.Configuration;
    using AIMeeting.Core.Models;

    /// <summary>
    /// Base class for implementing agents in a meeting.
    /// </summary>
    public abstract class AgentBase : IAgent
    {
        /// <summary>
        /// Unique identifier for this agent.
        /// </summary>
        public string AgentId { get; protected set; } = null!;

        /// <summary>
        /// The agent's role (e.g., "Project Manager").
        /// </summary>
        public string RoleName => Configuration.Role;

        /// <summary>
        /// The agent's configuration.
        /// </summary>
        public AgentConfiguration Configuration { get; protected set; } = null!;

        /// <summary>
        /// Initializes the agent for a meeting.
        /// </summary>
        public virtual async Task InitializeAsync(
            MeetingContext context,
            CancellationToken cancellationToken)
        {
            // Default implementation - subclasses can override for custom initialization
            await Task.CompletedTask;
        }

        /// <summary>
        /// Generates the agent's response to the current discussion.
        /// </summary>
        public async Task<AgentResponse> RespondAsync(
            MeetingContext context,
            CancellationToken cancellationToken)
        {
            var content = await GenerateResponseAsync(context, cancellationToken);
            return new AgentResponse
            {
                Content = content,
                Type = MessageType.Statement
            };
        }

        /// <summary>
        /// Determines if the agent should participate in the next turn.
        /// </summary>
        public virtual async Task<bool> ShouldParticipateAsync(MeetingContext context)
        {
            // Default implementation - agent always participates
            // Subclasses can override for conditional participation
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Generates the response content for this agent's turn.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract Task<string> GenerateResponseAsync(
            MeetingContext context,
            CancellationToken cancellationToken);
    }
}
