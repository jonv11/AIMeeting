# 🚀 Getting Started with AIMeeting

**Welcome!** This guide will help you get up and running with AIMeeting in under 15 minutes.

---

## **Step 1: Prerequisites** (2 min)

Ensure you have:
- ✅ **.NET 8 SDK** or later ([Download](https://dotnet.microsoft.com/download))
  - Verify: `dotnet --version`
- ✅ **GitHub Copilot SDK for .NET** (integrated via NuGet in v0.1.1)
  - No separate installation required
- ✅ **Active GitHub Copilot subscription** (required for v0.1)
- ✅ **Git** ([Download](https://git-scm.com/))

---

## **Step 2: Clone & Setup** (3 min)

```bash
# Clone the repository
git clone https://github.com/jonv11/AIMeeting.git
cd AIMeeting

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests (optional)
AIMEETING_AGENT_MODE=stub dotnet test
```

**✓ If build succeeds, you're ready!**

---

## **Step 3: Run Your First Meeting** (5 min)

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Discuss microservices architecture" \
    --agents "config/agents/project-manager.txt" \
             "config/agents/senior-developer.txt" \
    --max-duration 15 \
    --max-messages 30
```

**Expected output:**
- Real-time agent discussion
- Transcript saved to `meetings/YYYYMMDD_HHMMSS_topic/transcript.md`
- Metadata in `meetings/YYYYMMDD_HHMMSS_topic/meeting.json`

---

## **Step 4: Explore the Project** (3 min)

```
AIMeeting/
├── README.md                    ← Start here (project overview)
├── ARCHITECTURE.md              ← System design
├── src/
│   ├── AIMeeting.CLI/           ← Command-line interface
│   ├── AIMeeting.Core/          ← Business logic
│   ├── AIMeeting.Copilot/       ← GitHub Copilot integration
│   └── AIMeeting.Infrastructure/← Logging, serialization
├── tests/
│   ├── AIMeeting.Core.Tests/
│   ├── AIMeeting.Copilot.Tests/
│   └── AIMeeting.Integration.Tests/
├── config/agents/               ← Agent configurations
└── docs/                        ← Documentation (you are here!)
    ├── reference/               ← API, extending, examples
    ├── guides/                  ← CLI, roadmap, standards
    ├── planning/                ← v0.1 planning & roadmap
    ├── reports/                 ← Status snapshots
    └── learning/                ← FAQ, roles
```

---

## **Step 5: Next Steps** (Choose Your Path)

### 👨‍💻 **I want to build features**
→ Read **[API Reference](reference/api.md)** and **[Development Standards](guides/standards/)**

### 🤖 **I want to create custom agents**
→ Read **[Agent Configuration](reference/agent-configuration.md)** and **[Extending Guide](reference/extending.md)**

### 🧪 **I want to write tests**
→ Read **[Testing Standards](guides/standards/testing.md)**

### 🔐 **I want to understand security**
→ Read **[Security Best Practices](guides/standards/security.md)**

### ❓ **I have questions**
→ Check **[FAQ](faq.md)** or ask in the team

---

## **Troubleshooting**

### "Copilot SDK connection error"
```bash
# The Copilot SDK is integrated via NuGet (no separate installation)
# Ensure you have an active GitHub Copilot subscription
# Check network connectivity and GitHub service status
```

### "Agent config validation failed"
```bash
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/your-agent.txt
```

### "Meeting timed out"
- Increase `--max-duration` (in minutes)
- Check internet connectivity (Copilot API requires it)
- Verify your GitHub Copilot subscription is active

### ".NET 8 not found"
```bash
dotnet --version  # Check installed version
# Install: https://dotnet.microsoft.com/download/dotnet/8.0
```

---

## **Common Commands**

```bash
# Validate agent configuration
dotnet run --project src/AIMeeting.CLI -- validate-config config/agents/your-agent.txt

# List available agent configurations
ls config/agents/

# Run tests with coverage
AIMEETING_AGENT_MODE=stub dotnet test /p:CollectCoverage=true

# Clean build
dotnet clean
dotnet build
```

---

## **Key Documentation**

| Need | Doc |
|------|-----|
| API/Interfaces | [API Reference](reference/api.md) |
| CLI commands | [CLI Guide](guides/cli.md) |
| Agent setup | [Agent Configuration](reference/agent-configuration.md) |
| Create custom agents | [Extending Guide](reference/extending.md) |
| Code standards | [Development Standards](guides/standards/) |
| Feature roadmap | [Product Roadmap](guides/roadmap.md) |

---

## **Communication & Support**

- 🐛 **Found a bug?** Create an issue on GitHub
- 💬 **Have a question?** Check [FAQ](faq.md)
- 📖 **Need guidance?** Read [Architecture](../ARCHITECTURE.md)
- 👥 **Team structure?** See [Roles & Responsibilities](roles.md)

---

## **Next Meeting?**

Try these topics to explore:
```bash
# API design discussion
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "How should we design the REST API?" \
    --agents "config/agents/project-manager.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/security-expert.txt" \
    --max-duration 20 \
    --max-messages 40

# Code review discussion
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Best practices for code review" \
    --agents "config/agents/senior-developer.txt" \
             "config/agents/quality-architect.txt" \
    --max-duration 15 \
    --max-messages 30
```

---

✅ **You're all set!** Welcome to the team.

**Questions?** → [FAQ](faq.md) | **Help?** → [Roles & Responsibilities](roles.md)

---

**Last Updated**: January 31, 2026
