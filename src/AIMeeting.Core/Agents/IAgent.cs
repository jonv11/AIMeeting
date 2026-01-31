namespace AIMeeting.Core.Agents
{
    using AIMeeting.Core.Models;

    /// <summary>
    /// Response from an agent for a turn.
    /// </summary>
    public class AgentResponse
    {
        /// <summary>
        /// The response content.
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// Type of message (Statement, Question, Response).
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Statement;
    }

    /// <summary>
    /// Interface for individual agents in a meeting.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Unique identifier for this agent.
        /// </summary>
        string AgentId { get; }

        /// <summary>
        /// The agent's role (e.g., "Project Manager").
        /// </summary>
        string RoleName { get; }

        /// <summary>
        /// Initializes the agent for a meeting.
        /// </summary>
        Task InitializeAsync(
            MeetingContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Generates the agent's response to the current discussion.
        /// </summary>
        Task<AgentResponse> RespondAsync(
            MeetingContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Determines if the agent should participate in the next turn.
        /// </summary>
        Task<bool> ShouldParticipateAsync(MeetingContext context);
    }
}
