namespace AIMeeting.Copilot
{
    using AIMeeting.Core.Exceptions;
    using GitHub.Copilot.SDK;

    /// <summary>
    /// Implementation of ICopilotClient that uses the GitHub Copilot SDK for .NET.
    /// </summary>
    public class CopilotClient : ICopilotClient
    {
        private GitHub.Copilot.SDK.CopilotClient? _sdkClient;
        private CopilotSession? _session;
        private bool _isConnected;
        private readonly object _lock = new();

        /// <summary>
        /// Gets whether the Copilot client is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (_lock)
                {
                    return _isConnected;
                }
            }
        }

        /// <summary>
        /// Initializes the Copilot connection using the GitHub Copilot SDK.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (_isConnected)
                    return;
            }

            try
            {
                // Create the Copilot SDK client
                _sdkClient = new GitHub.Copilot.SDK.CopilotClient(new CopilotClientOptions
                {
                    LogLevel = "error", // Minimize SDK logging
                    AutoStart = true
                });

                // Start the SDK client (this starts the copilot CLI in server mode)
                await _sdkClient.StartAsync(cancellationToken);

                // Create a persistent session for all requests
                _session = await _sdkClient.CreateSessionAsync(new SessionConfig
                {
                    Model = "gpt-4.1", // Default model
                    Streaming = false // We'll handle responses synchronously
                }, cancellationToken);

                lock (_lock)
                {
                    _isConnected = true;
                }
            }
            catch (Exception ex)
            {
                throw new CopilotApiException(
                    $"Failed to initialize Copilot SDK: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        /// Closes the Copilot connection.
        /// </summary>
        public async Task StopAsync()
        {
            lock (_lock)
            {
                if (!_isConnected)
                    return;
            }

            try
            {
                if (_session != null)
                {
                    await _session.DisposeAsync();
                    _session = null;
                }

                if (_sdkClient != null)
                {
                    await _sdkClient.StopAsync();
                    _sdkClient.Dispose();
                    _sdkClient = null;
                }

                lock (_lock)
                {
                    _isConnected = false;
                }
            }
            catch (Exception)
            {
                // Suppress exceptions during cleanup
                lock (_lock)
                {
                    _isConnected = false;
                }
            }
        }

        /// <summary>
        /// Generates a completion for the given prompt using the GitHub Copilot SDK.
        /// </summary>
        public async Task<string> GenerateAsync(
            string prompt,
            CopilotOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            if (!IsConnected || _session == null)
                throw new CopilotApiException("Copilot client is not connected. Call StartAsync first.");

            options ??= new CopilotOptions();

            try
            {
                // Use a timeout cancellation token
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(options.Timeout);

                // Send the message and wait for the response
                var response = await _session.SendAndWaitAsync(
                    new MessageOptions { Prompt = prompt },
                    options.Timeout,
                    cts.Token);

                var result = response?.Data.Content ?? string.Empty;

                // Apply max length if specified
                if (options.MaxResponseLength.HasValue && result.Length > options.MaxResponseLength.Value)
                {
                    result = result[..options.MaxResponseLength.Value];
                }

                return result;
            }
            catch (OperationCanceledException)
            {
                throw new CopilotApiException(
                    $"Copilot request timed out after {options.Timeout.TotalSeconds:F1} seconds");
            }
            catch (Exception ex) when (!(ex is CopilotApiException))
            {
                throw new CopilotApiException(
                    $"Failed to generate response from Copilot: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        /// Disposes the client and releases resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            GC.SuppressFinalize(this);
        }
    }
}
