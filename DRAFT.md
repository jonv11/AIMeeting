# Agent Meeting System - Project Draft

**Version:** 0.1.0  
**Created:** January 30, 2026  
**Status:** Draft  
**Language:** C# (.NET 8+)  
**Primary SDK:** GitHub Copilot SDK  

---

## Executive Summary

A CLI-based multi-agent meeting system where AI agents with different roles engage in structured discussions about specific topics. The system simulates real-world meetings with agents taking turns, sharing perspectives, and collaborating to reach conclusions. Each meeting produces artifacts including transcripts, summaries, action items, and follow-ups.

### Core Value Proposition
- **Structured AI Collaboration**: Enable multiple AI agents to discuss complex topics from different perspectives
- **Configurable Roles**: Text-based configuration allows any role/personality without code changes
- **Meeting Artifacts**: Automatic generation of transcripts, summaries, and action items
- **Event-Driven Architecture**: Message bus pattern ensures clean agent coordination
- **Extensible Design**: Support wide range of use cases beyond software development

---

## Technical Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLI Interface                            │
│              (Meeting Initialization & Monitoring)               │
└───────────────────────────┬─────────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────────┐
│                    Meeting Orchestrator                          │
│  - Meeting lifecycle management                                  │
│  - Hard limit enforcement (time, message count)                  │
│  - Agent coordination                                            │
└───────────┬─────────────────────────────┬───────────────────────┘
            │                             │
┌───────────▼─────────────┐   ┌───────────▼──────────────────────┐
│    Message Bus          │   │   Meeting Room (Shared Space)    │
│  - Event pub/sub        │   │   - Read/write files             │
│  - Agent turn queue     │   │   - Locking mechanism            │
│  - Event notifications  │   │   - Artifact storage             │
└───────────┬─────────────┘   └──────────────────────────────────┘
            │
    ┌───────┴────────┬─────────────┬──────────────┐
    │                │             │              │
┌───▼────┐    ┌──────▼──┐   ┌──────▼──┐   ┌──────▼──────┐
│ Agent  │    │ Agent   │   │ Agent   │   │  Moderator  │
│  (PM)  │    │  (Dev)  │   │  (QA)   │   │    Agent    │
└────────┘    └─────────┘   └─────────┘   └─────────────┘
     │              │             │              │
     └──────────────┴─────────────┴──────────────┘
                    │
        ┌───────────▼────────────┐
        │   GitHub Copilot SDK   │
        │   - Session management  │
        │   - Event handling      │
        │   - CLI communication   │
        └───────────┬────────────┘
                    │
        ┌───────────▼────────────┐
        │   GitHub Copilot CLI   │
        │   (Separate install)    │
        └────────────────────────┘
```

### Component Breakdown

#### 1. CLI Interface
**Purpose**: Human interaction layer for initiating and monitoring meetings

**Responsibilities**:
- Parse command-line arguments (meeting topic, participants, limits)
- Load agent configurations from text files
- Display real-time meeting progress
- Handle user interrupts (graceful shutdown)

**Technology**: System.CommandLine library

#### 2. Meeting Orchestrator
**Purpose**: Central coordinator for meeting execution

**Responsibilities**:
- Initialize meeting workspace (folder structure)
- Load and instantiate agents based on configuration
- Enforce hard limits (time, message count, token usage)
- Coordinate with moderator agent for soft stops
- Generate final meeting artifacts
- Maintain meeting state and history

**Key Classes**:
- `MeetingOrchestrator`: Main coordination logic
- `MeetingContext`: Current meeting state
- `MeetingConfiguration`: User-provided settings
- `MeetingLimits`: Hard stop conditions

#### 3. Message Bus (Event System)
**Purpose**: Decoupled communication between agents and orchestrator

**Pattern**: In-memory Publish/Subscribe pattern

**Core Events**:
```csharp
// Agent lifecycle
AgentJoinedEvent
AgentReadyEvent
AgentLeftEvent

// Turn management
TurnStartedEvent(AgentId, TurnNumber)
TurnCompletedEvent(AgentId, Message)
TurnSkippedEvent(AgentId, Reason)

// Meeting control
MeetingStartedEvent
MeetingPausedEvent      // Future
MeetingResumedEvent     // Future
MeetingEndingEvent(Reason)
MeetingEndedEvent(Summary)

// Communication
MessagePostedEvent(AgentId, Content, ReplyTo)
QuestionAskedEvent(AgentId, Question, TargetAgent)
VoteInitiatedEvent      // Future
ConsensusReachedEvent   // Future

// File operations
FileCreatedEvent(AgentId, FilePath)
FileModifiedEvent(AgentId, FilePath)
FileLockedEvent(FilePath, AgentId)
FileUnlockedEvent(FilePath)
```

**Implementation**: Custom EventBus class with thread-safe subscription management

#### 4. Agent System
**Purpose**: Individual AI participants with specific roles and perspectives

**Base Agent Architecture**:
```csharp
public abstract class AgentBase
{
    public string AgentId { get; }
    public string RoleName { get; }
    public AgentConfiguration Configuration { get; }
    
    // Core capabilities
    protected abstract Task<string> GenerateResponseAsync(
        MeetingContext context, 
        CancellationToken cancellationToken);
    
    protected abstract Task<bool> ShouldParticipateAsync(
        MeetingContext context);
    
    // File operations
    protected Task<string> ReadFileAsync(string relativePath);
    protected Task WriteFileAsync(string relativePath, string content);
    protected Task<IEnumerable<string>> SearchArtifactsAsync(string query);
    
    // Communication
    protected void PostMessage(string content, string replyToMessageId = null);
    protected void AskQuestion(string question, string targetAgentId = null);
}

public class StandardAgent : AgentBase
{
    private readonly ICopilotClient _copilotClient;
    private readonly IPromptBuilder _promptBuilder;
    
    // Copilot SDK integration for generating responses
}

public class ModeratorAgent : AgentBase
{
    // Special capabilities:
    // - Decide when to end meeting
    // - Recognize consensus or deadlock
    // - Generate meeting summary
    // - Identify action items
    // - Suggest follow-ups
    
    public Task<bool> ShouldEndMeetingAsync(MeetingContext context);
    public Task<MeetingSummary> GenerateSummaryAsync();
}

public class ScribeAgent : AgentBase
{
    // Special capabilities:
    // - Maintain detailed transcript
    // - Extract key decisions
    // - Document action items
    // - Format meeting minutes
}
```

#### 5. Configuration System
**Purpose**: Text-based configuration for maximum flexibility

**Agent Configuration File Format** (`agents/project-manager.txt`):
```
ROLE: Project Manager
DESCRIPTION: Oversees project timeline, resources, and deliverables

PERSONA:
- Focused on practical outcomes and deadlines
- Asks clarifying questions about scope and priorities
- Advocates for realistic timelines
- Concerned with resource allocation

INSTRUCTIONS:
- Always consider project constraints (time, budget, team size)
- Push for clear action items and ownership
- Flag risks early and suggest mitigations
- Summarize decisions and next steps
- Challenge scope creep

INITIAL_MESSAGE_TEMPLATE:
As Project Manager, I'd like to understand: {topic}
Let's ensure we align on objectives and deliverables.

