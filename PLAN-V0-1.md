# Plan for v0.1 (based on repository evidence)

This plan consolidates v0.1 MVP requirements and implementation work items based strictly on repository documentation and current project state.

## Evidence Sources

- `PLAN.md`
- `ROADMAP.md`
- `README.md`
- `ARCHITECTURE.md`
- `API.md`
- `AGENT_CONFIGURATION_GUIDE.md`
- `EXAMPLES.md`
- `STATUS.md`
- `READINESS.md`
- `SUMMARY.md`
- `DEV_QUESTIONS_ANSWERS.md`
- `QA_COMPLETION_STATUS.md`
- `QA_LEAD_COMPLETION_REPORT.md`
- `config/agents/README.md`
- `Program.cs`

## Feature / Story / Ticket / Subtask Plan

- [ ] `F01` Solution foundation & project scaffolding
  - [ ] `S01.01` As a developer, I want a consistent solution structure and dependencies, so that v0.1 features can be implemented and built.
    - [ ] `T01.01.01` Create solution and project structure aligned to documented layout (`src/`, `tests/`)  
      **Priority:** P0 | **Effort:** M (multiple projects + references) | **Deps:** none | **Outcome:** Solution contains CLI, Core, Copilot, Infrastructure, and test projects.
      - Description: Create projects and references as described in `README.md` and `PLAN.md`.
      - Acceptance Criteria:
        - [ ] `AIMeeting.sln` includes `AIMeeting.CLI`, `AIMeeting.Core`, `AIMeeting.Copilot`, `AIMeeting.Infrastructure`, and test projects under `tests/`.
        - [ ] Projects build with `dotnet build`.
    - [ ] `T01.01.02` Add required package references for v0.1 tooling  
      **Priority:** P0 | **Effort:** S (package additions) | **Deps:** T01.01.01 | **Outcome:** CLI, logging, and testing packages are referenced.
      - Description: Add `System.CommandLine`, `Serilog`, test packages (`xUnit`, `Moq`, optional `coverlet`), and any documented CLI UI package (e.g., `Spectre.Console`).
      - Acceptance Criteria:
        - [ ] Package references align with `PLAN.md`, `README.md`, and `ARCHITECTURE.md`.
        - [ ] `dotnet restore` succeeds.
    - [ ] `T01.01.03` Ensure coding standards configuration is present and applied  
      **Priority:** P1 | **Effort:** S (config update) | **Deps:** T01.01.01 | **Outcome:** `.editorconfig` applies coding conventions.
      - Description: Confirm `.editorconfig` exists and aligns with repo guidance.
      - Acceptance Criteria:
        - [ ] `.editorconfig` present at repo root and used by projects.

- [ ] `F02` Agent configuration system (parser + validator)
  - [ ] `S02.01` As a user, I want to define agents in text files, so that roles can be configured without code changes.
    - [ ] `T02.01.01` Implement agent configuration parser  
      **Priority:** P0 | **Effort:** M (format rules + edge cases) | **Deps:** T01.01.02 | **Outcome:** Config files parse into `AgentConfiguration` reliably.
      - Description: Implement parsing rules from `AGENT_CONFIGURATION_GUIDE.md` (UTF-8, max 64 KB, headers, sections, comments/whitespace, line normalization).
      - Acceptance Criteria:
        - [ ] Required/optional sections parse correctly.
        - [ ] Unknown headers are captured for warnings (not fatal).
        - [ ] Size/encoding constraints enforced.
    - [ ] `T02.01.02` Implement agent configuration validation  
      **Priority:** P0 | **Effort:** M (rules + error reporting) | **Deps:** T02.01.01 | **Outcome:** Validation enforces required fields with clear errors.
      - Description: Validate required fields (`ROLE`, `DESCRIPTION`, `INSTRUCTIONS`) and optional field constraints (e.g., `MAX_MESSAGE_LENGTH` numeric) per `AGENT_CONFIGURATION_GUIDE.md` and QA guidance.
      - Acceptance Criteria:
        - [ ] Missing required fields fail validation with line-aware errors.
        - [ ] Unknown fields emit warnings but do not fail validation.
        - [ ] Exit code behavior aligns with `AGENT_CONFIGURATION_GUIDE.md`.

