# GitHub Copilot Instructions for AIMeeting

## Repository Overview

AIMeeting is a CLI-based multi-agent meeting orchestration system where AI agents with different roles engage in structured discussions. The system simulates real-world meetings with agents taking turns, sharing perspectives, and collaborating to reach conclusions.

- **Language**: C# / .NET 8.0
- **Project Type**: Console application with library components
- **Target Runtime**: .NET 8.0+ (cross-platform: Windows, Linux, macOS)
- **Repository Size**: ~60 files, focused codebase
- **Current Version**: v0.1.1 (Released February 1, 2026)
- **Repository Status**: Public

## Build & Development Commands

### Prerequisites
- .NET 8.0 SDK or later
- GitHub Copilot CLI (for running actual meetings, not required for testing)
- Git

### Build Commands
```bash
# Restore dependencies (always run first)
dotnet restore

# Build solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

### Testing Commands
**IMPORTANT**: Always set `AIMEETING_AGENT_MODE=stub` when running tests to avoid external API dependencies.

```bash
# Run all tests (PowerShell)
$env:AIMEETING_AGENT_MODE="stub"; dotnet test

# Run tests with coverage
$env:AIMEETING_AGENT_MODE="stub"; dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=80 /p:ThresholdType=line

# Run specific test project
$env:AIMEETING_AGENT_MODE="stub"; dotnet test tests/AIMeeting.Core.Tests
```

**Test Requirements**:
- All tests must pass before submitting PR
- Minimum 80% code coverage for new code
- Critical paths (parser, validation, orchestration) require higher coverage

### Running the Application
```bash
# Validate agent configuration
dotnet run --project src/AIMeeting.CLI -- validate-config <path-to-config>

# Start a meeting
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Your topic" \
    --agents "config/agents/agent1.txt" "config/agents/agent2.txt" \
    --max-duration 30 \
    --max-messages 50