RESPONSE_STYLE: Professional, direct, action-oriented
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Planning, Risk Management, Stakeholder Communication
```

**Meeting Configuration**:
```json
{
  "meetingTopic": "Discuss architecture for payment service refactoring",
  "agentConfigs": [
    "agents/project-manager.txt",
    "agents/senior-developer.txt",
    "agents/security-expert.txt",
    "agents/qa-engineer.txt",
    "agents/moderator.txt"
  ],
  "hardLimits": {
    "maxDurationMinutes": 30,
    "maxTotalMessages": 100,
    "maxMessagesPerAgent": 25,
    "maxTokensTotal": 50000
  },
  "turnTakingStrategy": "FIFO",
  "meetingRoomPath": "./meetings/{timestamp}_{topic_slug}",
  "enableArtifactSearch": true,
  "logLevel": "Information"
}
```

#### 6. Meeting Room (Shared Workspace)
**Purpose**: Isolated file system space for agent collaboration

**Structure**:
```
meetings/
└── 20260130_143022_payment_refactoring/
    ├── config.json                    # Meeting configuration snapshot
    ├── transcript.md                  # Real-time conversation log
    ├── summary.md                     # Final summary (generated at end)
    ├── action_items.md                # Extracted action items
    ├── decisions.md                   # Key decisions made
    ├── agents/                        # Agent-specific notes
    │   ├── project_manager_notes.md
    │   ├── developer_notes.md
    │   └── security_notes.md
    ├── artifacts/                     # Shared documents
    │   ├── architecture_proposal.md
    │   ├── risk_assessment.md
    │   └── timeline.md
    └── .metadata/                     # System files
        ├── events.log                 # Event stream log
        ├── metrics.json               # Meeting metrics
        └── locks.json                 # File lock state
```

**File Locking Strategy**:
```csharp
public interface IFileLocker
{
    Task<IFileLock> AcquireLockAsync(
        string filePath, 
        string agentId,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}

public interface IFileLock : IDisposable
{
    string FilePath { get; }
    string LockedByAgentId { get; }
    DateTime AcquiredAt { get; }
}

// Implementation: Simple file-based locking with timeout
// No deadlock possible - single lock at a time per agent
```

#### 7. Turn-Taking Strategy
**Purpose**: Fair and orderly agent participation

**FIFO Queue (Default)**:
```csharp
public class FifoTurnManager : ITurnManager
{
    private readonly Queue<string> _turnQueue;
    
    public string GetNextAgent()
    {
        var agent = _turnQueue.Dequeue();
        _turnQueue.Enqueue(agent);  // Re-add to end
        return agent;
    }
}
```

**Alternative Strategies** (Future):
- **Priority-Based**: Moderator > Experts > General
- **Dynamic**: Agents "raise hand" based on relevance
- **Interrupt-Driven**: Agents can interrupt with priority flag

---

## Project Structure

```
AgentMeetingSystem/
├── README.md
├── LICENSE
├── .gitignore
├── .editorconfig
├── AgentMeetingSystem.sln
│
├── src/
│   ├── AgentMeetingSystem.CLI/
│   │   ├── Program.cs                          # Entry point
│   │   ├── Commands/
│   │   │   ├── StartMeetingCommand.cs
│   │   │   ├── ListConfigsCommand.cs
│   │   │   └── ValidateConfigCommand.cs
│   │   ├── Display/
│   │   │   ├── MeetingProgressDisplay.cs       # Real-time UI
│   │   │   └── ConsolePrinter.cs
│   │   └── AgentMeetingSystem.CLI.csproj
│   │
│   ├── AgentMeetingSystem.Core/
│   │   ├── Models/
│   │   │   ├── Meeting.cs
│   │   │   ├── MeetingContext.cs
│   │   │   ├── MeetingConfiguration.cs
│   │   │   ├── MeetingSummary.cs
│   │   │   ├── Message.cs
│   │   │   └── AgentConfiguration.cs
│   │   ├── Agents/
│   │   │   ├── AgentBase.cs
│   │   │   ├── StandardAgent.cs
│   │   │   ├── ModeratorAgent.cs
│   │   │   ├── ScribeAgent.cs
│   │   │   └── AgentFactory.cs
│   │   ├── Orchestration/
│   │   │   ├── MeetingOrchestrator.cs
│   │   │   ├── ITurnManager.cs
│   │   │   ├── FifoTurnManager.cs
│   │   │   └── MeetingStateMachine.cs
│   │   ├── Events/
│   │   │   ├── IEventBus.cs
│   │   │   ├── InMemoryEventBus.cs
│   │   │   ├── MeetingEvents.cs
│   │   │   ├── AgentEvents.cs
│   │   │   └── FileEvents.cs
│   │   ├── FileSystem/
│   │   │   ├── IMeetingRoom.cs
│   │   │   ├── MeetingRoom.cs
│   │   │   ├── IFileLocker.cs
│   │   │   ├── FileLocker.cs
│   │   │   └── ArtifactSearcher.cs
│   │   ├── Configuration/
│   │   │   ├── IConfigurationLoader.cs
│   │   │   ├── AgentConfigurationLoader.cs
│   │   │   ├── MeetingConfigurationLoader.cs
│   │   │   └── ConfigurationValidator.cs
│   │   ├── Prompts/
│   │   │   ├── IPromptBuilder.cs
│   │   │   ├── PromptBuilder.cs
│   │   │   └── PromptTemplates.cs
│   │   └── AgentMeetingSystem.Core.csproj
│   │
│   ├── AgentMeetingSystem.Copilot/
│   │   ├── ICopilotClient.cs
│   │   ├── CopilotClient.cs                    # GitHub Copilot SDK wrapper
│   │   ├── CopilotConfiguration.cs
│   │   ├── TokenManager.cs
│   │   └── AgentMeetingSystem.Copilot.csproj
│   │
│   └── AgentMeetingSystem.Infrastructure/
│       ├── Logging/
│       │   ├── MeetingLogger.cs
│       │   ├── EventLogger.cs
│       │   └── StructuredLogging.cs
│       ├── Metrics/
│       │   ├── MeetingMetrics.cs
│       │   └── MetricsCollector.cs
│       ├── Serialization/
│       │   ├── JsonSerializer.cs
│       │   └── TranscriptSerializer.cs
│       └── AgentMeetingSystem.Infrastructure.csproj
│
├── tests/
│   ├── AgentMeetingSystem.Core.Tests/
│   │   ├── Agents/
│   │   │   ├── AgentBaseTests.cs
│   │   │   └── ModeratorAgentTests.cs
│   │   ├── Orchestration/
│   │   │   ├── MeetingOrchestratorTests.cs
│   │   │   └── TurnManagerTests.cs
│   │   ├── Events/
│   │   │   └── EventBusTests.cs
│   │   ├── FileSystem/
│   │   │   ├── MeetingRoomTests.cs
│   │   │   └── FileLockerTests.cs
│   │   └── Configuration/
│   │       └── ConfigurationLoaderTests.cs
│   │
│   ├── AgentMeetingSystem.Integration.Tests/
│   │   ├── Scenarios/
│   │   │   ├── BasicMeetingScenario.cs
│   │   │   ├── ConcurrentFileAccessScenario.cs
│   │   │   └── TimeoutScenario.cs
│   │   └── Fixtures/
│   │       └── MeetingTestFixture.cs
│   │
│   └── AgentMeetingSystem.Copilot.Tests/
│       └── CopilotClientTests.cs
│
├── config/
│   ├── agents/
│   │   ├── project-manager.txt
│   │   ├── senior-developer.txt
│   │   ├── security-expert.txt
│   │   ├── qa-engineer.txt
│   │   ├── devops-engineer.txt
│   │   ├── moderator.txt
│   │   └── scribe.txt
│   └── meetings/
│       ├── default-meeting.json
│       └── quick-meeting.json
│
├── docs/
│   ├── ARCHITECTURE.md
│   ├── AGENT_CONFIGURATION_GUIDE.md
│   ├── API.md
│   ├── EXTENDING.md
│   └── EXAMPLES.md
│
└── scripts/
    ├── setup.ps1
    └── run-example-meeting.ps1
```

---

## Naming Conventions

Following C# and .NET standards:

| Type | Convention | Example |
|------|------------|---------|
| **Namespaces** | PascalCase | `AgentMeetingSystem.Core.Agents` |
| **Classes** | PascalCase | `MeetingOrchestrator`, `AgentBase` |
| **Interfaces** | I + PascalCase | `IEventBus`, `IMeetingRoom` |
| **Methods** | PascalCase | `StartMeetingAsync`, `GetNextTurn` |
| **Properties** | PascalCase | `AgentId`, `MeetingContext` |
| **Fields (private)** | _camelCase | `_eventBus`, `_copilotClient` |
| **Parameters** | camelCase | `agentId`, `cancellationToken` |
| **Local Variables** | camelCase | `currentAgent`, `messageCount` |
| **Constants** | PascalCase | `MaxRetries`, `DefaultTimeout` |
| **Enums** | PascalCase | `MeetingState`, `TurnStrategy` |
| **Files** | Match class name | `MeetingOrchestrator.cs` |
| **Async Methods** | Suffix with Async | `GenerateResponseAsync` |

---

## Error Handling Strategy

### Exception Hierarchy

```csharp
// Base exception
public class AgentMeetingException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Context { get; }
}