- [ ] `F03` CLI commands & UX
  - [ ] `S03.01` As a CLI user, I want to validate configurations, so that I can fix issues before running meetings.
    - [ ] `T03.01.01` Implement `validate-config` command  
      **Priority:** P0 | **Effort:** S (single command) | **Deps:** T02.01.02 | **Outcome:** CLI validates a config file with clear output and exit codes.
      - Description: Add `validate-config <path>` command via `System.CommandLine`.
      - Acceptance Criteria:
        - [ ] Exit code `0` for valid configs, `1` for invalid.
        - [ ] Output includes actionable error messages (line numbers when available).
  - [ ] `S03.02` As a CLI user, I want to start and monitor a meeting, so that I can observe progress in real time.
    - [ ] `T03.02.01` Implement `start-meeting` command argument parsing and validation  
      **Priority:** P0 | **Effort:** M (args + config binding) | **Deps:** T01.01.02, T04.02.03, T06.01.02 | **Outcome:** CLI starts a meeting with topic, agents, and limits.
      - Description: Implement CLI parsing for `--topic`, `--agents`, `--max-duration`, `--max-messages`.
      - Acceptance Criteria:
        - [ ] Invalid arguments yield user-friendly errors.
        - [ ] Meeting starts with valid inputs and outputs progress.
    - [ ] `T03.02.02` Implement real-time progress display  
      **Priority:** P1 | **Effort:** M (event-driven UI) | **Deps:** T04.02.01, T04.02.03 | **Outcome:** Console shows turn progress (agent, turn number, status).
      - Description: Use events to render turn progress as per `PLAN.md` and `ARCHITECTURE.md`.
      - Acceptance Criteria:
        - [ ] Turn start/end updates are shown.
        - [ ] Time/messages remaining are displayed when limits are configured.
    - [ ] `T03.02.03` Handle Ctrl+C graceful shutdown  
      **Priority:** P1 | **Effort:** S (cancellation flow) | **Deps:** T04.02.03 | **Outcome:** CLI cancels meeting cleanly and preserves artifacts.
      - Description: Use cancellation tokens and `Console.CancelKeyPress` as shown in `API.md`.
      - Acceptance Criteria:
        - [ ] Meeting ends gracefully with cancellation reason.
        - [ ] Artifacts remain available after cancellation.

