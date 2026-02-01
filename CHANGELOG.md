# Changelog

All notable changes to this project will be documented in this file.

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
