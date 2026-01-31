# DEV QUESTIONS - PRODUCT STRATEGIST ANSWERS

**Answered by**: Product Strategist Role  
**Date**: January 30, 2026  
**Status**: Strategic Decisions for MVP v0.1

---

## Questions Addressed to Product Manager / Project Owner

### Q1. Primary goal and vision

**Question**: 
- What is the primary goal of the AIMeeting project? (MVP / long-term vision)
- Who are the primary users and stakeholders?
- What success criteria or acceptance tests define a working MVP?

**Answer**:

**Primary Goal (MVP v0.1)**:
Deliver a functional CLI-based multi-agent meeting system that demonstrates the core value proposition: enabling structured AI collaboration through configurable roles without requiring code changes.

**Success Criteria for MVP**:
1. **Core Functionality**:
   - System runs a meeting with 2-4 agents from start to completion
   - Agents exchange at least 10 meaningful turns on a given topic
   - Meeting produces a readable transcript and basic summary
   - Time and message limits are enforced correctly

2. **Configuration Flexibility**:
   - Users can define new agent roles using text files only
   - No code compilation required to add/modify agent personas
   - Validation catches common configuration errors before runtime

3. **Developer Experience**:
   - Installation takes <10 minutes for developer with Copilot subscription
   - First meeting runs successfully after following README quick-start
   - Error messages are actionable (not stack traces)

**Primary Users (MVP)**:
- **Developers** exploring AI agent collaboration patterns
- **AI researchers** prototyping multi-agent systems
- **Teams** experimenting with AI-assisted decision making

**Long-Term Vision (Post-MVP)**:
Enable organizations to conduct structured AI-powered discussions for complex decision-making, combining multiple expert perspectives, with RAG integration for domain-specific knowledge and export to production decision systems.

**Stakeholders**:
- Development team (us)
- Early adopters in AI/ML community
- Open source contributors

---

### Q2. Scope and acceptance

**Question**:
- What success criteria or acceptance tests define a working MVP?
- Any critical "out of scope" features that should be explicitly excluded from v0.1?

**Answer**:

**MVP Acceptance Tests** (Must Pass):

| Test ID | Scenario | Pass Criteria |
|---------|----------|---------------|
| AT-001 | Config Validation | `validate-config` catches missing ROLE field |
| AT-002 | Meeting Start | CLI starts meeting with valid config and topic |
| AT-003 | Turn-Taking | Agents take turns in FIFO order |
| AT-004 | Time Limit | Meeting stops within 5% of max-duration |
| AT-005 | Message Limit | Meeting stops at max-messages count |
| AT-006 | Transcript | Transcript.md contains all agent messages |
| AT-007 | Error Handling | File lock timeout produces clear error message |
| AT-008 | Multi-Platform | Builds and runs on Windows, Linux, macOS |

**Explicitly OUT of MVP v0.1**:

✗ **HTTP API / REST endpoints** - CLI only for v0.1  
✗ **RAG integration** - Agents use training data only  
✗ **Dynamic turn-taking** - FIFO only; no voting or moderator control  
✗ **Multi-provider support** - GitHub Copilot CLI only  
✗ **Web UI / Dashboard** - Terminal output only  
✗ **Meeting templates** - Simple CLI args only  
✗ **Agent memory across meetings** - Stateless agents  
✗ **Action item extraction** - Basic summary only  
✗ **Real-time collaboration** - Sequential processing  
✗ **Authentication system** - Relies on Copilot CLI auth  
✗ **Cloud deployment** - Local execution only  

**Rationale**: These features add complexity without proving core value. We validate the agent coordination model first, then layer on advanced features in v0.2+.

---

### Q3. MVP feature set

**Question**:
- What are the must-have features for the first release (MVP)?
- Which features are nice-to-have and can be deferred?
- Are there prioritized milestones or deadlines?

**Answer**:

**MUST-HAVE (v0.1) - Non-Negotiable**:

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

**NICE-TO-HAVE (v0.2) - Defer**:

- Summary generation (beyond raw transcript)
- Action item extraction
- Moderator intervention (skip turn, end meeting)
- Token budget tracking (requires API metering)
- Meeting replay/resume capability
- Multiple turn strategies (beyond FIFO)

**CONSIDER FOR v0.3+**:

- RAG integration for domain knowledge
- Multi-provider LLM support
- HTTP API
- Advanced artifact generation (decisions.md, diagrams)
- Meeting templates

**Timeline Estimate**:

| Milestone | Features | Target | Status |
|-----------|----------|--------|--------|
| M1: Foundation | Config parser, agent model | Week 2 | Not Started |
| M2: Orchestration | Meeting lifecycle, turn-taking | Week 4 | Not Started |
| M3: Integration | Copilot client, file ops | Week 6 | Not Started |
| M4: CLI & UX | Commands, console display | Week 7 | Not Started |
| M5: Testing | Unit + integration tests | Week 8 | Not Started |
| M6: MVP Release | v0.1.0 tagged | Week 9 | Not Started |

**Deadline**: No hard deadline for MVP. Target 6-8 weeks based on team availability.

---

### Q4. Scope creep guards

**Question**:
- How do we validate that we're not adding features beyond the MVP scope?
- Any features in the documentation (EXAMPLES.md, EXTENDING.md) that are aspirational vs. MVP?

**Answer**:

**Scope Validation Process**:

1. **Feature Gate Question**:
   Before implementing any feature, ask:
   - "Can we demonstrate core value without this?"
   - "Does this block the 8 acceptance tests?"
   - "Can this be added in v0.2 without major refactoring?"
   
   If answers are YES, YES, YES → Defer to v0.2

2. **Weekly Scope Review**:
   - Every Friday: Review commits against MVP feature list
   - Flag any work that's not on MUST-HAVE list
   - Document rationale if exception approved

3. **Documentation Tagging**:
   Update docs with version tags:
   - `[v0.1]` = MVP must-have
   - `[v0.2]` = Next release
   - `[Future]` = Roadmap item

**Documentation Audit - Aspirational Features**:

| Feature | Doc Reference | MVP? | Version |
|---------|---------------|------|---------|
| Config validation | AGENT_CONFIGURATION_GUIDE.md | ✅ Yes | v0.1 |
| FIFO turn-taking | ARCHITECTURE.md | ✅ Yes | v0.1 |
| Basic transcript | README.md | ✅ Yes | v0.1 |
| Meeting room isolation | ARCHITECTURE.md | ✅ Yes | v0.1 |
| Summary generation | README.md (output section) | ❌ No | v0.2 |
| Action item extraction | README.md (output section) | ❌ No | v0.2 |
| Decisions.md artifact | README.md | ❌ No | v0.2 |
| Agent-specific notes | README.md | ❌ No | v0.2 |
| RAG integration | FAQ section | ❌ No | v0.4 |
| Dynamic turn strategies | EXTENDING.md | ❌ No | v0.3 |
| Multi-provider LLM | FAQ section | ❌ No | v0.3 |
| HTTP API | API.md hints | ❌ No | v0.5 |
| Meeting templates | README config section | ❌ No | v0.2 |

**Recommendation**: 
- Update README.md to clearly mark "Meeting Output" section as v0.2 roadmap
- Add version badges to feature descriptions
- Create ROADMAP.md separating v0.1 from future phases

**Red Flags to Watch**:
- Implementing event handlers beyond core orchestration needs
- Adding database/persistence beyond filesystem
- Building abstraction layers for multi-provider support
- Creating web UI "prototypes"

---

### Q9. HTTP API scope

**Question**:
- Is there a public HTTP API planned for MVP? If so, what endpoints and auth are required?
- Or is CLI-only sufficient for v0.1?
- If API, should it be REST, gRPC, or something else?

**Answer**:

**Decision: CLI-Only for v0.1**

