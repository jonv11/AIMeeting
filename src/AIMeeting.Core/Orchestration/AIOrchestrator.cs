namespace AIMeeting.Core.Orchestration
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Prompts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// AI-driven orchestrator that uses Copilot CLI for decision-making.
    /// Implements IOrchestratorDecisionMaker interface.
    /// </summary>
    public class AIOrchestrator : IOrchestratorDecisionMaker
    {
        private readonly IPromptBuilder _promptBuilder;
        private dynamic? _copilotClient; // Use dynamic to avoid hard dependency
        private IEventBus? _eventBus;
        private MeetingContext? _currentContext;
        private IDisposable? _turnRequestSubscription;
        private IDisposable? _turnCompletedSubscription;
        private int _currentAgentIndex = 0;
        
        /// <summary>
        /// Unique identifier for this orchestrator.
        /// </summary>
        public string OrchestratorId { get; }
        
        /// <summary>
        /// Creates a new AI orchestrator.
        /// </summary>
        /// <param name="orchestratorId">Unique identifier</param>
        /// <param name="promptBuilder">Prompt builder for creating orchestrator prompts</param>
        /// <param name="copilotClient">Copilot client for LLM decisions (optional, uses stub if null)</param>
        public AIOrchestrator(
            string orchestratorId,
            IPromptBuilder promptBuilder,
            dynamic? copilotClient = null)
        {
            OrchestratorId = orchestratorId ?? throw new ArgumentNullException(nameof(orchestratorId));
            _promptBuilder = promptBuilder ?? throw new ArgumentNullException(nameof(promptBuilder));
            _copilotClient = copilotClient;
        }
        
        /// <summary>
        /// Initializes the orchestrator for a meeting.
        /// </summary>
        public async Task InitializeAsync(
            MeetingContext context,
            IEventBus eventBus,
            CancellationToken cancellationToken = default)
        {
            _currentContext = context ?? throw new ArgumentNullException(nameof(context));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Starts the orchestrator (begins listening for events).
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_eventBus == null)
            {
                throw new InvalidOperationException("Orchestrator must be initialized before starting");
            }
            
            // Subscribe to turn request events
            _turnRequestSubscription = _eventBus.Subscribe<OrchestratorTurnRequestEvent>(OnTurnRequested);
            _turnCompletedSubscription = _eventBus.Subscribe<TurnCompletedEvent>(OnTurnCompleted);
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Stops the orchestrator gracefully.
        /// </summary>
        public Task StopAsync()
        {
            _turnRequestSubscription?.Dispose();
            _turnCompletedSubscription?.Dispose();
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Handles turn request events.
        /// </summary>
        private async Task OnTurnRequested(OrchestratorTurnRequestEvent request)
        {
            // Make decision: who speaks next, or phase change, or end meeting
            var decision = await MakeDecisionAsync(request);
            
            // Publish decision
            await _eventBus!.PublishAsync(decision);
        }
        
        /// <summary>
        /// Makes a decision about the next turn.
        /// </summary>
        private async Task<OrchestratorDecisionEvent> MakeDecisionAsync(
            OrchestratorTurnRequestEvent request)
        {
            // Check if we're in stub mode
            var agentMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE")?.ToLowerInvariant();
            
            if (agentMode == "stub" || _copilotClient == null)
            {
                return MakeStubDecision(request);
            }
            
            // Build orchestrator prompt with meeting context
            var prompt = BuildOrchestratorPrompt(request);
            
            // Call Copilot CLI to get decision
            string response;
            try
            {
                response = await _copilotClient.GetResponseAsync(prompt);
            }
            catch (Exception ex)
            {
                // Fall back to stub on Copilot failure
                Console.WriteLine($"Copilot call failed, using stub: {ex.Message}");
                return MakeStubDecision(request);
            }
            
            // Parse and validate the decision
            try
            {
                return ParseDecision(response, request.MeetingId);
            }
            catch (Exception ex)
            {
                // Fall back to stub on parse failure
                Console.WriteLine($"Decision parsing failed, using stub: {ex.Message}");
                return MakeStubDecision(request);
            }
        }
        
        /// <summary>
        /// Makes a simple stub decision using round-robin logic.
        /// </summary>
        private OrchestratorDecisionEvent MakeStubDecision(OrchestratorTurnRequestEvent request)
        {
            if (_currentContext == null)
            {
                throw new InvalidOperationException("Context not initialized");
            }
            
            // Get list of agents (excluding the orchestrator itself)
            var agents = _currentContext.Agents.Keys
                .Where(id => id != OrchestratorId)
                .ToList();
            
            if (agents.Count == 0)
            {
                // No agents available, end meeting
                return new OrchestratorDecisionEvent
                {
                    MeetingId = request.MeetingId,
                    Type = DecisionType.EndMeeting,
                    EndReason = "No agents available",
                    Rationale = "Meeting has no participating agents"
                };
            }
            
            // Simple round-robin: cycle through agents
            var nextAgent = agents[_currentAgentIndex % agents.Count];
            _currentAgentIndex++;
            return new OrchestratorDecisionEvent
            {
                MeetingId = request.MeetingId,
                Type = DecisionType.ContinueMeeting,
                NextAgentId = nextAgent,
                Rationale = $"Round-robin selection (stub mode, turn {request.CurrentTurnNumber})"
            };
        }
        
        /// <summary>
        /// Handles turn completed events (updates internal state).
        /// </summary>
        private Task OnTurnCompleted(TurnCompletedEvent evt)
        {
            // TODO: Phase 7 - Track state (hypotheses, decisions, etc.)
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Builds a context-aware prompt for the orchestrator.
        /// </summary>
        private string BuildOrchestratorPrompt(OrchestratorTurnRequestEvent request)
        {
            if (_currentContext == null)
            {
                throw new InvalidOperationException("Context not initialized");
            }
            
            var prompt = new StringBuilder();
            
            // Meeting metadata
            prompt.AppendLine("=== MEETING CONTEXT ===");
            prompt.AppendLine($"Topic: {_currentContext.Topic}");
            prompt.AppendLine($"Current Phase: {request.CurrentPhase}");
            prompt.AppendLine($"Turn Number: {request.CurrentTurnNumber}");
            prompt.AppendLine($"Messages So Far: {_currentContext.Messages.Count}");
            prompt.AppendLine();
            
            // Available agents (exclude orchestrator)
            prompt.AppendLine("=== AVAILABLE AGENTS ===");
            var availableAgents = _currentContext.Agents.Keys
                .Where(id => id != OrchestratorId)
                .ToList();
            
            if (availableAgents.Count == 0)
            {
                prompt.AppendLine("(No agents available)");
            }
            else
            {
                foreach (var agentId in availableAgents)
                {
                    prompt.AppendLine($"- {agentId}");
                }
            }
            prompt.AppendLine();
            
            // Recent message history (last 5 messages)
            prompt.AppendLine("=== RECENT MESSAGES ===");
            var recentMessages = _currentContext.Messages
                .TakeLast(5)
                .ToList();
            
            if (recentMessages.Count == 0)
            {
                prompt.AppendLine("(No messages yet)");
            }
            else
            {
                foreach (var msg in recentMessages)
                {
                    prompt.AppendLine($"[{msg.AgentId}]: {msg.Content}");
                }
            }
            prompt.AppendLine();
            
            // Response format requirements
            prompt.AppendLine("=== RESPONSE FORMAT ===");
            prompt.AppendLine("You MUST respond with ONLY valid JSON. No additional text.");
            prompt.AppendLine("Format:");
            prompt.AppendLine("{");
            prompt.AppendLine("  \"decision\": \"continue\" | \"change_phase\" | \"end_meeting\",");
            prompt.AppendLine("  \"next_agent_id\": \"agent-id\" (required if decision=continue),");
            prompt.AppendLine("  \"new_phase\": \"PhaseName\" (required if decision=change_phase),");
            prompt.AppendLine("  \"end_reason\": \"reason\" (required if decision=end_meeting),");
            prompt.AppendLine("  \"rationale\": \"your detailed reasoning\" (always required)");
            prompt.AppendLine("}");
            prompt.AppendLine();
            prompt.AppendLine("Valid phases: ProblemClarification, OptionGeneration, Evaluation, Decision, ExecutionPlanning, Conclusion");
            prompt.AppendLine();
            
            // Decision guidance
            prompt.AppendLine("=== DECISION GUIDANCE ===");
            prompt.AppendLine("Choose based on:");
            prompt.AppendLine("- Who has relevant expertise for current phase");
            prompt.AppendLine("- Who hasn't spoken recently");
            prompt.AppendLine("- What perspective is missing");
            prompt.AppendLine("- Whether discussion is converging or cycling");
            prompt.AppendLine();
            prompt.AppendLine("End meeting when:");
            prompt.AppendLine("- Clear decision reached with action items");
            prompt.AppendLine("- Consensus achieved");
            prompt.AppendLine("- Deadlock with no path forward");
            prompt.AppendLine();
            prompt.AppendLine("Change phase when:");
            prompt.AppendLine("- Current phase objectives met");
            prompt.AppendLine("- Need to shift from exploration to convergence");
            
            return prompt.ToString();
        }
        
        /// <summary>
        /// Parses orchestrator JSON response into a decision event.
        /// </summary>
        private OrchestratorDecisionEvent ParseDecision(string jsonResponse, string meetingId)
        {
            // Extract JSON from response (handle markdown code blocks if present)
            var json = ExtractJson(jsonResponse);
            
            // Parse JSON
            OrchestratorResponse? response;
            try
            {
                response = JsonSerializer.Deserialize<OrchestratorResponse>(json);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse orchestrator JSON: {ex.Message}", ex);
            }
            
            if (response == null)
            {
                throw new InvalidOperationException("Orchestrator response deserialized to null");
            }
            
            // Validate and convert to decision event
            return response.Decision.ToLowerInvariant() switch
            {
                "continue" => ValidateContinueDecision(response, meetingId),
                "change_phase" => ValidateChangePhaseDecision(response, meetingId),
                "end_meeting" => ValidateEndMeetingDecision(response, meetingId),
                _ => throw new InvalidOperationException($"Invalid decision type: {response.Decision}")
            };
        }
        
        /// <summary>
        /// Extracts JSON from response, handling markdown code blocks.
        /// </summary>
        private string ExtractJson(string response)
        {
            var trimmed = response.Trim();
            
            // Check for markdown code block
            if (trimmed.StartsWith("```json"))
            {
                var startIndex = trimmed.IndexOf('\n') + 1;
                var endIndex = trimmed.LastIndexOf("```");
                if (endIndex > startIndex)
                {
                    return trimmed.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }
            else if (trimmed.StartsWith("```"))
            {
                var startIndex = trimmed.IndexOf('\n') + 1;
                var endIndex = trimmed.LastIndexOf("```");
                if (endIndex > startIndex)
                {
                    return trimmed.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }
            
            return trimmed;
        }
        
        /// <summary>
        /// Validates continue decision and creates event.
        /// </summary>
        private OrchestratorDecisionEvent ValidateContinueDecision(
            OrchestratorResponse response, string meetingId)
        {
            if (string.IsNullOrWhiteSpace(response.NextAgentId))
            {
                throw new InvalidOperationException("Continue decision requires next_agent_id");
            }
            
            if (string.IsNullOrWhiteSpace(response.Rationale))
            {
                throw new InvalidOperationException("Decision requires rationale");
            }
            
            return new OrchestratorDecisionEvent
            {
                MeetingId = meetingId,
                Type = DecisionType.ContinueMeeting,
                NextAgentId = response.NextAgentId,
                Rationale = response.Rationale
            };
        }
        
        /// <summary>
        /// Validates change phase decision and creates event.
        /// </summary>
        private OrchestratorDecisionEvent ValidateChangePhaseDecision(
            OrchestratorResponse response, string meetingId)
        {
            if (string.IsNullOrWhiteSpace(response.NewPhase))
            {
                throw new InvalidOperationException("Change phase decision requires new_phase");
            }
            
            if (string.IsNullOrWhiteSpace(response.Rationale))
            {
                throw new InvalidOperationException("Decision requires rationale");
            }
            
            // Parse phase enum
            if (!Enum.TryParse<MeetingPhase>(response.NewPhase, ignoreCase: true, out var newPhase))
            {
                throw new InvalidOperationException($"Invalid phase name: {response.NewPhase}");
            }
            
            return new OrchestratorDecisionEvent
            {
                MeetingId = meetingId,
                Type = DecisionType.ChangePhase,
                NewPhase = newPhase,
                Rationale = response.Rationale
            };
        }
        
        /// <summary>
        /// Validates end meeting decision and creates event.
        /// </summary>
        private OrchestratorDecisionEvent ValidateEndMeetingDecision(
            OrchestratorResponse response, string meetingId)
        {
            if (string.IsNullOrWhiteSpace(response.EndReason))
            {
                throw new InvalidOperationException("End meeting decision requires end_reason");
            }
            
            if (string.IsNullOrWhiteSpace(response.Rationale))
            {
                throw new InvalidOperationException("Decision requires rationale");
            }
            
            return new OrchestratorDecisionEvent
            {
                MeetingId = meetingId,
                Type = DecisionType.EndMeeting,
                EndReason = response.EndReason,
                Rationale = response.Rationale
            };
        }
    }
}
