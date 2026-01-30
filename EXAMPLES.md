# Examples

This document provides practical examples of using AIMeeting for various scenarios.

## Table of Contents

1. [Basic Meeting](#basic-meeting)
2. [Software Architecture Review](#software-architecture-review)
3. [Risk Assessment](#risk-assessment)
4. [Decision Making](#decision-making)
5. [Problem Solving](#problem-solving)
6. [Brainstorming](#brainstorming)
7. [Code Review Simulation](#code-review-simulation)
8. [Programmatic Usage](#programmatic-usage)

## Basic Meeting

### Scenario

Run a simple meeting with three agents to discuss a project topic.

### Command Line

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Plan the Q1 roadmap for our platform" \
    --agents "config/agents/project-manager.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/qa-engineer.txt" \
    --max-duration 20 \
    --max-messages 30 \
    --log-level Information
```

### Configuration File

Create `config/meetings/q1-planning.json`:

```json
{
  "meetingTopic": "Plan the Q1 roadmap for our platform",
  "agentConfigs": [
    "agents/project-manager.txt",
    "agents/senior-developer.txt",
    "agents/qa-engineer.txt"
  ],
  "hardLimits": {
    "maxDurationMinutes": 20,
    "maxTotalMessages": 30,
    "maxMessagesPerAgent": 15
  },
  "turnTakingStrategy": "FIFO",
  "enableArtifactSearch": true
}
```

Then run:

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --config "config/meetings/q1-planning.json"
```

## Software Architecture Review

### Scenario

Review a proposed microservices architecture with diverse perspectives.

### Agents

```
- Project Manager: Timeline and resource constraints
- Senior Developer: Implementation feasibility and code quality
- Security Expert: Data protection and threat modeling
- DevOps Engineer: Deployment and operational aspects
- Moderator: Keep discussion on track and identify consensus
```

### Configuration

Create `config/meetings/architecture-review.json`:

```json
{
  "meetingTopic": "Review proposed microservices architecture for payment system",
  "agentConfigs": [
    "agents/project-manager.txt",
    "agents/senior-developer.txt",
    "agents/security-expert.txt",
    "agents/devops-engineer.txt",
    "agents/moderator.txt"
  ],
  "hardLimits": {
    "maxDurationMinutes": 45,
    "maxTotalMessages": 75,
    "maxMessagesPerAgent": 20,
    "maxTokensTotal": 75000
  }
}
```

### Command

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --config "config/meetings/architecture-review.json"
```

### Expected Output

```
[14:30:22 INF] Meeting initialized: arch_review_20260130_143022
[14:30:24 INF] Loading 5 agents...
[14:30:26 INF] Meeting started

┌──────────────────────────────────────────────────────────────┐
│ Meeting: Review proposed microservices architecture...        │
│ Duration: 00:00:00 / 00:45:00  │  Messages: 0 / 75           │
└──────────────────────────────────────────────────────────────┘

[Project Manager]: Let me understand the scope. What are we trying to 
achieve with this microservices refactor, and what are the main drivers?

[Senior Developer]: From a technical perspective, I want to explore the 
service boundaries. How are we handling...

[Security Expert]: Before we go further, I need to understand how we'll 
handle authentication across services...

... (meeting continues) ...

[14:45:30 INF] Meeting ended: Consensus reached on architecture
[14:45:32 INF] Generating summary...
[14:45:35 INF] Meeting artifacts saved to:
  - meetings/arch_review_20260130_143022/transcript.md
  - meetings/arch_review_20260130_143022/summary.md
  - meetings/arch_review_20260130_143022/action_items.md
  - meetings/arch_review_20260130_143022/decisions.md
```

## Risk Assessment

### Scenario

Identify and prioritize risks for a new feature launch.

### Agents

```
- Project Manager: Timeline and resource risks
- Senior Developer: Technical implementation risks
- Security Expert: Security and compliance risks
- QA Lead: Testing and quality risks
- DevOps Engineer: Operational and deployment risks
```

### Custom Agent Configuration

Create `config/agents/risk-assessor.txt`:

```
ROLE: Risk Assessment Lead
DESCRIPTION: Synthesizes risks from all perspectives and prioritizes them

PERSONA:
- Systematic and comprehensive
- Focuses on impact and likelihood
- Develops mitigation strategies
- Prioritizes urgent risks

INSTRUCTIONS:
- Summarize identified risks
- Rate each risk (High/Medium/Low)
- Suggest concrete mitigations
- Identify interdependencies between risks
- Recommend monitoring approaches

RESPONSE_STYLE: Organized, analytical, action-focused
MAX_MESSAGE_LENGTH: 800
EXPERTISE_AREAS: Risk Management, Process Improvement, Contingency Planning
```

### Configuration

```json
{
  "meetingTopic": "Assess risks for Q1 feature launch",
  "agentConfigs": [
    "agents/project-manager.txt",
    "agents/senior-developer.txt",
    "agents/security-expert.txt",
    "agents/qa-engineer.txt",
    "agents/devops-engineer.txt",
    "agents/risk-assessor.txt",
    "agents/moderator.txt"
  ],
  "hardLimits": {
    "maxDurationMinutes": 60,
    "maxTotalMessages": 100
  }
}
```

## Decision Making

### Scenario

Make a decision between competing technology options.

### Question

"Should we use PostgreSQL or MongoDB for our new analytics platform?"

### Agents

```
- Project Manager: Cost, timeline, vendor support
- Senior Developer: Developer experience, scalability, ecosystem
- DevOps Engineer: Operations, backup, monitoring
- Data Scientist: Query patterns, analytics capabilities
```

### Custom Instructions for This Decision

Update agent configurations with specific prompts:

```
INITIAL_MESSAGE_TEMPLATE: 
Regarding {topic}, I'd like to contribute my perspective. Here are 
the key considerations from my area of expertise...

INSTRUCTIONS:
- List specific pros and cons for your area of expertise
- Ask clarifying questions about requirements
- Identify assumptions in the proposal
- Suggest criteria for evaluation
```

## Problem Solving

### Scenario

Troubleshoot a production issue and develop a resolution plan.

### Meeting Topic

"Production API response times degraded by 40% after yesterday's deployment. Root cause and action plan?"

### Agents

```
- DevOps Engineer: Infrastructure and metrics
- Senior Developer: Code changes and performance
- On-call Engineer: Incident response and rollback options
- Database Specialist: Database performance and queries
- QA Lead: Testing and validation before re-deployment
```

### Command

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Production API response times degraded by 40%. Root cause and action plan?" \
    --agents "config/agents/devops-engineer.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/qa-engineer.txt" \
    --max-duration 15 \
    --max-messages 25
```

## Brainstorming

### Scenario

Generate and evaluate ideas for improving user engagement.

### Agents

```
- Product Manager: User impact and alignment
- Designer: UX and design feasibility
- Developer: Technical implementation
- Marketing: Market positioning and messaging
- Business Analyst: Revenue implications and metrics
```

### Meeting Setup

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "How can we improve user engagement in the mobile app?" \
    --agents "config/agents/product-manager.txt" \
             "config/agents/designer.txt" \
             "config/agents/developer.txt" \
             "config/agents/marketing-lead.txt" \
             "config/agents/business-analyst.txt" \
    --max-duration 30 \
    --max-messages 60
```

### Expected Outcomes

The meeting artifacts will include:

1. **transcript.md**: Full conversation with all ideas discussed
2. **summary.md**: Synthesized summary of top ideas
3. **action_items.md**: Next steps for each promising idea
4. **decisions.md**: Which ideas will be explored further

## Code Review Simulation

### Scenario

Simulate a code review from multiple perspectives.

### Agents

```
- Code Quality Expert: Style, patterns, maintainability
- Performance Expert: Algorithms, optimization opportunities
- Security Expert: Vulnerabilities, input validation
- Testing Expert: Test coverage, test strategy
- Architecture Expert: Design patterns, system integration
```

### Custom Agent: Code Quality Reviewer

```
ROLE: Code Quality Expert
DESCRIPTION: Reviews code for style, patterns, and maintainability

PERSONA:
- Detail-oriented about code standards
- Focused on long-term maintainability
- Values clarity and consistency
- Advocates for best practices

INSTRUCTIONS:
- Point out style violations and inconsistencies
- Suggest refactoring opportunities
- Identify design pattern opportunities
- Flag overly complex code sections
- Praise good practices
- Consider future maintenance burden

RESPONSE_STYLE: Constructive, specific, examples-oriented
MAX_MESSAGE_LENGTH: 600
EXPERTISE_AREAS: Code Standards, Design Patterns, Refactoring
```

### Example Code to Review

```csharp
public class OrderProcessor
{
    public async Task<OrderResult> ProcessAsync(Order order)
    {
        if (order == null) throw new ArgumentNullException();
        
        var customer = await GetCustomerAsync(order.CustomerId);
        if (customer == null) return new OrderResult { Success = false };
        
        var inventory = await CheckInventoryAsync(order.Items);
        if (!inventory) return new OrderResult { Success = false };
        
        var payment = await ProcessPaymentAsync(
            order.Amount, 
            customer.PaymentMethod);
        
        if (!payment.Success)
            return new OrderResult { Success = false };
        
        var shippingAddress = customer.Addresses
            .FirstOrDefault(a => a.IsDefault == true);
        
        var shipment = await CreateShipmentAsync(
            order.Items, 
            shippingAddress);
        
        await UpdateInventoryAsync(order.Items);
        
        return new OrderResult
        {
            Success = true,
            OrderId = order.Id,
            TrackingNumber = shipment.TrackingNumber
        };
    }
}
```

### Meeting Command

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Code review of OrderProcessor.ProcessAsync method" \
    --agents "config/agents/code-quality-expert.txt" \
             "config/agents/performance-expert.txt" \
             "config/agents/security-expert.txt" \
             "config/agents/testing-expert.txt" \
    --max-duration 20 \
    --max-messages 40
```

## Programmatic Usage

### Complete Example

```csharp
using AIMeeting.Core;
using AIMeeting.Core.Configuration;
using AIMeeting.Core.Orchestration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Example 1: Basic Meeting
async Task RunBasicMeeting()
{
    // Setup services
    var services = new ServiceCollection();
    services.AddMeetingServices();           // Core services
    services.AddCopilotIntegration();         // Copilot SDK
    services.AddInfrastructureServices();     // Logging, etc.
    
    var serviceProvider = services.BuildServiceProvider();
    
    // Initialize Copilot
    var copilotClient = serviceProvider.GetRequiredService<ICopilotClient>();
    await copilotClient.StartAsync();
    
    try
    {
        // Create configuration
        var config = new MeetingConfiguration
        {
            MeetingTopic = "Plan the next product release",
            AgentConfigPaths = new List<string>
            {
                "config/agents/project-manager.txt",
                "config/agents/senior-developer.txt",
                "config/agents/qa-engineer.txt",
                "config/agents/moderator.txt"
            },
            HardLimits = new MeetingLimits
            {
                MaxDurationMinutes = 30,
                MaxTotalMessages = 50,
                MaxTokensTotal = 50000
            }
        };
        
        // Run meeting
        var orchestrator = serviceProvider.GetRequiredService<IMeetingOrchestrator>();
        var result = await orchestrator.RunMeetingAsync(config);
        
        // Display results
        Console.WriteLine($"Meeting Status: {result.State}");
        Console.WriteLine($"Duration: {result.Duration}");
        Console.WriteLine($"Total Messages: {result.MessageCount}");
        Console.WriteLine($"Tokens Used: {result.TokensUsed}");
        Console.WriteLine($"\nArtifacts:");
        Console.WriteLine($"  Transcript: {result.TranscriptPath}");
        Console.WriteLine($"  Summary: {result.SummaryPath}");
        
        Console.WriteLine("\nAction Items:");
        foreach (var item in result.ActionItems)
        {
            Console.WriteLine($"  ✓ {item}");
        }
    }
    finally
    {
        await copilotClient.StopAsync();
    }
}

// Example 2: Meeting with Event Monitoring
async Task RunMeetingWithMonitoring()
{
    var serviceProvider = CreateServiceProvider();
    var copilotClient = serviceProvider.GetRequiredService<ICopilotClient>();
    await copilotClient.StartAsync();
    
    try
    {
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        
        // Subscribe to events
        var turnCount = 0;
        eventBus.Subscribe<TurnCompletedEvent>(async (evt) =>
        {
            turnCount++;
            Console.WriteLine($"\n[Turn {turnCount}] {evt.AgentId}:");
            Console.WriteLine(evt.Message);
        });
        
        eventBus.Subscribe<MeetingEndedEvent>(async (evt) =>
        {
            Console.WriteLine($"\n✓ Meeting ended: {evt.EndReason}");
            Console.WriteLine($"Total turns: {turnCount}");
        });
        
        // Run meeting
        var config = LoadMeetingConfiguration("config/meetings/default-meeting.json");
        var orchestrator = serviceProvider.GetRequiredService<IMeetingOrchestrator>();
        await orchestrator.RunMeetingAsync(config);
    }
    finally
    {
        await copilotClient.StopAsync();
    }
}

// Example 3: Access and Analyze Results
async Task AccessMeetingResults(string meetingRoomPath)
{
    var services = new ServiceCollection();
    services.AddScoped(_ => new MeetingRoom(meetingRoomPath));
    var serviceProvider = services.BuildServiceProvider();
    
    var meetingRoom = serviceProvider.GetRequiredService<IMeetingRoom>();
    
    // Read transcript
    var transcript = await meetingRoom.ReadFileAsync("transcript.md");
    Console.WriteLine("=== TRANSCRIPT ===");
    Console.WriteLine(transcript);
    
    // Read summary
    var summary = await meetingRoom.ReadFileAsync("summary.md");
    Console.WriteLine("\n=== SUMMARY ===");
    Console.WriteLine(summary);
    
    // Search for specific topics
    var securityMentions = await meetingRoom.SearchContentAsync("security");
    Console.WriteLine($"\n=== SECURITY MENTIONS ({securityMentions.Count()}) ===");
    foreach (var mention in securityMentions.Take(5))
    {
        Console.WriteLine($"  - {mention}");
    }
}

// Example 4: Custom Meeting with Interruption Handling
async Task RunMeetingWithErrorHandling()
{
    using var cts = new CancellationTokenSource();
    
    // Allow cancellation after 5 minutes
    cts.CancelAfter(TimeSpan.FromMinutes(5));
    
    // Allow user to cancel with Ctrl+C
    Console.CancelKeyPress += (sender, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
    };
    
    try
    {
        var orchestrator = CreateOrchestrator();
        var config = LoadMeetingConfiguration("config/meetings/default.json");
        
        var result = await orchestrator.RunMeetingAsync(config, cts.Token);
        Console.WriteLine($"Meeting completed: {result.State}");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Meeting cancelled by user");
    }
    catch (MeetingConfigurationException ex)
    {
        Console.Error.WriteLine($"Invalid configuration: {ex.Message}");
    }
    catch (CopilotApiException ex)
    {
        Console.Error.WriteLine("Failed to connect to Copilot CLI");
        Console.Error.WriteLine("Ensure GitHub Copilot CLI is installed and in PATH");
    }
}

// Helper methods
private static IServiceProvider CreateServiceProvider()
{
    var services = new ServiceCollection();
    services.AddMeetingServices();
    services.AddCopilotIntegration();
    services.AddInfrastructureServices();
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddFile("logs/meeting.log");
    });
    return services.BuildServiceProvider();
}

private static MeetingConfiguration LoadMeetingConfiguration(string path)
{
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<MeetingConfiguration>(json);
}

private static IMeetingOrchestrator CreateOrchestrator()
{
    return CreateServiceProvider().GetRequiredService<IMeetingOrchestrator>();
}

// Run examples
await RunBasicMeeting();
```

## Tips and Tricks

### 1. Quick Test with Single Agent

Test agent configuration quickly:

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Test topic" \
    --agents "config/agents/your-agent.txt" \
    --max-duration 5 \
    --max-messages 3
```

### 2. Load from Configuration File

Keep your meetings documented:

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --config "config/meetings/your-meeting.json"
```

### 3. Capture Output for Analysis

Save everything to a file:

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --config "config/meetings/default.json" \
    > meeting-run-$(date +%Y%m%d-%H%M%S).log 2>&1
```

### 4. Monitor via Events

Subscribe to events for real-time monitoring and custom logic:

```csharp
eventBus.Subscribe<TurnCompletedEvent>(async (evt) =>
{
    // Log to external system
    // Update UI
    // Trigger webhooks
    // Store in database
});
```

### 5. Generate Custom Reports

Process artifacts after meeting:

```csharp
var meetingRoom = serviceProvider.GetRequiredService<IMeetingRoom>();

// Generate CSV of all messages
var transcript = await meetingRoom.ReadFileAsync("transcript.md");
var csv = ConvertTranscriptToCSV(transcript);
await File.WriteAllTextAsync("meeting-messages.csv", csv);
```

---

**Version**: 1.0  
**Last Updated**: January 30, 2026