// Specific exceptions
public class MeetingConfigurationException : AgentMeetingException
{
    // Invalid configuration files or parameters
}

public class AgentInitializationException : AgentMeetingException
{
    public string AgentId { get; }
    // Agent failed to initialize or load configuration
}

public class TurnTimeoutException : AgentMeetingException
{
    public string AgentId { get; }
    public TimeSpan Timeout { get; }
    // Agent exceeded turn time limit
}

public class FileLockException : AgentMeetingException
{
    public string FilePath { get; }
    public string RequestedByAgentId { get; }
    // Failed to acquire file lock
}

public class CopilotApiException : AgentMeetingException
{
    public CopilotApiException(string message, Exception innerException = null)
        : base(message, innerException) { }
    // Copilot SDK/CLI failure
}

public class MeetingLimitExceededException : AgentMeetingException
{
    public string LimitType { get; }  // "Time", "Messages", "Tokens"
    // Hard limit reached
}
```

### Error Handling Patterns

```csharp
// Agent response generation
public async Task<string> GenerateResponseAsync(
    MeetingContext context,
    CancellationToken cancellationToken)
{
    try
    {
        var response = await _copilotClient.GenerateAsync(
            prompt: BuildPrompt(context),
            cancellationToken: cancellationToken);
            
        return response;
    }
    catch (TurnTimeoutException ex)
    {
        // Timeout - possibly retry with simpler prompt
        _logger.LogWarning(ex,
            "Agent {AgentId} timed out, retrying with truncated context",
            AgentId);
        // Could implement retry logic here
        throw;
    }
    catch (CopilotApiException ex) when (ex.InnerException is FileNotFoundException)
    {
        // Copilot CLI not installed
        _logger.LogCritical(ex,
            "Copilot CLI not found. Cannot continue.");
        throw new AgentInitializationException(
            "Copilot CLI is not installed or not in PATH",
            AgentId,
            ex);
    }
    catch (CopilotApiException ex)
    {
        // Other Copilot errors
        _logger.LogError(ex, 
            "Copilot SDK error for agent {AgentId}", 
            AgentId);
        throw new AgentInitializationException(
            $"Agent {AgentId} cannot generate response: {ex.Message}",
            AgentId,
            ex);
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation(
            "Response generation canceled for agent {AgentId}",
            AgentId);
        throw;
    }
}

// File locking
public async Task<IFileLock> AcquireLockAsync(
    string filePath,
    string agentId,
    TimeSpan timeout,
    CancellationToken cancellationToken)
{
    var deadline = DateTime.UtcNow.Add(timeout);
    
    while (DateTime.UtcNow < deadline)
    {
        if (TryAcquireLock(filePath, agentId, out var fileLock))
        {
            _logger.LogDebug(
                "Lock acquired on {FilePath} by {AgentId}",
                filePath,
                agentId);
            return fileLock;
        }
        
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
    }
    
    var currentLock = GetCurrentLock(filePath);
    throw new FileLockException(
        $"Could not acquire lock on {filePath} within {timeout}. " +
        $"Locked by {currentLock?.LockedByAgentId}",
        filePath,
        agentId);
}
```

### Graceful Degradation

```csharp
// Meeting continues even if one agent fails
public async Task ExecuteTurnAsync(string agentId)
{
    try
    {
        var agent = _agents[agentId];
        var response = await agent.GenerateResponseAsync(
            _context,
            _cancellationToken);
            
        await _eventBus.PublishAsync(
            new TurnCompletedEvent(agentId, response));
    }
    catch (TurnTimeoutException ex)
    {
        _logger.LogWarning(ex,
            "Agent {AgentId} timed out, skipping turn",
            agentId);
            
        await _eventBus.PublishAsync(
            new TurnSkippedEvent(agentId, "Timeout"));
            
        // Meeting continues with next agent
    }
    catch (AgentInitializationException ex)
    {
        _logger.LogError(ex,
            "Agent {AgentId} failed critically, removing from meeting",
            agentId);
            
        _agents.Remove(agentId);
        
        // If too few agents remain, end meeting
        if (_agents.Count < MinimumAgents)
        {
            throw new MeetingConfigurationException(
                "Insufficient agents to continue meeting");
        }
    }
}
```

---

## Logging Strategy

### Log Levels

```csharp
// Trace: Agent prompt details, event flow
_logger.LogTrace("Building prompt for {AgentId}: {PromptPreview}",
    agentId, prompt.Substring(0, 100));

// Debug: Turn coordination, file operations
_logger.LogDebug("Agent {AgentId} acquired lock on {FilePath}",
    agentId, filePath);

// Information: Meeting lifecycle, key decisions
_logger.LogInformation(
    "Meeting started: {Topic} with {AgentCount} agents",
    meetingConfig.Topic,
    agents.Count);

// Warning: Recoverable errors, degraded performance
_logger.LogWarning("Agent {AgentId} response exceeded {MaxTokens} tokens, truncating",
    agentId, maxTokens);

// Error: Failed operations, unrecoverable agent errors
_logger.LogError(exception,
    "Failed to generate summary for meeting {MeetingId}",
    meetingId);

// Critical: System-level failures
_logger.LogCritical("Cannot initialize Copilot SDK: {Error}",
    exception.Message);
```

### Structured Logging

```csharp
using Serilog;

public class MeetingLogger
{
    public void LogMeetingStart(MeetingConfiguration config)
    {
        _logger.Information(
            "Meeting started " +
            "{MeetingId} " +
            "{Topic} " +
            "{AgentCount} " +
            "{MaxDuration} " +
            "{MaxMessages}",
            config.MeetingId,
            config.Topic,
            config.AgentConfigs.Count,
            config.HardLimits.MaxDurationMinutes,
            config.HardLimits.MaxTotalMessages);
    }
    
