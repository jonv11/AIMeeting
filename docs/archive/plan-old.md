# PLAN

**Project**: AIMeeting Multi-Agent Meeting System  
**Target**: .NET 8 CLI application  
**MVP Version**: v0.1.0  
**Status**: All Critical Inputs Complete — Ready to Start v0.1

---

## MVP v0.1 Scope (Confirmed by Product Strategist)

**Goal**: Deliver a functional CLI-based multi-agent meeting system demonstrating structured AI collaboration through configurable roles.

**Success Gate**: Pass all 8 acceptance tests (AT-001 through AT-008)

### MUST-HAVE Features (Non-Negotiable)

1. **Agent Configuration System**
   - Parse text-based config files (ROLE, DESCRIPTION, PERSONA, INSTRUCTIONS)
   - Validate required fields
   - CLI command: `validate-config <path>`

2. **Meeting Orchestration**
   - Start meeting with topic + list of agent configs
   - FIFO turn-taking (simplest implementation)
   - Enforce hard limits: max duration, max messages
   - State transitions: NotStarted → InProgress → Completed

3. **Copilot Integration**
   - Wrapper around GitHub Copilot CLI
   - Generate agent responses based on role + context
   - Handle basic error cases (timeout, API error)

4. **Meeting Room (File System)**
   - Create isolated directory per meeting
   - Write transcript.md with timestamped messages
   - Basic file locking to prevent corruption

5. **CLI Interface**
   - `start-meeting --topic "..." --agents "..." --max-duration N --max-messages M`
   - `validate-config <config-file>`
   - Real-time console output showing turn progress

6. **Basic Logging**
   - Console output for key events (agent turn start/end)
   - Error logging to file for debugging

### Explicitly OUT of v0.1

✗ HTTP API / REST endpoints  
✗ RAG integration  
✗ Dynamic turn-taking  
✗ Multi-provider support  
✗ Web UI / Dashboard  
✗ Meeting templates  
✗ Agent memory across meetings  
✗ Action item extraction  
✗ Real-time collaboration  
✗ Authentication system  
✗ Cloud deployment

---

## Implementation Principles

- Keep commits small and focused (100-300 lines max)
- Validate builds after each change
- Test-driven or test-after (add tests for each story)
- Stop and ask stakeholders when architecture or requirements are unclear
- Weekly scope review: Flag any work not on MUST-HAVE list

---

## Milestones (6-8 Week Timeline)

### M1: Foundation (Week 1-2) — ✅ Ready

**Dependencies**: All critical inputs complete; no blockers

**Stories**:
1. **Project Scaffolding**
   - Set up solution structure (CLI, Core, Copilot, Infrastructure, Tests)
   - Add package references (System.CommandLine, Serilog, xUnit)
   - Configure .editorconfig and coding standards
   - Set up CI/CD placeholder

2. **Agent Configuration Parser**
   - Implement parser for `.txt` config format
   - Handle ROLE, DESCRIPTION, PERSONA, INSTRUCTIONS, RESPONSE_STYLE, etc.
   - Support comments and whitespace
   - Unit tests for parser

3. **Agent Configuration Validator**
   - Validate required fields (ROLE, DESCRIPTION, INSTRUCTIONS)
   - Check field constraints (MAX_MESSAGE_LENGTH numeric, etc.)
   - Generate clear error messages
   - Unit tests for validator

4. **CLI `validate-config` Command**
   - Implement System.CommandLine command
   - Accept file path argument
   - Output validation results (pass/fail with details)
   - Integration test

**Acceptance**: AT-001 passes

---

### M2: Orchestration (Week 3-4) — ⏳ Pending M1

**Stories**:
5. **Agent Model & Factory**
   - Define `IAgent` interface
   - Implement `AgentBase` class
   - Agent factory creates agents from config
   - Agent lifecycle management (initialization, cleanup)
   - Unit tests

6. **Event System**
   - Define core event types (MeetingStarted, TurnStarted, TurnCompleted, MeetingEnded)
   - Implement in-memory `IEventBus`
   - Event publishing and subscription
   - Unit tests

7. **Meeting Orchestrator — Core Logic**
   - State machine (NotStarted → Initializing → InProgress → EndingGracefully → Completed)
   - FIFO turn queue management
   - Turn coordination (request turn, wait for completion)
   - Unit tests (without Copilot integration)

8. **Meeting Orchestrator — Limit Enforcement**
   - Time limit enforcement (max-duration)
   - Message limit enforcement (max-messages)
   - Graceful shutdown on limit reached
   - Unit tests

**Acceptance**: AT-003, AT-004, AT-005 pass (with stub agents)

---

