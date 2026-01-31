using AIMeeting.Core.Exceptions;
using Serilog;

namespace AIMeeting.CLI.Errors
{
    /// <summary>
    /// Maps known exceptions to user-friendly CLI messages.
    /// </summary>
    public static class CliErrorHandler
    {
        public static int HandleException(Exception ex)
        {
            if (ex == null)
            {
                Console.Error.WriteLine("Unexpected error: unknown failure");
                return 1;
            }

            Log.Error(ex, "CLI error");

            switch (ex)
            {
                case MeetingConfigurationException configException:
                    Console.Error.WriteLine($"Invalid configuration: {configException.Message}");
                    Console.Error.WriteLine($"Error code: {configException.ErrorCode}");
                    return 1;

                case AgentInitializationException initException:
                    Console.Error.WriteLine($"Failed to initialize agent {initException.AgentId}");
                    Console.Error.WriteLine($"Reason: {initException.Message}");
                    return 1;

                case CopilotApiException:
                    Console.Error.WriteLine("Failed to connect to Copilot CLI");
                    Console.Error.WriteLine("Make sure GitHub Copilot CLI is installed and in PATH");
                    return 1;

                case MeetingLimitExceededException limitException:
                    Console.WriteLine($"Meeting ended: {limitException.LimitType} limit reached");
                    return 0;

                case OperationCanceledException:
                    Console.WriteLine("Meeting cancelled by user");
                    return 1;

                case AgentMeetingException agentException:
                    Console.Error.WriteLine($"Meeting failed: {agentException.Message}");
                    return 1;

                default:
                    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                    return 1;
            }
        }
    }
}