    public void LogAgentTurn(string agentId, int turnNumber, int messageLength)
    {
        _logger.Information(
            "Agent turn " +
            "{AgentId} " +
            "{TurnNumber} " +
            "{MessageLength} " +
            "{Timestamp}",
            agentId,
            turnNumber,
            messageLength,
            DateTime.UtcNow);
    }
}
```

### Log Outputs

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/meeting-.log",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "meetings/{MeetingId}/.metadata/events.log",
          "outputTemplate": "{Timestamp:o} [{Level}] {Message}{NewLine}"
        }
      }
    ]
  }
}
```

---

## Testing Strategy

### Unit Tests (70-80% Coverage Target)

```csharp
// Example: Testing FIFO turn manager
public class FifoTurnManagerTests
{
    [Fact]
    public void GetNextAgent_ReturnsAgentsInOrder()
    {
        // Arrange
        var agents = new[] { "agent1", "agent2", "agent3" };
        var manager = new FifoTurnManager(agents);
        
        // Act & Assert
        Assert.Equal("agent1", manager.GetNextAgent());
        Assert.Equal("agent2", manager.GetNextAgent());
        Assert.Equal("agent3", manager.GetNextAgent());
        Assert.Equal("agent1", manager.GetNextAgent()); // Wraps around
    }
    
    [Fact]
    public void GetNextAgent_WithSingleAgent_AlwaysReturnsSame()
    {
        // Arrange
        var manager = new FifoTurnManager(new[] { "agent1" });
        
        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal("agent1", manager.GetNextAgent());
        }
    }
}

// Example: Testing file locker
public class FileLockerTests
{
    [Fact]
    public async Task AcquireLock_WhenAvailable_ReturnsLock()
    {
        // Arrange
        var locker = new FileLocker();
        
        // Act
        using var lock = await locker.AcquireLockAsync(
            "test.md",
            "agent1",
            TimeSpan.FromSeconds(5),
            CancellationToken.None);
        
        // Assert
        Assert.NotNull(lock);
        Assert.Equal("agent1", lock.LockedByAgentId);
    }
    
    [Fact]
    public async Task AcquireLock_WhenLocked_ThrowsAfterTimeout()
    {
        // Arrange
        var locker = new FileLocker();
        using var existingLock = await locker.AcquireLockAsync(
            "test.md", "agent1", TimeSpan.FromSeconds(5), CancellationToken.None);
        
        // Act & Assert
        await Assert.ThrowsAsync<FileLockException>(
            () => locker.AcquireLockAsync(
                "test.md",
                "agent2",
                TimeSpan.FromMilliseconds(100),
                CancellationToken.None));
    }
}

// Example: Testing event bus
public class EventBusTests
{
    [Fact]
    public async Task Publish_NotifiesSubscribers()
    {
        // Arrange
        var eventBus = new InMemoryEventBus();
        var receivedEvent = false;
        
        eventBus.Subscribe<TurnStartedEvent>(evt =>
        {
            receivedEvent = true;
            return Task.CompletedTask;
        });
        
        // Act
        await eventBus.PublishAsync(new TurnStartedEvent("agent1", 1));
        
        // Assert
        Assert.True(receivedEvent);
    }
}
```

### Integration Tests

```csharp
public class BasicMeetingScenarioTests : IClassFixture<MeetingTestFixture>
{
    private readonly MeetingTestFixture _fixture;
    
    public BasicMeetingScenarioTests(MeetingTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task CompleteMeeting_GeneratesAllArtifacts()
    {
        // Arrange
        var config = _fixture.CreateTestConfiguration(
            agentCount: 3,
            maxMessages: 10);
            
        var orchestrator = _fixture.CreateOrchestrator(config);
        
        // Act
        var result = await orchestrator.RunMeetingAsync(CancellationToken.None);
        
        // Assert
        Assert.Equal(MeetingState.Completed, result.State);
        Assert.True(File.Exists(result.TranscriptPath));
        Assert.True(File.Exists(result.SummaryPath));
        Assert.InRange(result.MessageCount, 1, 10);
    }
    
    [Fact]
    public async Task Meeting_RespectsTimeLimit()
    {
        // Arrange
        var config = _fixture.CreateTestConfiguration(
            maxDurationMinutes: 1);
        var orchestrator = _fixture.CreateOrchestrator(config);
        var startTime = DateTime.UtcNow;
        
        // Act
        var result = await orchestrator.RunMeetingAsync(CancellationToken.None);
        var duration = DateTime.UtcNow - startTime;
        
        // Assert
        Assert.True(duration <= TimeSpan.FromMinutes(1.1)); // 10% buffer
        Assert.Equal("TimeLimit", result.EndReason);
    }
}
```

### Test Doubles

```csharp
// Mock Copilot client for testing
public class MockCopilotClient : ICopilotClient
{
    private readonly Queue<string> _responses = new();
    
    public void QueueResponse(string response)
    {
        _responses.Enqueue(response);
    }
    
    public Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        if (_responses.Count == 0)
            return Task.FromResult("Default test response");
            
        return Task.FromResult(_responses.Dequeue());
    }
}

// Fake event bus for testing
public class FakeEventBus : IEventBus
{
    public List<object> PublishedEvents { get; } = new();
    
    public Task PublishAsync<TEvent>(TEvent @event)
    {
        PublishedEvents.Add(@event);
        return Task.CompletedTask;
    }
    
    public void Subscribe<TEvent>(Func<TEvent, Task> handler)
    {
        // No-op for testing
    }
}
```

---

## Security Considerations

### 1. Configuration File Security

```csharp
// Prevent path traversal in agent config loading
public class AgentConfigurationLoader
{
    private readonly string _baseConfigPath;
    
    public async Task<AgentConfiguration> LoadAsync(string configFileName)
    {
        // Validate: no directory traversal
        if (configFileName.Contains("..") || 
            configFileName.Contains("/") ||
            configFileName.Contains("\\"))
        {
            throw new SecurityException(
                $"Invalid config file name: {configFileName}");
        }
        
        var fullPath = Path.Combine(_baseConfigPath, configFileName);
        
        // Ensure resolved path is within base directory
        var resolvedPath = Path.GetFullPath(fullPath);
        var resolvedBase = Path.GetFullPath(_baseConfigPath);
        
        if (!resolvedPath.StartsWith(resolvedBase))
        {
            throw new SecurityException(
                "Configuration file outside allowed directory");
        }
        
        return await LoadFromPathAsync(resolvedPath);
    }
}
```

### 2. Meeting Room Isolation

```csharp
// Each meeting gets isolated directory
public class MeetingRoom : IMeetingRoom
{
    private readonly string _meetingRoomPath;
    private readonly HashSet<string> _allowedPaths;
    
    public async Task WriteFileAsync(string relativePath, string content)
    {
        // Validate: only write within meeting room
        var fullPath = Path.Combine(_meetingRoomPath, relativePath);
        var resolvedPath = Path.GetFullPath(fullPath);
        
        if (!resolvedPath.StartsWith(_meetingRoomPath))
        {
            throw new SecurityException(
                "Cannot write outside meeting room");
        }
        
        // Validate: path was declared in allowed list (optional)
        if (_allowedPaths.Count > 0 && !_allowedPaths.Contains(relativePath))
        {
            throw new SecurityException(
                $"File path not in allowed list: {relativePath}");
        }
        
        await File.WriteAllTextAsync(resolvedPath, content);
    }
}
```

### 3. Copilot CLI Availability

