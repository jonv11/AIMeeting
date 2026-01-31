# API Reference

## Overview

AIMeeting is CLI-first for v0.1. This document describes internal interfaces used by the CLI and core services. These APIs are **not yet a stable public contract** and may change before v0.2.

## Table of Contents

1. [Core Interfaces](#core-interfaces)
2. [Data Models](#data-models)
3. [Usage Examples](#usage-examples)
4. [Error Handling](#error-handling)
5. [Events](#events)
6. [Best Practices](#best-practices)

## Core Interfaces

### IMeetingOrchestrator

Central interface for running meetings and managing their lifecycle.

```csharp
public interface IMeetingOrchestrator
{
    /// <summary>
    /// Runs a meeting to completion or until a limit is exceeded.
    /// </summary>
    Task<MeetingResult> RunMeetingAsync(
        MeetingConfiguration configuration,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the current state of a running meeting.
    /// </summary>
    Task<MeetingState> GetMeetingStateAsync(string meetingId);
    
    /// <summary>
    /// Stops a meeting gracefully and generates final artifacts.
    /// </summary>
    Task StopMeetingAsync(string meetingId, string reason);
}
```

**Usage**:
```csharp
var orchestrator = serviceProvider.GetRequiredService<IMeetingOrchestrator>();

var result = await orchestrator.RunMeetingAsync(
    configuration,
    CancellationToken.None);

Console.WriteLine($"Meeting ended: {result.State}");
Console.WriteLine($"Transcript: {result.TranscriptPath}");
```

### IEventBus

Pub/sub event notification system for meeting events.

```csharp
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    
    /// <summary>
    /// Subscribes to events of a specific type.
    /// </summary>
    IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) 
        where TEvent : class;
}
```

**Usage**:
```csharp
var eventBus = serviceProvider.GetRequiredService<IEventBus>();

// Subscribe to turn events
eventBus.Subscribe<TurnCompletedEvent>(async (evt) =>
{
    Console.WriteLine($"Agent {evt.AgentId} spoke: {evt.Message.Substring(0, 50)}...");
});

// Subscribe to meeting end
eventBus.Subscribe<MeetingEndedEvent>(async (evt) =>
{
    Console.WriteLine($"Meeting ended: {evt.EndReason}");
});
```

### IMeetingRoom

Interface for accessing and modifying meeting artifacts and shared files.

```csharp
public interface IMeetingRoom
{
    /// <summary>
    /// Gets the absolute path to the meeting room directory.
    /// </summary>
    string MeetingRoomPath { get; }
    
    /// <summary>
    /// Reads content from a file in the meeting room.
    /// </summary>
    Task<string> ReadFileAsync(string relativePath);
    
    /// <summary>
    /// Writes content to a file in the meeting room.
    /// </summary>
    Task WriteFileAsync(string relativePath, string content);
    
    /// <summary>
    /// Checks if a file exists in the meeting room.
    /// </summary>
    Task<bool> FileExistsAsync(string relativePath);
    
    /// <summary>
    /// Lists all files in a directory within the meeting room.
    /// </summary>
    Task<IEnumerable<string>> ListFilesAsync(string relativeDirectory = "");
    
    /// <summary>
    /// Acquires an exclusive lock on a file.
    /// </summary>
    Task<IFileLock> AcquireLockAsync(
        string relativePath,
        string agentId,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}
```

**Usage**:
```csharp
var meetingRoom = serviceProvider.GetRequiredService<IMeetingRoom>();

// Read transcript
var transcript = await meetingRoom.ReadFileAsync("transcript.md");

// Write to transcript
await meetingRoom.WriteFileAsync(
    "transcript.md",
    "Meeting content...");

// Acquire lock and write atomically
using var fileLock = await meetingRoom.AcquireLockAsync(
    "transcript.md",
    agentId: "agent-1",
    timeout: TimeSpan.FromSeconds(5),
    CancellationToken.None);

await meetingRoom.WriteFileAsync("transcript.md", newContent);
```

### IAgent

Interface for individual agents in a meeting.

```csharp
public interface IAgent
{
    /// <summary>
    /// Unique identifier for this agent.
    /// </summary>
    string AgentId { get; }
    
    /// <summary>
    /// The agent's role (e.g., "Project Manager").
    /// </summary>
    string RoleName { get; }
    
    /// <summary>
    /// Initializes the agent for a meeting.
    /// </summary>
    Task InitializeAsync(
        MeetingContext context,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Generates the agent's response to the current discussion.
    /// </summary>
    Task<AgentResponse> RespondAsync(
        MeetingContext context,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Determines if the agent should participate in the next turn.
    /// </summary>
    Task<bool> ShouldParticipateAsync(MeetingContext context);
}
```

**Usage**:
```csharp
var agent = agents["agent-1"];

// Initialize agent
await agent.InitializeAsync(meetingContext, cancellationToken);

// Get agent response
var response = await agent.RespondAsync(meetingContext, cancellationToken);
Console.WriteLine($"Agent says: {response.Content}");

// Check if agent should continue
if (await agent.ShouldParticipateAsync(meetingContext))
{
    // Agent will participate in next turn
}
```

### ITurnManager

Interface for coordinating which agent speaks next.

```csharp
public interface ITurnManager
{
    /// <summary>
    /// Registers an agent with the turn manager.
    /// </summary>
    void RegisterAgent(string agentId);
    
    /// <summary>
    /// Removes an agent from the turn queue.
    /// </summary>
    void UnregisterAgent(string agentId);
    
    /// <summary>
    /// Gets the next agent to speak.
    /// </summary>
    string GetNextAgent();
    
    /// <summary>
    /// Gets whether there are agents remaining.
    /// </summary>
    bool HasAgents { get; }
}
```

**Usage**:
```csharp
var turnManager = new FifoTurnManager();

turnManager.RegisterAgent("agent-1");
turnManager.RegisterAgent("agent-2");
turnManager.RegisterAgent("agent-3");

var nextAgent = turnManager.GetNextAgent();  // "agent-1"
nextAgent = turnManager.GetNextAgent();      // "agent-2"
nextAgent = turnManager.GetNextAgent();      // "agent-3"
nextAgent = turnManager.GetNextAgent();      // "agent-1" (wraps)
```

### IPromptBuilder

Interface for building context-aware prompts for agents.

```csharp
public interface IPromptBuilder
{
    /// <summary>
    /// Builds a prompt for a standard agent's turn.
    /// </summary>
    string BuildAgentPrompt(
        AgentConfiguration agentConfig,
        MeetingContext context);
    
    /// <summary>
    /// Builds a prompt for the moderator agent.
    /// </summary>
    string BuildModeratorPrompt(MeetingContext context);
}
```

### ICopilotClient

Interface for interacting with GitHub Copilot.

```csharp
public interface ICopilotClient : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Copilot connection.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Closes the Copilot connection.
    /// </summary>
    Task StopAsync();
    
    /// <summary>
    /// Generates a completion for the given prompt.
    /// </summary>
    Task<string> GenerateAsync(
        string prompt,
        CopilotOptions options = null,
        CancellationToken cancellationToken = default);
}
```

**Usage**:
```csharp
var copilotClient = serviceProvider.GetRequiredService<ICopilotClient>();

// Start the client
await copilotClient.StartAsync();

try
{
    // Generate a response
    var prompt = "You are a project manager. Respond to...";
    var response = await copilotClient.GenerateAsync(prompt);
    Console.WriteLine($"Response: {response}");
}
finally
{
    // Clean up
    await copilotClient.StopAsync();
}
```

## Data Models

### MeetingConfiguration

Defines the parameters for a meeting.

```csharp
public class MeetingConfiguration
{
    /// <summary>
    /// Optional meeting ID to use instead of auto-generation.
    /// </summary>
    public string? MeetingId { get; set; }

    /// <summary>
    /// The main topic or question for the meeting.
    /// </summary>
    public string MeetingTopic { get; set; }
    
    /// <summary>
    /// Paths to agent configuration files.
    /// </summary>
    public List<string> AgentConfigPaths { get; set; }
    
    /// <summary>
    /// Hard limits for the meeting.
    /// </summary>
    public MeetingLimits HardLimits { get; set; }
    
    /// <summary>
    /// Base directory for meeting artifacts (optional).
    /// </summary>
    public string OutputDirectory { get; set; }
}
```

### MeetingLimits

Defines hard boundaries for meeting execution.

```csharp
public class MeetingLimits
{
    /// <summary>
    /// Maximum meeting duration in minutes.
    /// </summary>
    public int? MaxDurationMinutes { get; set; }
    
    /// <summary>
    /// Maximum total messages across all agents.
    /// </summary>
    public int? MaxTotalMessages { get; set; }
}
```

### MeetingContext

Current state and history of a meeting.

```csharp
public class MeetingContext
{
    /// <summary>
    /// Unique identifier for the meeting.
    /// </summary>
    public string MeetingId { get; set; }
    
    /// <summary>
    /// The meeting topic.
    /// </summary>
    public string Topic { get; set; }
    
    /// <summary>
    /// All messages in the meeting.
    /// </summary>
    public List<Message> Messages { get; set; }
    
    /// <summary>
    /// Available agents (key: AgentId, value: IAgent).
    /// </summary>
    public Dictionary<string, IAgent> Agents { get; set; }
    
    /// <summary>
    /// Current state of the meeting.
    /// </summary>
    public MeetingState State { get; set; }
    
    /// <summary>
    /// When the meeting started.
    /// </summary>
    public DateTime StartedAt { get; set; }
    
    /// <summary>
    /// Which agent is currently speaking (if any).
    /// </summary>
    public string CurrentSpeakingAgentId { get; set; }
}
```

### Message

A single message in the meeting transcript.

```csharp
public class Message
{
    /// <summary>
    /// Unique message identifier.
    /// </summary>
    public string MessageId { get; set; }
    
    /// <summary>
    /// ID of the agent who sent this message.
    /// </summary>
    public string AgentId { get; set; }
    
    /// <summary>
    /// The message content.
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// When the message was sent.
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// ID of the message this is a reply to (if any).
    /// </summary>
    public string ReplyToMessageId { get; set; }
    
    /// <summary>
    /// Type of message (Statement, Question, Response).
    /// </summary>
    public MessageType Type { get; set; }
}

public enum MessageType
{
    Statement,
    Question,
    Response
}
```

### MeetingResult

Results and artifacts from a completed meeting.

```csharp
public class MeetingResult
{
    /// <summary>
    /// Unique identifier for the meeting.
    /// </summary>
    public string MeetingId { get; set; }
    
    /// <summary>
    /// Final state of the meeting.
    /// </summary>
    public MeetingState State { get; set; }
    
    /// <summary>
    /// Reason the meeting ended.
    /// </summary>
    public string EndReason { get; set; }
    
    /// <summary>
    /// How long the meeting lasted.
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Total messages exchanged.
    /// </summary>
    public int MessageCount { get; set; }
    
    /// <summary>
    /// Path to the transcript file.
    /// </summary>
    public string TranscriptPath { get; set; }
    
    /// <summary>
    /// Path to the meeting metadata file.
    /// </summary>
    public string MeetingMetadataPath { get; set; }
    
    /// <summary>
    /// Path to the error log file (if any).
    /// </summary>
    public string ErrorLogPath { get; set; }
}
```

### MeetingState

Enumeration of possible meeting states.

```csharp
public enum MeetingState
{
    /// <summary>
    /// Meeting not yet started.
    /// </summary>
    NotStarted,
    
    /// <summary>
    /// Meeting is initializing agents and validating configuration.
    /// </summary>
    Initializing,
    
    /// <summary>
    /// Meeting is actively running.
    /// </summary>
    InProgress,
    
    /// <summary>
    /// Meeting is wrapping up and generating artifacts.
    /// </summary>
    EndingGracefully,
    
    /// <summary>
    /// Meeting completed successfully.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Meeting failed with an error.
    /// </summary>
    Failed,
    
    /// <summary>
    /// Meeting was cancelled by user.
    /// </summary>
    Cancelled
}
```

## Usage Examples

### Example 1: Run a Simple Meeting

```csharp
using AIMeeting.Core;
using AIMeeting.Core.Orchestration;
using Microsoft.Extensions.DependencyInjection;

// Set up dependency injection
var services = new ServiceCollection();
services.AddMeetingServices();
services.AddCopilotIntegration();
services.AddInfrastructureServices();

var serviceProvider = services.BuildServiceProvider();

// Initialize Copilot
var copilotClient = serviceProvider.GetRequiredService<ICopilotClient>();
await copilotClient.StartAsync();

try
{
    // Create meeting configuration
    var config = new MeetingConfiguration
    {
        MeetingTopic = "Discuss microservices architecture",
        AgentConfigPaths = new List<string>
        {
            "config/agents/project-manager.txt",
            "config/agents/senior-developer.txt",
            "config/agents/security-expert.txt"
        },
        HardLimits = new MeetingLimits
        {
            MaxDurationMinutes = 30,
            MaxTotalMessages = 50
        }
    };
    
    // Run meeting
    var orchestrator = serviceProvider.GetRequiredService<IMeetingOrchestrator>();
    var result = await orchestrator.RunMeetingAsync(config);
    
    // Report results
    Console.WriteLine($"Meeting ended: {result.State}");
    Console.WriteLine($"Duration: {result.Duration}");
    Console.WriteLine($"Messages: {result.MessageCount}");
    Console.WriteLine($"Transcript: {result.TranscriptPath}");
}
finally
{
    await copilotClient.StopAsync();
}
```

### Example 2: Subscribe to Meeting Events

```csharp
var eventBus = serviceProvider.GetRequiredService<IEventBus>();

// Track turn-taking
var turnCount = 0;
eventBus.Subscribe<TurnCompletedEvent>(async (evt) =>
{
    turnCount++;
    Console.WriteLine($"[Turn {turnCount}] {evt.AgentId}: {evt.Message[..50]}...");
});

// Track meeting progress
eventBus.Subscribe<MeetingStartedEvent>(async (evt) =>
{
    Console.WriteLine($"Meeting started: {evt.Configuration.MeetingTopic}");
    Console.WriteLine($"Agents: {evt.Configuration.AgentConfigPaths.Count}");
});

// Track when meeting ends
eventBus.Subscribe<MeetingEndedEvent>(async (evt) =>
{
    Console.WriteLine("Meeting ended!");
    Console.WriteLine($"Reason: {evt.EndReason}");
    Console.WriteLine($"Total turns: {turnCount}");
});

// Run meeting (events will be published)
var result = await orchestrator.RunMeetingAsync(config);
```

### Example 3: Access Meeting Artifacts Programmatically

```csharp
var meetingRoom = serviceProvider.GetRequiredService<IMeetingRoom>();

// Read transcript
var transcript = await meetingRoom.ReadFileAsync("transcript.md");
Console.WriteLine("Transcript:");
Console.WriteLine(transcript);

// List all files
var files = await meetingRoom.ListFilesAsync();
Console.WriteLine("\nAll files:");
foreach (var file in files)
{
    Console.WriteLine($"  - {file}");
}
```

## Error Handling

### Exception Hierarchy

```csharp
public abstract class AgentMeetingException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Context { get; }
}

public class MeetingConfigurationException : AgentMeetingException
    // Invalid configuration
    
public class AgentInitializationException : AgentMeetingException
    // Agent failed to initialize
    public string AgentId { get; }
    
public class TurnTimeoutException : AgentMeetingException
    // Agent exceeded response timeout
    public string AgentId { get; }
    public TimeSpan Timeout { get; }
    
public class FileLockException : AgentMeetingException
    // Failed to acquire file lock
    public string FilePath { get; }
    public string RequestedByAgentId { get; }
    
public class CopilotApiException : AgentMeetingException
    // Copilot CLI error
    
public class MeetingLimitExceededException : AgentMeetingException
    // Hard limit reached
    public string LimitType { get; }  // "Time", "Messages"
```

### Handling Exceptions

```csharp
try
{
    var result = await orchestrator.RunMeetingAsync(config);
}
catch (MeetingConfigurationException ex)
{
    Console.Error.WriteLine($"Invalid configuration: {ex.Message}");
    Console.Error.WriteLine($"Error code: {ex.ErrorCode}");
    return 1;
}
catch (AgentInitializationException ex)
{
    Console.Error.WriteLine($"Failed to initialize agent {ex.AgentId}");
    Console.Error.WriteLine($"Reason: {ex.Message}");
    return 1;
}
catch (CopilotApiException ex)
{
    Console.Error.WriteLine("Failed to connect to Copilot CLI");
    Console.Error.WriteLine("Make sure GitHub Copilot CLI is installed and in PATH");
    return 1;
}
catch (MeetingLimitExceededException ex)
{
    Console.WriteLine($"Meeting ended: {ex.LimitType} limit reached");
    // This is expected, not necessarily an error
    return 0;
}
catch (AgentMeetingException ex)
{
    Console.Error.WriteLine($"Meeting failed: {ex.Message}");
    return 1;
}
```

## Events

### Available Events

```csharp
// Agent lifecycle
public class AgentJoinedEvent { public string AgentId { get; set; } }
public class AgentReadyEvent { public string AgentId { get; set; } }
public class AgentLeftEvent { public string AgentId { get; set; } }

// Meeting lifecycle
public class MeetingStartedEvent 
{ 
    public MeetingConfiguration Configuration { get; set; }
}
public class MeetingEndingEvent 
{ 
    public string Reason { get; set; }
}
public class MeetingEndedEvent 
{ 
    public string EndReason { get; set; }
}

// Turn management
public class TurnStartedEvent 
{ 
    public string AgentId { get; set; }
    public int TurnNumber { get; set; }
}
public class TurnCompletedEvent 
{ 
    public string AgentId { get; set; }
    public string Message { get; set; }
}
public class TurnSkippedEvent 
{ 
    public string AgentId { get; set; }
    public string Reason { get; set; }
}

// File operations
public class FileCreatedEvent 
{ 
    public string AgentId { get; set; }
    public string FilePath { get; set; }
}
public class FileModifiedEvent 
{ 
    public string AgentId { get; set; }
    public string FilePath { get; set; }
}
public class FileLockedEvent 
{ 
    public string FilePath { get; set; }
    public string AgentId { get; set; }
}
public class FileUnlockedEvent 
{ 
    public string FilePath { get; set; }
}
```

## Best Practices

### 1. Always Initialize and Clean Up Copilot Client

```csharp
var copilotClient = serviceProvider.GetRequiredService<ICopilotClient>();
await copilotClient.StartAsync();

try
{
    // Use the client
    var result = await orchestrator.RunMeetingAsync(config);
}
finally
{
    // Always clean up
    await copilotClient.StopAsync();
}
```

### 2. Use CancellationToken for Graceful Shutdown

```csharp
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    var result = await orchestrator.RunMeetingAsync(config, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Meeting cancelled by user");
}
```

### 3. Set Reasonable Limits

```csharp
var config = new MeetingConfiguration
{
    HardLimits = new MeetingLimits
    {
        MaxDurationMinutes = 30,
        MaxTotalMessages = 50
    }
};
```

### 4. Monitor Events for Diagnostics

```csharp
var eventBus = serviceProvider.GetRequiredService<IEventBus>();

eventBus.Subscribe<TurnSkippedEvent>(async (evt) =>
{
    logger.LogWarning("Agent {AgentId} skipped turn: {Reason}",
        evt.AgentId, evt.Reason);
});

eventBus.Subscribe<MeetingEndedEvent>(async (evt) =>
{
    logger.LogInformation("Meeting ended: {Reason}",
        evt.EndReason);
});
```

### 5. Store Meeting Results

```csharp
var result = await orchestrator.RunMeetingAsync(config);

// Save result metadata
var metadata = new
{
    result.MeetingId,
    result.State,
    result.EndReason,
    Duration = result.Duration.TotalSeconds,
    result.MessageCount
};

var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions 
{ 
    WriteIndented = true 
});
await File.WriteAllTextAsync("meeting-metadata.json", json);
```

---

**Version**: 1.0  
**Last Updated**: January 30, 2026