```

## Project Structure

```
AIMeeting/
├── src/
│   ├── AIMeeting.CLI/           # Command-line interface
│   │   ├── Commands/            # CLI command handlers (StartMeetingCommand, ValidateConfigCommand)
│   │   ├── Display/             # Console UI (MeetingProgressDisplay)
│   │   ├── Errors/              # Error handling
│   │   └── Program.cs           # Entry point
│   ├── AIMeeting.Core/          # Core business logic
│   │   ├── Agents/              # Agent implementations (AgentBase, StandardAgent)
│   │   ├── Configuration/       # Config parsing (AgentConfigurationParser, AgentConfigurationValidator)
│   │   ├── Events/              # Event system (IEventBus, InMemoryEventBus)
│   │   ├── FileSystem/          # Meeting room & file operations (IMeetingRoom)
│   │   ├── Models/              # Data models (MeetingConfiguration, AgentConfiguration)
│   │   ├── Orchestration/       # Meeting coordination (MeetingOrchestrator, FifoTurnManager)
│   │   └── Prompts/             # Prompt engineering (IPromptBuilder)
│   ├── AIMeeting.Copilot/       # GitHub Copilot CLI integration
│   │   ├── CopilotClient.cs     # CLI wrapper
│   │   └── ICopilotClient.cs    # Interface
│   └── AIMeeting.Infrastructure/ # Cross-cutting concerns
│       ├── Logging/             # Serilog configuration
│       └── Serialization/       # JSON helpers
├── tests/
│   ├── AIMeeting.Core.Tests/          # Unit tests for Core
│   ├── AIMeeting.Copilot.Tests/       # Unit tests for Copilot
│   └── AIMeeting.Integration.Tests/   # Integration tests
├── config/agents/               # Agent configuration files (20+ samples)
├── docs/                        # Documentation hub
│   ├── guides/standards/        # Coding standards (13 files - MANDATORY reading)
│   ├── reference/               # API, configuration, extending guides
│   ├── learning/                # Getting started, FAQ, roles
│   └── planning/                # Version planning
├── .github/
│   └── workflows/ci.yml         # CI/CD pipeline
├── README.md                    # Project overview
├── ARCHITECTURE.md              # System design
└── CONTRIBUTING.md              # Contribution guidelines
```

## Critical Architecture Concepts

### Event-Driven Design
The system uses an in-memory event bus (`IEventBus`) for decoupled communication:
- `TurnStartedEvent`, `TurnCompletedEvent` - Turn management
- `MeetingStartedEvent`, `MeetingEndedEvent` - Lifecycle
- `FileCreatedEvent` - File operations

### Agent Configuration Format
Agents are configured via text files in `config/agents/`:
- **Required fields**: ROLE, DESCRIPTION, INSTRUCTIONS
- **Optional fields**: PERSONA, RESPONSE_STYLE, MAX_MESSAGE_LENGTH, EXPERTISE_AREAS
- Parser: `AgentConfigurationParser` (UTF-8, max 64KB)
- Validator: `AgentConfigurationValidator`

### Turn Management
- v0.1: FIFO (First-In-First-Out) turn-taking via `FifoTurnManager`
- v0.2+: Dynamic turn-taking (planned)

### File System Isolation
Each meeting operates in an isolated directory under `meetings/` (created at runtime):
- `transcript.md` - Conversation log
- `meeting.json` - Meeting metadata
- `errors.log` - Error details (if any)

## Validation & CI/CD

### GitHub Actions CI
The `.github/workflows/ci.yml` pipeline runs on all branches:
- **Matrix**: ubuntu-latest, windows-latest, macos-latest
- **Steps**:
  1. Checkout code
  2. Setup .NET 8.0
  3. Restore dependencies
  4. Build (no warnings)
  5. Test with coverage (80% threshold required)
  6. Upload coverage artifacts

**Branch Protection**: The `main` branch is protected:
- ✅ Requires 1 PR approval
- ✅ All CI checks must pass (all 3 platforms)
- ✅ Branches must be up-to-date before merging
- ✅ Conversations must be resolved
- ✅ No direct commits to main (PRs only)

### Pre-commit Validation
Before creating a PR:
1. Run all tests: `$env:AIMEETING_AGENT_MODE="stub"; dotnet test`
2. Build in Release: `dotnet build -c Release`
3. Validate no errors: Check output for warnings/errors
4. Update documentation if adding public APIs

## Coding Standards (MANDATORY)

All contributors must follow these standards (see `docs/guides/standards/` for full details):

### Critical Standards
1. **Security** (`security.md`)
   - Path traversal protection (validate all file paths)
   - Input validation (config files limited to 64KB)
   - No secrets in code (use environment variables)

2. **Error Handling** (`error-handling.md`)
   - Use specific exception types: `AgentMeetingException` hierarchy
   - Include context in exceptions
   - Log errors with structured logging (Serilog)

3. **Testing** (`testing.md`)
   - Unit tests for all public APIs
   - Integration tests for cross-component features
   - Use `AIMEETING_AGENT_MODE=stub` for tests
   - Minimum 80% coverage

4. **Git Workflow** (`git-workflow.md`)
   - Create feature branches: `feature/your-feature-name`
   - **Never push directly to main** (branch protection enforced)
   - Commit messages: Present tense, < 50 chars first line
   - All changes via Pull Requests

### Important Standards
1. **Naming Conventions** (`naming-conventions.md`)
   - Classes: PascalCase (`MeetingOrchestrator`)
   - Interfaces: I + PascalCase (`IEventBus`)
   - Methods: PascalCase + Async suffix (`GenerateResponseAsync`)
   - Fields: _camelCase (`_eventBus`)
   - Properties: PascalCase (`AgentId`)

2. **API Design** (`api-design.md`)
   - Async all the way (use `async`/`await` consistently)
   - CancellationToken as last parameter
   - Return `Task<T>` or `Task` for async methods

## Common Pitfalls & Workarounds

### Issue 1: Tests Fail Without Stub Mode
**Problem**: Tests try to call GitHub Copilot CLI  
**Solution**: Always set `AIMEETING_AGENT_MODE=stub` before running tests  
**Command**: `$env:AIMEETING_AGENT_MODE="stub"; dotnet test`

### Issue 2: Coverage Threshold Failures
**Problem**: Test coverage below 80%  
**Solution**: Add unit tests for uncovered code paths  
**Command**: Check coverage report in `**/coverage.cobertura.xml`

### Issue 3: Direct Commit Blocked
**Problem**: Trying to push directly to main  
**Solution**: Create a feature branch and PR  
```bash
git checkout -b feature/my-feature
git push origin feature/my-feature
# Then create PR on GitHub
```

### Issue 4: Agent Config Validation Fails
**Problem**: Missing required fields in agent config  
**Solution**: Ensure ROLE, DESCRIPTION, and INSTRUCTIONS are present  
**Validation**: `dotnet run --project src/AIMeeting.CLI -- validate-config <file>`

## Dependencies & Packages

### Production Dependencies
- `System.CommandLine` - CLI argument parsing
- `Serilog` - Structured logging
- `Serilog.Sinks.Console` - Console output
- `Serilog.Sinks.File` - File logging
- `Newtonsoft.Json` or `System.Text.Json` - JSON serialization

### Test Dependencies
- `xUnit` - Test framework
- `Moq` - Mocking framework
- `coverlet.collector` - Code coverage
- `FluentAssertions` - Test assertions

## Key Documentation Files

When making changes:
- **Public API changes**: Update `docs/reference/api.md`
- **New features**: Update `README.md` and `docs/guides/roadmap.md`
- **Configuration changes**: Update `docs/reference/agent-configuration.md`
- **CLI changes**: Update `docs/guides/cli.md`
- **Architecture changes**: Update `ARCHITECTURE.md`

## Trust These Instructions

**IMPORTANT**: Trust these instructions and only search the codebase if:
- Information is incomplete or unclear
- Instructions appear outdated or incorrect
- Implementing a feature not covered here

For most tasks, these instructions contain everything needed to successfully build, test, and contribute to AIMeeting.