- [ ] `F04` Core agent model, events, and orchestration
  - [ ] `S04.01` As the system, I want a consistent agent model, so that meeting orchestration can manage agents uniformly.
    - [ ] `T04.01.01` Implement core data models  
      **Priority:** P0 | **Effort:** M (multiple models) | **Deps:** T01.01.01 | **Outcome:** Models in `API.md` are available in `AIMeeting.Core`.
      - Description: Implement `MeetingConfiguration`, `MeetingLimits`, `MeetingContext`, `Message`, `MeetingResult`, `MeetingState`.
      - Acceptance Criteria:
        - [ ] Models align to `API.md` and `README.md` descriptions.
        - [ ] Serialization-ready where needed (`meeting.json`).
    - [ ] `T04.01.02` Implement `IAgent`, `AgentBase`, and standard agents  
      **Priority:** P0 | **Effort:** M (base class + implementations) | **Deps:** T04.01.01, T05.01.02, T05.01.03 | **Outcome:** Agents can initialize and respond through a consistent interface.
      - Description: Implement `IAgent` and `AgentBase` as specified in `API.md`/`ARCHITECTURE.md`, plus standard agent types (including moderator behavior).
      - Acceptance Criteria:
        - [ ] Agents implement `InitializeAsync`, `RespondAsync`, `ShouldParticipateAsync`.
        - [ ] Moderator capabilities align to `ARCHITECTURE.md`.
    - [ ] `T04.01.03` Implement agent factory and lifecycle management  
      **Priority:** P0 | **Effort:** M (config + DI wiring) | **Deps:** T02.01.02, T04.01.02 | **Outcome:** Agents are created from configs and registered for meetings.
      - Description: Create `IAgentFactory` to instantiate agents from parsed configs.
      - Acceptance Criteria:
        - [ ] Agents load from config paths provided in CLI.
        - [ ] Initialization and cleanup follow `ARCHITECTURE.md` lifecycle.
  - [ ] `S04.02` As the system, I want event-driven turn coordination, so that agents can take turns deterministically.
    - [ ] `T04.02.01` Implement in-memory event bus and event types  
      **Priority:** P0 | **Effort:** M (pub/sub + events) | **Deps:** T04.01.01 | **Outcome:** Events flow between orchestrator, agents, and UI.
      - Description: Implement `IEventBus` and core events defined in `API.md`/`ARCHITECTURE.md`.
      - Acceptance Criteria:
        - [ ] Subscribers receive `TurnStarted`, `TurnCompleted`, `MeetingStarted`, `MeetingEnded`, file events.
        - [ ] Thread-safe publish/subscribe behavior.
    - [ ] `T04.02.02` Implement FIFO turn manager  
      **Priority:** P0 | **Effort:** S (queue logic) | **Deps:** T04.02.01 | **Outcome:** Turn order is deterministic and cyclic.
      - Description: Implement `ITurnManager` FIFO behavior per `ARCHITECTURE.md`.
      - Acceptance Criteria:
        - [ ] Agents rotate in FIFO order.
        - [ ] No agent is skipped without explicit reason.
    - [ ] `T04.02.03` Implement meeting orchestrator + state machine  
      **Priority:** P0 | **Effort:** L (core workflow) | **Deps:** T04.01.03, T04.02.01, T04.02.02, T06.01.01 | **Outcome:** Meetings run end-to-end with proper state transitions.
      - Description: Implement lifecycle states from `ARCHITECTURE.md` and APIs from `API.md`.
      - Acceptance Criteria:
        - [ ] State transitions follow NotStarted → Initializing → InProgress → EndingGracefully → Completed/Failed.
        - [ ] Meeting context updates per turn.
  - [ ] `S04.03` As the system, I want hard limits enforced, so that meetings stop predictably.
    - [ ] `T04.03.01` Enforce max-duration and max-messages limits  
      **Priority:** P0 | **Effort:** M (time + count tracking) | **Deps:** T04.02.03 | **Outcome:** Meetings stop when limits are reached.
      - Description: Use `MeetingLimits` to enforce duration and message count.
      - Acceptance Criteria:
        - [ ] Meeting ends within ±5% of configured duration (AT-004).
        - [ ] Meeting ends at configured message count (AT-005).

- [ ] `F05` Copilot integration & prompt building
  - [ ] `S05.01` As the system, I want to generate agent responses via GitHub Copilot CLI, so that agents can contribute to the meeting.
    - [ ] `T05.01.01` Implement `ICopilotClient` wrapper for `gh copilot`  
      **Priority:** P0 | **Effort:** M (process wrapper + errors) | **Deps:** T01.01.02 | **Outcome:** Copilot CLI requests return responses or clear errors.
      - Description: Implement start/stop, request/response handling, timeout/error mapping per `PLAN.md` and `ARCHITECTURE.md`.
      - Acceptance Criteria:
        - [ ] Timeouts and CLI failures surface as `CopilotApiException`.
        - [ ] Client can be started/stopped cleanly.
    - [ ] `T05.01.02` Implement prompt builder  
      **Priority:** P0 | **Effort:** M (prompt formatting) | **Deps:** T02.01.01, T04.01.01 | **Outcome:** Prompts include topic, role, persona, instructions, and recent messages.
      - Description: Implement `IPromptBuilder` as outlined in `ARCHITECTURE.md`.
      - Acceptance Criteria:
        - [ ] Prompts include role/description, persona traits, instructions, and recent discussion.
        - [ ] Max message length is respected.
    - [ ] `T05.01.03` Integrate Copilot client into standard agents + stub mode  
      **Priority:** P0 | **Effort:** M (DI + environment switch) | **Deps:** T04.01.02, T05.01.01 | **Outcome:** Agents use Copilot in normal mode and stubs in tests.
      - Description: Implement agent response generation via Copilot and environment-based stub mode (`AIMEETING_AGENT_MODE=stub`).
      - Acceptance Criteria:
        - [ ] Stub mode enables deterministic responses in tests.
        - [ ] Copilot mode uses real CLI in runtime.