```csharp
// Verify Copilot CLI is installed and accessible
public class CopilotClient : ICopilotClient
{
    private bool _isStarted;
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Initializing Copilot CLI connection...");
            await _client.StartAsync();
            _isStarted = true;
            _logger.LogInformation("Successfully connected to Copilot CLI");
        }
        catch (FileNotFoundException ex)
        {
            throw new InvalidOperationException(
                "GitHub Copilot CLI not found. Please ensure it is installed and in your PATH. " +
                "Visit https://github.com/github/copilot-cli for installation instructions.",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Copilot client");
            throw new InvalidOperationException(
                "Failed to initialize Copilot CLI connection. " +
                "Please verify your GitHub Copilot subscription is active.",
                ex);
        }
    }
    
    public override string ToString()
    {
        return $"CopilotClient(Started: {_isStarted})";
    }
}
```

### 4. Input Sanitization

```csharp
// Sanitize agent-generated content before file writes
public class ContentSanitizer
{
    public string SanitizeForFilePath(string input)
    {
        // Remove invalid file path characters
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(
            input.Where(c => !invalid.Contains(c)));
    }
    
    public string SanitizeContent(string content, int maxLength = 1_000_000)
    {
        // Prevent extremely large content
        if (content.Length > maxLength)
        {
            _logger.LogWarning(
                "Content truncated from {Original} to {Max} chars",
                content.Length,
                maxLength);
            return content.Substring(0, maxLength);
        }
        
        return content;
    }
}
```

### 5. Concurrent Request Management

```csharp
// Manage concurrent requests to Copilot (CLI handles rate limiting internally)
public class ThrottledCopilotClient : ICopilotClient
{
    private readonly ICopilotClient _inner;
    private readonly SemaphoreSlim _concurrencyLimiter;
    
    public ThrottledCopilotClient(
        ICopilotClient inner,
        int maxConcurrentRequests = 3)
    {
        _inner = inner;
        _concurrencyLimiter = new SemaphoreSlim(maxConcurrentRequests);
    }
    
    public async Task<string> GenerateAsync(
        string prompt,
        CopilotOptions options = null,
        CancellationToken cancellationToken = default)
    {
        // Limit concurrent requests to avoid overwhelming the CLI
        await _concurrencyLimiter.WaitAsync(cancellationToken);
        
        try
        {
            return await _inner.GenerateAsync(prompt, options, cancellationToken);
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _inner.StartAsync(cancellationToken);
    }
    
    public async Task StopAsync()
    {
        await _inner.StopAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _inner.DisposeAsync();
    }
}
```

---

## API Design (Internal)

### Core Interfaces

```csharp
// Meeting orchestration
public interface IMeetingOrchestrator
{
    Task<MeetingResult> RunMeetingAsync(
        MeetingConfiguration configuration,
        CancellationToken cancellationToken = default);
        
    Task<MeetingState> GetMeetingStateAsync(string meetingId);
    
    Task StopMeetingAsync(string meetingId, string reason);
}

// Agent interaction
public interface IAgent
{
    string AgentId { get; }
    string RoleName { get; }
    
    Task InitializeAsync(
        MeetingContext context,
        CancellationToken cancellationToken);
        
    Task<AgentResponse> RespondAsync(
        MeetingContext context,
        CancellationToken cancellationToken);
        
    Task<bool> ShouldParticipateAsync(MeetingContext context);
}

// Turn management
public interface ITurnManager
{
    void RegisterAgent(string agentId);
    void UnregisterAgent(string agentId);
    string GetNextAgent();
    bool HasAgents { get; }
}

// Event system
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}

// Meeting room access
public interface IMeetingRoom
{
    string MeetingRoomPath { get; }
    
    Task<string> ReadFileAsync(string relativePath);
    Task WriteFileAsync(string relativePath, string content);
    Task<bool> FileExistsAsync(string relativePath);
    Task<IEnumerable<string>> ListFilesAsync(string relativeDirectory = "");
    Task<IEnumerable<string>> SearchContentAsync(string query);
    Task<IFileLock> AcquireLockAsync(
        string relativePath,
        string agentId,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}

// Copilot integration
public interface ICopilotClient : IAsyncDisposable
{
    Task StartAsync(CancellationToken cancellationToken = default);
    
    Task StopAsync();
    
    Task<string> GenerateAsync(
        string prompt,
        CopilotOptions options = null,
        CancellationToken cancellationToken = default);
        
    Task<int> EstimateTokensAsync(string text);
}

// Prompt building
public interface IPromptBuilder
{
    string BuildAgentPrompt(
        AgentConfiguration agentConfig,
        MeetingContext context);
        
    string BuildModeratorPrompt(MeetingContext context);
    
    string BuildSummaryPrompt(
        IEnumerable<Message> messages,
        MeetingConfiguration config);
}
```

### Key Models

```csharp
public class MeetingConfiguration
{
    public string MeetingTopic { get; set; }
    public List<string> AgentConfigPaths { get; set; }
    public MeetingLimits HardLimits { get; set; }
    public TurnTakingStrategy TurnStrategy { get; set; }
    public string MeetingRoomBasePath { get; set; }
    public bool EnableArtifactSearch { get; set; }
    public LogLevel LogLevel { get; set; }
}

public class MeetingLimits
{
    public int? MaxDurationMinutes { get; set; }
    public int? MaxTotalMessages { get; set; }
    public int? MaxMessagesPerAgent { get; set; }
    public int? MaxTokensTotal { get; set; }
    public TimeSpan? MaxTurnDuration { get; set; }
}

public class MeetingContext
{
    public string MeetingId { get; set; }
    public string Topic { get; set; }
    public List<Message> Messages { get; set; }
    public Dictionary<string, IAgent> Agents { get; set; }
    public MeetingState State { get; set; }
    public DateTime StartedAt { get; set; }
    public int TotalTokensUsed { get; set; }
    public string CurrentSpeakingAgentId { get; set; }
}

public class Message
{
    public string MessageId { get; set; }
    public string AgentId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string ReplyToMessageId { get; set; }
    public MessageType Type { get; set; }  // Statement, Question, Response
}

public class AgentConfiguration
{
    public string Role { get; set; }
    public string Description { get; set; }
    public List<string> PersonaTraits { get; set; }
    public List<string> Instructions { get; set; }
    public string InitialMessageTemplate { get; set; }
    public string ResponseStyle { get; set; }
    public int MaxMessageLength { get; set; }
    public List<string> ExpertiseAreas { get; set; }
}

public class MeetingResult
{
    public string MeetingId { get; set; }
    public MeetingState State { get; set; }
    public string EndReason { get; set; }
    public TimeSpan Duration { get; set; }
    public int MessageCount { get; set; }
    public int TokensUsed { get; set; }
    public string TranscriptPath { get; set; }
    public string SummaryPath { get; set; }
    public List<string> ActionItems { get; set; }
    public Dictionary<string, int> AgentParticipation { get; set; }
}

public enum MeetingState
{
    NotStarted,
    Initializing,
    InProgress,
    Paused,
    EndingGracefully,
    Completed,
    Failed,
    Cancelled
}

public enum TurnTakingStrategy
{
    FIFO,
    Priority,      // Future
    Dynamic        // Future
}
```

---

## GitHub Copilot SDK Integration

### SDK Setup

