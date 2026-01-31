using AIMeeting.Core.Events;
using AIMeeting.Core.Models;
using Spectre.Console;

namespace AIMeeting.CLI.Display
{
    /// <summary>
    /// Renders real-time meeting progress to the console.
    /// </summary>
    public class MeetingProgressDisplay
    {
        private readonly object _lock = new();
        private DateTime? _startedAt;
        private int _messageCount;
        private int? _maxMessages;
        private int? _maxDurationMinutes;

        public string? LastStatus { get; private set; }

        public void Attach(IEventBus eventBus, MeetingConfiguration configuration)
        {
            if (eventBus == null)
                throw new ArgumentNullException(nameof(eventBus));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _maxMessages = configuration.HardLimits?.MaxTotalMessages;
            _maxDurationMinutes = configuration.HardLimits?.MaxDurationMinutes;

            eventBus.Subscribe<MeetingStartedEvent>(async evt =>
            {
                lock (_lock)
                {
                    _startedAt = DateTime.UtcNow;
                }

                WriteLine($"Meeting started: {evt.Configuration.MeetingTopic}");
                await Task.CompletedTask;
            });

            eventBus.Subscribe<TurnStartedEvent>(async evt =>
            {
                WriteLine($"Turn {evt.TurnNumber} started by {evt.AgentId}. {BuildStatus()}");
                await Task.CompletedTask;
            });

            eventBus.Subscribe<TurnCompletedEvent>(async evt =>
            {
                lock (_lock)
                {
                    _messageCount++;
                }

                WriteLine($"Turn completed by {evt.AgentId} ({evt.Message.Length} chars). {BuildStatus()}");
                await Task.CompletedTask;
            });

            eventBus.Subscribe<MeetingEndedEvent>(async evt =>
            {
                WriteLine($"Meeting ended: {evt.EndReason}");
                await Task.CompletedTask;
            });
        }

        private string BuildStatus()
        {
            var parts = new List<string>();

            lock (_lock)
            {
                if (_maxDurationMinutes.HasValue && _startedAt.HasValue)
                {
                    var deadline = _startedAt.Value.AddMinutes(_maxDurationMinutes.Value);
                    var remaining = deadline - DateTime.UtcNow;
                    if (remaining < TimeSpan.Zero)
                        remaining = TimeSpan.Zero;

                    parts.Add($"Time remaining: {FormatDuration(remaining)}");
                }

                if (_maxMessages.HasValue)
                {
                    var remainingMessages = Math.Max(_maxMessages.Value - _messageCount, 0);
                    parts.Add($"Messages remaining: {remainingMessages}");
                }
            }

            var status = parts.Count == 0 ? "" : string.Join(" | ", parts);
            LastStatus = status;
            return status;
        }

        private static string FormatDuration(TimeSpan duration)
        {
            var minutes = (int)duration.TotalMinutes;
            var seconds = duration.Seconds;
            return $"{minutes:D2}:{seconds:D2}";
        }

        private static void WriteLine(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            AnsiConsole.MarkupLine(Markup.Escape(message));
        }
    }
}
