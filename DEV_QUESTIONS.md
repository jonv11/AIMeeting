# DEV QUESTIONS

This file lists questions needed to proceed with implementation and to create a practical, testable plan. Please answer or provide references for each item.

## Project purpose & scope
- What is the primary goal of the AIMeeting project? (MVP / long-term vision)
- Who are the primary users and stakeholders?
- What success criteria or acceptance tests define a working MVP?

## Features & Priorities
- What are the must-have features for the first release (MVP)?
- Which features are nice-to-have and can be deferred?
- Are there prioritized milestones or deadlines?

## Agent behavior & configuration
- Confirm agent configuration format: Should `config/agents/*.txt` be the canonical source? Any parser examples or constraints?
- How should agents be validated? (CLI `validate-config` exists in docs—expected behavior?)
- Expected runtime behavior for agents (turn-taking, message limits, concurrency)?

## CLI and API
- Which CLI commands must be supported for MVP? (docs show `validate-config`, `start-meeting` — any others?)
- Is there a public HTTP API planned? If so, what endpoints and auth are required?

## Integrations & LLM provider
- Which LLM provider(s) will be supported (OpenAI, Azure OpenAI, local models)? Are API keys/secrets provided by the environment?
- Any existing SDKs or client libraries we must use or avoid?

## Data storage & persistence
- What data must be persisted (meeting transcripts, agent configs, logs, user data)?
- Preferred storage backend(s) for MVP (filesystem, SQLite, cloud DB)?
- Retention and export requirements?

## Security & Compliance
- Any compliance requirements (HIPAA, GDPR)?
- Authentication and authorization expectations for CLI/API and any UI?
- Secrets management approach for API keys and credentials?

## Testing & Quality
- Required test coverage or testing strategy (unit, integration, E2E)?
- Preferred test frameworks (xUnit, NUnit)?
- Any existing CI/CD pipeline or preferred providers?

## Observability & Operations
- Expected logging and metrics (structured logs, traces)?
- Monitoring / alerting requirements for production?

## Performance & Scale
- Expected concurrency and scale for MVP (single-user desktop, multi-tenant server)?
- Latency and throughput SLAs if any?

## Developer Experience
- Supported development environments and OSes (Windows, Linux, macOS)?
- Project format is .NET 8 — any specific SDK or tool versions required?
- Are there contribution guidelines or code style rules beyond existing `.editorconfig`?

## Documentation & Examples
- Which user scenarios should be covered by examples (single agent test, multi-agent meeting, export transcript)?
- Any real-world sample agent configurations to include?

## Licensing & Legal
- Any IP or license constraints for bundled dependencies or models?

## Deployment & Release
- Target deployment environment(s) for v1 (local CLI, Docker, cloud)?
- Release cadence and tagging strategy?

## Access & Credentials
- Will you provide any API keys, test accounts, or cloud resources needed for integration tests?

---

If these questions are answered, I will produce a `PLAN.md` with a step-by-step implementation plan, break the work into small commits, and begin implementing the first incremental tasks.