- [ ] `F06` Meeting room & artifacts (filesystem)
  - [ ] `S06.01` As a user, I want meeting artifacts written to disk, so that results are available after execution.
    - [ ] `T06.01.01` Implement meeting room directory creation and metadata file  
      **Priority:** P0 | **Effort:** M (path/slug handling) | **Deps:** T04.01.01 | **Outcome:** Per-meeting directory contains `meeting.json`.
      - Description: Create timestamp-based directory names with sanitized topic slug and write meeting metadata.
      - Acceptance Criteria:
        - [ ] Meeting directories are isolated under `meetings/`.
        - [ ] `meeting.json` reflects meeting configuration and identifiers.
    - [ ] `T06.01.02` Implement transcript generation with timestamps  
      **Priority:** P0 | **Effort:** M (streamed writes) | **Deps:** T04.02.01, T06.01.01 | **Outcome:** `transcript.md` records all messages in order.
      - Description: Append timestamped messages on each `TurnCompletedEvent`.
      - Acceptance Criteria:
        - [ ] Transcript includes all agent messages in order (AT-006).
        - [ ] Writes are incremental (not buffered).
    - [ ] `T06.01.03` Implement file locking with timeout  
      **Priority:** P0 | **Effort:** M (lock table + timeout) | **Deps:** T06.01.01 | **Outcome:** Concurrent writes are protected with lock timeouts.
      - Description: Implement `IFileLocker` behavior from `ARCHITECTURE.md`.
      - Acceptance Criteria:
        - [ ] Lock acquisition times out with clear error (AT-007).
        - [ ] Lock state is released on dispose.
    - [ ] `T06.01.04` Implement `errors.log` artifact writing  
      **Priority:** P1 | **Effort:** S (error stream) | **Deps:** T06.01.01, T07.01.02 | **Outcome:** Failures are captured in `errors.log`.
      - Description: Write error details to meeting-specific `errors.log` as per `README.md`.
      - Acceptance Criteria:
        - [ ] Errors are persisted even when the meeting fails.

- [ ] `F07` Logging & error handling
  - [ ] `S07.01` As a user, I want clear errors and logs, so that failures are actionable.
    - [ ] `T07.01.01` Implement exception hierarchy  
      **Priority:** P0 | **Effort:** M (base + derived) | **Deps:** T04.01.01 | **Outcome:** Errors are classified and contextual.
      - Description: Implement `AgentMeetingException` and derived exceptions in `ARCHITECTURE.md`/`API.md`.
      - Acceptance Criteria:
        - [ ] Exceptions include error codes/context where specified.
        - [ ] Error types map to configuration, agent, file lock, Copilot, and limit failures.
    - [ ] `T07.01.02` Configure Serilog logging (console + file)  
      **Priority:** P0 | **Effort:** S (logging configuration) | **Deps:** T01.01.02 | **Outcome:** Structured logs go to console and rolling files.
      - Description: Configure Serilog outputs per `ARCHITECTURE.md`/`README.md`.
      - Acceptance Criteria:
        - [ ] Console logs show key lifecycle events.
        - [ ] File logs are written to `logs/meeting-*.log`.
    - [ ] `T07.01.03` Implement user-friendly CLI error handling  
      **Priority:** P1 | **Effort:** M (error mapping) | **Deps:** T03.01.01, T03.02.01, T07.01.01 | **Outcome:** CLI errors are readable without stack traces.
      - Description: Map exceptions to clean messages per `API.md` error handling guidance.
      - Acceptance Criteria:
        - [ ] Known errors render actionable CLI messages.
        - [ ] Detailed errors still logged for diagnostics.

