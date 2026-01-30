# PLAN

This plan describes incremental steps to build the AIMeeting project. It will be updated as implementation progresses and as answers to `DEV_QUESTIONS.md` are provided.

Principles
- Keep commits small and focused
- Validate builds after each change
- Prefer minimal viable implementations for features, then iterate
- Stop and ask stakeholders when architecture or requirements are unclear

Milestones (high level)
1. Project scaffolding and development setup (CI, coding conventions)
2. Agent configuration parser and validator (CLI `validate-config`)
3. CLI skeleton and basic commands (`validate-config`, `start-meeting`)
4. Meeting runtime: simple turn-based orchestrator with in-memory agents
5. Integrate minimum LLM provider (pluggable client interface)
6. Persistence: optional transcript storage (file/SQLite)
7. Tests and examples
8. Packaging and release (CLI executable, Dockerfile)

Initial iteration (for next commits)
A. Commit: Add `PLAN.md` and `DEV_QUESTIONS.md` (done)
B. Commit: Implement CLI skeleton and improve `Program.cs` to use `System.CommandLine` and basic `validate-config` command that reads an agent file and runs a basic validation (syntax checks).
C. Commit: Implement agent config parser and validator logic.
D. Commit: Add tests for parser/validator.

Dependencies and notes
- Use .NET 8
- Prefer `System.CommandLine` for CLI parsing
- Keep external dependencies minimal initially

Next actions
- Wait for answers to `DEV_QUESTIONS.md` or assume defaults for MVP:
  - Agent configs in `config/agents/*.txt`
  - CLI only for MVP
  - LLM provider pluggable, default to a stub implementation for tests


