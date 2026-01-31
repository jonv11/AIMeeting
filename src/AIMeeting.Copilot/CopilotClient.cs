namespace AIMeeting.Copilot
{
    using System.Diagnostics;
    using System.Text;
    using AIMeeting.Core.Exceptions;

    /// <summary>
    /// Implementation of ICopilotClient that wraps GitHub Copilot CLI.
    /// </summary>
    public class CopilotClient : ICopilotClient
    {
        private Process? _process;
        private StreamWriter? _stdin;
        private StreamReader? _stdout;
        private bool _isConnected;
        private readonly object _lock = new();
        private const string CopilotExe = "gh";
        private const string CopilotSubcommand = "copilot";

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
        /// Initializes the Copilot connection.
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
                // Verify gh copilot is available
                var verifyProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = CopilotExe,
                        Arguments = $"{CopilotSubcommand} --version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                verifyProcess.Start();
                var exitCode = await Task.Run(() => 
                {
                    verifyProcess.WaitForExit(5000);
                    return verifyProcess.ExitCode;
                }, cancellationToken);

                if (exitCode != 0)
                {
                    throw new CopilotApiException(
                        "GitHub Copilot CLI not found or not properly installed. " +
                        "Please visit https://github.com/github/copilot-cli for installation instructions.");
                }

                // Start the interactive copilot process
                _process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = CopilotExe,
                        Arguments = $"{CopilotSubcommand} -s",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                _process.Start();

                _stdin = _process.StandardInput;
                _stdout = _process.StandardOutput;

                lock (_lock)
                {
                    _isConnected = true;
                }
            }
            catch (Exception ex) when (!(ex is CopilotApiException))
            {
                throw new CopilotApiException(
                    $"Failed to initialize Copilot CLI: {ex.Message}",
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
                if (_stdin != null)
                {
                    _stdin.WriteLine("exit");
                    _stdin.Flush();
                    _stdin.Dispose();
                }

                if (_process != null)
                {
                    await Task.Run(() =>
                    {
                        _process.WaitForExit(5000);
                    });
                    _process.Dispose();
                }

                lock (_lock)
                {
                    _isConnected = false;
                    _process = null;
                    _stdin = null;
                    _stdout = null;
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
        /// Generates a completion for the given prompt.
        /// </summary>
        public async Task<string> GenerateAsync(
            string prompt,
            CopilotOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            if (!IsConnected)
                throw new CopilotApiException("Copilot client is not connected. Call StartAsync first.");

            options ??= new CopilotOptions();

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(options.Timeout);

                // Send prompt to copilot
                _stdin!.WriteLine(prompt);
                _stdin.WriteLine("---END---");
                _stdin.Flush();

                // Read response
                var response = new StringBuilder();
                string? line;

                while ((line = await _stdout!.ReadLineAsync(cts.Token)) != null)
                {
                    if (line == "---END---")
                        break;

                    if (response.Length > 0)
                        response.Append('\n');

                    response.Append(line);
                }

                var result = response.ToString();

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
