# DEV QUESTIONS

This file lists questions needed to proceed with implementation and to create a practical, testable plan. Each question includes a suggested **Responsible Role** (the person/team best positioned to answer).

---

## Project purpose & scope

**Q1. Primary goal and vision**
- What is the primary goal of the AIMeeting project? (MVP / long-term vision)
- Who are the primary users and stakeholders?
- What success criteria or acceptance tests define a working MVP?

**Responsible Role**: Product Manager / Project Owner

**Q2. Scope and acceptance**
- What success criteria or acceptance tests define a working MVP?
- Any critical "out of scope" features that should be explicitly excluded from v0.1?

**Responsible Role**: Product Manager / QA Lead

---

## Features & Priorities

**Q3. MVP feature set**
- What are the must-have features for the first release (MVP)?
- Which features are nice-to-have and can be deferred?
- Are there prioritized milestones or deadlines?

**Responsible Role**: Product Manager / Engineering Lead

**Q4. Scope creep guards**
- How do we validate that we're not adding features beyond the MVP scope?
- Any features in the documentation (EXAMPLES.md, EXTENDING.md) that are aspirational vs. MVP?

**Responsible Role**: Project Manager / Technical Lead

---

## Agent behavior & configuration

**Q5. Agent configuration file format**
- Confirm agent configuration format: Should `config/agents/*.txt` be the canonical source?
- Are there parser examples or constraints in the existing codebase?
- Maximum file size, character encoding, or syntax rules?

**Responsible Role**: Technical Lead / Architecture Owner

**Q6. Agent validation behavior**
- How should agents be validated via CLI `validate-config` command?
- Expected error messages and validation depth (syntax only vs. semantic)?
- Should configuration parsing be strict or permissive?

**Responsible Role**: Technical Lead / QA Lead

**Q7. Agent runtime behavior**
- Expected runtime behavior for agents (turn-taking, message limits, concurrency)?
- Should agents be initialized once per meeting or per-turn?
- What happens if an agent fails to respond (retry, skip, end meeting)?

**Responsible Role**: Architecture Owner / Senior Developer

---

## CLI and API

**Q8. CLI commands for MVP**
- Which CLI commands must be supported for MVP? (docs show `validate-config`, `start-meeting` — any others?)
- Should there be a `list-agents` or `help` command?
- Command syntax and argument validation rules?

**Responsible Role**: Technical Lead / UX Designer (for CLI)

**Q9. HTTP API scope**
- Is there a public HTTP API planned for MVP? If so, what endpoints and auth are required?
- Or is CLI-only sufficient for v0.1?
- If API, should it be REST, gRPC, or something else?

**Responsible Role**: API Architect / Product Manager

---

## Integrations & LLM provider

**Q10. LLM provider selection**
- Which LLM provider(s) will be supported (OpenAI, Azure OpenAI, local models)?
- Are API keys/secrets provided by the environment or user config?
- Should GitHub Copilot CLI be optional or mandatory for MVP?

**Responsible Role**: Technical Lead / Infrastructure Owner

**Q11. SDK constraints and dependencies**
- Any existing SDKs or client libraries we must use or avoid?
- GitHub Copilot CLI version requirements or constraints?
- Fallback or stub implementations for testing without Copilot?

**Responsible Role**: Senior Developer / DevOps

---

## Data storage & persistence

**Q12. Persistence scope**
- What data must be persisted (meeting transcripts, agent configs, logs, user data)?
- For MVP, is filesystem storage sufficient or should we design for DB?
- Retention and export requirements?

**Responsible Role**: Technical Lead / Data Owner

**Q13. Storage backend**
- Preferred storage backend(s) for MVP (filesystem, SQLite, cloud DB)?
- Directory structure for meeting artifacts (suggested in README)?
- Should we implement versioning or just overwrite?

**Responsible Role**: Infrastructure Owner / Backend Lead

---

## Security & Compliance

**Q14. Compliance requirements**
- Any compliance requirements (HIPAA, GDPR)?
- Are there sensitive data classification or handling rules?

**Responsible Role**: Security / Compliance Officer

**Q15. Authentication and authorization**
- Authentication and authorization expectations for CLI/API and any UI?
- Should Copilot CLI auth be the only auth method for MVP?

**Responsible Role**: Security Lead / Infrastructure Owner

**Q16. Secrets management**
- Secrets management approach for API keys and credentials?
- Environment variables, config files, or vault integration?
- How should users provide Copilot credentials?

**Responsible Role**: Security Lead / DevOps

---

## Testing & Quality