```csharp
// Install package
// dotnet add package GitHub.Copilot.SDK --version <latest>

// Note: No API key needed! The SDK uses the Copilot CLI which handles authentication
// Users must have GitHub Copilot access and the CLI installed

// Configuration
public class CopilotConfiguration
{
    public string Model { get; set; } = "gpt-5";
    public int MaxTokens { get; set; } = 1000;
    public double Temperature { get; set; } = 0.7;
    public TimeSpan ResponseTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

// Client implementation
public class CopilotClient : ICopilotClient
{
    private readonly GitHub.Copilot.SDK.CopilotClient _client;
    private readonly CopilotConfiguration _configuration;
    private readonly ILogger<CopilotClient> _logger;
    private bool _isStarted;
    
    public CopilotClient(
        CopilotConfiguration configuration,
        ILogger<CopilotClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _client = new GitHub.Copilot.SDK.CopilotClient();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isStarted) return;
        
        _logger.LogInformation("Starting Copilot client...");
        await _client.StartAsync();
        _isStarted = true;
        _logger.LogInformation("Copilot client started successfully");
    }
    
    public async Task StopAsync()
    {
        if (!_isStarted) return;
        
        _logger.LogInformation("Stopping Copilot client...");
        var errors = await _client.StopAsync();
        if (errors.Count > 0)
        {
            _logger.LogWarning("Cleanup errors: {Errors}", string.Join(", ", errors));
        }
        _isStarted = false;
    }
    
    public async Task<string> GenerateAsync(
        string prompt,
        CopilotOptions options = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isStarted)
        {
            throw new InvalidOperationException(
                "CopilotClient must be started before generating responses. Call StartAsync() first.");
        }
        
        options ??= new CopilotOptions();
        
        _logger.LogDebug(
            "Generating completion: Model={Model}, PromptLength={PromptLength}",
            options.Model ?? _configuration.Model,
            prompt.Length);
        
        try
        {
            // Create a temporary session for this single request
            var session = await _client.CreateSessionAsync(new SessionConfig
            {
                Model = options.Model ?? _configuration.Model
            });
            
            try
            {
                var responseBuilder = new StringBuilder();
                var responseComplete = new TaskCompletionSource<string>();
                
                // Subscribe to events
                session.On(evt =>
                {
                    if (evt is AssistantMessageEvent msg)
                    {
                        responseBuilder.Append(msg.Data.Content);
                    }
                    else if (evt is MessageDoneEvent)
                    {
                        responseComplete.TrySetResult(responseBuilder.ToString());
                    }
                    else if (evt is ErrorEvent errorEvt)
                    {
                        responseComplete.TrySetException(
                            new CopilotApiException(errorEvt.Data.Message));
                    }
                });
                
                // Send the prompt
                await session.SendAsync(new MessageOptions { Prompt = prompt });
                
                // Wait for response with timeout
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(_configuration.ResponseTimeout);
                
                var response = await responseComplete.Task.WaitAsync(cts.Token);
                
                _logger.LogDebug(
                    "Completion generated: ResponseLength={ResponseLength}",
                    response.Length);
                    
                return response;
            }
            finally
            {
                // Clean up session
                await session.DisposeAsync();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request timed out after {Timeout}", _configuration.ResponseTimeout);
            throw new TurnTimeoutException(
                $"Request timed out after {_configuration.ResponseTimeout}",
                "unknown",
                _configuration.ResponseTimeout);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Copilot CLI not found. Please install it first.");
            throw new CopilotApiException(
                "Copilot CLI not found. Please ensure GitHub Copilot CLI is installed.",
                innerException: ex);
        }
        catch (Exception ex) when (ex is not CopilotApiException)
        {
            _logger.LogError(ex, "Unexpected error during completion generation");
            throw new CopilotApiException(
                "Failed to generate completion",
                innerException: ex);
        }
    }
    
    public async Task<int> EstimateTokensAsync(string text)
    {
        // Use tokenizer from SDK or rough estimation
        // ~4 characters per token for English text
        return await Task.FromResult(text.Length / 4);
    }
    
    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}

public class CopilotOptions
{
    public string Model { get; set; }
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }
    public List<string> StopSequences { get; set; }
}
```

### Prompt Engineering

```csharp
public class PromptBuilder : IPromptBuilder
{
    public string BuildAgentPrompt(
        AgentConfiguration agentConfig,
        MeetingContext context)
    {
        var sb = new StringBuilder();
        
        // System context
        sb.AppendLine("# Meeting Context");
        sb.AppendLine($"Topic: {context.Topic}");
        sb.AppendLine($"Participants: {string.Join(", ", context.Agents.Keys)}");
        sb.AppendLine($"Current message count: {context.Messages.Count}");
        sb.AppendLine();
        
        // Role definition
        sb.AppendLine($"# Your Role: {agentConfig.Role}");
        sb.AppendLine(agentConfig.Description);
        sb.AppendLine();
        
        // Persona
        sb.AppendLine("# Your Persona:");
        foreach (var trait in agentConfig.PersonaTraits)
        {
            sb.AppendLine($"- {trait}");
        }
        sb.AppendLine();
        
        // Instructions
        sb.AppendLine("# Instructions:");
        foreach (var instruction in agentConfig.Instructions)
        {
            sb.AppendLine($"- {instruction}");
        }
        sb.AppendLine();
        
        // Recent conversation history (last 10 messages)
        sb.AppendLine("# Recent Conversation:");
        var recentMessages = context.Messages
            .TakeLast(10)
            .ToList();
            
        if (recentMessages.Count == 0)
        {
            sb.AppendLine("(Meeting just started, no messages yet)");
        }
        else
        {
            foreach (var msg in recentMessages)
            {
                var agent = context.Agents[msg.AgentId];
                sb.AppendLine($"[{msg.Timestamp:HH:mm:ss}] {agent.RoleName}: {msg.Content}");
            }
        }
        sb.AppendLine();
        
        // Response guidance
        sb.AppendLine("# Your Response:");
        sb.AppendLine($"Style: {agentConfig.ResponseStyle}");
        sb.AppendLine($"Max length: {agentConfig.MaxMessageLength} characters");
        sb.AppendLine();
        sb.AppendLine("Provide your next contribution to the meeting discussion.");
        sb.AppendLine("Stay in character and follow your role's instructions.");
        
        return sb.ToString();
    }
    
    public string BuildModeratorPrompt(MeetingContext context)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("# Moderator Decision Required");
        sb.AppendLine();
        sb.AppendLine($"Topic: {context.Topic}");
        sb.AppendLine($"Duration: {DateTime.UtcNow - context.StartedAt:hh\\:mm\\:ss}");
        sb.AppendLine($"Messages: {context.Messages.Count}");
        sb.AppendLine();
        
        sb.AppendLine("# Recent Discussion:");
        foreach (var msg in context.Messages.TakeLast(5))
        {
            var agent = context.Agents[msg.AgentId];
            sb.AppendLine($"{agent.RoleName}: {msg.Content.Substring(0, Math.Min(100, msg.Content.Length))}...");
        }
        sb.AppendLine();
        
        sb.AppendLine("# Your Task:");
        sb.AppendLine("Analyze the meeting progress and decide:");
        sb.AppendLine("1. Should the meeting continue? (YES/NO)");
        sb.AppendLine("2. Reason for your decision");
        sb.AppendLine();
        sb.AppendLine("Decision criteria:");
        sb.AppendLine("- Has a consensus been reached?");
        sb.AppendLine("- Are agents repeating themselves?");
        sb.AppendLine("- Is the discussion productive?");
        sb.AppendLine("- Are all important points covered?");
        sb.AppendLine("- Is there unresolved conflict?");
        sb.AppendLine();
        sb.AppendLine("Respond in format:");
        sb.AppendLine("CONTINUE: [YES/NO]");
        sb.AppendLine("REASON: [brief explanation]");
        
        return sb.ToString();
    }
}
```

