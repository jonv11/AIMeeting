namespace AIMeeting.Core.Orchestration
{
    using AIMeeting.Core.Agents;
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;

    /// <summary>
    /// Default implementation of IMeetingOrchestrator.
    /// Manages meeting lifecycle, agent coordination, and artifact generation.
    /// </summary>
    public class MeetingOrchestrator : IMeetingOrchestrator
    {
        private readonly IAgentFactory _agentFactory;
        private readonly IEventBus _eventBus;
        private readonly ITurnManager _turnManager;
        private readonly Dictionary<string, MeetingContext> _meetings = new();
        private readonly object _meetingLock = new();

        /// <summary>
        /// Initializes a new instance of the MeetingOrchestrator class.
        /// </summary>
        public MeetingOrchestrator(
            IAgentFactory agentFactory,
            IEventBus eventBus,
            ITurnManager turnManager)
        {
            _agentFactory = agentFactory ?? throw new ArgumentNullException(nameof(agentFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _turnManager = turnManager ?? throw new ArgumentNullException(nameof(turnManager));
        }

        /// <summary>
        /// Runs a meeting to completion or until a limit is exceeded.
        /// </summary>
        public async Task<MeetingResult> RunMeetingAsync(
            MeetingConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var meetingId = string.IsNullOrWhiteSpace(configuration.MeetingId)
                ? GenerateMeetingId()
                : configuration.MeetingId;
            var startTime = DateTime.UtcNow;
            IOrchestratorDecisionMaker? orchestrator = null;
            var endReason = "Meeting completed normally";

            try
            {
                // State: NotStarted -> Initializing
                var context = new MeetingContext
                {
                    MeetingId = meetingId,
                    Topic = configuration.MeetingTopic,
                    State = MeetingState.Initializing,
                    StartedAt = startTime,
                    Messages = new(),
                    Agents = new()
                };

                lock (_meetingLock)
                {
                    _meetings[meetingId] = context;
                }

                // Detect orchestrator (optional)
                orchestrator = await _agentFactory.DetectOrchestratorAsync(
                    configuration.AgentConfigPaths,
                    cancellationToken);

                // Load and initialize agents
                var agents = await _agentFactory.CreateAgentsAsync(
                    configuration.AgentConfigPaths,
                    cancellationToken);

                if (agents.Count == 0)
                {
                    throw new InvalidOperationException(
                        "No agents loaded from provided configuration paths");
                }

                ITurnManager activeTurnManager = _turnManager;
                ITurnManager fallbackTurnManager = _turnManager;

                // Initialize orchestrator if present
                if (orchestrator != null)
                {
                    await orchestrator.InitializeAsync(context, _eventBus, cancellationToken);
                    await orchestrator.StartAsync(cancellationToken);

                    activeTurnManager = new OrchestratorDrivenTurnManager(
                        _eventBus,
                        orchestrator.OrchestratorId,
                        meetingId,
                        context);
                }

                // Register agents with turn manager
                foreach (var agentId in agents.Keys)
                {
                    activeTurnManager.RegisterAgent(agentId);
                    if (!ReferenceEquals(activeTurnManager, fallbackTurnManager))
                    {
                        fallbackTurnManager.RegisterAgent(agentId);
                    }
                    context.Agents[agentId] = agents[agentId];
                }

                // Publish meeting started event
                await _eventBus.PublishAsync(new MeetingStartedEvent
                {
                    Configuration = configuration
                });

                // Initialize all agents
                foreach (var agent in agents.Values)
                {
                    await agent.InitializeAsync(context, cancellationToken);
                    await _eventBus.PublishAsync(new AgentReadyEvent
                    {
                        AgentId = agent.AgentId
                    });
                }

                // State: Initializing -> InProgress
                context.State = MeetingState.InProgress;

                // Run meeting turns
                var turnNumber = 0;
                while (activeTurnManager.HasAgents && !cancellationToken.IsCancellationRequested)
                {
                    turnNumber++;

                    // Check hard limits
                    var elapsedMinutes = (DateTime.UtcNow - startTime).TotalMinutes;
                    if (configuration.HardLimits?.MaxDurationMinutes.HasValue == true)
                    {
                        if (elapsedMinutes >= configuration.HardLimits.MaxDurationMinutes.Value)
                        {
                            endReason = "Max duration exceeded";
                            await _eventBus.PublishAsync(new MeetingEndingEvent
                            {
                                Reason = "Max duration exceeded"
                            });
                            break;
                        }
                    }

                    if (configuration.HardLimits?.MaxTotalMessages.HasValue == true)
                    {
                        if (context.Messages.Count >= configuration.HardLimits.MaxTotalMessages.Value)
                        {
                            endReason = "Max messages exceeded";
                            await _eventBus.PublishAsync(new MeetingEndingEvent
                            {
                                Reason = "Max messages exceeded"
                            });
                            break;
                        }
                    }

                    // Get next agent
                    string nextAgentId;
                    try
                    {
                        nextAgentId = activeTurnManager.GetNextAgent();
                    }
                    catch (PhaseChangeRequestedException phaseChange)
                    {
                        var oldPhase = context.CurrentPhase;
                        context.CurrentPhase = phaseChange.NewPhase;

                        await _eventBus.PublishAsync(new PhaseChangeRequestedEvent
                        {
                            OldPhase = oldPhase,
                            NewPhase = phaseChange.NewPhase,
                            Reason = phaseChange.Rationale
                        });

                        await _eventBus.PublishAsync(new PhaseChangedEvent
                        {
                            OldPhase = oldPhase,
                            NewPhase = phaseChange.NewPhase
                        });

                        continue;
                    }
                    catch (MeetingEndRequestedException endMeeting)
                    {
                        endReason = endMeeting.EndReason;
                        await _eventBus.PublishAsync(new MeetingEndingEvent
                        {
                            Reason = endMeeting.EndReason
                        });
                        break;
                    }
                    catch (TimeoutException)
                    {
                        // Fallback to FIFO if orchestrator does not respond
                        nextAgentId = fallbackTurnManager.GetNextAgent();
                    }
                    var agent = (IAgent)context.Agents[nextAgentId];

                    // Check if agent should participate
                    var shouldParticipate = await agent.ShouldParticipateAsync(context);
                    if (!shouldParticipate)
                    {
                        await _eventBus.PublishAsync(new TurnSkippedEvent
                        {
                            AgentId = agent.AgentId,
                            Reason = "Agent chose not to participate"
                        });
                        continue;
                    }

                    // Start turn
                    context.CurrentSpeakingAgentId = agent.AgentId;
                    await _eventBus.PublishAsync(new TurnStartedEvent
                    {
                        AgentId = agent.AgentId,
                        TurnNumber = turnNumber
                    });

                    // Get response
                    var response = await agent.RespondAsync(context, cancellationToken);

                    // Record message
                    var message = new Message
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        AgentId = agent.AgentId,
                        Content = response.Content,
                        Timestamp = DateTime.UtcNow,
                        Type = response.Type
                    };

                    context.Messages.Add(message);

                    // Complete turn
                    await _eventBus.PublishAsync(new TurnCompletedEvent
                    {
                        AgentId = agent.AgentId,
                        Message = response.Content
                    });

                    context.CurrentSpeakingAgentId = null;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                // State: InProgress -> EndingGracefully -> Completed
                context.State = MeetingState.EndingGracefully;

                await _eventBus.PublishAsync(new MeetingEndedEvent
                {
                    EndReason = endReason
                });

                context.State = MeetingState.Completed;

                var duration = DateTime.UtcNow - startTime;
                return new MeetingResult
                {
                    MeetingId = meetingId,
                    State = context.State,
                    EndReason = endReason,
                    Duration = duration,
                    MessageCount = context.Messages.Count,
                    TranscriptPath = $"meetings/{meetingId}/transcript.md",
                    MeetingMetadataPath = $"meetings/{meetingId}/meeting.json"
                };
            }
            catch (OperationCanceledException)
            {
                // User cancelled the meeting
                lock (_meetingLock)
                {
                    if (_meetings.TryGetValue(meetingId, out var context))
                    {
                        context.State = MeetingState.Cancelled;
                    }
                }

                await _eventBus.PublishAsync(new MeetingEndedEvent
                {
                    EndReason = "Meeting cancelled by user"
                });

                var duration = DateTime.UtcNow - startTime;
                return new MeetingResult
                {
                    MeetingId = meetingId,
                    State = MeetingState.Cancelled,
                    EndReason = "Meeting cancelled by user",
                    Duration = duration,
                    MessageCount = 0,
                    TranscriptPath = $"meetings/{meetingId}/transcript.md",
                    MeetingMetadataPath = $"meetings/{meetingId}/meeting.json"
                };
            }
            catch (Exception ex)
            {
                // Meeting failed
                lock (_meetingLock)
                {
                    if (_meetings.TryGetValue(meetingId, out var context))
                    {
                        context.State = MeetingState.Failed;
                    }
                }

                await _eventBus.PublishAsync(new MeetingEndedEvent
                {
                    EndReason = $"Meeting failed: {ex.Message}"
                });

                var duration = DateTime.UtcNow - startTime;
                return new MeetingResult
                {
                    MeetingId = meetingId,
                    State = MeetingState.Failed,
                    EndReason = $"Meeting failed: {ex.Message}",
                    Duration = duration,
                    MessageCount = 0,
                    TranscriptPath = $"meetings/{meetingId}/transcript.md",
                    MeetingMetadataPath = $"meetings/{meetingId}/meeting.json",
                    ErrorLogPath = $"meetings/{meetingId}/errors.log"
                };
            }
            finally
            {
                if (orchestrator != null)
                {
                    await orchestrator.StopAsync();
                    
                    // Output orchestrator metrics if available
                    if (orchestrator is AIOrchestrator aiOrchestrator)
                    {
                        Console.WriteLine();
                        Console.WriteLine(aiOrchestrator.Metrics.GetSummary());
                        Console.WriteLine();
                    }
                }

                lock (_meetingLock)
                {
                    _meetings.Remove(meetingId);
                }
            }
        }

        /// <summary>
        /// Gets the current state of a running meeting.
        /// </summary>
        public async Task<MeetingState> GetMeetingStateAsync(string meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting ID cannot be null or empty", nameof(meetingId));

            lock (_meetingLock)
            {
                if (_meetings.TryGetValue(meetingId, out var context))
                {
                    return context.State;
                }
            }

            throw new InvalidOperationException($"Meeting {meetingId} not found");
        }

        /// <summary>
        /// Stops a meeting gracefully and generates final artifacts.
        /// </summary>
        public async Task StopMeetingAsync(string meetingId, string reason)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Meeting ID cannot be null or empty", nameof(meetingId));

            lock (_meetingLock)
            {
                if (_meetings.TryGetValue(meetingId, out var context))
                {
                    context.State = MeetingState.EndingGracefully;
                }
            }

            await _eventBus.PublishAsync(new MeetingEndingEvent
            {
                Reason = reason
            });
        }

        /// <summary>
        /// Generates a unique meeting ID.
        /// </summary>
        private string GenerateMeetingId()
        {
            // Format: YYYYMMDD_HHMMSS_<guid>
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyyMMdd_HHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];
            return $"{timestamp}_{guid}";
        }
    }
}