**Rationale**:
1. **User Focus**: Early adopters are developers comfortable with CLI
2. **Complexity**: HTTP API adds auth, deployment, scaling concerns unrelated to core value
3. **Validation Speed**: CLI provides fastest feedback loop for testing agent collaboration
4. **Resource Efficiency**: Avoids premature architectural decisions

**v0.1 CLI Commands** (Sufficient):
```bash
# Validate agent configuration
aimeeting validate-config config/agents/developer.txt

# Start meeting
aimeeting start-meeting \
  --topic "Architecture review" \
  --agents config/agents/pm.txt config/agents/dev.txt \
  --max-duration 30 \
  --max-messages 50

# (Future) List available agents
aimeeting list-agents
```

**API Considerations for v0.2+**:

If we add HTTP API in future releases:

**Option A: REST API** (Recommended for v0.2)
- Familiar to most developers
- Easy to document with OpenAPI/Swagger
- Simple authentication (API keys initially)
- Endpoints:
  - `POST /meetings` - Start meeting
  - `GET /meetings/{id}` - Get status
  - `GET /meetings/{id}/transcript` - Download transcript
  - `POST /agents/validate` - Validate config

**Option B: gRPC** (Consider for v0.3+)
- Better performance for streaming
- Useful if real-time turn updates become requirement
- Steeper learning curve for users

**Decision Point**: Defer API discussion until v0.1 is used by 10+ external developers. Gather feedback on whether API is actually needed.

**For MVP**: Focus on making CLI experience excellent. If API is needed, a well-designed CLI will inform better API design.

---

### Q21. Concurrency and scale expectations

**Question**:
- Expected concurrency and scale for MVP (single-user desktop, multi-tenant server)?
- Latency and throughput SLAs if any?
- Maximum concurrent meetings?

**Answer**:

**MVP v0.1 Scale Target: Single-User Desktop**

**Concurrency Model**:
- **1 meeting at a time per process** (no concurrent meetings)
- **Sequential turn-taking** (no parallel agent processing)
- **Single-threaded orchestration** (simplifies state management)

**Performance Expectations**:

| Metric | Target | Rationale |
|--------|--------|-----------|
| Meeting startup | <3 seconds | CLI responsiveness |
| Agent response time | 5-15 seconds | Copilot API latency |
| Turn overhead | <500ms | Event bus + file ops |
| Meeting duration | 5-30 minutes | Configurable, user-controlled |
| Max agents per meeting | 2-6 | Conversational quality over scale |
| Transcript write | <100ms | Non-blocking file I/O |

**No SLAs for v0.1** - This is developer tool, not production service

**Scale Limits (Intentional)**:

✓ **Supported**:
- 1 developer running 1 meeting on laptop
- Meeting artifacts stored on local disk
- GitHub Copilot API rate limits apply (user's quota)

✗ **Not Supported** (v0.1):
- Multiple concurrent meetings
- Distributed/cloud deployment
- Multi-user access to same meeting
- Real-time collaboration
- Meeting persistence across process restarts

**Future Scale Considerations** (v0.3+):

If adoption grows and we need multi-user support:

**Phase 1**: Add concurrent meeting support (separate directories)  
**Phase 2**: Containerize for cloud deployment  
**Phase 3**: Add meeting persistence (SQLite or similar)  
**Phase 4**: Real-time collaboration features

**Decision**: Build for single-user CLI first. Scale concerns are premature optimization until we validate core value proposition.

---

### Q25. Documentation scope

**Question**:
- Which user scenarios should be covered by examples (single agent test, multi-agent meeting, export transcript)?
- Any real-world sample agent configurations to include?
- Should we include troubleshooting or FAQ sections?

**Answer**:

**MVP v0.1 Documentation Priority**:

**MUST-HAVE Examples**:

1. **Quick Start** (Already in README.md) ✅
   - 3-agent meeting example
   - Shows CLI command structure
   - Expected output format

2. **Agent Configuration Examples** (Create):
   ```
   config/agents/
   ├── product-manager.txt       [INCLUDE]
   ├── senior-developer.txt      [INCLUDE]
   ├── security-expert.txt       [INCLUDE]
   ├── moderator.txt            [INCLUDE - simple]
   └── examples/
       ├── qa-engineer.txt       [NICE-TO-HAVE]
       ├── architect.txt         [NICE-TO-HAVE]
       └── ux-designer.txt       [NICE-TO-HAVE]
   ```

3. **Basic Troubleshooting** (Add to README.md):
   ```markdown
   ## Troubleshooting
   
   ### "Copilot CLI not found"
   - Install: https://github.com/github/copilot-cli
   - Verify: `gh copilot --version`
   
   ### "Agent config validation failed"
   - Check required fields: ROLE, DESCRIPTION, INSTRUCTIONS
   - Run: `aimeeting validate-config <file>`
   
   ### "Meeting timed out"
   - Increase --max-duration
   - Check Copilot API status
   
   ### "File lock timeout"
   - Close other processes accessing meeting directory
   - Check disk space
   ```

4. **Sample Meeting Scenarios** (2-3 examples):

   **Scenario A: Architecture Decision**
   ```bash
   # Should we use microservices or monolith?
   aimeeting start-meeting \
     --topic "Evaluate microservices vs monolith for payment service" \
     --agents pm.txt architect.txt developer.txt \
     --max-duration 20
   ```

   **Scenario B: Security Review**
   ```bash
   # Review authentication implementation
   aimeeting start-meeting \
     --topic "Review OAuth2 implementation security" \
     --agents security.txt developer.txt qa.txt \
     --max-duration 15
   ```

   **Scenario C: Simple Test**
   ```bash
   # Minimal 2-agent meeting for testing
   aimeeting start-meeting \
     --topic "Test agent interaction" \
     --agents developer.txt moderator.txt \
     --max-duration 5 \
     --max-messages 10
   ```

**DEFER to v0.2**:
- Export formats (JSON, PDF) - v0.1 produces markdown only
- Integration examples - No API yet
- Advanced configuration patterns - Keep it simple for MVP
- CI/CD integration - Premature for v0.1

**FAQ Section** (Essential for MVP):

Add to README.md:

```markdown
## FAQ

**Q: Do I need a paid Copilot subscription?**
A: Yes, GitHub Copilot CLI requires active subscription.

**Q: Can I use without internet?**
A: No, Copilot API requires internet connectivity.

**Q: How much does a meeting cost?**
A: Depends on your Copilot plan. Each agent turn uses API quota.

**Q: Can agents remember previous meetings?**
A: Not in v0.1. Agents are stateless within each meeting.

**Q: What if an agent fails?**
A: Meeting continues with remaining agents (if >= 2 agents left).

**Q: How do I stop a meeting early?**
A: Ctrl+C to cancel. Meeting artifacts saved up to cancellation point.
```

**Documentation Structure** (v0.1):

```
Documentation Priority:
1. README.md - Quick start, FAQ, basic troubleshooting [MVP]
2. AGENT_CONFIGURATION_GUIDE.md - Full config reference [MVP]
3. EXAMPLES.md - 3 sample scenarios [MVP]
4. ARCHITECTURE.md - Already complete [MVP]
5. API.md - Defer most content to v0.2 [Minimal for v0.1]
6. EXTENDING.md - Defer to v0.2 [Not MVP]
```

**Success Metric**: New user runs first meeting in <10 minutes using only README.md

---

## Technical Lead Answers

**Answered by**: Technical Lead Role  
**Date**: January 30, 2026  
**Status**: Technical Decisions for MVP v0.1

---

### Q5. Agent configuration file format

**Answer**:

**Canonical Source**: `config/agents/*.txt` is the canonical source for agent definitions.

**Format (MVP Spec)**:
- Plain text, UTF-8, max size **64 KB** per file.
- Section headers are uppercase keys ending with `:` (e.g., `ROLE:`, `DESCRIPTION:`).
- Section content is free-form text, optionally with `- ` list items.
- Blank lines are allowed; they delimit paragraphs inside a section.
- Allowed keys (MVP): `ROLE`, `DESCRIPTION`, `PERSONA`, `INSTRUCTIONS`, `RESPONSE_STYLE`, `MAX_MESSAGE_LENGTH`, `EXPERTISE_AREAS`, `COMMUNICATION_APPROACH`.
- Unknown keys are allowed but produce a warning (future extensibility).

**Parsing Rules**:
- A section starts at `^[A-Z0-9_ ]+:` and ends at the next header or EOF.
- Preserve line breaks within section content.
- Normalize line endings to `\n`.

---

### Q6. Agent validation behavior

**Answer**:

**Validation Depth**: Syntax + basic semantic validation.
- **Required**: `ROLE`, `DESCRIPTION`, `INSTRUCTIONS` must be present and non-empty.
- **Optional**: `MAX_MESSAGE_LENGTH` must be a positive integer if provided.
- **Lists**: `PERSONA`, `EXPERTISE_AREAS`, `COMMUNICATION_APPROACH` may be bullet lists; accept either bullets or paragraph text.

**Strictness**:
- **Strict on required fields** (fail validation).
- **Permissive on unknown headers** (warn only).

**CLI Output**:
- Exit code `0` for success, `1` for validation failure.
- Errors include section name and line number when available.

Example error:
- `Missing required field: ROLE (config/agents/pm.txt:1)`

---

### Q8. CLI commands for MVP

**Answer**:

**Required Commands**:
- `validate-config <file>`
- `start-meeting --topic <text> --agents <file> [<file> ...]`

**Optional for v0.1 (Nice-to-have)**:
- `list-agents [<directory>]` (default to `config/agents`)

**Argument Rules**:
- `--topic` required; non-empty.
- `--agents` requires 2+ agent files.
- `--max-duration` default 20 minutes; integer minutes.
- `--max-messages` default 50; integer.
- `--output-dir` optional; default `meetings/<timestamp>_<slug>`.

---

### Q10. LLM provider selection

**Answer**:

**Decision**: GitHub Copilot CLI is **mandatory** for MVP.
- No OpenAI/Azure/Open-source provider support in v0.1.
- Authentication relies on `gh` CLI auth (`gh auth status`).

**Technical Constraint**:
- Abstract behind an interface to enable future providers without changing orchestrator.

---

### Q12. Persistence scope

**Answer**:

**Persisted Artifacts (MVP)**:
- `transcript.md` (full meeting transcript)
- `meeting.json` (metadata: topic, agents, timestamps, limits)
- `errors.log` (optional if failures occur)

**Storage**: Filesystem only (no DB).

**Retention**: Keep all meeting directories; no auto-cleanup in v0.1.

---

### Q17. Test strategy

**Question**:
- Required test coverage or testing strategy (unit, integration, E2E)?
- Preferred test frameworks (xUnit, NUnit)?
- Any existing CI/CD pipeline or preferred providers?

**Answer**:

**Overall Test Strategy for MVP v0.1**: Test pyramid with emphasis on unit + integration, minimal E2E to save time.

```
        /\         E2E (CLI tests)
       /  \        ~5 tests | 10%
      /____\
     /      \     Integration (subsystems)
    /        \    ~20 tests | 30%
   /________\
  /          \   Unit (classes/methods)
 /            \  ~60 tests | 60%
/____________\
```

**Test Framework & Tools**:

| Component | Framework | Tool | Rationale |
|-----------|-----------|------|-----------|
| Unit tests | xUnit | Xunit assertions | Standard .NET core choice |
| Mocking | Moq | Moq + It.IsAny patterns | Lightweight, familiar |
| Assertions | FluentAssertions | Optional if available | Readable, chainable |
| CLI testing | xUnit facts | Custom CliTestHelper | Spawn process, capture output |
| Coverage | coverlet | Codecov/Cobertura | Standard coverage tracking |
| CI/CD | GitHub Actions | Actions workflow | Repository native |

**Code Coverage Target**:

| Component | Target | Rationale |
|-----------|--------|-----------|
| Agent parser | **95%** | Critical path, must be bug-free |
| Validation logic | **90%** | Core business logic |
| Event bus | **85%** | Core orchestration |
| Orchestrator | **85%** | Critical state machine |
| CLI commands | **70%** | Less critical, E2E tests cover |
| Utilities | **80%** | Standard |
| **Overall** | **≥80%** | Exception: V-Model constraints reduce E2E coverage slightly |

**Test Types & Scope**:

**1. UNIT TESTS** (~60 tests):

Agent Config Parser, ConfigValidator, EventBus, Orchestrator State Machine, MeetingRoom (Filesystem), Copilot Client Adapter covering:
- Parse valid/invalid configs
- Validate required fields
- Event subscription and publishing
- State transitions and turn-taking
- File operations and retry logic
- Response handling and timeouts

**2. INTEGRATION TESTS** (~20 tests):

Parser + Validator Integration, Orchestrator + EventBus + MeetingRoom, CLI Argument Parsing, Orchestrator + Stubbed Copilot Client covering:
- Full config parsing and validation
- Multi-agent meeting cycles
- Event-driven transcript writes
- Timeout and message limits
- Failed agent recovery
- Argument validation

**3. E2E / GOLDEN FILE TESTS** (~5 tests):

CLI End-to-End covering:
- CLI command validation
- Meeting execution with stubs
- Transcript matching golden files
- Metadata correctness
- Error handling

**Testability Requirements**:

**Dependency Injection**:
- Inject `IEventBus`, `IMeetingRoom`, `IAgentClient` into Orchestrator
- Allows substituting mocks/stubs in tests

**Stub Strategy for Copilot**:
- Create `StubAgentClient` that returns canned responses
- Canned responses keyed by (role, turn_number) for deterministic replay
- Use in integration + E2E tests to avoid actual Copilot API calls

**Config File Fixtures**:
```
tests/
  ├── fixtures/
  │   ├── configs/
  │   │   ├── valid-pm.txt
  │   │   ├── minimal-dev.txt
  │   │   ├── missing-role.txt
  │   │   └── oversize-config.txt
  │   ├── transcripts/
  │   │   ├── golden-2agent-5msg.md
  │   │   └── golden-4agent-20msg.md
```

**CI/CD Integration** (GitHub Actions):

```yaml
on: [push, pull_request]
jobs:
  test:
    runs-on: [windows-latest, ubuntu-latest, macos-latest]
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test --logger trx --collect:"XPlat Code Coverage"
      - run: codecov upload (if configured)
```

**Test Metrics to Track**:

| Metric | Target | Frequency |
|--------|--------|-----------|
| Code Coverage | ≥80% | Per commit |
| Test Pass Rate | 100% | Per commit |
| Test Execution Time | <60s | Per commit |

---

### Q18. Testability requirements

**Question**:
- How should we test without live Copilot API (mocks, stubs)?
- Should the system support offline testing?
- Any performance or benchmark tests required?

**Answer**:

**Testing Without Live Copilot: Stub Strategy**

**1. Copilot Abstraction Layer**:

Create `IAgentClient` interface to isolate Copilot:

```csharp
public interface IAgentClient
{
    Task<string> GetResponseAsync(
        string role, 
        string instructions, 
        string conversationContext, 
        CancellationToken cancellationToken);
    
    Task ValidateConnectionAsync();
}

public class CopilotCliClient : IAgentClient { /* real implementation */ }
public class StubAgentClient : IAgentClient { /* deterministic responses */ }
```

**2. Stub Response Strategy**:

For testing, use `StubAgentClient` with deterministic responses:

```csharp
public class StubAgentClient : IAgentClient
{
    private readonly Dictionary<(string role, int turn), string> _responses = new()
    {
        { ("Product Manager", 1), "I propose we prioritize..." },
        { ("Developer", 1), "From a technical perspective..." },
    };
    
    public Task<string> GetResponseAsync(string role, string instructions, 
        string conversationContext, CancellationToken ct)
    {
        var key = (role, _currentTurn);
        return Task.FromResult(_responses.ContainsKey(key) 
            ? _responses[key] 
            : "Stub response for " + role);
    }
}
```

**3. Test Levels with Stubs**:

| Test Level | Real Copilot? | Speed | Example |
|------------|---------------|-------|---------|
| Unit | ❌ No | <1ms | Test parser |
| Integration | ❌ No (Stub) | <100ms | Test orchestrator |
| E2E (Offline) | ❌ No (Stub) | <1s | Full CLI meeting |
| E2E (Optional Live) | ✅ Yes | 20-60s | Manual testing |

**Offline Testing Support (MVP)**:

**Decision**: MVP targets offline testing by default.
- Environment variable: `AIMEETING_AGENT_MODE=stub` (default for CI/CD)
- Override: `AIMEETING_AGENT_MODE=copilot` (real API)

**Configuration**:

```csharp
var agentMode = Environment.GetEnvironmentVariable("AIMEETING_AGENT_MODE") ?? "stub";
IAgentClient client = agentMode switch
{
    "copilot" => new CopilotCliClient(),
    "stub" => new StubAgentClient(),
    _ => throw new InvalidOperationException($"Unknown agent mode: {agentMode}")
};
```

**No live Copilot in CI/CD** (v0.1):
- GitHub Actions runs all tests with `AIMEETING_AGENT_MODE=stub`
- Prevents quota exhaustion and flaky tests
- Manual E2E with real Copilot is optional

---

**Performance & Benchmark Tests (MVP)**:

**Scope**: No formal benchmarking in v0.1. Profile if issues arise.

**Performance Targets** (Informational):

| Scenario | Target | Notes |
|----------|--------|-------|
| Parser: Single config | <5ms | Should be fast |
| Validator: Full file | <10ms | Not a bottleneck |
| Orchestrator: Start meeting | <100ms | Initialization |
| Event bus: Publish | <1ms | In-memory |
| Transcript write | <50ms | File I/O |
| Full 2-agent, 10-turn (stub) | <2s | Offline throughput |
| Full 4-agent, 20-message (stub) | <5s | Scale measurement |

**No SLA enforced for MVP** - Developer tool, local machine execution.

---

## Quality Architect Recommendations for Implementation

**Before Starting Coding**:

1. ✅ **Establish Test Directory Structure**:
   ```
   tests/
   ├── AIMeeting.Tests/
   │   ├── Parser/
   │   ├── Orchestration/
   │   ├── Integration/
   │   ├── Fixtures/
   │   └── Stubs/
   ```

2. ✅ **Create Test Base Classes**:
   - `ConfigTestFixture` for parsing test data
   - `OrchestratorTestFixture` for meeting state setup
   - `CliTestHelper` for process spawning

3. ✅ **Establish Code Coverage Dashboard**:
   - Add `coverlet` to test project
   - Configure GitHub Actions tracking

4. ✅ **Define Mock/Stub Naming Conventions**:
   - `Stub*` = Hardcoded responses
   - `Mock*` = Behavior verification (Moq objects)

5. ✅ **Write First Test Early** (TDD):
   - Implement parser test first
   - Drives interface design

**Risk Areas Requiring Extra Testing**:

| Risk | Mitigation | Test Focus |
|------|-----------|-----------|
| Config parser runtime mismatch | Golden file tests | Boundary cases |
| File locking errors (Windows) | Platform-specific tests | Windows CI |
| Agent failure cascades | Resilience tests | Skip + retry logic |
| Turn order scramble | Deterministic stubs | Exact message order |
| Timeout values wrong | Configurable + benchmarks | Real latency measurement |

---

**Summary of QA Lead Contributions**:

✅ **Validation Strategy**: Strict required, permissive optional  
✅ **Test Strategy**: ~85 tests across unit/integration/E2E, 80%+ coverage  
✅ **Offline Testing**: Stub-based, no live Copilot in CI/CD  
✅ **Acceptance Criteria**: Defined for each story  
✅ **Risk Prioritization**: File locking, agent failures, turn order  

---

## Infrastructure Owner / DevOps Answers

**Answered by**: Platform Engineer Role  
**Date**: January 30, 2026  
**Status**: Infrastructure Decisions for MVP v0.1

---

### Q13. Storage backend

**Answer**:

**Decision**: Filesystem-only storage for v0.1.

**Directory Structure**:
```
meetings/
  <timestamp>_<topic-slug>/
    meeting.json
    transcript.md
    errors.log (only on failure)
```

**Rules**:
- Always create a new directory per meeting (no overwrites).
- Use `meetings/<yyyyMMdd_HHmmss>_<slug>` as default output.
- No versioning or database in v0.1.

---

### Q19. Logging requirements

**Answer**:

**Decision**: Serilog with console + rolling file output.

**Minimum**:
- Console: `Information` and above.
- File: `Information` and above, rolling daily in `logs/meeting-.log`.
- Include correlation fields: `MeetingId`, `AgentId`, `TurnNumber` when available.

**MVP Scope**:
- No centralized log aggregation in v0.1.
- Logs are local only.

---

### Q28. Deployment model

**Answer**:

**Decision**: Local CLI distribution only for v0.1.

**Packaging**:
- `dotnet publish` for platform-specific binaries.
- Optional GitHub Releases with zipped binaries.
- No Docker images or cloud deployment in v0.1.

---

### Q30. Integration test resources

**Answer**:

**Decision**: No external test resources required for CI.

**Approach**:
- Default to `AIMEETING_AGENT_MODE=stub` for all CI/CD runs.
- No Copilot API keys or subscriptions required in CI.
- Optional manual integration tests with live Copilot for maintainers only.

---

## Security Lead Answers

**Answered by**: Security Guardian Role  
**Date**: January 30, 2026  
**Status**: Security Decisions for MVP v0.1

---

### Q14. Compliance requirements

**Answer**:

**Decision**: No formal compliance requirements for v0.1.

**Notes**:
- Treat meeting content as local developer data.
- Do not store regulated data (HIPAA/GDPR) in sample configs.

---

### Q15. Authentication and authorization

**Answer**:

**Decision**: CLI-only with GitHub Copilot CLI authentication.

**Rules**:
- No additional app-level auth in v0.1.
- If `gh auth status` fails, surface a clear error and exit.

---

### Q16. Secrets management

**Answer**:

**Decision**: No secrets stored by AIMeeting.

**Rules**:
- Users authenticate via `gh auth login` for Copilot CLI.
- Do not accept API keys in config files.
- Keep any future secret support out of v0.1 scope.

---

## Senior Developer Answers

**Answered by**: Principal Engineer Role  
**Date**: January 30, 2026  
**Status**: Implementation Constraints for MVP v0.1

---

### Q11. SDK constraints and dependencies

**Answer**:

**Decision**:
- .NET 8 SDK required.
- GitHub CLI + Copilot extension required (`gh copilot --version` must succeed).
- Core dependencies: System.CommandLine, Serilog, xUnit, Moq.
- No additional SDKs required for v0.1.

---

### Q22. Performance constraints

**Answer**:

**Defaults (v0.1)**:
- Max duration: 20 minutes (configurable via CLI).
- Max messages: 50 (configurable via CLI).
- Agent response timeout: 30 seconds default.
- File lock timeout: 5 seconds default.

**Recommended Limits**:
- 2–6 agents per meeting for quality.
- Keep total meeting duration within 5–30 minutes.

---

## PRIORITY ROLES FOR NEXT ANSWERS (UPDATED)

**Status**: ✅ All critical roles answered. No blockers remain for v0.1 start.

---

**Document Completion**: ✅ All required role answers captured