### M3: Integration (Week 5-6) — ⏳ Pending M2

**Dependencies**: None (infrastructure inputs complete)

**Stories**:
9. **Copilot Client Wrapper**
   - Implement `ICopilotClient` interface
   - Process wrapper for `gh copilot` CLI
   - Request/response handling
   - Timeout and error handling
   - Unit tests with mocks

10. **Agent-Copilot Integration**
    - Connect agent to Copilot client
    - Build prompts from agent config + meeting context
    - Parse and validate responses
    - Integration tests with stub Copilot

11. **Meeting Room — File System**
    - Create isolated meeting directory (timestamp-based)
    - Write transcript.md in real-time
    - File locking mechanism (timeout-based)
    - Unit tests for file operations

12. **Meeting Room — Transcript Generation**
    - Format agent messages with timestamps
    - Write incrementally (not buffered)
    - Handle file write errors
    - Integration tests

**Acceptance**: AT-006, AT-007 pass

---

### M4: CLI & UX (Week 7) — ⏳ Pending M3

**Stories**:
13. **CLI `start-meeting` Command**
    - Implement System.CommandLine command
    - Parse arguments (--topic, --agents, --max-duration, --max-messages)
    - Validate arguments
    - Integration test

14. **CLI Progress Display**
    - Real-time console output for turn progress
    - Show agent name, turn number, status
    - Display time remaining / messages remaining
    - Handle Ctrl+C gracefully

15. **Error Handling & User Messages**
    - Custom exception hierarchy
    - User-friendly error messages (not stack traces)
    - Log detailed errors to file
    - Integration tests for error scenarios

**Acceptance**: AT-002 passes

---

### M5: Testing (Week 8) — ⏳ Pending M4

**Dependencies**: QA strategy complete; no blockers

**Stories**:
16. **Unit Test Coverage**
    - Config parser tests
    - Validator tests
    - Agent factory tests
    - Orchestrator tests
    - Event bus tests
    - Target: ≥80% coverage

17. **Integration Test Suite**
    - End-to-end meeting with stub agents
    - File operations (transcript generation)
    - Limit enforcement
    - Error scenarios

18. **Test Fixtures & Samples**
    - Sample agent configs (PM, developer, security expert, moderator)
    - Mock Copilot responses
    - Test meeting scenarios

**Acceptance**: AT-008 passes (multi-platform build)

---

### M6: MVP Release (Week 9) — ⏳ Pending M5

**Stories**:
19. **Documentation Updates**
    - Update README.md with version badges
    - Add Troubleshooting section
    - Update EXAMPLES.md with 3 scenarios
    - Code comments and XML docs

20. **Sample Agent Configurations**
    - Create 4 sample configs (PM, developer, security, moderator)
    - Add to `config/agents/` directory
    - Document in AGENT_CONFIGURATION_GUIDE.md

21. **Release Preparation**
    - Version tagging (v0.1.0)
    - Release notes
    - Build verification on Windows, Linux, macOS
    - Package CLI executable

22. **MVP Validation**
    - Run all 8 acceptance tests
    - End-to-end smoke test
    - Performance check (meeting startup <3s)
    - Documentation review

**Acceptance**: All 8 acceptance tests pass; MVP ready for release

---

## Current Status

| Milestone | Status | Blocker |
|-----------|--------|---------|
| M1: Foundation | ⏳ Not Started | None |
| M2: Orchestration | ⏳ Not Started | M1 completion |
| M3: Integration | ⏳ Not Started | M2 completion |
| M4: CLI & UX | ⏳ Not Started | M3 completion |
| M5: Testing | ⏳ Not Started | M4 completion |
| M6: MVP Release | ⏳ Not Started | M5 completion |

---

## Dependencies & Technology Stack

**Confirmed**:
- .NET 8.0 SDK
- System.CommandLine (CLI framework)
- xUnit + Moq (testing)
- GitHub Copilot CLI (only LLM provider for MVP)
- Serilog (logging)
- Filesystem storage (meeting artifacts)
- Config files: UTF-8, max 64 KB

**Storage**: Filesystem only (no database for MVP)

---

## Next Actions

### Start M1 Implementation (Now)
1. Create config parser + validator
2. Implement `validate-config` command
3. Add unit tests for parser/validator

### Follow-Up (Non-Blocking)
1. Technical Writer answers Q26 (README onboarding focus)
2. Legal/Compliance answers Q27 (licensing)
3. Infrastructure Owner answers Q20 (monitoring/alerting)

---

**Version**: 2.2  
**Last Updated**: January 30, 2026 (Updated with infrastructure/security/senior dev decisions)  
**Next Update**: After M1 completion


