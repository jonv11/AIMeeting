# AIMeeting v0.1.1 Release Notes

**Release Date:** February 1, 2026

## Overview

AIMeeting v0.1.1 is a patch release focused on stabilizing GitHub Copilot integration and CI reliability.

## Changes

### GitHub Copilot Integration
- Switched Copilot client implementation to the GitHub Copilot SDK for .NET
- Simplified connection lifecycle with SDK-managed sessions
- Updated error messaging to reflect SDK/CLI dependency

### Testing & CI
- Updated Copilot client tests for SDK-based implementation
- CI now runs Core and Integration tests with coverage thresholds
- Copilot tests run separately without coverage collection due to external CLI dependency

## System Requirements

- .NET 8.0 SDK or later
- Windows, Linux, or macOS
- GitHub Copilot CLI installed and authenticated (required for non-stub mode)

## Documentation

- [README.md](README.md) - Project overview and quick start
- [API.md](docs/reference/api.md) - API reference

---

For issues or questions, visit: https://github.com/jonv11/AIMeeting/issues
