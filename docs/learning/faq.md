# ❓ Frequently Asked Questions (FAQ)

Find answers to common questions about AIMeeting. Can't find your answer? Check [Roles & Responsibilities](roles.md) or [Getting Started](getting-started.md).

---

## **Installation & Setup**

### **Q: Do I need a paid Copilot subscription?**
**A:** Yes, GitHub Copilot CLI requires an active GitHub Copilot subscription. Free tier does not include access to Copilot CLI.

### **Q: What .NET version do I need?**
**A:** .NET 8.0 or later. Verify with: `dotnet --version`

### **Q: Can I use without internet?**
**A:** No, GitHub Copilot API requires internet connectivity for agent responses. AIMeeting is cloud-dependent in v0.1.

### **Q: Where should I clone the repository?**
**A:** Any location works. Example:
```bash
git clone https://github.com/jonv11/AIMeeting.git
cd AIMeeting
dotnet build
```

---

## **Running Meetings**

### **Q: What's the minimum number of agents needed?**
**A:** At least 2 agents. Meetings with fewer agents may not generate meaningful discussion.

### **Q: How many agents can participate in a meeting?**
**A:** Recommended 2-6 agents for quality conversations. Technical limit is higher, but response time increases with more agents. v0.1 supports any number; v0.3 will optimize for dynamic agent selection.

### **Q: How much does a meeting cost?**
**A:** Depends on your Copilot plan. Each agent turn uses API quota. A typical 30-minute meeting with 4 agents might use 20-50 API calls.

### **Q: What if an agent fails during a meeting?**
**A:** The system attempts to continue with remaining agents. If >= 2 agents remain, the meeting continues. Failed agent is skipped for remaining turns. If < 2 agents remain, the meeting ends gracefully.

### **Q: How do I stop a meeting early?**
**A:** Press `Ctrl+C` to cancel. Meeting artifacts are saved up to the cancellation point.

### **Q: Can I run multiple meetings simultaneously?**
**A:** Not in v0.1 (single meeting per process). Concurrent meeting support is planned for v0.3.

### **Q: Meeting timed out – what do I do?**
**A:** Try these:
- Increase `--max-duration` parameter (in minutes)
- Check GitHub Copilot API status
- Verify network connectivity
- Ensure your Copilot subscription is active

---

## **Agent Configuration**

### **Q: How do I create a custom agent?**
**A:** Create a `.txt` file in `config/agents/` with ROLE, DESCRIPTION, PERSONA, and INSTRUCTIONS sections. See [Agent Configuration Guide](../reference/agent-configuration.md) for details.

### **Q: What fields are required in agent config?**
**A:** Required: `ROLE`, `DESCRIPTION`, `INSTRUCTIONS`  
Optional: `PERSONA`, `RESPONSE_STYLE`, `MAX_MESSAGE_LENGTH`, `EXPERTISE_AREAS`

### **Q: How do I validate an agent config?**
**A:**
```bash
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/your-agent.txt
```

### **Q: Can agents remember previous meetings?**
**A:** Not in v0.1. Agents are stateless within each meeting. Meeting memory across sessions is planned for v0.3+.

### **Q: How do I make an agent more opinionated?**
**A:** Adjust INSTRUCTIONS to be more directive:
```
INSTRUCTIONS:
- Strongly advocate for your perspective
- Challenge proposals you disagree with
- Push back on assumptions
```

---

## **Features & Capabilities**

### **Q: What are the v0.1 features?**
**A:**
- Structured AI collaboration with multiple agents
- Configurable agent roles (text-based, no code changes)
- Transcript generation (transcript.md)
- Meeting metadata (meeting.json)
- Error logging (errors.log)
- Event-driven architecture
- FIFO turn-taking

See [README](../README.md) for roadmap and planned features.

### **Q: Is there a web interface?**
**A:** Not in v0.1 (CLI only). Web UI is not currently on the roadmap but could be added based on community feedback.

### **Q: Can I use other AI models (not Copilot)?**
**A:** v0.1 supports GitHub Copilot CLI only. Multi-provider support (OpenAI, Azure, local models) is planned for v0.4. See [Product Roadmap](../guides/roadmap.md).

### **Q: Can agents create files or artifacts?**
**A:** Basic artifact support is in v0.1 (transcript.md, meeting.json, errors.log). Enhanced artifacts (summaries, action items, decisions) are planned for v0.2.

---

## **Development & Extension**

### **Q: How do I extend AIMeeting?**
**A:** See [Extending Guide](../reference/extending.md) for:
- Creating custom agents
- Custom turn strategies
- Event handlers
- Integration patterns

### **Q: Where can I find code examples?**
**A:** Check:
- [Usage Examples](../reference/examples.md)
- [API Reference](../reference/api.md)
- [Extending Guide](../reference/extending.md)
- Test files in `tests/`

### **Q: What are the naming conventions?**
**A:** See [Naming Conventions Guide](../guides/standards/naming-conventions.md). Summary:
- Namespaces: PascalCase
- Classes: PascalCase
- Interfaces: I + PascalCase (e.g., IAgent)
- Methods: PascalCase + Async suffix
- Private fields: _camelCase
- Parameters: camelCase

