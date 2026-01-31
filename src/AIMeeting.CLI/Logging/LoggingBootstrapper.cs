using Serilog;

namespace AIMeeting.CLI.Logging
{
    /// <summary>
    /// Initializes Serilog logging for the CLI.
    /// </summary>
    public static class LoggingBootstrapper
    {
        private const string DefaultLogDirectory = "logs";
        private const string DefaultLogFileName = "meeting-.log";
        private const string ConsoleTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Configure Serilog with console and rolling file sinks.
        /// </summary>
        /// <param name="logDirectory">Optional log directory. Defaults to "logs".</param>
        /// <returns>Resolved log file path pattern.</returns>
        public static string Configure(string? logDirectory = null)
        {
            logDirectory ??= DefaultLogDirectory;
            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(logDirectory, DefaultLogFileName);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: ConsoleTemplate)
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Logging initialized");
            return logPath;
        }
    }
}
