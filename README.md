# AIMeeting - Multi-Agent Meeting System

A CLI-based multi-agent meeting system where AI agents with different roles engage in structured discussions about specific topics. The system simulates real-world meetings with agents taking turns, sharing perspectives, and collaborating to reach conclusions.

## Features

**[v0.1]** — MVP Features (Available in v0.1.0):
- **Structured AI Collaboration**: Enable multiple AI agents to discuss complex topics from different perspectives
- **Configurable Roles**: Text-based configuration allows any role/personality without code changes
- **Basic Transcript Generation**: Meeting conversations saved to transcript.md
- **Meeting Metadata**: Meeting details saved to meeting.json
- **Error Logging**: errors.log captured on failures
- **Event-Driven Architecture**: Message bus pattern ensures clean agent coordination
- **FIFO Turn-Taking**: Simple sequential turn coordination
- **Hard Limits Enforcement**: Time and message count limits

**[v0.2]** — Enhanced Artifacts (Planned):
- **Meeting Summaries**: Automatic generation of meeting summaries
- **Action Item Extraction**: Extract and track action items from discussions
- **Meeting Templates**: Pre-configured meeting setups
- **Decisions Tracking**: Document key decisions made during meetings
- **Agent-Specific Notes**: Per-agent perspective on meeting outcomes

**[v0.4+]** — Advanced Features (Roadmap):
- **RAG Integration**: Connect agents to domain-specific knowledge bases
- **Multi-Provider Support**: OpenAI, Azure OpenAI, local models
- **Dynamic Turn-Taking**: Voting, priority-based agent selection

See [ROADMAP.md](ROADMAP.md) for complete feature timeline.

## Quick Start

### Prerequisites

