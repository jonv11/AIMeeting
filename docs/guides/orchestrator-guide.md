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

## Troubleshooting

**Orchestrator does not respond**
- The turn manager will timeout after 30 seconds
- The system falls back to FIFO for that turn

**Invalid JSON responses**
- Responses are validated by `OrchestratorResponseValidator`
- Invalid responses should be retried or replaced by stub decision logic

**Meeting ends immediately**
- Ensure non-orchestrator agents are provided
- Orchestrator should not select itself as next agent

## Related Documentation

- [Orchestrator response format](../reference/orchestrator-response-format.md)
- [CLI guide](cli.md)
- [Agent configuration reference](../reference/agent-configuration.md)
