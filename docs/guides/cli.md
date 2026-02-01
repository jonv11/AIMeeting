# AIMeeting CLI Quick Reference Guide

## Current Status: v0.1.1 Released
✅ **validate-config** command - Fully functional  
✅ **start-meeting** command - Fully functional

---

## Building the Project

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Run all tests
dotnet test
```

---

## CLI Commands

### 1. validate-config - Validate Agent Configuration Files

**Purpose:** Check if an agent configuration file is valid before using it in meetings

**Syntax:**
```bash
dotnet run --project src/AIMeeting.CLI -- validate-config <path>
```

**Arguments:**
- `<path>` - Path to the agent configuration file to validate

**Exit Codes:**
- `0` - Configuration is valid
- `1` - Configuration has errors

**Output Format:**
- On success: ✓ plus config details
- On error: ✗ plus error messages with line numbers
- Warnings: Shown but don't cause failure

---

## Examples

### Example 1: Validate a Valid Config

```bash
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/test-agent.txt
```

**Output:**
```
✓ Configuration is valid: config/agents/test-agent.txt
  Role: Test Developer
  Description: A test agent for validation
  Instructions: 2
```

### Example 2: Validate with Errors

**Command:**
```bash
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/invalid.txt
```

**File Contents (invalid.txt):**
```
DESCRIPTION: Missing role
INSTRUCTIONS:
- Consider complexity
```

**Output:**
```
Error: Missing required field: ROLE
✗ Configuration validation failed
```

### Example 3: Validate with Warnings

**Command:**
```bash
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/with-warning.txt
```

**File Contents (with-warning.txt):**
```
ROLE: Developer
DESCRIPTION: Test agent
INSTRUCTIONS:
- Test instruction
CUSTOM_FIELD: value
```

**Output:**
```
Warning: Line 5: Unknown header 'CUSTOM_FIELD': will be stored but not used
✓ Configuration is valid: config/agents/with-warning.txt
  Role: Developer
  Description: Test agent
  Instructions: 1
```

### Example 4: Get Help

```bash
dotnet run --project src/AIMeeting.CLI -- --help
```

**Output:**
```
AIMeeting - Multi-Agent Meeting System

Usage:
  AIMeeting [command] [options]

Commands:
  validate-config    Validate an agent configuration file

Options:
  --help            Show help information
```

---

## Configuration File Format

### Required Fields

```
ROLE: <agent role name>
DESCRIPTION: <what this agent does>
INSTRUCTIONS:
- <instruction 1>
- <instruction 2>
- <instruction N>
```

### Optional Fields

```
PERSONA:
- <trait 1>
- <trait 2>

RESPONSE_STYLE: <style description>
MAX_MESSAGE_LENGTH: <number>
EXPERTISE_AREAS: <area1>, <area2>, <area3>
COMMUNICATION_APPROACH: <approach>
INITIAL_MESSAGE_TEMPLATE: <template with {topic} placeholder>
```

### Example Complete Config

**File: config/agents/senior-developer.txt**
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
- Ask clarifying questions about scalability

RESPONSE_STYLE: Technical, code-focused
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Backend Architecture, Performance, Code Quality
COMMUNICATION_APPROACH: Direct and practical
INITIAL_MESSAGE_TEMPLATE: Let me evaluate {topic} from a technical perspective.
```

---

## Rules & Constraints

### Parser Rules
- **Encoding:** UTF-8 only
- **Max File Size:** 64 KB
- **Line Endings:** Any (Windows \r\n, Unix \n, Mac \r)
- **Comments:** Lines starting with # are ignored
- **Whitespace:** Trimmed from values
- **Sections:** PERSONA and INSTRUCTIONS support multi-line format

### Validation Rules
- **Required:** ROLE, DESCRIPTION, INSTRUCTIONS (at least 1)
- **MAX_MESSAGE_LENGTH:** Must be a positive integer
- **EXPERTISE_AREAS:** Comma-separated list
- **Unknown Fields:** Stored but generate warnings

---

## Testing Configuration Parsing

### Run All Configuration Tests

```bash
dotnet test tests/AIMeeting.Core.Tests/Configuration/
```