**Q17. Test strategy**
- Required test coverage or testing strategy (unit, integration, E2E)?
- Preferred test frameworks (xUnit, NUnit)?
- Any existing CI/CD pipeline or preferred providers?

**Responsible Role**: QA Lead / Technical Lead

**Q18. Testability requirements**
- How should we test without live Copilot API (mocks, stubs)?
- Should the system support offline testing?
- Any performance or benchmark tests required?

**Responsible Role**: QA Lead / Senior Developer

---

## Observability & Operations

**Q19. Logging requirements**
- Expected logging and metrics (structured logs, traces)?
- Minimum log level and output destinations (console, file, cloud)?
- Logging libraries (Serilog, NLog, ILogger)?

**Responsible Role**: Infrastructure Owner / SRE

**Q20. Monitoring and alerting**
- Monitoring / alerting requirements for production?
- Metrics to track (meeting duration, agent response time, errors)?
- Health checks or readiness probes needed?

**Responsible Role**: SRE / Infrastructure Owner

---

## Performance & Scale

**Q21. Concurrency and scale expectations**
- Expected concurrency and scale for MVP (single-user desktop, multi-tenant server)?
- Latency and throughput SLAs if any?
- Maximum concurrent meetings?

**Responsible Role**: Technical Lead / Product Manager

**Q22. Performance constraints**
- Maximum acceptable meeting duration?
- Maximum agents per meeting?
- Timeout values for agent responses and file operations?

**Responsible Role**: Technical Lead / Senior Developer

---

## Developer Experience

**Q23. Development environment**
- Supported development environments and OSes (Windows, Linux, macOS)?
- Project format is .NET 8 — any specific SDK or tool versions required?
- Recommended IDEs or editors?

**Responsible Role**: Technical Lead / DevOps

**Q24. Code conventions and contribution**
- Are there contribution guidelines or code style rules beyond existing `.editorconfig`?
- Commit message format and PR review process?
- Breaking change policy?

**Responsible Role**: Technical Lead / Architecture Reviewer

---

## Documentation & Examples

**Q25. Documentation scope**
- Which user scenarios should be covered by examples (single agent test, multi-agent meeting, export transcript)?
- Any real-world sample agent configurations to include?
- Should we include troubleshooting or FAQ sections?

**Responsible Role**: Technical Writer / Product Manager

**Q26. README and onboarding**
- Should the README guide new developers or just end-users?
- Any prerequisites or setup steps beyond what's in README.md?

**Responsible Role**: Technical Writer / DevOps

---

## Licensing & Legal

**Q27. Licensing and dependencies**
- Any IP or license constraints for bundled dependencies or models?
- Copilot CLI licensing terms that affect our distribution?
- License for meeting artifacts and outputs?

**Responsible Role**: Legal / Compliance Officer

---

## Deployment & Release

**Q28. Deployment model**
- Target deployment environment(s) for v1 (local CLI, Docker, cloud)?
- Distribution method (NuGet package, GitHub releases, executable)?
- Should we provide pre-built binaries?

**Responsible Role**: DevOps / Release Manager

**Q29. Release strategy**
- Release cadence and tagging strategy?
- Semantic versioning (MAJOR.MINOR.PATCH)?
- Deprecation and breaking change policy?

**Responsible Role**: Release Manager / Technical Lead

---

## Access & Credentials

**Q30. Integration test resources**
- Will you provide any API keys, test accounts, or cloud resources needed for integration tests?
- How do we mock or stub Copilot CLI for CI/CD?
- Are there test GitHub Copilot subscriptions available?

**Responsible Role**: DevOps / Infrastructure Owner

---

## Architecture & Design Decisions (Inferred from Docs)

**Q31. Confirmation of assumptions from documentation**
- The docs suggest using GitHub Copilot CLI (not SDK). Should we confirm this assumption is still valid?
- Event-driven architecture with in-memory `IEventBus` for MVP—is this confirmed?
- FIFO turn-taking strategy—is this the only required implementation for MVP?

**Responsible Role**: Technical Lead / Architecture Owner

**Q32. Configuration file parsing library**
- Should we use a parsing library (e.g., ini-style) or build a simple custom parser?
- The `AGENT_CONFIGURATION_GUIDE.md` shows a text-based format—do we have a reference parser?

**Responsible Role**: Senior Developer / Technical Lead

---

## Summary

**Next Steps**:
1. Stakeholders review and assign owners to each role.
2. Provide answers with supporting context (docs, links, examples).
3. Once answered, I will update `PLAN.md` with concrete implementation steps.
4. We will proceed with small, reviewable commits aligned to the MVP scope.

---

**Version**: 1.0  
**Last Updated**: January 30, 2026
