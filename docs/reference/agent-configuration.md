# Agent Configuration Guide

## Overview

AIMeeting agents are configured via simple text files, requiring no code changes. This guide explains how to create and customize agent configurations for your specific use cases.

## Configuration File Format

Agent configuration files are plain text files (`.txt`) located in the `config/agents/` directory. Each file defines a single agent role with its characteristics and behavior.

### Basic Structure

```
ROLE: [Agent Role Name]
DESCRIPTION: [What this agent does]

PERSONA:
- [Characteristic 1]
- [Characteristic 2]
- [Characteristic 3]

INSTRUCTIONS:
- [Instruction 1]
- [Instruction 2]
- [Instruction 3]

INITIAL_MESSAGE_TEMPLATE: [Template for agent's first message]
RESPONSE_STYLE: [Communication style description]
MAX_MESSAGE_LENGTH: [Integer - maximum characters per response]
EXPERTISE_AREAS: [Comma-separated list of expertise areas]
COMMUNICATION_APPROACH: [Optional guidance for how the agent communicates]
```

## Field Reference

### ROLE (Required)

The agent's professional title or role in the meeting.

**Examples**:
```
ROLE: Project Manager
ROLE: Senior Developer
ROLE: Security Expert
ROLE: DevOps Engineer
ROLE: QA Lead
```

### DESCRIPTION (Required)

A concise description of the agent's purpose and value in the meeting. This helps other agents understand this agent's perspective.

**Examples**:
```
DESCRIPTION: Oversees project timeline, resources, and deliverables
DESCRIPTION: Evaluates technical feasibility and implementation approach
DESCRIPTION: Ensures system security and compliance with security standards
```

### PERSONA (Optional)

A bulleted list of personality traits and characteristics. These guide how the agent approaches discussions and what they prioritize.

**Format**:
```
PERSONA:
- Characteristic 1
- Characteristic 2
- Characteristic 3
- ...
```

### INSTRUCTIONS (Required)

A bulleted list of specific guidelines for how the agent should behave. These are more specific than persona traits and guide decision-making.

**Format**:
```
INSTRUCTIONS:
- [Action or guideline 1]
- [Action or guideline 2]
- ...
```

### Validation Rules

Per project decisions, the CLI `validate-config` enforces the following rules:

- Required fields: `ROLE`, `DESCRIPTION`, and `INSTRUCTIONS` must be present and non-empty. Missing these fields causes validation to fail (exit code `1`).
- Optional fields: `PERSONA`, `INITIAL_MESSAGE_TEMPLATE`, `RESPONSE_STYLE`, `MAX_MESSAGE_LENGTH`, `EXPERTISE_AREAS`, `COMMUNICATION_APPROACH` are optional. If present, they are validated for basic syntax (e.g., `MAX_MESSAGE_LENGTH` must be a positive integer).
- Unknown headers are permitted and produce a warning only (validation still succeeds with exit code `0`).
- Error messages include the section name and line number when available to help users fix issues quickly.

Example error:
```
Missing required field: ROLE (config/agents/pm.txt:1)
```

### INITIAL_MESSAGE_TEMPLATE (Optional)

A template for the agent's opening message in a meeting. Use `{topic}` placeholder for the meeting topic.

**Format**:
```
INITIAL_MESSAGE_TEMPLATE: [Message with optional {topic} placeholder]
```

**Examples**:
```
INITIAL_MESSAGE_TEMPLATE: As Project Manager, let me understand: {topic}. What are our key objectives and constraints?
```

### RESPONSE_STYLE (Optional)

Describes the communication style and tone for the agent's responses.

**Examples**:
```
RESPONSE_STYLE: Professional, direct, action-oriented
RESPONSE_STYLE: Technical, code-focused, pragmatic
```

### MAX_MESSAGE_LENGTH (Optional)

Maximum number of characters per response. Helps keep responses concise and focused. If provided, must be a positive integer.

**Typical values**:
- `400-500`: Encourages concise, focused responses
- `600-800`: Standard conversation length
- `1000+`: Allows detailed technical explanations

**Example**:
```
MAX_MESSAGE_LENGTH: 500
```

### EXPERTISE_AREAS (Optional)

Comma-separated list of areas where this agent has special knowledge or focus. Helps contextualize their perspective.

**Format**:
```
EXPERTISE_AREAS: [Area 1], [Area 2], [Area 3]
```

## Parser Constraints

- Files must be UTF-8 encoded, maximum size **64 KB**.
- A section header matches `^[A-Z0-9_ ]+:` and continues until the next header or EOF. Line breaks inside a section are preserved.
- Blank lines are allowed and used to separate paragraphs.
- Line endings are normalized to `\n` during parsing.

## CLI Usage

Validate an agent configuration file via the CLI:

```bash
# Returns exit code 0 on success, 1 on validation errors
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/your-agent.txt
```

## v0.1 Sample Configurations

The repository includes sample configurations aligned with v0.1 documentation:

- `config/agents/project-manager.txt`
- `config/agents/senior-developer.txt`
- `config/agents/security-expert.txt`
- `config/agents/moderator.txt`

## Orchestrator Configuration

Orchestrator agents control meeting flow and **must** return valid JSON decisions. Use the orchestrator config template:

- `config/agents/orchestrator.txt`

Key requirements for orchestrators:
- `ROLE` must be set to `Orchestrator`
- `INSTRUCTIONS` must enforce **JSON-only** responses
- Responses must match the orchestrator response format

See the response format reference:
- [docs/reference/orchestrator-response-format.md](orchestrator-response-format.md)

## Troubleshooting Configuration Issues

### Agent Responds Off-Topic

**Issue**: Agent isn't staying focused on their role

**Solution**: Review `INSTRUCTIONS` and `PERSONA`. Add more specific guidance:
```
INSTRUCTIONS:
- Stay focused on your area of expertise
- Redirect off-topic suggestions back to your domain
- Ask clarifying questions if proposals seem outside your area
```

### Agent Responses Too Long/Short

**Issue**: Responses aren't the right length

**Solution**: Adjust `MAX_MESSAGE_LENGTH`:
- Too verbose? Reduce from 800 to 500
- Too terse? Increase from 300 to 500

### Agent Not Participating Enough

**Issue**: Agent rarely speaks up

**Solution**: Review `INSTRUCTIONS`. Add:
```
INSTRUCTIONS:
- Ask at least one question per turn
- Challenge assumptions you disagree with
- Don't hold back your perspective
```

## Configuration File Checklist

- [ ] ROLE is clear and descriptive
- [ ] DESCRIPTION explains the agent's value
- [ ] PERSONA has 3-5 characteristics (if provided)
- [ ] INSTRUCTIONS have 5-7 specific guidelines
- [ ] RESPONSE_STYLE is consistent with PERSONA (if provided)
- [ ] MAX_MESSAGE_LENGTH is reasonable (400-800, if provided)
- [ ] EXPERTISE_AREAS are specific and relevant (if provided)
- [ ] No contradictions between PERSONA and INSTRUCTIONS
- [ ] Configuration file has no syntax errors
- [ ] File is located in `config/agents/` directory

## Next Steps

1. Copy a template agent configuration
2. Customize ROLE, DESCRIPTION, PERSONA, and INSTRUCTIONS
3. Test with `validate-config` command
4. Run a test meeting
5. Refine based on agent behavior
6. Add to your project's agent roster

---

**Version**: 1.0  
**Last Updated**: January 30, 2026
