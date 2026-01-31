namespace AIMeeting.Core.Configuration
{
    /// <summary>
    /// Represents an agent's configuration, parsed from a text file.
    /// </summary>
    public class AgentConfiguration
    {
        /// <summary>
        /// The agent's professional role (required).
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Description of what the agent does (required).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// List of persona traits or characteristics (optional, may be empty).
        /// </summary>
        public List<string> PersonaTraits { get; set; } = new();

        /// <summary>
        /// Instructions for how the agent should behave (required, at least one).
        /// </summary>
        public List<string> Instructions { get; set; } = new();

        /// <summary>
        /// Template for the agent's first message, may contain {topic} placeholder (optional).
        /// </summary>
        public string? InitialMessageTemplate { get; set; }

        /// <summary>
        /// Communication style description (optional).
        /// </summary>
        public string? ResponseStyle { get; set; }

        /// <summary>
        /// Maximum character length for responses (optional).
        /// </summary>
        public int? MaxMessageLength { get; set; }

        /// <summary>
        /// Comma-separated list of expertise areas (optional).
        /// </summary>
        public List<string> ExpertiseAreas { get; set; } = new();

        /// <summary>
        /// Guidance for how the agent communicates (optional).
        /// </summary>
        public string? CommunicationApproach { get; set; }

        /// <summary>
        /// Any unknown/custom headers found during parsing (for future extensibility).
        /// </summary>
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }
}
