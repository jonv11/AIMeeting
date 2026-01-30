# AIMeeting - Multi-Agent Meeting System

A CLI-based multi-agent meeting system where AI agents with different roles engage in structured discussions about specific topics. The system simulates real-world meetings with agents taking turns, sharing perspectives, and collaborating to reach conclusions.

## Features

- **Structured AI Collaboration**: Enable multiple AI agents to discuss complex topics from different perspectives
- **Configurable Roles**: Text-based configuration allows any role/personality without code changes
- **Meeting Artifacts**: Automatic generation of transcripts, summaries, and action items
- **Event-Driven Architecture**: Message bus pattern ensures clean agent coordination
- **Extensible Design**: Support wide range of use cases beyond software development

## Quick Start

### Prerequisites

- .NET 8.0 or later
- GitHub Copilot CLI installed ([installation guide](https://github.com/github/copilot-cli))
- Active GitHub Copilot subscription
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
   ```

3. Verify installation:
   ```bash
   dotnet build
   dotnet test
   ```

### Run Your First Meeting

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
│   ├── agents/                         # Agent configurations
│   │   ├── project-manager.txt
│   │   ├── senior-developer.txt
│   │   └── ...
│   └── meetings/                       # Meeting templates
│       └── default-meeting.json
│
└── docs/                               # Additional documentation
    └── (generated meeting artifacts)
```

## Development

### Building

```bash
# Build solution
dotnet build

# Build with specific configuration
dotnet build -c Release
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Running

```bash
# Development mode
dotnet run --project src/AIMeeting.CLI

# With arguments
dotnet run --project src/AIMeeting.CLI -- start-meeting --topic "Your topic"
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
    │ GitHub Copilot SDK   │
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

### Meeting Configuration

Create a `.json` file in `config/meetings/`:

```json
{
  "meetingTopic": "Discuss system scalability",
  "agentConfigs": [
    "agents/project-manager.txt",
    "agents/senior-developer.txt",
    "agents/moderator.txt"
  ],
  "hardLimits": {
    "maxDurationMinutes": 30,
    "maxTotalMessages": 50
  }
}
```

## Meeting Output

Each meeting generates the following artifacts:

```
meetings/
└── 20260130_143022_topic_slug/
    ├── config.json                 # Meeting configuration snapshot
    ├── transcript.md               # Full conversation log
    ├── summary.md                  # Meeting summary
    ├── action_items.md             # Extracted action items
    ├── decisions.md                # Key decisions made
    ├── agents/                     # Agent-specific notes
    │   ├── project_manager_notes.md
    │   └── developer_notes.md
    ├── artifacts/                  # Shared documents
    │   └── (agent-created documents)
    └── .metadata/                  # System files
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
- **Rate Limiting**: Throttles concurrent Copilot requests

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed security considerations.

## FAQ

**Q: Do I need an API key for GitHub Copilot?**  
A: No! The system uses GitHub Copilot CLI which handles authentication automatically. You just need an active GitHub Copilot subscription.

**Q: How long does a meeting take?**  
A: Depends on your configuration. Default is 30 minutes with limits on message count. Can be configured from 5-120 minutes.

**Q: Can I use this system without Copilot?**  
A: Currently, the system is built to use GitHub Copilot SDK. Alternative AI providers could be integrated by implementing `ICopilotClient`.

**Q: How do I customize agent behavior?**  
A: Create a `.txt` configuration file in `config/agents/` with role, persona, and instructions. No code changes needed!

**Q: Can agents access external information?**  
A: Version 1.0 relies on agents' training. RAG integration is planned for Phase 4.

**Q: What happens if an agent fails?**  
A: The system attempts to retry, then removes the failed agent and continues with remaining agents if possible.

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Make changes following naming conventions
4. Add tests for new functionality
5. Submit a pull request with clear description

See [ARCHITECTURE.md](ARCHITECTURE.md) for commit message conventions.

## Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/AIMeeting.Core.Tests

# Run with verbose output
dotnet test -v detailed

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

Target: >75% code coverage

## Roadmap

**Phase 1** (v0.1): Core foundation - basic working system  
**Phase 2** (v0.2): Meeting room & artifacts - full artifact generation  
**Phase 3** (v0.3): Robustness - comprehensive error handling  
**Phase 4** (v0.4): Advanced features - dynamic turn-taking, RAG integration  
**Phase 5** (v1.0): Production release - documentation, optimization, hardening  

See [DRAFT.md](DRAFT.md) for detailed roadmap.

## License

This project is licensed under the MIT License - see [LICENSE](LICENSE) file for details.

## Support

- 📖 **Documentation**: See docs/ directory
- 💬 **Issues**: GitHub Issues
- 📧 **Email**: support@example.com
- 🔗 **Discussions**: GitHub Discussions

## Acknowledgments

- Built with C# and .NET 8
- Powered by GitHub Copilot SDK
- Inspired by real-world meeting dynamics

---

**Project Status**: Pre-release (v0.1.0)  
**Last Updated**: January 30, 2026  
**Maintained by**: [Your Team]