- [ ] `F08` Testing & quality gates
  - [ ] `S08.01` As a QA lead, I want comprehensive automated tests, so that acceptance tests can be passed reliably.
    - [ ] `T08.01.01` Unit tests for parser  
      **Priority:** P0 | **Effort:** M (edge cases) | **Deps:** T02.01.01 | **Outcome:** Parser correctness validated across formats.
      - Description: Implement tests covering valid/invalid input, size limits, and whitespace handling.
      - Acceptance Criteria:
        - [ ] Parser tests match rules in `AGENT_CONFIGURATION_GUIDE.md` and QA guidance.
    - [ ] `T08.01.02` Unit tests for validator  
      **Priority:** P0 | **Effort:** M (error cases) | **Deps:** T02.01.02 | **Outcome:** Validation rules are enforced with correct errors.
      - Description: Cover required fields, optional fields, and warning behavior.
      - Acceptance Criteria:
        - [ ] Tests verify exit code behavior and error messaging.
    - [ ] `T08.01.03` Unit tests for event bus and turn manager  
      **Priority:** P1 | **Effort:** S (core logic) | **Deps:** T04.02.01, T04.02.02 | **Outcome:** Deterministic event routing and FIFO order.
      - Description: Verify pub/sub behavior and FIFO sequencing.
      - Acceptance Criteria:
        - [ ] Event handlers receive events in expected order.
    - [ ] `T08.01.04` Unit tests for orchestrator limits  
      **Priority:** P0 | **Effort:** M (limit paths) | **Deps:** T04.03.01 | **Outcome:** Limit enforcement passes AT-004/AT-005.
      - Description: Validate meeting stops at time and message limits.
      - Acceptance Criteria:
        - [ ] Tests verify time and message limit behavior with stubs.
    - [ ] `T08.01.05` Integration tests for meeting workflows with stubs  
      **Priority:** P1 | **Effort:** M (multi-component) | **Deps:** T05.01.03, T06.01.02, T04.02.03 | **Outcome:** End-to-end meeting flow works without Copilot API.
      - Description: Use stub agents per `AIMEETING_AGENT_MODE=stub`.
      - Acceptance Criteria:
        - [ ] Meetings complete with deterministic transcripts.
    - [ ] `T08.01.06` CLI E2E tests for `validate-config` and `start-meeting`  
      **Priority:** P1 | **Effort:** M (process tests) | **Deps:** T03.01.01, T03.02.01 | **Outcome:** CLI acceptance tests cover AT-001/AT-002.
      - Description: Run CLI in test mode and compare outputs/golden files.
      - Acceptance Criteria:
        - [ ] AT-001 and AT-002 scenarios are covered.
    - [ ] `T08.01.07` Coverage targets and CI test execution  
      **Priority:** P0 | **Effort:** M (CI setup + coverage) | **Deps:** T01.01.01 | **Outcome:** Automated tests run on all platforms with coverage reporting.
      - Description: Configure GitHub Actions to run tests on Windows/Linux/macOS with ≥80% coverage targets per QA guidance.
      - Acceptance Criteria:
        - [ ] CI runs on three platforms and reports coverage.
        - [ ] AT-008 is satisfied.

