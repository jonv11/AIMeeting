# Examples

This document provides MVP-focused examples for running AIMeeting from the CLI.

## Table of Contents

1. [Architecture Decision](#architecture-decision)
2. [Security Review](#security-review)
3. [Simple Test Meeting](#simple-test-meeting)

## Architecture Decision

### Scenario

Evaluate a technical architecture choice from multiple perspectives.

### Command Line

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Evaluate microservices vs monolith for payment service" \
    --agents "config/agents/project-manager.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/security-expert.txt" \
             "config/agents/moderator.txt" \
    --max-duration 20 \
    --max-messages 40
```

### Expected Output (v0.1)

The meeting creates a new directory under `meetings/` containing:
- `meeting.json`
- `transcript.md`
- `errors.log` (only if failures occur)

## Security Review

### Scenario

Review an authentication design for security risks.

### Command Line

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Review OAuth2 implementation security" \
    --agents "config/agents/security-expert.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/project-manager.txt" \
    --max-duration 15 \
    --max-messages 30
```

## Simple Test Meeting

### Scenario

Minimal two-agent meeting for quick validation.

### Command Line

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Test agent interaction" \
    --agents "config/agents/senior-developer.txt" \
             "config/agents/moderator.txt" \
    --max-duration 5 \
    --max-messages 10
```

---

**Version**: 1.0  
**Last Updated**: January 31, 2026