**Expected Output:**
```
Test Run Successful.
Total tests: 27
     Passed: 27
     Failed: 0
 Skipped: 0
```

### Run Specific Test

```bash
dotnet test tests/AIMeeting.Core.Tests/Configuration/ -v detailed --filter "Parse_ValidMinimalConfig"
```

---

## Troubleshooting

### Issue: "Invalid UTF-8 encoding" error
**Cause:** File is not saved in UTF-8 encoding  
**Solution:** Open file in text editor and save as UTF-8

### Issue: "File size exceeds maximum" error
**Cause:** Configuration file is larger than 64 KB  
**Solution:** Simplify the configuration or break into multiple files

### Issue: "Missing required field" error
**Cause:** One of ROLE, DESCRIPTION, or INSTRUCTIONS is missing  
**Solution:** Ensure all three required fields are present

### Issue: "Invalid line format" error
**Cause:** Line doesn't have a colon (`:`) or is malformed  
**Solution:** Check format - should be `KEY: VALUE` or `KEY:` (for multi-line sections)

---

## Environment Variables

### AIMEETING_AGENT_MODE
Controls agent response generation mode

**Values:**
- `stub` - Use deterministic stub responses (for testing)
- (empty/unset) - Try Copilot first, fallback to stub if unavailable

**Example:**
```bash
AIMEETING_AGENT_MODE=stub dotnet test
```

### DEBUG
Enable detailed error output

**Example:**
```bash
DEBUG=1 dotnet run --project src/AIMeeting.CLI -- validate-config file.txt
```

---

## Project Structure Reference

```
src/
├── AIMeeting.CLI/                 # Command-line interface
│   ├── Commands/                  # CLI command implementations
│   │   └── ValidateConfigCommand.cs
│   └── Program.cs
├── AIMeeting.Core/                # Core business logic
│   ├── Models/                    # Data models
│   ├── Agents/                    # Agent implementations
│   ├── Events/                    # Event system
│   ├── Orchestration/             # Meeting coordination
│   ├── FileSystem/                # File operations
│   ├── Prompts/                   # Prompt generation
│   ├── Exceptions/                # Exception types
│   └── Configuration/             # Configuration parsing
├── AIMeeting.Copilot/             # Copilot CLI integration
└── AIMeeting.Infrastructure/      # Infrastructure services

tests/
├── AIMeeting.Core.Tests/          # Core unit tests
│   └── Configuration/             # 27 passing tests
├── AIMeeting.Integration.Tests/   # Integration tests
└── AIMeeting.Copilot.Tests/       # Copilot tests
```

---

## Next Features (Coming Soon)

### start-meeting Command
```bash
dotnet run --project src/AIMeeting.CLI -- start-meeting \
  "Discuss microservices architecture" \
  --agents config/agents/project-manager.txt \
           config/agents/senior-developer.txt \
           config/agents/security-expert.txt \
  --max-duration 30 \
  --max-messages 50
```

**Features:**
- Real-time meeting progress display
- Transcript generation
- Artifact creation (meeting.json, transcript.md)
- Graceful shutdown with Ctrl+C

---

## Useful Commands for Development

### Watch Build Changes
```bash
dotnet watch -p src/AIMeeting.CLI build
```

### Run Tests with Coverage Report
```bash
dotnet test /p:CollectCoverage=true
```

### List All Available Commands
```bash
dotnet run --project src/AIMeeting.CLI -- --help
```

### Run Tests in Parallel
```bash
dotnet test -p:ParallelizeTestCollections=true
```

---

## Support & Documentation

- **Architecture:** See `ARCHITECTURE.md`
- **API Reference:** See `API.md`
- **Agent Configuration:** See `AGENT_CONFIGURATION_GUIDE.md`
- **Examples:** See `EXAMPLES.md`
- **Full Report:** See `IMPLEMENTATION_REPORT.md`

---

## Version Information

| Component | Status |
|-----------|--------|
| .NET Target | 8.0 |
| C# Version | 12.0 |
| Build Status | ✅ Successful |
| Tests Passing | 27/27 ✅ |

**Last Updated:** January 31, 2026
