# Changelog

All notable changes to this project will be documented in this file.

## Changelog Guidelines

- **Single source of truth**: Use this file for release notes. Do not add separate `RELEASE_NOTES_*` files.
- **Format**: Keep entries under the release header using these headings: Added, Changed, Fixed, Deprecated, Removed, Security.
- **Dates**: Use ISO format $YYYY-MM-DD$.
- **Scope**: Include user-visible changes, dependency/CI changes, and documentation updates that affect usage.
- **Immutability**: Do not edit prior release entries except to correct factual errors.

## [v0.1.2] - Unreleased

### Added
- Orchestrator abstraction (`IOrchestratorDecisionMaker`) and orchestrator-driven turn manager.
- Orchestrator response model, validation, and event types.
- AI orchestrator stub implementation for testing and CI.
- Orchestrator configuration template and response format reference.
- **Phase 4 - Copilot Integration:**
  - Orchestrator prompt building with meeting context, available agents, message history, and decision guidance
  - JSON decision parsing with markdown code block extraction
  - Retry logic with exponential backoff (3 attempts, 500ms → 1000ms → 2000ms delays)
  - Error handling with automatic fallback to stub mode
  - 16 new unit tests for prompt building and decision parsing
  - Support for all decision types: continue meeting, change phase, end meeting

### Changed
- Meeting orchestration now supports optional orchestrator-driven turn-taking.
- Agent factory detects orchestrator configurations and excludes them from standard agent creation.
- Documentation updated with orchestrator guide, retry logic details, and integration status.
- Orchestrator guide updated with Phase 4 features and troubleshooting guidance.

### Implementation Status
- ✅ Phase 1: Orchestrator foundation (interfaces, models, events)
- ✅ Phase 2: Stub implementation for testing
- ✅ Phase 3: Turn manager integration
- ✅ Phase 4: Prompt building and decision parsing
- ⏳ Phase 5: Real Copilot CLI testing (pending)
- ⏳ Phase 6: Prompt tuning based on meeting outcomes
- ⏳ Phase 7: State tracking (hypotheses, decisions, open questions)

## [v0.1.1] - 2026-02-01

### Changed
- Switched Copilot client implementation to the GitHub Copilot SDK for .NET.
- Updated CI to run Core and Integration tests with coverage thresholds, and Copilot tests separately without coverage collection.
- Updated documentation to reflect v0.1.1 status and SDK-based Copilot dependency.

## [v0.1.0] - 2026-01-31

### Added
- Initial CLI-based multi-agent meeting orchestration system.
- Agent configuration parser/validator and CLI validation command.
- Meeting orchestration with FIFO turn-taking, hard limits, and artifacts.
- GitHub Copilot integration with stub mode for testing.
- Structured logging and error handling.
