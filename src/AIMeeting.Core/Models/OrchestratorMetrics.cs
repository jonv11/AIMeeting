namespace AIMeeting.Core.Models
{
    using System;

    /// <summary>
    /// Metrics for monitoring orchestrator performance and behavior.
    /// </summary>
    public class OrchestratorMetrics
    {
        /// <summary>
        /// Total number of decisions made.
        /// </summary>
        public int TotalDecisions { get; set; }

        /// <summary>
        /// Number of decisions that succeeded on first attempt.
        /// </summary>
        public int SuccessfulFirstAttempts { get; set; }

        /// <summary>
        /// Number of decisions that required retries.
        /// </summary>
        public int RetriedDecisions { get; set; }

        /// <summary>
        /// Number of decisions that fell back to stub mode.
        /// </summary>
        public int StubFallbacks { get; set; }

        /// <summary>
        /// Total number of retry attempts across all decisions.
        /// </summary>
        public int TotalRetryAttempts { get; set; }

        /// <summary>
        /// Number of JSON parse errors encountered.
        /// </summary>
        public int JsonParseErrors { get; set; }

        /// <summary>
        /// Number of Copilot API call failures.
        /// </summary>
        public int ApiCallFailures { get; set; }

        /// <summary>
        /// Total time spent waiting for decisions (milliseconds).
        /// </summary>
        public long TotalDecisionTimeMs { get; set; }

        /// <summary>
        /// Minimum decision time (milliseconds).
        /// </summary>
        public long MinDecisionTimeMs { get; set; } = long.MaxValue;

        /// <summary>
        /// Maximum decision time (milliseconds).
        /// </summary>
        public long MaxDecisionTimeMs { get; set; }

        /// <summary>
        /// Number of 'continue' decisions made.
        /// </summary>
        public int ContinueDecisions { get; set; }

        /// <summary>
        /// Number of 'change_phase' decisions made.
        /// </summary>
        public int PhaseChangeDecisions { get; set; }

        /// <summary>
        /// Number of 'end_meeting' decisions made.
        /// </summary>
        public int EndMeetingDecisions { get; set; }

        /// <summary>
        /// Calculates the average decision time in milliseconds.
        /// </summary>
        public double AverageDecisionTimeMs =>
            TotalDecisions > 0 ? (double)TotalDecisionTimeMs / TotalDecisions : 0;

        /// <summary>
        /// Calculates the stub fallback rate as a percentage.
        /// </summary>
        public double StubFallbackRate =>
            TotalDecisions > 0 ? (double)StubFallbacks / TotalDecisions * 100 : 0;

        /// <summary>
        /// Calculates the retry rate as a percentage.
        /// </summary>
        public double RetryRate =>
            TotalDecisions > 0 ? (double)RetriedDecisions / TotalDecisions * 100 : 0;

        /// <summary>
        /// Calculates the average number of retries per decision.
        /// </summary>
        public double AverageRetriesPerDecision =>
            TotalDecisions > 0 ? (double)TotalRetryAttempts / TotalDecisions : 0;

        /// <summary>
        /// Returns a formatted summary of the metrics.
        /// </summary>
        public string GetSummary()
        {
            return $@"Orchestrator Performance Metrics:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Decisions:
  Total:                 {TotalDecisions}
  Continue:              {ContinueDecisions}
  Phase Changes:         {PhaseChangeDecisions}
  End Meeting:           {EndMeetingDecisions}

Success Rate:
  First Attempt Success: {SuccessfulFirstAttempts} ({GetPercentage(SuccessfulFirstAttempts, TotalDecisions)}%)
  Required Retries:      {RetriedDecisions} ({RetryRate:F1}%)
  Stub Fallbacks:        {StubFallbacks} ({StubFallbackRate:F1}%)

Errors:
  JSON Parse Errors:     {JsonParseErrors}
  API Call Failures:     {ApiCallFailures}
  Total Retry Attempts:  {TotalRetryAttempts}

Performance:
  Avg Decision Time:     {AverageDecisionTimeMs:F0}ms
  Min Decision Time:     {(MinDecisionTimeMs == long.MaxValue ? 0 : MinDecisionTimeMs)}ms
  Max Decision Time:     {MaxDecisionTimeMs}ms
  Total Decision Time:   {TotalDecisionTimeMs}ms
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";
        }

        private static double GetPercentage(int part, int total)
        {
            return total > 0 ? (double)part / total * 100 : 0;
        }
    }
}
