# Orchestrator Guide

This guide explains how the orchestrator works, how to enable it, and how it integrates with meeting flow.

## What the Orchestrator Does

The orchestrator is a meta-controller that decides:

- Which agent speaks next
- When to change meeting phase
- When to end the meeting

It does **not** contribute content; it controls process and structure.

## How It Works

1. `MeetingOrchestrator` needs a next turn decision
2. `OrchestratorDrivenTurnManager` publishes `OrchestratorTurnRequestEvent`
3. The orchestrator responds with `OrchestratorDecisionEvent`
4. The turn manager resumes with the decision

## Enabling Orchestrator Mode

To enable orchestrator-driven meetings, include an orchestrator configuration file in your agent list:

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Design review" \
    --agents "config/agents/orchestrator.txt" \
             "config/agents/solutions-architect.txt" \
             "config/agents/principal-engineer.txt" \
             "config/agents/security-guardian.txt" \
    --max-duration 30 \
    --max-messages 50
```

If no orchestrator config is present, the system defaults to FIFO turn-taking.

## Orchestrator Response Format

Orchestrators must return strict JSON. See the reference format:

- [docs/reference/orchestrator-response-format.md](../reference/orchestrator-response-format.md)

## Stub Mode

For testing and CI, set:

```bash
$env:AIMEETING_AGENT_MODE="stub"
```

In stub mode, the orchestrator uses round-robin decisions and does not call Copilot.

## Real Copilot Integration (v0.1.2)

The orchestrator includes full prompt building and decision parsing logic for GitHub Copilot CLI integration:

### Prompt Building
- Meeting context (topic, phase, turn number)
- Available agent list (excluding orchestrator itself)
- Recent message history (last 5 messages)
- JSON response format specification
- Decision-making guidance

### Decision Parsing
- Extracts JSON from markdown code blocks
- Validates required fields per decision type
- Converts to type-safe `OrchestratorDecisionEvent`
- Validates MeetingPhase enum values

### Error Handling & Retry Logic
- **3 retry attempts** with exponential backoff
- Delay schedule: 500ms → 1000ms → 2000ms (capped at 5000ms)
- Retries on Copilot call failures and JSON parse errors
- Falls back to stub mode after exhausting retries
- Console logging for retry attempts and failures

### Integration Status
- ✅ Prompt building implemented
- ✅ JSON decision parsing implemented
- ✅ Retry logic with exponential backoff
- ⏳ Real Copilot CLI integration (pending Phase 5 testing)
- ⏳ Manual testing with live LLM responses
- ⏳ Prompt tuning based on decision quality

To test with real Copilot CLI once integrated, ensure `AIMEETING_AGENT_MODE` is **not** set to `stub`.

## Troubleshooting

**Orchestrator does not respond**
- The turn manager will timeout after 30 seconds
- The system falls back to FIFO for that turn
- Check logs for retry attempt details

**Invalid JSON responses**
- The orchestrator automatically retries (up to 3 attempts)
- Falls back to stub mode after failed retries
- Check console output for specific parse errors

**Meeting ends immediately**
- Ensure non-orchestrator agents are provided
- Orchestrator should not select itself as next agent

**Copilot integration errors**
- Verify GitHub Copilot SDK for .NET is installed (integrated in v0.1.1)
- Check network connectivity
- Review retry attempt logs in console output

## Related Documentation

- [Orchestrator response format](../reference/orchestrator-response-format.md)
- [CLI guide](cli.md)
- [Agent configuration reference](../reference/agent-configuration.md)