- [ ] `F09` Documentation, samples, and release preparation
  - [ ] `S09.01` As a user, I want clear documentation and samples, so that I can run v0.1 successfully.
    - [ ] `T09.01.01` Update README with v0.1 instructions and troubleshooting  
      **Priority:** P1 | **Effort:** S (doc updates) | **Deps:** T03.02.01, T06.01.02 | **Outcome:** README matches v0.1 behavior and outputs.
      - Description: Ensure quick start, CLI usage, and troubleshooting align to implementation.
      - Acceptance Criteria:
        - [ ] README examples match CLI argument requirements and outputs.
    - [ ] `T09.01.02` Update `EXAMPLES.md` and `AGENT_CONFIGURATION_GUIDE.md`  
      **Priority:** P1 | **Effort:** S (doc alignment) | **Deps:** T03.02.01, T02.01.02 | **Outcome:** Documentation matches v0.1 CLI and config validation behavior.
      - Description: Keep examples and configuration guidance synchronized with implemented rules.
      - Acceptance Criteria:
        - [ ] Example commands run with v0.1 CLI syntax.
        - [ ] Validation rules match actual validator behavior.
    - [ ] `T09.01.03` Ensure sample agent configs for v0.1  
      **Priority:** P1 | **Effort:** S (config checks) | **Deps:** T02.01.01 | **Outcome:** Sample configs exist for PM, developer, security, moderator roles.
      - Description: Provide at least four sample configs as described in `PLAN.md` and `ROADMAP.md`.
      - Acceptance Criteria:
        - [ ] Sample configs are in `config/agents/` and pass validation.
  - [ ] `S09.02` As a release owner, I want a repeatable release process, so that v0.1 can be shipped reliably.
    - [ ] `T09.02.01` Execute acceptance test suite (AT-001..AT-008)  
      **Priority:** P0 | **Effort:** M (test run + verification) | **Deps:** T08.01.07 | **Outcome:** All acceptance tests pass prior to release.
      - Description: Run documented acceptance tests in `ROADMAP.md`.
      - Acceptance Criteria:
        - [ ] Evidence captured for all AT-001..AT-008 passes.
    - [ ] `T09.02.02` Package CLI and tag v0.1.0 release  
      **Priority:** P0 | **Effort:** M (build + tagging) | **Deps:** T09.02.01 | **Outcome:** Release artifacts and version tag are created.
      - Description: Build CLI executable, generate release notes, and tag `v0.1.0` as specified in `PLAN.md`.
      - Acceptance Criteria:
        - [ ] Release tag `v0.1.0` created with notes.
        - [ ] CLI executable packaged for distribution.

## Milestones / Execution Order

1. Foundation: `T01.01.01` → `T01.01.02` → `T01.01.03`
2. Config system: `T02.01.01` → `T02.01.02` → `T03.01.01`
3. Core models/agents: `T04.01.01` → `T04.01.02` → `T04.01.03`
4. Eventing & orchestration: `T04.02.01` → `T04.02.02` → `T04.02.03` → `T04.03.01`
5. Copilot integration: `T05.01.01` → `T05.01.02` → `T05.01.03`
6. Meeting room/artifacts: `T06.01.01` → `T06.01.03` → `T06.01.02` → `T06.01.04`
7. Logging & errors: `T07.01.01` → `T07.01.02` → `T07.01.03`
8. CLI meeting UX: `T03.02.01` → `T03.02.02` → `T03.02.03`
9. Testing & CI: `T08.01.01` → `T08.01.02` → `T08.01.03` → `T08.01.04` → `T08.01.05` → `T08.01.06` → `T08.01.07`
10. Docs & release: `T09.01.01` → `T09.01.02` → `T09.01.03` → `T09.02.01` → `T09.02.02`

## Open Questions / Missing Info

- Confirm the final solution/project layout: `README.md` documents `src/` and `tests/` structure, but the current repo only shows root `AIMeeting.csproj` and `Program.cs`.
- Confirm whether `Spectre.Console` is required for v0.1 CLI UI (listed in `ARCHITECTURE.md` but not elsewhere).
- Confirm packaging requirements for v0.1 CLI (self-contained vs. framework-dependent) beyond the `PLAN.md` release note.
- Pending stakeholder inputs (non-blocking): Q20 monitoring/alerting, Q26 README onboarding focus, Q27 licensing guidance (`STATUS.md`, `ASSESSMENT.md`, `READINESS.md`).

## Change Log (Plan Updates)

- `YYYY-MM-DD` — `Author` — Initial v0.1 plan created from repository evidence.
- `YYYY-MM-DD` — `Author` — _Update summary here._