---

## Usage Examples

### Basic Meeting

```bash
# Command line
dotnet run --project src/AgentMeetingSystem.CLI/AgentMeetingSystem.CLI.csproj -- \
    start-meeting \
    --topic "Discuss architecture for payment service refactoring" \
    --agents "config/agents/project-manager.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/security-expert.txt" \
             "config/agents/moderator.txt" \
    --max-duration 30 \
    --max-messages 50

# Output:
# [14:30:22 INF] Meeting initialized: payment_service_refactoring_20260130_143022
# [14:30:23 INF] Loading 4 agents...
# [14:30:24 INF] Agent loaded: Project Manager (pm-001)
# [14:30:25 INF] Agent loaded: Senior Developer (dev-001)
# [14:30:26 INF] Agent loaded: Security Expert (sec-001)
# [14:30:27 INF] Agent loaded: Moderator (mod-001)
# [14:30:28 INF] Meeting started
#
# ┌─────────────────────────────────────────────────────────────┐
# │ Meeting: payment_service_refactoring_20260130_143022        │
# │ Topic: Discuss architecture for payment service refactoring │
# │ Agents: 4 | Messages: 0/50 | Duration: 00:00:00/00:30:00   │
# └─────────────────────────────────────────────────────────────┘
#
# [Project Manager]: As Project Manager, I'd like to understand the current
# pain points with our payment service and what goals we're trying to achieve
# with this refactoring. What's driving this initiative?
#
# [Senior Developer]: The main issues are scalability and maintainability.
# Our monolithic payment processor is becoming a bottleneck...
#
# ... (meeting continues) ...
#
# [14:45:30 INF] Meeting ended: Moderator decision - Consensus reached
# [14:45:32 INF] Generating summary...
# [14:45:35 INF] Meeting artifacts saved to:
#   - meetings/payment_service_refactoring_20260130_143022/transcript.md
#   - meetings/payment_service_refactoring_20260130_143022/summary.md
#   - meetings/payment_service_refactoring_20260130_143022/action_items.md
```

### Configuration File

```bash
# Using JSON configuration
dotnet run --project src/AgentMeetingSystem.CLI/AgentMeetingSystem.CLI.csproj -- \
    start-meeting \
    --config "config/meetings/architecture-review.json"
```

```json
// config/meetings/architecture-review.json
{
  "meetingTopic": "Review microservices architecture proposal",
  "agentConfigs": [
    "agents/project-manager.txt",
    "agents/senior-developer.txt",
    "agents/devops-engineer.txt",
    "agents/security-expert.txt",
    "agents/moderator.txt"
  ],
  "hardLimits": {
    "maxDurationMinutes": 45,
    "maxTotalMessages": 100,
    "maxMessagesPerAgent": 30,
    "maxTurnDuration": "00:02:00"
  },
  "turnTakingStrategy": "FIFO",
  "meetingRoomPath": "./meetings/{timestamp}_{topic_slug}",
  "enableArtifactSearch": true,
  "logLevel": "Information"
}
```

### Programmatic Usage

```csharp
// Program.cs or custom application
var services = new ServiceCollection();

// Configure services
services.AddSingleton<IEventBus, InMemoryEventBus>();
services.AddSingleton<ITurnManager, FifoTurnManager>();
services.AddSingleton<IPromptBuilder, PromptBuilder>();
services.AddSingleton<IMeetingOrchestrator, MeetingOrchestrator>();

// Configure Copilot (no API key needed!)
services.AddSingleton(new CopilotConfiguration
{
    Model = "gpt-5",  // or "claude-sonnet-4.5"
    MaxTokens = 1000,
    ResponseTimeout = TimeSpan.FromSeconds(30)
});
services.AddSingleton<ICopilotClient, CopilotClient>();

// Configure logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddFile("logs/meeting-.log");
});

var serviceProvider = services.BuildServiceProvider();

// Initialize Copilot client
var copilotClient = serviceProvider.GetRequiredService<ICopilotClient>();
await copilotClient.StartAsync();

try
{
    // Create meeting configuration
var meetingConfig = new MeetingConfiguration
{
    MeetingTopic = "Discuss API versioning strategy",
    AgentConfigPaths = new List<string>
    {
        "config/agents/project-manager.txt",
        "config/agents/senior-developer.txt",
        "config/agents/moderator.txt"
    },
    HardLimits = new MeetingLimits
    {
        MaxDurationMinutes = 30,
        MaxTotalMessages = 50
    },
    TurnStrategy = TurnTakingStrategy.FIFO
};

// Run meeting
var orchestrator = serviceProvider.GetRequiredService<IMeetingOrchestrator>();
var result = await orchestrator.RunMeetingAsync(
    meetingConfig,
    CancellationToken.None);

    Console.WriteLine($"Meeting completed: {result.State}");
    Console.WriteLine($"Duration: {result.Duration}");
    Console.WriteLine($"Messages: {result.MessageCount}");
    Console.WriteLine($"Summary: {result.SummaryPath}");
}
finally
{
    // Clean up Copilot client
    await copilotClient.StopAsync();
}
```

---

## Development Roadmap

### Phase 1: Core Foundation (v0.1)
**Goal**: Basic working system with essential features

- [ ] Project structure setup
- [ ] Event bus implementation
- [ ] Basic agent system
- [ ] FIFO turn manager
- [ ] Meeting orchestrator
- [ ] Copilot SDK integration
- [ ] File-based agent configuration
- [ ] Simple CLI interface
- [ ] Meeting transcript generation
- [ ] Unit tests for core components

**Deliverable**: Can run 3-agent meeting with transcript output

### Phase 2: Meeting Room & Artifacts (v0.2)
**Goal**: Shared workspace and artifact generation

- [ ] Meeting room file system
- [ ] File locking mechanism
- [ ] Moderator agent implementation
- [ ] Scribe agent implementation
- [ ] Summary generation
- [ ] Action items extraction
- [ ] Decision log
- [ ] Artifact search functionality
- [ ] Integration tests

**Deliverable**: Complete meeting with all artifacts generated

### Phase 3: Robustness & Monitoring (v0.3)
**Goal**: Production-ready error handling and observability

- [ ] Comprehensive error handling
- [ ] Structured logging (Serilog)
- [ ] Meeting metrics
- [ ] Hard limit enforcement
- [ ] Graceful degradation
- [ ] Agent timeout handling
- [ ] Rate limiting
- [ ] Retry policies
- [ ] Health checks

**Deliverable**: Reliable system with good observability

### Phase 4: Advanced Features (v0.4)
**Goal**: Enhanced capabilities and flexibility

- [ ] Priority-based turn taking
- [ ] Dynamic turn allocation
- [ ] Vote/consensus mechanisms
- [ ] Meeting pause/resume
- [ ] Agent hot-reload
- [ ] Custom event handlers
- [ ] Plugin system
- [ ] Advanced prompt templates
- [ ] Multi-meeting orchestration

**Deliverable**: Flexible system supporting complex scenarios

### Phase 5: Polish & Documentation (v1.0)
**Goal**: Production release

- [ ] Complete documentation
- [ ] Example configurations
- [ ] Tutorial videos/guides
- [ ] Performance optimization
- [ ] Security audit
- [ ] Accessibility improvements
- [ ] Deployment scripts
- [ ] Docker support
- [ ] CI/CD pipeline