- .NET 8.0 or later
- GitHub Copilot CLI installed ([installation guide](https://github.com/github/copilot-cli))
- Active GitHub Copilot subscription (required for v0.1 Copilot provider)
- ~500MB disk space for meeting artifacts

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/jonv11/AIMeeting.git
   cd AIMeeting
   ```

2. Install GitHub Copilot CLI:
   ```bash
   # Follow instructions at https://github.com/github/copilot-cli
   gh copilot --version  # Verify installation
   ```

3. Verify installation:
   ```bash
   dotnet build
   dotnet test
   ```

### Run Your First Meeting **[v0.1]**

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Discuss architecture for payment service refactoring" \
    --agents "config/agents/project-manager.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/security-expert.txt" \
             "config/agents/moderator.txt" \
    --max-duration 30 \
    --max-messages 50
```

See [EXAMPLES.md](EXAMPLES.md) for more usage scenarios.

## Project Structure

```
AIMeeting/
├── README.md                           # This file
├── ARCHITECTURE.md                     # Detailed architecture documentation
├── AGENT_CONFIGURATION_GUIDE.md        # Agent configuration reference
├── API.md                              # API documentation
├── EXTENDING.md                        # Extension guide
├── EXAMPLES.md                         # Usage examples
├── LICENSE
├── .gitignore
├── .editorconfig
├── AIMeeting.sln
│
├── src/
│   ├── AIMeeting.CLI/                  # Command-line interface
│   │   ├── Program.cs                  # Entry point
│   │   ├── Commands/                   # CLI commands
│   │   ├── Display/                    # Console UI
│   │   └── AIMeeting.CLI.csproj
│   │
│   ├── AIMeeting.Core/                 # Core business logic
│   │   ├── Models/                     # Data models
│   │   ├── Agents/                     # Agent implementations
│   │   ├── Orchestration/              # Meeting orchestration
│   │   ├── Events/                     # Event system
│   │   ├── FileSystem/                 # Meeting room & file ops
│   │   ├── Configuration/              # Configuration loading
│   │   ├── Prompts/                    # Prompt engineering
│   │   └── AIMeeting.Core.csproj
│   │
│   ├── AIMeeting.Copilot/              # GitHub Copilot integration
│   │   ├── ICopilotClient.cs
│   │   ├── CopilotClient.cs
│   │   └── AIMeeting.Copilot.csproj
│   │
│   └── AIMeeting.Infrastructure/       # Infrastructure services
│       ├── Logging/                    # Logging setup
│       ├── Metrics/                    # Metrics collection
│       ├── Serialization/              # Serialization helpers
│       └── AIMeeting.Infrastructure.csproj
│
├── tests/
│   ├── AIMeeting.Core.Tests/
│   ├── AIMeeting.Integration.Tests/
│   └── AIMeeting.Copilot.Tests/
│
├── config/
│   └── agents/                         # Agent configurations
│       ├── project-manager.txt
│       ├── senior-developer.txt
│       └── ...
│
└── docs/                               # Additional documentation
    └── (generated meeting artifacts)
```

## Architecture Overview

The system is built on a clean, modular architecture:

```
┌──────────────────────────────┐
│     CLI Interface            │
│  (Meeting Initialization)    │
└───────────────┬──────────────┘
                │
┌───────────────▼──────────────────────┐
│   Meeting Orchestrator               │
│   - Lifecycle management             │
│   - Limit enforcement                │
│   - Agent coordination               │
└───────┬──────────────────┬──────────┘
        │                  │
┌───────▼─────────┐   ┌────▼─────────────┐
│  Message Bus    │   │  Meeting Room    │
│  - Event Pub/Sub│   │  - Shared Space  │
│  - Turn Queue   │   │  - File Locking  │
└────────┬────────┘   └──────────────────┘
         │
    ┌────┴─────────┬──────────┬──────────┐
    │              │          │          │
┌───▼──┐    ┌──────▼──┐  ┌───▼──┐  ┌───▼─────┐
│Agent1│    │ Agent2  │  │Agent3│  │Moderator│
└───────┘    └─────────┘  └──────┘  └────────┘
    │              │          │          │
    └──────────────┴──────────┴──────────┘
              │
    ┌─────────▼────────────┐
    │ GitHub Copilot CLI   │
    │ (Session Management) │
    └──────────────────────┘
```

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed documentation.

## Configuration

### Agent Configuration

Create a `.txt` file in `config/agents/`:

```
ROLE: Senior Developer
DESCRIPTION: Evaluates technical feasibility and implementation details

PERSONA:
- Pragmatic and detail-oriented
- Focuses on implementation challenges
- Advocates for code quality and maintainability

INSTRUCTIONS:
- Consider implementation complexity
- Identify potential technical debt
- Suggest practical solutions

RESPONSE_STYLE: Technical, code-focused
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Backend Architecture, Performance, Code Quality
```

See [AGENT_CONFIGURATION_GUIDE.md](AGENT_CONFIGURATION_GUIDE.md) for complete reference.

### Meeting Configuration **[v0.2]**

Meeting templates are planned for v0.2 and are not part of the v0.1 MVP.

## Meeting Output

### v0.1 Output (Current)

Each meeting generates:

```
meetings/
└── 20260130_143022_topic_slug/
    ├── meeting.json                # Meeting metadata snapshot [v0.1]
    ├── transcript.md               # Full conversation log [v0.1]
    └── errors.log                  # Error details (if failures occur) [v0.1]
```

### v0.2+ Output (Planned)

Future releases will include enhanced artifacts:

```
meetings/
└── 20260130_143022_topic_slug/
    ├── meeting.json                # [v0.1]
    ├── transcript.md               # [v0.1]
    ├── summary.md                  # [v0.2] Meeting summary
    ├── action_items.md             # [v0.2] Extracted action items
    ├── decisions.md                # [v0.2] Key decisions made
    ├── agents/                     # [v0.2] Agent-specific notes
    │   ├── project_manager_notes.md
    │   └── developer_notes.md
    ├── artifacts/                  # [v0.2] Shared documents
    │   └── (agent-created documents)
    └── .metadata/                  # [v0.2] System files
        ├── events.log              # Event stream
        └── metrics.json            # Meeting metrics
```

## API Reference

The system provides several key interfaces for integration:

```csharp
// Run a meeting programmatically
var orchestrator = serviceProvider.GetRequiredService<IMeetingOrchestrator>();
var result = await orchestrator.RunMeetingAsync(configuration, cancellationToken);

// Subscribe to events
var eventBus = serviceProvider.GetRequiredService<IEventBus>();
eventBus.Subscribe<TurnCompletedEvent>(async (evt) => {
    Console.WriteLine($"Agent {evt.AgentId} spoke");
});

// Access meeting room
var meetingRoom = serviceProvider.GetRequiredService<IMeetingRoom>();
var content = await meetingRoom.ReadFileAsync("transcript.md");
```

See [API.md](API.md) for complete API documentation.

## Extending

The system is designed to be extended:

- **Custom Agents**: Inherit from `AgentBase`
- **Turn Strategies**: Implement `ITurnManager`
- **Event Handlers**: Subscribe to any event type
- **File Operations**: Extend `IMeetingRoom`

See [EXTENDING.md](EXTENDING.md) for detailed extension guide.

## Naming Conventions

This project follows C# and .NET standards:

| Type | Convention | Example |
|------|------------|---------|
| **Namespaces** | PascalCase | `AIMeeting.Core.Agents` |
| **Classes** | PascalCase | `MeetingOrchestrator` |
| **Interfaces** | I + PascalCase | `IEventBus` |
| **Methods** | PascalCase + Async suffix | `GenerateResponseAsync` |
| **Properties** | PascalCase | `AgentId` |
| **Fields** | _camelCase | `_eventBus` |
| **Parameters** | camelCase | `agentId` |
| **Local Variables** | camelCase | `currentAgent` |

## Error Handling

The system provides a hierarchy of specific exceptions:

- `AgentMeetingException` - Base exception
- `MeetingConfigurationException` - Invalid configuration
- `AgentInitializationException` - Agent setup failed
- `TurnTimeoutException` - Agent exceeded time limit
- `FileLockException` - File lock acquisition failed
- `MeetingLimitExceededException` - Hard limit reached

All errors include contextual information for debugging and recovery.

## Logging

The system uses structured logging with Serilog:

```csharp
// Configure logging levels
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    { "Name": "Console" },
    { "Name": "File", "Args": { "path": "logs/meeting-.log" } }
  ]
}
```

Log levels:
- **Debug**: Turn coordination, file operations
- **Information**: Meeting lifecycle, key decisions
- **Warning**: Recoverable errors, degraded performance
- **Error**: Failed operations, agent errors
- **Critical**: System-level failures

## Security

Key security features:

- **Configuration Isolation**: Agents load only from designated config directories
- **Meeting Room Isolation**: Each meeting has its own isolated file space
- **Path Traversal Protection**: All file operations validate paths
- **File Locking**: Prevents concurrent file corruption

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed security considerations.

## Troubleshooting

### "Copilot CLI not found"
**Problem**: System cannot find `gh copilot` command  
**Solution**:
- Install GitHub Copilot CLI: https://github.com/github/copilot-cli
- Verify installation: `gh copilot --version`
- Ensure `gh` is in your PATH

### "Agent config validation failed"
**Problem**: Agent configuration file has errors  
**Solution**:
- Check required fields: ROLE, DESCRIPTION, INSTRUCTIONS
- Run validation: `aimeeting validate-config <file>`
- See [AGENT_CONFIGURATION_GUIDE.md](AGENT_CONFIGURATION_GUIDE.md) for format

### "Meeting timed out"
**Problem**: Meeting exceeded configured duration  
**Solution**:
- Increase `--max-duration` parameter (in minutes)
- Check GitHub Copilot API status
- Verify network connectivity

### "File lock timeout"
**Problem**: Cannot acquire lock on meeting directory  
**Solution**:
- Close other processes accessing the meeting directory
- Check disk space availability
- Verify write permissions on `meetings/` directory

### "No agents available"
**Problem**: No valid agent configurations found  
**Solution**:
- Ensure agent config files exist in `config/agents/`
- Validate each config: `aimeeting validate-config <file>`
- Check file paths are correct in CLI arguments

### Build Errors
**Problem**: `dotnet build` fails  
**Solution**:
- Ensure .NET 8 SDK installed: `dotnet --version`
- Restore packages: `dotnet restore`
- Clean solution: `dotnet clean`

## FAQ

**Q: Do I need a paid Copilot subscription?**  
A: Yes, GitHub Copilot CLI requires an active GitHub Copilot subscription.

**Q: Can I use without internet?**  
A: No, GitHub Copilot API requires internet connectivity for agent responses.

**Q: How much does a meeting cost?**  
A: Depends on your Copilot plan. Each agent turn uses API quota. Typical 30-minute meeting with 4 agents might use 20-50 API calls.

**Q: Can agents remember previous meetings?**  
A: Not in v0.1. Agents are stateless within each meeting. Meeting memory across sessions is planned for v0.3+.

**Q: What if an agent fails?**  
A: Meeting continues with remaining agents (if >= 2 agents remain). Failed agent is skipped for remaining turns.

**Q: How do I stop a meeting early?**  
A: Press Ctrl+C to cancel. Meeting artifacts are saved up to the cancellation point.

**Q: How many agents can participate in a meeting?**  
A: Recommended 2-6 agents for quality conversations. Technical limit is higher but response time increases with more agents.

**Q: Can I use this with other AI models (not Copilot)?**  
A: v0.1 supports GitHub Copilot CLI only. Multi-provider support (OpenAI, Azure, local models) is planned for v0.4. See [ROADMAP.md](ROADMAP.md).

**Q: Is there a web interface?**  
A: Not in v0.1 (CLI only). Web UI is not currently on the roadmap but could be added based on community feedback.

**Q: Can I run multiple meetings simultaneously?**  
A: Not in v0.1 (single meeting per process). Concurrent meeting support is planned for v0.3.

## Testing

```bash
# Run all tests
AIMEETING_AGENT_MODE=stub dotnet test

# Run specific test project
AIMEETING_AGENT_MODE=stub dotnet test tests/AIMeeting.Core.Tests

# Run with verbose output
AIMEETING_AGENT_MODE=stub dotnet test -v detailed

# Generate coverage report
AIMEETING_AGENT_MODE=stub dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

Target: ≥80% code coverage (overall), critical paths higher (parser/validation)

## Roadmap

**v0.1** (Current/MVP): Core foundation - CLI, config validation, FIFO turn-taking, transcript generation  
**v0.2** (Planned): Enhanced artifacts - summaries, action items, decisions, templates  
**v0.3** (Planned): Robustness & scale - concurrent meetings, dynamic turn-taking, persistence  
**v0.4** (Planned): Advanced features - RAG integration, multi-provider support  
**v1.0** (Planned): Production release - HTTP API, cloud deployment, hardening  

See [ROADMAP.md](ROADMAP.md) for detailed feature timeline and version planning.

---

**Project Status**: Pre-release (v0.1.0 in development)  
**Last Updated**: January 30, 2026  
**Maintained by**: [Your Team]
