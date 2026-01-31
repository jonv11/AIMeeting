# 📚 AIMeeting Documentation

Welcome to the AIMeeting documentation hub. This is your central navigation point for all project documentation.

## 📂 Documentation Structure

- **[learning/](learning/)** - Foundational onboarding content for new contributors (Getting Started, FAQ, Roles)
- **[guides/](guides/)** - Task-oriented how-to documents (CLI usage, standards, roadmap)
- **[reference/](reference/)** - Stable API documentation and technical specifications
- **[planning/](planning/)** - Version-specific planning documents and roadmaps
- **[reports/](reports/)** - Timestamped status reports and assessments
- **[archive/](archive/)** - Historical documents and old reports

---

## 🎯 **By Audience**

### 👤 **New Contributors**
Start here if you're new to AIMeeting:
- **[Getting Started](learning/getting-started.md)** – First 5 minutes guide
- **[FAQ](learning/faq.md)** – Common questions and answers
- **[Roles & Responsibilities](learning/roles.md)** – Team structure
- **[Coding Standards](guides/standards/)** – Best practices

### 👨‍💻 **Developers**
Building features or extending AIMeeting:
- **[API Reference](reference/api.md)** – Core interfaces and usage
- **[Agent Configuration](reference/agent-configuration.md)** – How to configure agents
- **[Extending AIMeeting](reference/extending.md)** – Custom agents, handlers, turn strategies
- **[Usage Examples](reference/examples.md)** – Code samples
- **[CLI Guide](guides/cli.md)** – Command-line reference
- **[Development Standards](guides/standards/)** – Testing, error handling, security

### 🏗️ **Architects**
Understanding the system design:
- **[Architecture Reference](../ARCHITECTURE.md)** – System design and components
- **[API Reference](reference/api.md)** – Interface contracts
- **[Extending Guide](reference/extending.md)** – Extension points and patterns
- **[Security Best Practices](guides/standards/security.md)** – Security design

### 📋 **Project Managers**
Tracking progress and planning:
- **[Product Roadmap](guides/roadmap.md)** – Feature timeline and versions
- **[v0.1 Planning](planning/v0.1/plan.md)** – Current version scope
- **[v0.1 Deliverables](planning/v0.1/deliverables.md)** – What's in v0.1
- **[Status Reports](reports/)** – Timestamped status snapshots

### ✅ **QA & Testers**
Quality and testing:
- **[QA Consolidated Report](archive/qa-status.md)** – Historical QA status report
- **[Test Execution Report](reports/2026-01-31/test-report.md)** – Test results
- **[Testing Standards](guides/standards/testing.md)** – Testing best practices
- **[Security Best Practices](guides/standards/security.md)** – Security testing

---

## 📂 **By Topic**

### Reference Material (Permanent, Stable)
- **[API.md](reference/api.md)** – Complete API reference
- **[Agent Configuration Guide](reference/agent-configuration.md)** – Agent config specification
- **[Extending AIMeeting](reference/extending.md)** – Extensibility patterns
- **[Usage Examples](reference/examples.md)** – Code samples and walkthroughs

### How-To Guides
- **[CLI Guide](guides/cli.md)** – Command-line usage
- **[Product Roadmap](guides/roadmap.md)** – Feature timeline
- **[Getting Started](learning/getting-started.md)** – First-time setup

### Best Practices & Standards
All best practices are in **[guides/standards/](guides/standards/)**:
- [API Design Conventions](guides/standards/api-design.md)
- [Code Review Best Practices](guides/standards/code-review.md)
- [Documentation Standards](guides/standards/documentation.md)
- [Error Handling & Logging](guides/standards/error-handling.md)
- [Git Workflow](guides/standards/git-workflow.md)
- [Naming Conventions](guides/standards/naming-conventions.md)
- [Security Best Practices](guides/standards/security.md)
- [Testing Strategy](guides/standards/testing.md)
- [Markdown Guidelines](guides/standards/markdown.md)
- [Project Structure](guides/standards/project-structure.md)
- [AI Prompt Engineering](guides/standards/ai-prompt-engineering.md)
- [GitHub Repo Files](guides/standards/github-repo-files.md)

### Planning & Roadmap
- **[Product Roadmap](guides/roadmap.md)** – Long-term vision
- **[v0.1 Planning](planning/v0.1/plan.md)** – Current sprint
- **[v0.1 Deliverables](planning/v0.1/deliverables.md)** – Version scope
- **[v0.1 Readiness](planning/v0.1/readiness.md)** – Release checklist

### Status Reports (Timestamped)
See **[reports/](reports/)** for status snapshots:
- **[Latest Status (2026-01-31)](reports/2026-01-31/)** – Current reports
  - [Executive Summary](reports/2026-01-31/executive-summary.md)
  - [Implementation Report](reports/2026-01-31/implementation-report.md)
  - [Test Report](reports/2026-01-31/test-report.md)
  - [Assessment](reports/2026-01-31/assessment.md)
  - [Visual Overview](reports/2026-01-31/visual-overview.md)
- **[Older Reports](reports/index.md)** – Archive of previous status snapshots

### Learning & FAQ
- **[Getting Started](learning/getting-started.md)** – New contributor guide
- **[FAQ](learning/faq.md)** – Frequently asked questions
- **[Roles & Responsibilities](learning/roles.md)** – Team structure and responsibilities

---

## 📖 **Document Structure Guide**

| Folder | Purpose | Update Frequency | Who Uses |
|--------|---------|------------------|----------|
| `/reference/` | Core API, configuration, extension specs | Rarely | Developers, Architects |
| `/guides/` | How-to guides, CLI, roadmap | Often | All audiences |
| `/guides/standards/` | Best practices and conventions | Occasionally | Developers, QA |
| `/planning/` | Version-specific planning (v0.1, v0.2, etc.) | Per release | Project managers |
| `/reports/` | Timestamped status snapshots | Per sprint | All audiences |
| `/learning/` | Onboarding, FAQ, roles | Occasionally | New contributors |
| `/archive/` | Deprecated or superseded docs | Never | Historical reference |

---

## 🔗 **Precedence & Authority**

If you find conflicting information, follow this hierarchy:

1. **[../ARCHITECTURE.md](../ARCHITECTURE.md)** – System design authority
2. **[reference/](reference/)** – API, config, extension specs
3. **[guides/standards/](guides/standards/)** – Coding and testing standards
4. **[planning/](planning/)** – Feature scope and roadmap
5. **[reports/](reports/)** – Status snapshots (informational, may be outdated)
6. **[learning/](learning/)** – Onboarding and FAQ (informational)

---

## ⚡ **Quick Links**

- 🏠 **[Back to Root README](../README.md)** – Project overview
- 🏗️ **[Architecture](../ARCHITECTURE.md)** – System design
- 📜 **[License](../LICENSE)**
- 📁 **[Agent Configurations](../config/agents/)** – Available agent roles

---

## 📝 **Contributing to Documentation**

When updating docs:
1. **Update the source file** (not copies)
2. **Link, don't duplicate** – Reference other docs instead of copying content
3. **Keep it organized** – Docs in correct category/folder
4. **Use clear headings** – Consistent with other files
5. **Update links** – When moving or renaming docs
6. **Timestamp reports** – Status reports should go in `/reports/YYYY-MM-DD/` folders

---

**Last Updated**: January 31, 2026  
**Documentation Format**: Markdown  
**Total Files**: ~40+ organized files across 8 categories
