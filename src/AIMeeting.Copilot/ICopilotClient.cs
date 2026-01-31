namespace AIMeeting.Copilot
{
    /// <summary>
    /// Options for Copilot API requests.
    /// </summary>
    public class CopilotOptions
    {
        /// <summary>
        /// Maximum time to wait for a response (default: 30 seconds).
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Maximum length of the response (optional).
        /// </summary>
        public int? MaxResponseLength { get; set; }
    }

    /// <summary>
    /// Interface for interacting with GitHub Copilot CLI.
    /// </summary>
    public interface ICopilotClient : IAsyncDisposable
    {
        /// <summary>
        /// Initializes the Copilot connection.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Closes the Copilot connection.
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Generates a completion for the given prompt.
        /// </summary>
        /// <param name="prompt">The prompt to send to Copilot</param>
        /// <param name="options">Request options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The generated response</returns>
        Task<string> GenerateAsync(
            string prompt,
            CopilotOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets whether the Copilot client is currently connected.
        /// </summary>
        bool IsConnected { get; }
    }
}
