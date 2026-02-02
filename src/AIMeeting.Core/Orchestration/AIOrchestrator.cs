namespace AIMeeting.Core.Orchestration
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using AIMeeting.Core.Prompts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            
            // TODO: Phase 4 - Real Copilot integration
            // For now, fall back to stub
            return MakeStubDecision(request);
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
    }
}
