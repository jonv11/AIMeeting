# Orchestrator Response Format

This document defines the required JSON schema for orchestrator decisions. Orchestrators must return **valid JSON only** with no additional text.

## JSON Schema

```json
{
  "decision": "continue" | "change_phase" | "end_meeting",
  "next_agent_id": "agent-id" (required if decision=continue),
  "new_phase": "PhaseName" (required if decision=change_phase),
  "end_reason": "reason" (required if decision=end_meeting),
  "rationale": "detailed reasoning" (always required)
}
```

## Decision Types

| Decision | Required Fields | Description |
|----------|----------------|-------------|
| `continue` | `next_agent_id`, `rationale` | Continue meeting with specified agent |
| `change_phase` | `new_phase`, `rationale` | Transition to a new meeting phase |
| `end_meeting` | `end_reason`, `rationale` | End meeting gracefully |

## Valid Phases

The `new_phase` field must be one of the following values:

- `Initialization`
- `ProblemClarification`
- `OptionGeneration`
- `Evaluation`
- `Decision`
- `ExecutionPlanning`
- `Conclusion`

## Examples

### Continue Decision

```json
{
  "decision": "continue",
  "next_agent_id": "engineer-001",
  "rationale": "Engineer has expertise on schema design"
}
```

### Change Phase Decision

```json
{
  "decision": "change_phase",
  "new_phase": "Evaluation",
  "rationale": "Sufficient options gathered; begin trade-off analysis"
}
```

### End Meeting Decision

```json
{
  "decision": "end_meeting",
  "end_reason": "Consensus reached on option B",
  "rationale": "Decision made with action items defined"
}
```

## Validation Rules

All orchestrator responses are validated by `OrchestratorResponseValidator`:

- `decision` must be one of `continue`, `change_phase`, or `end_meeting`
- `rationale` is required for all decisions
- `next_agent_id` is required for `continue` and must exist in `MeetingContext.Agents`
- `new_phase` is required for `change_phase` and must match `MeetingPhase`
- `end_reason` is required for `end_meeting`

Invalid responses are rejected and should fall back to stub decision logic.
