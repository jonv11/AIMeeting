namespace AIMeeting.Core.Orchestration
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Turn manager that delegates decisions to an orchestrator (AI or human) via event bus.
    /// Implementation-agnostic: works with any IOrchestratorDecisionMaker.
    /// </summary>
    public class OrchestratorDrivenTurnManager : ITurnManager
    {
        private readonly IEventBus _eventBus;
        private readonly string _orchestratorId;
        private readonly string _meetingId;
        private readonly MeetingContext _context;
        private readonly List<string> _agents = new();
        private readonly object _lock = new();
        private OrchestratorDecisionEvent? _lastDecision;
        private readonly SemaphoreSlim _decisionSignal = new(0, 1);
        private readonly TimeSpan _decisionTimeout = TimeSpan.FromSeconds(30);
        private int _currentTurnNumber = 0;
        
        /// <summary>
        /// Creates a new orchestrator-driven turn manager.
        /// </summary>
        /// <param name="eventBus">Event bus for communication</param>
        /// <param name="orchestratorId">ID of the orchestrator agent</param>
        /// <param name="meetingId">Meeting identifier</param>
        /// <param name="context">Meeting context (for phase tracking)</param>
        public OrchestratorDrivenTurnManager(
            IEventBus eventBus,
            string orchestratorId,
            string meetingId,
            MeetingContext context)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _orchestratorId = orchestratorId ?? throw new ArgumentNullException(nameof(orchestratorId));
            _meetingId = meetingId ?? throw new ArgumentNullException(nameof(meetingId));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            
            // Subscribe to orchestrator decisions
            _eventBus.Subscribe<OrchestratorDecisionEvent>(OnDecisionReceived);
        }
        
        /// <summary>
        /// Registers an agent with the turn manager.
        /// </summary>
        public void RegisterAgent(string agentId)
        {
            if (agentId == _orchestratorId)
            {
                // Don't register the orchestrator itself as a regular agent
                return;
            }
            
            lock (_lock)
            {
                if (!_agents.Contains(agentId))
                {
                    _agents.Add(agentId);
                }
            }
        }
        
        /// <summary>
        /// Removes an agent from the turn queue.
        /// </summary>
        public void UnregisterAgent(string agentId)
        {
            lock (_lock)
            {
                _agents.Remove(agentId);
            }
        }
        
        /// <summary>
        /// Requests a turn decision from orchestrator agent.
        /// Blocks until decision is received or timeout occurs.
        /// </summary>
        public string GetNextAgent()
        {
            _currentTurnNumber++;
            _lastDecision = null;
            
            // Publish request event
            var request = new OrchestratorTurnRequestEvent
            {
                MeetingId = _meetingId,
                CurrentTurnNumber = _currentTurnNumber,
                CurrentPhase = _context.CurrentPhase
            };
            
            _eventBus.PublishAsync(request).Wait();
            
            // Wait for decision with timeout
            var received = _decisionSignal.Wait(_decisionTimeout);
            
            if (!received)
            {
                throw new TimeoutException(
                    $"Orchestrator did not respond within {_decisionTimeout.TotalSeconds} seconds");
            }
            
            if (_lastDecision == null)
            {
                throw new InvalidOperationException("Decision event was null");
            }
            
            // Handle decision type
            switch (_lastDecision.Type)
            {
                case DecisionType.ContinueMeeting:
                    if (string.IsNullOrWhiteSpace(_lastDecision.NextAgentId))
                    {
                        throw new InvalidOperationException(
                            "Orchestrator decided to continue but did not specify next agent");
                    }
                    return _lastDecision.NextAgentId;
                
                case DecisionType.ChangePhase:
                    throw new PhaseChangeRequestedException(
                        _lastDecision.NewPhase ?? MeetingPhase.Conclusion,
                        _lastDecision.Rationale ?? "Phase change requested");
                
                case DecisionType.EndMeeting:
                    throw new MeetingEndRequestedException(
                        _lastDecision.EndReason ?? "Orchestrator decided to end meeting",
                        _lastDecision.Rationale ?? "Meeting ended by orchestrator");
                
                default:
                    throw new InvalidOperationException(
                        $"Unknown decision type: {_lastDecision.Type}");
            }
        }
        
        /// <summary>
        /// Gets whether there are agents remaining.
        /// </summary>
        public bool HasAgents
        {
            get
            {
                lock (_lock)
                {
                    return _agents.Count > 0;
                }
            }
        }
        
        /// <summary>
        /// Handles orchestrator decision events.
        /// </summary>
        private Task OnDecisionReceived(OrchestratorDecisionEvent decision)
        {
            if (decision.MeetingId != _meetingId)
            {
                // Not for this meeting, ignore
                return Task.CompletedTask;
            }
            
            _lastDecision = decision;
            
            // Release the waiting GetNextAgent call
            try
            {
                _decisionSignal.Release();
            }
            catch (SemaphoreFullException)
            {
                // Already released, ignore
            }
            
            return Task.CompletedTask;
        }
    }
    
    /// <summary>
    /// Exception thrown when orchestrator requests a phase change.
    /// </summary>
    public class PhaseChangeRequestedException : Exception
    {
        /// <summary>
        /// The new phase to transition to.
        /// </summary>
        public MeetingPhase NewPhase { get; }
        
        /// <summary>
        /// Rationale for the phase change.
        /// </summary>
        public string Rationale { get; }
        
        public PhaseChangeRequestedException(MeetingPhase newPhase, string rationale)
            : base($"Phase change requested to {newPhase}: {rationale}")
        {
            NewPhase = newPhase;
            Rationale = rationale;
        }
    }
    
    /// <summary>
    /// Exception thrown when orchestrator requests meeting end.
    /// </summary>
    public class MeetingEndRequestedException : Exception
    {
        /// <summary>
        /// Reason for ending the meeting.
        /// </summary>
        public string EndReason { get; }
        
        /// <summary>
        /// Rationale for ending.
        /// </summary>
        public string Rationale { get; }
        
        public MeetingEndRequestedException(string endReason, string rationale)
            : base($"Meeting end requested: {endReason} - {rationale}")
        {
            EndReason = endReason;
            Rationale = rationale;
        }
    }
}