**Deliverable**: Production-ready v1.0 release

---

## Git Workflow

### Branch Strategy: GitHub Flow

```
main (always deployable)
├── feature/event-bus-implementation
├── feature/agent-system
├── feature/copilot-integration
├── feature/meeting-orchestrator
└── hotfix/file-lock-deadlock
```

### Commit Message Convention

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `refactor`: Code refactoring
- `test`: Test additions/changes
- `chore`: Maintenance tasks

**Examples**:
```
feat(agents): implement ModeratorAgent with meeting end detection

- Add ShouldEndMeetingAsync method
- Integrate with MeetingOrchestrator
- Add unit tests for decision logic

Closes #42

---

fix(file-lock): prevent deadlock when multiple agents access same file

Previously, agents could deadlock if they acquired locks in different
orders. Now implements timeout-based locking with proper cleanup.

Fixes #67

---

docs(readme): add quick start guide and examples

---

test(orchestrator): add integration tests for time limit enforcement
```

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing performed

## Checklist
- [ ] Code follows project naming conventions
- [ ] Self-review performed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings generated
```

---

## Dependencies

### Core Dependencies

```xml
<!-- AgentMeetingSystem.Core.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <!-- Microsoft packages -->
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
    
    <!-- Serialization -->
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
    
    <!-- Utilities -->
    <PackageReference Include="Humanizer.Core" Version="2.14.1" />
  </ItemGroup>
</Project>

<!-- AgentMeetingSystem.CLI.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\AgentMeetingSystem.Core\AgentMeetingSystem.Core.csproj" />
    <ProjectReference Include="..\AgentMeetingSystem.Copilot\AgentMeetingSystem.Copilot.csproj" />
    <ProjectReference Include="..\AgentMeetingSystem.Infrastructure\AgentMeetingSystem.Infrastructure.csproj" />
    
    <!-- CLI framework -->
    <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.22272.1" />
    
    <!-- Console UI -->
    <PackageReference Include="Spectre.Console" Version="0.47.0" />
  </ItemGroup>
</Project>

<!-- AgentMeetingSystem.Copilot.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\AgentMeetingSystem.Core\AgentMeetingSystem.Core.csproj" />
    
    <!-- GitHub Copilot SDK (no API key required) -->
    <PackageReference Include="GitHub.Copilot.SDK" Version="1.0.0" />
    <!-- Note: Requires GitHub Copilot CLI to be installed separately -->
  </ItemGroup>
</Project>

<!-- AgentMeetingSystem.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\AgentMeetingSystem.Core\AgentMeetingSystem.Core.csproj" />
    
    <!-- Logging -->
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
    <PackageReference Include="Serilog.Extensions.Logging" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### Test Dependencies

```xml
<!-- AgentMeetingSystem.Core.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\AgentMeetingSystem.Core\AgentMeetingSystem.Core.csproj" />
    
    <!-- Testing frameworks -->
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4" />
    
    <!-- Mocking -->
    <PackageReference Include="Moq" Version="4.20.70" />
    
    <!-- Assertions -->
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    
    <!-- Coverage -->
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
  </ItemGroup>
</Project>
```

---

## Prerequisites

### GitHub Copilot CLI
This system requires the GitHub Copilot CLI to be installed and accessible in PATH:
- Users must have an active GitHub Copilot subscription
- Install the CLI: https://github.com/github/copilot-cli
- No API keys or tokens needed - CLI handles authentication

### System Requirements
- .NET 8.0 or later
- GitHub Copilot CLI installed
- Active GitHub Copilot subscription
- ~500MB disk space for meetings

---

## Open Questions & Decisions Needed

### 1. RAG Integration
**Question**: Should agents have access to external knowledge bases?
- **Option A**: No RAG, agents rely only on their prompts and meeting context (simpler)
- **Option B**: Integrate vector database for document retrieval (more powerful)
- **Recommendation**: Start with Option A, add RAG in Phase 4

### 2. Agent Memory
**Question**: Should agents maintain memory across meetings?
- **Option A**: Each meeting is isolated, no cross-meeting memory
- **Option B**: Agents can reference previous meetings
- **Recommendation**: Option A initially, with artifact search within current meeting

### 3. Turn Duration
**Question**: How long should an agent have to respond?
- **Option A**: Fixed timeout (e.g., 30 seconds)
- **Option B**: Dynamic based on complexity
- **Option C**: User-configurable per agent
- **Recommendation**: Option C, with sensible defaults

### 4. Interruptions
**Question**: Can agents interrupt each other?
- **Option A**: Strict turn-taking, no interruptions
- **Option B**: Agents can "raise hand" for urgent input
- **Recommendation**: Option A for v1.0, Option B for future

### 5. Error Recovery
**Question**: What happens if an agent fails mid-meeting?
- **Option A**: End meeting immediately
- **Option B**: Remove agent, continue with remaining
- **Option C**: Retry failed agent, then proceed to Option B
- **Recommendation**: Option C with max retry limit

---

## Success Metrics

### Technical Metrics
- **Meeting Completion Rate**: >95% of meetings complete successfully
- **Average Meeting Duration**: Matches configured limits ±10%
- **Agent Response Time**: <30 seconds per turn average
- **Test Coverage**: >75% code coverage
- **Build Success Rate**: >98% on main branch

### Quality Metrics
- **Artifact Generation**: 100% of completed meetings produce all artifacts
- **File Lock Contention**: <5% of file operations wait for locks
- **Error Rate**: <2% of agent turns fail
- **Log Clarity**: All errors include actionable context

### User Experience Metrics
- **Setup Time**: <5 minutes from clone to first meeting
- **Configuration Clarity**: Users can create custom agents without code
- **Output Quality**: Meeting summaries capture key points and decisions

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| **Copilot SDK API changes** | High | Medium | Abstract SDK behind interface, pin versions |
| **Agent response quality varies** | Medium | High | Comprehensive prompt engineering, moderator oversight |
| **File lock deadlocks** | High | Low | Timeout-based locking, single lock per agent rule |
| **Copilot CLI not installed** | High | Medium | Clear error messages, setup validation, documentation |
| **Meeting never ends** | High | Low | Hard time limits, moderator decision, max messages |
| **Agent context window exceeded** | Medium | Medium | Sliding window history, summarization |
| **Concurrent file write corruption** | High | Low | Proper file locking, atomic writes |

---

## Conclusion

This draft outlines a comprehensive agent meeting system built with C# and the GitHub Copilot SDK. The architecture emphasizes:

1. **Modularity**: Clean separation between orchestration, agents, events, and file system
2. **Configurability**: Text-based agent configuration supports any use case
3. **Robustness**: Comprehensive error handling and graceful degradation
4. **Extensibility**: Event-driven design enables future enhancements
5. **Observability**: Structured logging and metrics throughout

The system is designed to scale from simple 3-agent discussions to complex multi-agent collaborations while maintaining clarity and maintainability.

---

## Next Steps

1. **Review & Refine**: Review this draft, clarify open questions, challenge assumptions
2. **Prototype**: Build minimal working version (Phase 1 scope)
3. **Iterate**: Test with real scenarios, gather feedback, refine
4. **Expand**: Add features incrementally following roadmap
5. **Document**: Maintain documentation as system evolves

---

**Document Status**: Draft v0.1  
**Last Updated**: January 30, 2026  
**Contributors**: [Your Name]  
**Next Review**: After prototype completion
