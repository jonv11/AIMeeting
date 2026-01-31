# Extending AIMeeting

This guide explains how to extend AIMeeting with custom agents, event handlers, turn strategies, and other features. Sections marked **[v0.2+]** or **[v0.3+]** refer to roadmap capabilities beyond the v0.1 MVP.

## Table of Contents

1. [Creating Custom Agents](#creating-custom-agents)
2. [Custom Turn Strategies](#custom-turn-strategies)
3. [Event Handlers](#event-handlers)
4. [File System Extensions](#file-system-extensions)
5. [Prompt Engineering](#prompt-engineering)
6. [Integration Patterns](#integration-patterns)

## Creating Custom Agents

### Basic Custom Agent

Inherit from `AgentBase` to create a specialized agent:

```csharp
using AIMeeting.Core.Agents;
using AIMeeting.Core.Orchestration;

public class CustomAgent : AgentBase
{
    private readonly ICopilotClient _copilotClient;
    private readonly IPromptBuilder _promptBuilder;
    private readonly ILogger<CustomAgent> _logger;
    
    public CustomAgent(
        string agentId,
        AgentConfiguration configuration,
        ICopilotClient copilotClient,
        IPromptBuilder promptBuilder,
        ILogger<CustomAgent> logger)
        : base(agentId, configuration)
    {
        _copilotClient = copilotClient;
        _promptBuilder = promptBuilder;
        _logger = logger;
    }
    
    protected override async Task<string> GenerateResponseAsync(
        MeetingContext context,
        CancellationToken cancellationToken)
    {
        // Build a custom prompt
        var prompt = _promptBuilder.BuildAgentPrompt(Configuration, context);
        
        try
        {
            // Generate response using Copilot
            var response = await _copilotClient.GenerateAsync(
                prompt,
                cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Agent {AgentId} generated response: {Length} chars",
                AgentId,
                response.Length);
            
            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Agent {AgentId} response timed out", AgentId);
            throw new TurnTimeoutException(
                $"Agent {AgentId} exceeded time limit",
                AgentId,
                TimeSpan.FromSeconds(30));
        }
    }
    
    protected override async Task<bool> ShouldParticipateAsync(
        MeetingContext context)
    {
        // Always participate
        return true;
    }
}
```

### Agent with Custom Logic

Create specialized agents for specific purposes:

```csharp
/// <summary>
/// Agent that analyzes meeting discussion and votes on proposals.
/// </summary>
public class VotingAgent : AgentBase
{
    private readonly ICopilotClient _copilotClient;
    private List<(string proposal, int voteCount)> _votes = new();
    
    protected override async Task<string> GenerateResponseAsync(
        MeetingContext context,
        CancellationToken cancellationToken)
    {
        // Find proposals in recent messages
        var proposals = ExtractProposalsFromMessages(context.Messages);
        
        if (proposals.Count == 0)
        {
            return "I don't see any clear proposals to vote on yet.";
        }
        
        // Generate voting analysis
        var prompt = BuildVotingPrompt(proposals, context);
        var analysis = await _copilotClient.GenerateAsync(prompt, cancellationToken: cancellationToken);
        
        // Extract vote from analysis
        var vote = ExtractVoteFromAnalysis(analysis);
        UpdateVoteTally(vote);
        
        return analysis;
    }
    
    private List<string> ExtractProposalsFromMessages(List<Message> messages)
    {
        return messages
            .Where(m => m.Type == MessageType.Statement)
            .Select(m => m.Content)
            .ToList();
    }
    
    private string BuildVotingPrompt(List<string> proposals, MeetingContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Analyze these proposals and vote for the best one:");
        foreach (var proposal in proposals)
        {
            sb.AppendLine($"- {proposal}");
        }
        sb.AppendLine("\nProvide your voting analysis and recommendation.");
        return sb.ToString();
    }
    
    private string ExtractVoteFromAnalysis(string analysis)
    {
        // Parse analysis to determine vote
        return analysis.Contains("recommend") ? analysis.Substring(0, 100) : "";
    }
    
    private void UpdateVoteTally(string vote)
    {
        // Update vote tracking
    }
}
```

### Registering Custom Agents

Register your custom agents in dependency injection:

```csharp
services.AddSingleton<IAgentFactory, CustomAgentFactory>();

public class CustomAgentFactory : IAgentFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public CustomAgentFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IAgent CreateAgent(
        string agentId,
        AgentConfiguration configuration)
    {
        return configuration.Role switch
        {
            "Voting Agent" => new VotingAgent(
                agentId,
                configuration,
                _serviceProvider.GetRequiredService<ICopilotClient>(),
                _serviceProvider.GetRequiredService<IPromptBuilder>(),
                _serviceProvider.GetRequiredService<ILogger<VotingAgent>>()),
            
            _ => new StandardAgent(
                agentId,
                configuration,
                _serviceProvider.GetRequiredService<ICopilotClient>(),
                _serviceProvider.GetRequiredService<IPromptBuilder>(),
                _serviceProvider.GetRequiredService<ILogger<StandardAgent>>())
        };
    }
}
```

## Custom Turn Strategies **[v0.3+]**

### Priority-Based Turn Strategy

Implement `ITurnManager` for custom turn-taking:

```csharp
/// <summary>
/// Turn strategy that prioritizes moderator and expert agents.
/// </summary>
public class PriorityBasedTurnManager : ITurnManager
{
    private readonly List<(string agentId, int priority)> _agentsByPriority = new();
    private int _currentIndex = 0;
    
    public void RegisterAgent(string agentId, int priority = 0)
    {
        _agentsByPriority.Add((agentId, priority));
        _agentsByPriority.Sort((a, b) => b.priority.CompareTo(a.priority));
    }
    
    public void UnregisterAgent(string agentId)
    {
        _agentsByPriority.RemoveAll(a => a.agentId == agentId);
    }
    
    public string GetNextAgent()
    {
        if (!HasAgents)
            throw new InvalidOperationException("No agents registered");
        
        var agent = _agentsByPriority[_currentIndex];
        _currentIndex = (_currentIndex + 1) % _agentsByPriority.Count;
        return agent.agentId;
    }
    
    public bool HasAgents => _agentsByPriority.Count > 0;
}
```

### Dynamic Turn Strategy **[v0.3+]**

Route to agents based on relevance:

```csharp
/// <summary>
/// Turn strategy that dynamically routes to most relevant agent.
/// </summary>
public class DynamicTurnManager : ITurnManager
{
    private readonly Dictionary<string, int> _agentRelevanceScores = new();
    private readonly ICopilotClient _copilotClient;
    
    public DynamicTurnManager(ICopilotClient copilotClient)
    {
        _copilotClient = copilotClient;
    }
    
    public async Task<string> GetNextAgentAsync(MeetingContext context)
    {
        // Evaluate each agent's relevance to current discussion
        var relevanceScores = new Dictionary<string, float>();
        
        foreach (var agentId in _agentRelevanceScores.Keys)
        {
            var score = await EvaluateRelevanceAsync(
                agentId,
                context.Messages.TakeLast(3).ToList(),
                context);
            relevanceScores[agentId] = score;
        }
        
        // Return most relevant agent
        return relevanceScores
            .OrderByDescending(x => x.Value)
            .First()
            .Key;
    }
    
    private async Task<float> EvaluateRelevanceAsync(
        string agentId,
        List<Message> recentMessages,
        MeetingContext context)
    {
        var prompt = $@"
Rate this agent's relevance to the discussion on a scale of 0-10.

Agent: {agentId}
Recent discussion:
{string.Join("\n", recentMessages.Select(m => m.Content))}

Provide only the numeric score (0-10).";
        
        var response = await _copilotClient.GenerateAsync(prompt);
        if (float.TryParse(response.Trim(), out var score))
            return score;
        
        return 5f; // Default neutral score
    }
    
    public void RegisterAgent(string agentId) => 
        _agentRelevanceScores[agentId] = 0;
    
    public void UnregisterAgent(string agentId) => 
        _agentRelevanceScores.Remove(agentId);
    
    public bool HasAgents => _agentRelevanceScores.Count > 0;
}
```

## Event Handlers

### Meeting Analytics

Collect metrics about meeting execution:

```csharp
public class MeetingAnalytics
{
    private readonly List<TurnCompletedEvent> _turns = new();
    private readonly Dictionary<string, int> _messageCount = new();
    private readonly Dictionary<string, int> _tokenCount = new();
    
    public void Subscribe(IEventBus eventBus)
    {
        eventBus.Subscribe<TurnCompletedEvent>(async evt =>
        {
            _turns.Add(evt);
            _messageCount.TryGetValue(evt.AgentId, out var count);
            _messageCount[evt.AgentId] = count + 1;
            _tokenCount[evt.AgentId] = _tokenCount.GetValueOrDefault(evt.AgentId, 0) 
                + EstimateTokens(evt.Message);
        });
    }
    
    public void GenerateReport()
    {
        Console.WriteLine("Meeting Analytics:");
        Console.WriteLine($"Total turns: {_turns.Count}");
        
        foreach (var (agentId, count) in _messageCount)
        {
            var tokens = _tokenCount.GetValueOrDefault(agentId, 0);
            Console.WriteLine($"  {agentId}: {count} messages, ~{tokens} tokens");
        }
        
        var averageTurnTime = _turns.Count > 0 
            ? "N/A" // Would calculate from timestamps
            : "0";
        Console.WriteLine($"Average turn time: {averageTurnTime}");
    }
    
    private int EstimateTokens(string text) => text.Length / 4;
}
```

## File System Extensions **[v0.2+]**

### Custom Artifact Generation

Extend meeting room with specialized artifact types:

```csharp
public class MeetingRoomExtensions
{
    private readonly IMeetingRoom _meetingRoom;
    
    public async Task GenerateDecisionLogAsync(
        List<Message> messages)
    {
        var decisions = ExtractDecisions(messages);
        var content = FormatDecisionLog(decisions);
        
        await _meetingRoom.WriteFileAsync("decisions.md", content);
    }
    
    public async Task GenerateRiskAssessmentAsync(
        List<Message> messages)
    {
        var risks = ExtractRisks(messages);
        var content = FormatRiskAssessment(risks);
        
        await _meetingRoom.WriteFileAsync("risks.md", content);
    }
    
    public async Task GenerateNextStepsAsync(
        List<Message> messages)
    {
        var actions = ExtractActions(messages);
        var content = FormatNextSteps(actions);
        
        await _meetingRoom.WriteFileAsync("next_steps.md", content);
    }
    
    private List<Decision> ExtractDecisions(List<Message> messages)
    {
        // Parse messages to find decisions
        return messages
            .Where(m => m.Content.Contains("decision") || m.Content.Contains("decided"))
            .Select(m => new Decision { Text = m.Content, AgentId = m.AgentId })
            .ToList();
    }
    
    private string FormatDecisionLog(List<Decision> decisions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Meeting Decisions\n");
        
        foreach (var decision in decisions)
        {
            sb.AppendLine($"## {decision.Text}");
            sb.AppendLine($"**Proposed by:** {decision.AgentId}\n");
        }
        
        return sb.ToString();
    }
    
    private List<string> ExtractRisks(List<Message> messages) => new();
    private string FormatRiskAssessment(List<string> risks) => "";
    private List<string> ExtractActions(List<Message> messages) => new();
    private string FormatNextSteps(List<string> actions) => "";
}
```

## Prompt Engineering

### Advanced Prompt Templates **[v0.2+]**

Create sophisticated prompts for specialized scenarios:

```csharp
public class AdvancedPromptBuilder : IPromptBuilder
{
    public string BuildDebatePrompt(
        AgentConfiguration config,
        List<Message> proposalMessages,
        MeetingContext context)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"# Debate: {context.Topic}");
        sb.AppendLine($"## Your Role: {config.Role}");
        sb.AppendLine(config.Description);
        sb.AppendLine();
        
        sb.AppendLine("## Competing Proposals:");
        foreach (var msg in proposalMessages)
        {
            sb.AppendLine($"- {msg.Content}");
        }
        sb.AppendLine();
        
        sb.AppendLine("## Your Task:");
        sb.AppendLine("1. Identify strengths and weaknesses of each proposal");
        sb.AppendLine("2. Highlight your specific concerns");
        sb.AppendLine("3. Ask clarifying questions");
        sb.AppendLine("4. Propose improvements if applicable");
        sb.AppendLine();
        
        sb.AppendLine($"Style: {config.ResponseStyle}");
        sb.AppendLine($"Max length: {config.MaxMessageLength} characters");
        
        return sb.ToString();
    }
}
```

## Integration Patterns **[v0.3+]**

### Integration with External Systems

```csharp
/// <summary>
/// Send meeting artifacts to external system.
/// </summary>
public class ExternalSystemIntegration
{
    private readonly HttpClient _httpClient;
    private readonly IMeetingRoom _meetingRoom;
    
    public async Task PublishMeetingResultsAsync(
        MeetingResult result,
        string externalSystemUrl)
    {
        // Gather artifacts
        var transcript = await _meetingRoom.ReadFileAsync("transcript.md");
        
        // Create payload
        var payload = new
        {
            meetingId = result.MeetingId,
            topic = result.State.ToString(),
            duration = result.Duration.TotalSeconds,
            messageCount = result.MessageCount,
            transcript,
            generatedAt = DateTime.UtcNow
        };
        
        // Send to external system
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(
            $"{externalSystemUrl}/meetings",
            content);
        
        response.EnsureSuccessStatusCode();
    }
}
```

### Database Integration **[v0.3+]**

```csharp
/// <summary>
/// Store meeting results in database.
/// </summary>
public class DatabaseIntegration
{
    private readonly IDbConnection _dbConnection;
    private readonly IMeetingRoom _meetingRoom;
    
    public async Task SaveMeetingAsync(MeetingResult result)
    {
        // Read all artifacts
        var artifacts = await _meetingRoom.ListFilesAsync();
        var artifactContents = new Dictionary<string, string>();
        
        foreach (var artifact in artifacts)
        {
            artifactContents[artifact] = await _meetingRoom.ReadFileAsync(artifact);
        }
        
        // Save to database
        const string sql = @"
            INSERT INTO Meetings (Id, Topic, State, Duration, MessageCount, 
                CreatedAt, Artifacts)
            VALUES (@id, @topic, @state, @duration, @messageCount, 
                @createdAt, @artifacts)";
        
        using var cmd = _dbConnection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("@id", result.MeetingId);
        cmd.Parameters.AddWithValue("@topic", result.EndReason ?? "");
        cmd.Parameters.AddWithValue("@state", result.State.ToString());
        cmd.Parameters.AddWithValue("@duration", result.Duration.TotalSeconds);
        cmd.Parameters.AddWithValue("@messageCount", result.MessageCount);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@artifacts", 
            JsonSerializer.Serialize(artifactContents));
        
        await cmd.ExecuteNonQueryAsync();
    }
}
```

## Best Practices

### 1. Use Dependency Injection

Always use DI for testability:

```csharp
public class CustomAgent : AgentBase
{
    private readonly ICopilotClient _copilot;
    private readonly ILogger<CustomAgent> _logger;
    private readonly IMeetingRoom _room;
    
    // All dependencies injected
    public CustomAgent(
        string agentId,
        AgentConfiguration config,
        ICopilotClient copilot,
        ILogger<CustomAgent> logger,
        IMeetingRoom room)
        : base(agentId, config)
    {
        _copilot = copilot;
        _logger = logger;
        _room = room;
    }
}
```

### 2. Log Extensively

Use structured logging for debugging:

```csharp
_logger.LogInformation(
    "Agent {AgentId} generated response with {Length} chars",
    AgentId,
    response.Length);

_logger.LogDebug(
    "Prompt for {AgentId}: {Prompt}",
    AgentId,
    prompt);

_logger.LogWarning(
    "Response from {AgentId} exceeded limit, truncating",
    AgentId);
```

### 3. Handle Cancellation

Support graceful shutdown:

```csharp
protected override async Task<string> GenerateResponseAsync(
    MeetingContext context,
    CancellationToken cancellationToken)
{
    try
    {
        return await _copilotClient.GenerateAsync(
            prompt,
            cancellationToken: cancellationToken);
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation("Agent {AgentId} cancelled", AgentId);
        throw;
    }
}
```

### 4. Validate Configuration

Validate custom configuration before use:

```csharp
public class CustomAgentValidator
{
    public void Validate(AgentConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.Role))
            throw new ArgumentException("Role is required");
        
        if (config.MaxMessageLength < 100)
            throw new ArgumentException("MaxMessageLength must be >= 100");
        
        if (config.PersonaTraits.Count < 2)
            throw new ArgumentException("At least 2 persona traits required");
    }
}
```

---

**Version**: 1.0  
**Last Updated**: January 30, 2026