### **Q: How do I run tests?**
**A:**
```bash
# Run all tests
AIMEETING_AGENT_MODE=stub dotnet test

# Run specific project
AIMEETING_AGENT_MODE=stub dotnet test tests/AIMeeting.Core.Tests

# With coverage
AIMEETING_AGENT_MODE=stub dotnet test /p:CollectCoverage=true
```

---

## **Troubleshooting**

### **Q: "Copilot CLI not found" error**
**A:** 
1. Install GitHub Copilot CLI: https://github.com/github/copilot-cli
2. Verify installation: `gh copilot --version`
3. Ensure `gh` is in your PATH
4. Restart terminal after installation

### **Q: "Agent config validation failed"**
**A:**
1. Check required fields: ROLE, DESCRIPTION, INSTRUCTIONS
2. Run: `dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/your-agent.txt`
3. See [Agent Configuration Guide](../reference/agent-configuration.md) for format
4. Check for typos in section headers

### **Q: "File lock timeout" error**
**A:**
1. Close other processes accessing the meeting directory
2. Check disk space availability
3. Verify write permissions on `meetings/` directory
4. Ensure no antivirus is blocking file operations

### **Q: Build fails with "dotnet not found"**
**A:**
1. Verify .NET 8 is installed: `dotnet --version`
2. Download from: https://dotnet.microsoft.com/download/dotnet/8.0
3. Add to PATH if not already
4. Restart terminal

### **Q: "No agents available" error**
**A:**
1. Ensure agent config files exist in `config/agents/`
2. Validate each: `dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/<file>`
3. Check file paths are correct in CLI arguments
4. Ensure files are readable

### **Q: Meeting generated empty transcript**
**A:**
1. Check that agents generated responses (see console output)
2. Verify Copilot CLI is connected: `gh copilot --version`
3. Check error log: `meetings/YYYYMMDD_HHMMSS_topic/errors.log`
4. Increase `--max-duration` if agents are timing out

---

## **File Locations & Outputs**

### **Q: Where are meeting results stored?**
**A:** In `meetings/` directory, organized by timestamp:
```
meetings/
└── 20260130_143022_topic_slug/
    ├── meeting.json          # Meeting metadata
    ├── transcript.md         # Full conversation
    └── errors.log            # Error details (if any)
```

### **Q: How are agent configurations stored?**
**A:** In `config/agents/` as plain text `.txt` files. Example:
```
config/agents/
├── project-manager.txt
├── senior-developer.txt
├── security-expert.txt
└── your-custom-agent.txt
```

### **Q: Can I customize the meetings directory?**
**A:** Yes, use the `--output-directory` parameter (planned for v0.2).

---

## **Performance & Limits**

### **Q: How long does a typical meeting take?**
**A:** Depends on configuration:
- 2-3 agents, 10 messages: ~2-5 minutes
- 4-5 agents, 30 messages: ~10-20 minutes
- Bottleneck: Copilot API latency (30-120 sec per response)

### **Q: What are the hard limits?**
**A:** Can be configured per meeting:
- `--max-duration`: Meeting time limit (minutes)
- `--max-messages`: Total message count limit
- `MAX_MESSAGE_LENGTH` (per agent config): Response character limit

### **Q: Can I customize time/message limits?**
**A:** Yes, via CLI parameters or MeetingConfiguration in code. See [API Reference](../reference/api.md).

---

## **Documentation & Help**

### **Q: Where should I start if I'm new?**
**A:** 
1. Read [Getting Started](getting-started.md) (5 min)
2. Run your first meeting (5 min)
3. Check [Agent Configuration Guide](../reference/agent-configuration.md) (10 min)
4. Explore [API Reference](../reference/api.md) for deeper integration

### **Q: Where is the architecture documented?**
**A:** In the root [ARCHITECTURE.md](../ARCHITECTURE.md). Also see [Extending Guide](../reference/extending.md) for design patterns.

### **Q: Where are best practices documented?**
**A:** In [guides/standards/](../guides/standards/). See:
- [Testing Strategy](../guides/standards/testing.md)
- [Security Best Practices](../guides/standards/security.md)
- [Code Review Best Practices](../guides/standards/code-review.md)

### **Q: How do I contribute?**
**A:** See root [README.md](../README.md) CONTRIBUTING section (if present) or check GitHub repository for contribution guidelines.

---

## **Roadmap & Future**

### **Q: What's coming in v0.2?**
**A:** 
- Meeting summaries
- Action item extraction
- Decision tracking
- Agent-specific notes
See [Product Roadmap](../guides/roadmap.md) for details.

### **Q: What's the long-term vision?**
**A:** 
- v0.3: Concurrent meetings, dynamic turn-taking, persistence
- v0.4: RAG integration, multi-provider support
- v1.0: Production release with HTTP API, cloud deployment

See [Product Roadmap](../guides/roadmap.md) for complete timeline.

### **Q: When is v1.0 expected?**
**A:** Not yet scheduled. v0.1 is MVP, v0.2 and v0.3 planned for early 2026.

---

## **Still Have Questions?**

- 📖 Check [Getting Started](getting-started.md)
- 👥 See [Roles & Responsibilities](roles.md)
- 🔗 Review [Documentation Hub](../README.md)
- 🐛 Report issues on GitHub
- 💬 Ask the team

---

**Last Updated**: January 31, 2026  
**Status**: For v0.1 release
