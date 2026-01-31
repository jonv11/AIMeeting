# Architecture Documentation

**See also**: [Full Documentation Hub](docs/) | [API Reference](docs/reference/api.md)

## Table of Contents

1. [High-Level Overview](#high-level-overview)
2. [Component Architecture](#component-architecture)
3. [Detailed Component Design](#detailed-component-design)
4. [Event System](#event-system)
5. [Turn-Taking Strategy](#turn-taking-strategy)
6. [File System & Locking](#file-system--locking)
7. [Error Handling](#error-handling)
8. [Logging Strategy](#logging-strategy)
9. [Security Considerations](#security-considerations)
10. [Performance Considerations](#performance-considerations)

## High-Level Overview

AIMeeting is a modular, event-driven system for orchestrating multi-agent meetings. The architecture emphasizes clean separation of concerns, testability, and extensibility.

### Architecture Diagram

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
        │ GitHub Copilot CLI     │
        │ - Prompt execution     │
        │ - CLI communication    │
        └────────────────────────┘
```

## Component Architecture

### 1. CLI Interface (`AIMeeting.CLI`)

**Purpose**: Entry point for users to initiate and monitor meetings

**Responsibilities**:
- Parse command-line arguments
- Load agent configurations
- Display real-time meeting progress
- Handle graceful shutdown on user interrupt

**Key Classes**:
- `Program.cs`: Application entry point
- `Commands/StartMeetingCommand.cs`: Meeting initiation
- `Commands/ListConfigsCommand.cs`: List available agents (optional for v0.1)
- `Commands/ValidateConfigCommand.cs`: Validate configurations
- `Display/MeetingProgressDisplay.cs`: Real-time meeting UI

**Dependencies**: System.CommandLine, Spectre.Console

### 2. Meeting Orchestrator (`AIMeeting.Core`)

**Purpose**: Central coordinator for meeting execution

**Responsibilities**:
- Initialize meeting workspace
- Load and instantiate agents
- Enforce hard limits (time, message count)
- Coordinate agent turns
- Generate final meeting artifacts

**Key Classes**:
- `Orchestration/MeetingOrchestrator.cs`: Main coordination logic
- `Orchestration/MeetingStateMachine.cs`: State management
- `Orchestration/FifoTurnManager.cs`: Turn sequencing

**State Machine**:
```
    NotStarted
        ↓
   Initializing (loading agents, validation)
        ↓
   InProgress (agents taking turns)
        ↓ (on limit hit or cancellation)
   EndingGracefully (finalizing artifacts)
        ↓
    Completed/Failed
```

### 3. Agent System (`AIMeeting.Core/Agents`)

**Base Architecture**:
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
}

// Standard agent using Copilot CLI
public class StandardAgent : AgentBase
{
    private readonly ICopilotClient _copilotClient;
    private readonly IPromptBuilder _promptBuilder;
}

// Special agent for meeting management
public class ModeratorAgent : AgentBase
{
    public Task<bool> ShouldEndMeetingAsync(MeetingContext context);
}
```

**Agent Lifecycle**:
1. **Initialization**: Load configuration, validate, prepare prompts
2. **Standby**: Wait for turn signal from orchestrator
3. **Active**: Generate response based on meeting context
4. **Response**: Publish message to event bus
5. **Documentation**: Register message in meeting transcript

### 4. Message Bus / Event System (`AIMeeting.Core/Events`)

**Purpose**: Decoupled pub/sub communication between agents and orchestrator

**Core Events**:
```csharp
// Agent lifecycle
public class AgentJoinedEvent { public string AgentId { get; set; } }
public class AgentLeftEvent { public string AgentId { get; set; } }

// Turn management
public class TurnStartedEvent { public string AgentId { get; set; } }
public class TurnCompletedEvent { public string AgentId { get; set; } public string Message { get; set; } }

// Meeting control
public class MeetingStartedEvent { public MeetingConfiguration Config { get; set; } }
public class MeetingEndingEvent { public string Reason { get; set; } }
public class MeetingEndedEvent { public string EndReason { get; set; } }

// File operations
public class FileCreatedEvent { public string AgentId { get; set; } public string FilePath { get; set; } }
public class FileLockedEvent { public string FilePath { get; set; } public string AgentId { get; set; } }
```

**Implementation**:
```csharp
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}

public class InMemoryEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    private readonly SemaphoreSlim _mutex = new(1, 1);
    
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        await _mutex.WaitAsync();
        try
        {
            if (_subscribers.TryGetValue(typeof(TEvent), out var handlers))
            {
                var tasks = handlers
                    .Cast<Func<TEvent, Task>>()
                    .Select(h => h(@event));
                await Task.WhenAll(tasks);
            }
        }
        finally
        {
            _mutex.Release();
        }
    }
    
    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (!_subscribers.ContainsKey(eventType))
            _subscribers[eventType] = new List<Delegate>();
        
        _subscribers[eventType].Add(handler);
        return new Subscription<TEvent>(this, handler);
    }
}
```

## Detailed Component Design

### Meeting Room (`AIMeeting.Core/FileSystem`)

Each meeting gets an isolated file system space:

```
meetings/
└── 20260130_143022_payment_refactoring/
    ├── meeting.json                   # Meeting metadata snapshot
    ├── transcript.md                  # Real-time conversation
    └── errors.log                     # Error details (if failures occur)
```

**File Locking Strategy**:
- Single lock per file
- Timeout-based acquisition (no deadlock)
- Automatic cleanup on disposal
- Lock state maintained in memory for MVP

```csharp
public interface IFileLocker
{
    Task<IFileLock> AcquireLockAsync(
        string filePath,
        string agentId,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}

public class FileLocker : IFileLocker
{
    private readonly Dictionary<string, FileLockInfo> _locks = new();
    private readonly SemaphoreSlim _lockTable = new(1, 1);
    
    public async Task<IFileLock> AcquireLockAsync(
        string filePath,
        string agentId,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.Add(timeout);
        
        while (DateTime.UtcNow < deadline)
        {
            await _lockTable.WaitAsync(cancellationToken);
            try
            {
                if (!_locks.ContainsKey(filePath))
                {
                    var lockInfo = new FileLockInfo
                    {
                        FilePath = filePath,
                        LockedByAgentId = agentId,
                        AcquiredAt = DateTime.UtcNow
                    };
                    _locks[filePath] = lockInfo;
                    return new FileLock(this, lockInfo);
                }
            }
            finally
            {
                _lockTable.Release();
            }
            
            // Wait before retrying
            await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
        }
        
        throw new FileLockException(
            $"Could not acquire lock on {filePath}",
            filePath,
            agentId);
    }
}
```

### Configuration System (`AIMeeting.Core/Configuration`)

**Agent Configuration Format** (text-based for user accessibility):

```
ROLE: Project Manager
DESCRIPTION: Oversees project timeline and resources

PERSONA:
- Focused on practical outcomes
- Asks clarifying questions about scope
- Advocates for realistic timelines

INSTRUCTIONS:
- Consider project constraints
- Push for clear action items
- Flag risks early
- Summarize decisions

RESPONSE_STYLE: Professional, direct, action-oriented
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Planning, Risk Management
```

**Loading Process**:
1. Parse configuration file with key-value pairs
2. Validate required fields (ROLE, DESCRIPTION, INSTRUCTIONS)
3. Build `AgentConfiguration` object
4. Create agent instance via `AgentFactory`
5. Store configuration snapshot in meeting room

### Prompt Engineering (`AIMeeting.Core/Prompts`)

**Strategy**: Build context-aware prompts for each turn

```csharp
public class PromptBuilder : IPromptBuilder
{
    public string BuildAgentPrompt(
        AgentConfiguration config,
        MeetingContext context)
    {
        var sb = new StringBuilder();
        
        // System context
        sb.AppendLine($"# Topic: {context.Topic}");
        sb.AppendLine($"# Participants: {string.Join(", ", context.Agents.Keys)}");
        
        // Role definition
        sb.AppendLine($"## Your Role: {config.Role}");
        sb.AppendLine(config.Description);
        
        // Persona traits (if provided)
        if (config.PersonaTraits.Count > 0)
        {
            sb.AppendLine("## Your Persona:");
            foreach (var trait in config.PersonaTraits)
            {
                sb.AppendLine($"- {trait}");
            }
        }
        
        // Instructions
        sb.AppendLine("## Your Instructions:");
        foreach (var instruction in config.Instructions)
        {
            sb.AppendLine($"- {instruction}");
        }
        
        // Recent conversation (sliding window)
        sb.AppendLine("## Recent Discussion:");
        var recentMessages = context.Messages.TakeLast(10);
        foreach (var msg in recentMessages)
        {
            var agent = context.Agents[msg.AgentId];
            sb.AppendLine($"[{agent.RoleName}]: {msg.Content}");
        }
        
        // Response guidance
        sb.AppendLine($"## Respond (max {config.MaxMessageLength} chars):");
        sb.AppendLine("Provide your next contribution to the discussion.");
        
        return sb.ToString();
    }
}
```

## Event System

### Event Flow

```
Agent generates response
    ↓
TurnCompletedEvent published to bus
    ↓
┌─────────────────────────────────────────────┐
│ Orchestrator receives event                 │
│ - Updates meeting context                   │
│ - Checks hard limits                        │
│ - Signals next agent's turn (if continuing) │
└─────────────────────────────────────────────┘
    ↓
FileSystem receives event (if applicable)
    │ - Updates transcript
    │ - Records metrics (future)
    │
MeetingRoom receives event
    │ - Publishes FileModifiedEvent
    │
Logger receives event
    - Records event to log stream
```

### Event Subscription Example

```csharp
// Subscribe to all turn completions
eventBus.Subscribe<TurnCompletedEvent>(async (evt) =>
{
    logger.LogInformation(
        "Agent {AgentId} completed turn with {CharCount} characters",
        evt.AgentId,
        evt.Message.Length);
    
    // Update transcript file
    await meetingRoom.WriteFileAsync(
        "transcript.md",
        $"[{evt.AgentId}]: {evt.Message}\n");
});

// Subscribe to meeting end
eventBus.Subscribe<MeetingEndedEvent>(async (evt) =>
{
    logger.LogInformation("Meeting ended: {Reason}", evt.EndReason);
});
```

## Turn-Taking Strategy

### FIFO (Default)

Agents take turns in a fixed order, cycling back to the beginning.

```csharp
public class FifoTurnManager : ITurnManager
{
    private readonly Queue<string> _agents = new();
    
    public string GetNextAgent()
    {
        var next = _agents.Dequeue();
        _agents.Enqueue(next);  // Re-add to end
        return next;
    }
}

// Usage
var manager = new FifoTurnManager(["agent1", "agent2", "agent3"]);
manager.GetNextAgent();  // → "agent1"
manager.GetNextAgent();  // → "agent2"
manager.GetNextAgent();  // → "agent3"
manager.GetNextAgent();  // → "agent1" (wraps)
```

### Future Strategies

**Priority-Based** (v0.3+): 
- Moderator always goes last
- Experts prioritized when raising hand
- General agents fill remaining slots

**Dynamic** (v0.3+):
- Agents "raise hand" based on relevance
- Orchestrator routes to most appropriate agent
- Enables interruption mechanism

## File System & Locking

### Isolation

Each meeting operates in a completely isolated directory:

```csharp
public class MeetingRoom : IMeetingRoom
{
    private readonly string _meetingRoomPath;
    private readonly IFileLocker _fileLocker;
    
    public async Task WriteFileAsync(string relativePath, string content)
    {
        // Validate path is within meeting room
        var fullPath = Path.Combine(_meetingRoomPath, relativePath);
        var resolved = Path.GetFullPath(fullPath);
        
        if (!resolved.StartsWith(_meetingRoomPath))
            throw new SecurityException("Path traversal attempt");
        
        // Acquire lock
        using var fileLock = await _fileLocker.AcquireLockAsync(
            resolved, 
            "system", 
            TimeSpan.FromSeconds(5));
        
        // Write atomically
        var tempFile = resolved + ".tmp";
        await File.WriteAllTextAsync(tempFile, content);
        File.Move(tempFile, resolved, overwrite: true);
    }
}
```

### Lock Timeout Mechanism

```csharp
// If agent A has lock, agent B waits
Timeline:
T0: Agent A acquires lock on transcript.md
T1: Agent B requests lock, timeout = 5s, deadline = T1 + 5s
T2: (wait 50ms, retry)
T3: (wait 50ms, retry)
...
T1+5s: Timeout, throw FileLockException
      Agent B's turn is skipped
      Next agent gets a turn
```

## Error Handling

### Exception Hierarchy

```csharp
// Base exception
public abstract class AgentMeetingException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Context { get; }
}

// Specific exceptions
public class MeetingConfigurationException : AgentMeetingException
    // Invalid files or parameters
    
public class AgentInitializationException : AgentMeetingException
    // Agent load/init failure
    // Contains AgentId
    
public class TurnTimeoutException : AgentMeetingException
    // Agent exceeded turn time
    // Contains AgentId, Timeout
    
public class FileLockException : AgentMeetingException
    // Failed to acquire file lock
    // Contains FilePath, RequestedByAgentId
    
public class CopilotApiException : AgentMeetingException
    // Copilot CLI failure
    
public class MeetingLimitExceededException : AgentMeetingException
    // Hard limit reached
    // Contains LimitType ("Time", "Messages")
```

### Graceful Degradation

```csharp
public async Task ExecuteTurnAsync(string agentId)
{
    try
    {
        var response = await _agents[agentId].GenerateResponseAsync(
            _context,
            _cancellationToken);
        
        await _eventBus.PublishAsync(
            new TurnCompletedEvent(agentId, response));
    }
    catch (TurnTimeoutException ex)
    {
        // Recoverable: skip turn, continue
        logger.LogWarning(ex, "Agent {AgentId} timed out", agentId);
        await _eventBus.PublishAsync(
            new TurnSkippedEvent(agentId, "Timeout"));
    }
    catch (AgentInitializationException ex)
    {
        // Partially recoverable: remove agent, continue if enough remain
        logger.LogError(ex, "Agent {AgentId} critical failure", agentId);
        _agents.Remove(agentId);
        
        if (_agents.Count < MinimumAgents)
            throw; // End meeting - not enough agents
    }
    catch (MeetingLimitExceededException ex)
    {
        // Expected: gracefully end meeting
        logger.LogInformation("Meeting limit exceeded: {LimitType}", ex.LimitType);
        await EndMeetingAsync($"Limit reached: {ex.LimitType}");
    }
}
```

## Logging Strategy

### Log Levels

```csharp
// Trace: Prompt details, detailed event flow
logger.LogTrace("Built prompt for {AgentId}: {PromptPreview}",
    agentId, prompt[..100]);

// Debug: Turn coordination, file operations, decisions
logger.LogDebug("Acquired lock on {FilePath} for {AgentId}",
    filePath, agentId);

// Information: Meeting lifecycle, key decisions
logger.LogInformation(
    "Meeting started: {Topic} with {AgentCount} agents",
    topic, agents.Count);

// Warning: Recoverable errors, degradation
logger.LogWarning("Agent {AgentId} response exceeded max length, truncating",
    agentId);

// Error: Failed operations, agent errors
logger.LogError(ex, "Failed to write transcript for {MeetingId}",
    meetingId);

// Critical: System failures
logger.LogCritical("Cannot initialize Copilot CLI: {Error}",
    ex.Message);
```

### Structured Logging

```csharp
// Log object properties for analysis
logger.LogInformation(
    "Meeting started: {MeetingId} {Topic} {AgentCount} {MaxDuration}",
    meetingId,
    topic,
    agentCount,
    maxDuration);

// Outputs:
// [2026-01-30 14:30:22 INF] Meeting started: mtg-001 "Payment Service" 4 30
```

### Log Output

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
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Security Considerations

### 1. Configuration Path Validation

```csharp
public class AgentConfigurationLoader
{
    private readonly string _baseConfigPath;
    
    public async Task<AgentConfiguration> LoadAsync(string configFileName)
    {
        // Prevent directory traversal
        if (configFileName.Contains("..") || 
            configFileName.Contains("/") ||
            configFileName.Contains("\\"))
            throw new SecurityException("Invalid config file name");
        
        var fullPath = Path.Combine(_baseConfigPath, configFileName);
        var resolvedPath = Path.GetFullPath(fullPath);
        var resolvedBase = Path.GetFullPath(_baseConfigPath);
        
        // Ensure resolved path is within base directory
        if (!resolvedPath.StartsWith(resolvedBase))
            throw new SecurityException("Path outside allowed directory");
        
        return await LoadFromPathAsync(resolvedPath);
    }
}
```

### 2. Meeting Room Isolation

Each meeting operates in its own isolated directory with no path traversal:

```csharp
public async Task WriteFileAsync(string relativePath, string content)
{
    var fullPath = Path.Combine(_meetingRoomPath, relativePath);
    var resolved = Path.GetFullPath(fullPath);
    var resolvedBase = Path.GetFullPath(_meetingRoomPath);
    
    // Validate: within meeting room
    if (!resolved.StartsWith(resolvedBase))
        throw new SecurityException("Cannot write outside meeting room");
    
    await File.WriteAllTextAsync(resolved, content);
}
```

### 3. Content Sanitization

```csharp
public class ContentSanitizer
{
    public string SanitizeForFilePath(string input)
    {
        // Remove invalid characters
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(input.Where(c => !invalid.Contains(c)));
    }
    
    public string SanitizeContent(string content, int maxLength = 1_000_000)
    {
        // Prevent extremely large content
        if (content.Length > maxLength)
        {
            logger.LogWarning(
                "Content truncated from {Original} to {Max}",
                content.Length,
                maxLength);
            return content[..maxLength];
        }
        
        return content;
    }
}
```

### 4. Copilot CLI Verification

```csharp
public async Task StartAsync(CancellationToken cancellationToken = default)
{
    try
    {
        logger.LogInformation("Initializing Copilot CLI connection...");
        await _client.StartAsync();
        logger.LogInformation("Successfully connected to Copilot CLI");
    }
    catch (FileNotFoundException ex)
    {
        throw new InvalidOperationException(
            "GitHub Copilot CLI not found. Please ensure it is installed " +
            "and in your PATH. Visit https://github.com/github/copilot-cli",
            ex);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to start Copilot client");
        throw new InvalidOperationException(
            "Failed to initialize Copilot CLI connection. " +
            "Verify your GitHub Copilot subscription is active.",
            ex);
    }
}
```

### 5. Rate Limiting (Future)

Rate limiting will be considered in v0.3+ when concurrency support is added.

## Performance Considerations

### 1. Memory Management

- **Event subscribers**: Use weak event patterns for long-lived subscribers
- **Message history**: Implement sliding window to prevent unbounded growth
- **Large files**: Stream-write instead of loading entirely into memory

### 2. Concurrency

- **Turn-taking**: Sequential execution prevents race conditions
- **File locking**: Prevents concurrent writes to same file
- **Event bus**: Thread-safe subscription management with SemaphoreSlim

### 3. Optimization Opportunities

**Current Phase 1**:
- In-memory event bus sufficient for single meeting
- File locks using simple dictionary (fast for small number of files)
- Transcript streaming for large meetings

**Future Phases**:
- Database-backed event log for multi-meeting scenarios
- Connection pooling for Copilot CLI
- Caching for frequently-accessed configurations
- Async file I/O for parallel artifact generation

### 4. Scalability

**Horizontal**:
- Each meeting runs independently
- No shared state between meetings
- Multi-meeting concurrency planned for v0.3

**Vertical**:
- Agent response time is bottleneck (Copilot CLI latency)
- Meeting duration scales with agent count and message limit
- Typical meeting: 3-5 agents × 5-10 min = 15-50 minutes

---

**Version**: 1.0  
**Last Updated**: January 30, 2026  
**Status**: Active
