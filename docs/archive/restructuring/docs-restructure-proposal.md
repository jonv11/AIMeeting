# Documentation Restructuring Proposal – AIMeeting

**Date**: January 31, 2026  
**Status**: Ready for Review & Approval  
**Scope**: Root-level documentation organization + existing docs/ folder optimization

---

## Executive Summary

The AIMeeting repository has **59 documentation/markdown files** spread across root and `/docs/` directory with:

- ✓ Strong **architecture and API documentation** (ARCHITECTURE.md, API.md, EXTENDING.md)
- ✓ Comprehensive **best practices guides** in `/docs/` (coding, testing, security, etc.)
- ✓ Solid **reference materials** (agent configuration, examples, CLI quick reference)
- ✗ **Scattered status/reporting files** at root level (13+ files)
- ✗ **Duplicate restructuring guides** (9+ files about restructuring itself!)
- ✗ **Unclear entry points** for different audiences (onboardees, contributors, maintainers, agents)
- ✗ **No single navigation hub** linking everything together

### Proposed Changes

1. **Create `/docs/` subdirectory structure** with audience-driven navigation
2. **Keep GitHub standards at root** (README.md, ARCHITECTURE.md, LICENSE)
3. **Move reference docs** (API.md, EXTENDING.md, AGENT_CONFIGURATION_GUIDE.md) → `/docs/reference/`
4. **Consolidate guides** → `/docs/guides/`
5. **Archive status reports & restructuring docs** → `/docs/archive/`
6. **Create navigation hub** in `/docs/README.md`
7. **Update root README.md** with link to documentation

**Result**: 
- Root: ~8 files (clean, GitHub-standard)
- `/docs/`: Organized, purpose-driven, audience-specific
- Reduced clutter by ~75%
- Clear entry points for all audiences

---

## Phase 1: Document Inventory

### A. Root-Level Files (Currently 59 total)

#### GitHub Standards (KEEP AT ROOT)
| File | Purpose | Status |
|------|---------|--------|
| README.md | Project overview, quick start | ✓ Keep |
| ARCHITECTURE.md | System design & architecture | ✓ Keep |
| LICENSE | License info | ✓ Keep |
| .gitignore | Git ignore rules | ✓ Keep |
| .editorconfig | Editor settings | ✓ Keep |

#### Reference Documentation (MOVE TO `/docs/reference/`)
| File | Purpose | New Path | Notes |
|------|---------|----------|-------|
| API.md | API reference | `/docs/reference/api.md` | Core reference |
| EXTENDING.md | Extension guide | `/docs/reference/extending.md` | Dev guide |
| AGENT_CONFIGURATION_GUIDE.md | Agent config reference | `/docs/reference/agent-configuration.md` | Config spec |
| EXAMPLES.md | Usage examples | `/docs/reference/examples.md` | Code samples |

#### Quick Reference / Getting Started (MOVE TO `/docs/guides/`)
| File | Purpose | New Path | Action |
|------|---------|----------|--------|
| CLI_QUICK_REFERENCE.md | CLI command reference | `/docs/guides/cli.md` | Rename & move |
| ROADMAP.md | Product roadmap | `/docs/guides/roadmap.md` | Move |

#### Status/Planning Docs (MOVE TO `/docs/planning/`)
| File | Purpose | New Path | Action |
|------|---------|----------|--------|
| PLAN.md | v0.1 planning | `/docs/planning/v0.1-plan.md` | Merge with PLAN-V0-1 |
| PLAN-V0-1.md | Current plan | `/docs/planning/v0.1-plan.md` | Merge |
| DELIVERABLES.md | v0.1 deliverables | `/docs/planning/v0.1-deliverables.md` | Move |
| READINESS.md | Release readiness | `/docs/planning/v0.1-readiness.md` | Move |

#### Status Reports (MOVE TO `/docs/reports/` with date stamps)
| File | Purpose | New Path | Note |
|------|---------|----------|------|
| EXECUTIVE_SUMMARY.md | Status snapshot | `/docs/reports/2026-01-31-executive-summary.md` | Timestamp |
| IMPLEMENTATION_REPORT.md | Implementation status | `/docs/reports/2026-01-31-implementation-report.md` | Timestamp |
| TEST_EXECUTION_REPORT.md | Test results | `/docs/reports/2026-01-31-test-report.md` | Timestamp |
| VISUAL_STATUS_OVERVIEW.md | Visual status | `/docs/reports/2026-01-31-visual-overview.md` | Timestamp |
| QA_COMPLETION_STATUS.md | QA status | `/docs/reports/2026-01-31-qa-status.md` | Merge to one QA report |
| QA_LEAD_COMPLETION_REPORT.md | QA details | `/docs/reports/2026-01-31-qa-status.md` | Merge |
| QA_LEAD_SUMMARY.md | QA summary | `/docs/reports/2026-01-31-qa-status.md` | Merge |
| ASSESSMENT.md | Assessment | `/docs/reports/2026-01-31-assessment.md` | Timestamp |

#### Metadata/Index Files (MOVE TO `/docs/` or DELETE)
| File | Purpose | New Path | Action |
|------|---------|----------|--------|
| INDEX.md | Old index | DELETE | Replaced by `/docs/README.md` |
| DOCUMENTATION_INDEX.md | Doc navigation | `/docs/README.md` | Rename as main index |
| ANSWERS_INDEX.md | Q&A index | `/docs/learning/faq.md` | Extract into FAQ |
| README_REPORTS.md | Report index | `/docs/reports/README.md` | Move as report index |
| REPORT_INVENTORY.md | Report inventory | `/docs/reports/README.md` | Merge |
| ROLES.md | Team roles | `/docs/learning/roles.md` | Move |
| DEV_QUESTIONS.md | Questions | DELETE | Replaced by DEV_QUESTIONS_ANSWERS |
| DEV_QUESTIONS_ANSWERS.md | Q&A | `/docs/learning/faq.md` | Extract into FAQ |

#### Cleanup/Archive (DELETE or ARCHIVE)
| File | Purpose | Action | Reason |
|------|---------|--------|--------|
| DOCUMENTATION_RESTRUCTURING_*.md | All 9 restructuring docs | ARCHIVE | Clutter; preserve one summary |
| DOCUMENTATION_UPDATE_SUMMARY.md | Update metadata | DELETE | Metadata only |
| DRAFT.md | Old draft (1962 lines) | ARCHIVE | Too large, scattered |
| STATUS.md | Status marker | DELETE | Metadata; replaced by timestamped reports |
| SUMMARY.md | Summary | DELETE | Duplicate of EXECUTIVE_SUMMARY |
| COMPLETION.md | Completion marker (12 lines) | DELETE | Metadata |
| STATUS_VISUAL.txt | Duplicate | DELETE | Duplicate of .md file |

### B. Existing `/docs/` Folder Files (12 files)

These are **best practices guides** – already well-organized:

```
docs/
├── 10_MOST_IMPORTANT_GITHUB_REPO_FILES.md
├── AI_PROMPT_ENGINEERING_GUIDE.md
├── API_DESIGN_CONVENTIONS.md
├── CODE_COMMENTS_AND_DOCUMENTATION.md
├── CODE_REVIEW_BEST_PRACTICES.md
├── ERROR_HANDLING_AND_LOGGING.md
├── GIT_WORKFLOW_AND_VERSION_CONTROL.md
├── MARKDOWN_DOCUMENTATION_GUIDE.md
├── NAMING_CONVENTIONS_GUIDE.md
├── PROJECT_STRUCTURE_GUIDE.md
├── SECURITY_BEST_PRACTICES.md
└── TESTING_STRATEGY_AND_BEST_PRACTICES.md
```

**Plan**: Move to `/docs/guides/standards/` to keep them organized with other guides.

### C. Configuration Files (15 files in `config/agents/`)

**Status**: Already well-organized, no changes needed. Optional: add `config/agents/README.md` with index.

---

## Phase 2: Proposed Directory Structure

```
AIMeeting/
│
├── README.md                              ← GitHub entry point (updated with /docs link)
├── ARCHITECTURE.md                        ← Stable design reference
├── LICENSE
├── .gitignore
├── .editorconfig
│
├── docs/                                  ← Documentation hub
│   │
│   ├── README.md                          ← Audience-driven navigation hub
│   │
│   ├── reference/                         ← Core permanent references
│   │   ├── api.md                         (moved from root API.md)
│   │   ├── agent-configuration.md         (moved & renamed)
│   │   ├── extending.md                   (moved from root)
│   │   └── examples.md                    (moved from root)
│   │
│   ├── guides/                            ← How-to & best practices
│   │   ├── cli.md                         (renamed from CLI_QUICK_REFERENCE)
│   │   ├── roadmap.md                     (moved from root)
│   │   │
│   │   └── standards/                     ← Best practices & conventions
│   │       ├── coding-standards.md
│   │       ├── testing.md
│   │       ├── security.md
│   │       ├── error-handling.md
│   │       ├── naming-conventions.md
│   │       ├── api-design.md
│   │       ├── code-review.md
│   │       ├── git-workflow.md
│   │       ├── markdown.md
│   │       ├── ai-prompt-engineering.md
│   │       ├── github-repo-files.md
│   │       └── README.md (optional index)
│   │
│   ├── planning/                          ← Versioned planning (v0.1, v0.2, etc.)
│   │   ├── README.md                      (links to versions)
│   │   └── v0.1/
│   │       ├── plan.md                    (merged PLAN.md + PLAN-V0-1.md)
│   │       ├── deliverables.md
│   │       └── readiness.md
│   │
│   ├── reports/                           ← Timestamped status reports
│   │   ├── README.md                      (points to latest report)
│   │   ├── 2026-01-31/                    (date-stamped folder)
│   │   │   ├── executive-summary.md
│   │   │   ├── implementation-report.md
│   │   │   ├── test-report.md
│   │   │   ├── visual-overview.md
│   │   │   ├── qa-status.md               (merged QA files)
│   │   │   └── assessment.md
│   │   └── 2026-01-15/                    (older reports, optional)
│   │       └── ...
│   │
│   ├── learning/                          ← Onboarding & FAQ
│   │   ├── getting-started.md             (new, for first-timers)
│   │   ├── faq.md                         (merged Q&A)
│   │   ├── roles.md                       (team roles)
│   │   └── CONTRIBUTING.md                (contribution workflow, if needed)
│   │
│   └── archive/                           ← Deprecated/old docs
│       ├── README.md                      (explanation of what's here & why)
│       ├── draft.md                       (old DRAFT.md)
│       ├── restructuring-summary.md       (one summary of all restructuring docs)
│       └── ...
│
├── config/
│   └── agents/                            (unchanged)
│       ├── README.md                      (optional: agent index)
│       └── *.txt                          (15 agent configs)
│
├── src/
│   └── ... (source code, unchanged)
│
└── tests/
    └── ... (tests, unchanged)
```

---

## Phase 3: Move/Rename Table (To Be Executed)

| Current Path | New Path | Action | Reason |
|--------------|----------|--------|--------|
| README.md | README.md | UPDATE | Add /docs link |
| ARCHITECTURE.md | ARCHITECTURE.md | KEEP | GitHub standard |
| LICENSE | LICENSE | KEEP | Standard |
| .gitignore | .gitignore | KEEP | Standard |
| .editorconfig | .editorconfig | KEEP | Standard |
| API.md | docs/reference/api.md | MOVE | Core reference |
| EXTENDING.md | docs/reference/extending.md | MOVE | Extension reference |
| AGENT_CONFIGURATION_GUIDE.md | docs/reference/agent-configuration.md | MOVE | Config reference |
| EXAMPLES.md | docs/reference/examples.md | MOVE | Usage examples |
| CLI_QUICK_REFERENCE.md | docs/guides/cli.md | MOVE | CLI guide |
| ROADMAP.md | docs/guides/roadmap.md | MOVE | Planning guide |
| PLAN.md | docs/planning/v0.1/plan.md | MERGE | Combine with PLAN-V0-1 |
| PLAN-V0-1.md | docs/planning/v0.1/plan.md | MERGE | Merge with PLAN.md |
| DELIVERABLES.md | docs/planning/v0.1/deliverables.md | MOVE | Version-specific |
| READINESS.md | docs/planning/v0.1/readiness.md | MOVE | Version-specific |
| EXECUTIVE_SUMMARY.md | docs/reports/2026-01-31/executive-summary.md | MOVE | Timestamp |
| IMPLEMENTATION_REPORT.md | docs/reports/2026-01-31/implementation-report.md | MOVE | Timestamp |
| TEST_EXECUTION_REPORT.md | docs/reports/2026-01-31/test-report.md | MOVE | Timestamp |
| VISUAL_STATUS_OVERVIEW.md | docs/reports/2026-01-31/visual-overview.md | MOVE | Timestamp |
| ASSESSMENT.md | docs/reports/2026-01-31/assessment.md | MOVE | Timestamp |
| QA_COMPLETION_STATUS.md | docs/reports/2026-01-31/qa-status.md | MERGE | Consolidate QA |
| QA_LEAD_COMPLETION_REPORT.md | docs/reports/2026-01-31/qa-status.md | MERGE | Consolidate QA |
| QA_LEAD_SUMMARY.md | docs/reports/2026-01-31/qa-status.md | MERGE | Consolidate QA |
| DOCUMENTATION_INDEX.md | docs/README.md | RENAME | Main nav hub |
| README_REPORTS.md | docs/reports/README.md | MOVE | Report index |
| REPORT_INVENTORY.md | docs/reports/README.md | MERGE | Merge into report index |
| ROLES.md | docs/learning/roles.md | MOVE | Learning material |
| DEV_QUESTIONS_ANSWERS.md | docs/learning/faq.md | EXTRACT | Q&A extraction |
| 10_MOST_IMPORTANT_GITHUB_REPO_FILES.md | docs/guides/standards/github-repo-files.md | MOVE | Standards |
| AI_PROMPT_ENGINEERING_GUIDE.md | docs/guides/standards/ai-prompt-engineering.md | MOVE | Standards |
| API_DESIGN_CONVENTIONS.md | docs/guides/standards/api-design.md | MOVE | Standards |
| CODE_COMMENTS_AND_DOCUMENTATION.md | docs/guides/standards/documentation.md | MOVE | Standards |
| CODE_REVIEW_BEST_PRACTICES.md | docs/guides/standards/code-review.md | MOVE | Standards |
| ERROR_HANDLING_AND_LOGGING.md | docs/guides/standards/error-handling.md | MOVE | Standards |
| GIT_WORKFLOW_AND_VERSION_CONTROL.md | docs/guides/standards/git-workflow.md | MOVE | Standards |
| MARKDOWN_DOCUMENTATION_GUIDE.md | docs/guides/standards/markdown.md | MOVE | Standards |
| NAMING_CONVENTIONS_GUIDE.md | docs/guides/standards/naming-conventions.md | MOVE | Standards |
| PROJECT_STRUCTURE_GUIDE.md | docs/guides/standards/project-structure.md | MOVE | Standards |
| SECURITY_BEST_PRACTICES.md | docs/guides/standards/security.md | MOVE | Standards |
| TESTING_STRATEGY_AND_BEST_PRACTICES.md | docs/guides/standards/testing.md | MOVE | Standards |
| DOCUMENTATION_RESTRUCTURING_*.md (9 files) | docs/archive/restructuring-summary.md | ARCHIVE | Consolidate |
| DOCUMENTATION_UPDATE_SUMMARY.md | (delete) | DELETE | Metadata |
| DRAFT.md | docs/archive/draft.md | ARCHIVE | Historical preservation |
| STATUS.md | (delete) | DELETE | Metadata |
| SUMMARY.md | (delete) | DELETE | Duplicate |
| COMPLETION.md | (delete) | DELETE | Metadata |
| STATUS_VISUAL.txt | (delete) | DELETE | Duplicate |
| ANSWERS_INDEX.md | (delete) | DELETE | Merged into FAQ |
| INDEX.md | (delete) | DELETE | Replaced by /docs/README.md |
| DEV_QUESTIONS.md | (delete) | DELETE | Obsolete |

---

## Phase 4: Precedence Rules (Authority Order)

In case of conflicting information, this hierarchy applies:

1. **ARCHITECTURE.md** (at root) – System design authority
2. **docs/reference/** – API, configuration, extension specs
3. **docs/guides/standards/** – Coding, testing, security standards
4. **docs/planning/** – Feature roadmap and version plans
5. **docs/reports/** – Status snapshots (informational, may be outdated)
6. **docs/learning/** – Onboarding and FAQ (informational)

**Example**: If docs/guides/standards/naming-conventions.md contradicts a comment in code, follow ARCHITECTURE.md for truth. If ARCHITECTURE.md doesn't address it, docs/guides/standards/ applies.

---

## Phase 5: Link Updates Required

### Links to Update in Markdown Files

| Current Link | New Link | Files to Update |
|--------------|----------|-----------------|
| `[API.md](API.md)` | `[API Documentation](docs/reference/api.md)` | README.md, EXTENDING.md |
| `[EXTENDING.md](EXTENDING.md)` | `[Extension Guide](docs/reference/extending.md)` | README.md, API.md |
| `[AGENT_CONFIGURATION_GUIDE.md](...)` | `[Agent Configuration](docs/reference/agent-configuration.md)` | README.md, EXAMPLES.md |
| `[EXAMPLES.md](EXAMPLES.md)` | `[Usage Examples](docs/reference/examples.md)` | README.md, API.md |
| `[ROADMAP.md](ROADMAP.md)` | `[Product Roadmap](docs/guides/roadmap.md)` | README.md |
| `[CLI_QUICK_REFERENCE.md](...)` | `[CLI Guide](docs/guides/cli.md)` | README.md |
| `[DOCUMENTATION_INDEX.md](...)` | `[Documentation](docs/)` | README.md |

### Files with External Links (No Changes)

- GitHub URLs (github.com/...) – no changes
- External resources – no changes

---

## Validation Checklist

- [ ] All root reference docs moved to `/docs/reference/`
- [ ] All guides moved to `/docs/guides/` and `/docs/guides/standards/`
- [ ] All planning docs moved to `/docs/planning/v0.1/` with new names
- [ ] All status reports moved to `/docs/reports/2026-01-31/` with timestamps
- [ ] QA files merged into single `/docs/reports/2026-01-31/qa-status.md`
- [ ] FAQ extracted from Q&A files into `/docs/learning/faq.md`
- [ ] Archive folder created with old/deprecated docs
- [ ] Root `/docs/README.md` created with audience-driven navigation
- [ ] Root README.md updated with link to `/docs/`
- [ ] All relative links in Markdown files updated
- [ ] No broken links remain
- [ ] Root directory has only 8 core files
- [ ] `/docs/` has clear folder hierarchy
- [ ] ARCHITECTURE.md remains at root as-is

---

## Benefits

### For New Contributors
- ✅ Clear entry point via root `/docs/README.md`
- ✅ `/docs/learning/getting-started.md` guide
- ✅ `/docs/learning/faq.md` with common questions
- ✅ `/docs/guides/standards/` for best practices

### For Developers
- ✅ `/docs/reference/` has all API & config docs
- ✅ `/docs/guides/` has CLI, examples, roadmap
- ✅ `/docs/guides/standards/` has coding standards
- ✅ Faster discovery (3-5 files instead of 60)

### For Maintainers
- ✅ Timestamped reports in `/docs/reports/2026-01-31/` prevent confusion
- ✅ `/docs/planning/v0.1/` contains all v0.1 planning in one place
- ✅ Archive preserves history but doesn't clutter root
- ✅ Clear precedence rules for conflicting docs

### For Agents / AI
- ✅ `/docs/reference/api.md` clearly explains interfaces
- ✅ `/docs/reference/extending.md` has code patterns
- ✅ `/docs/guides/standards/` has conventions
- ✅ Each guide has consistent formatting

### For Repository
- ✅ Root directory reduced by 75% (59 → ~8 files)
- ✅ Follows GitHub conventions (README.md, ARCHITECTURE.md at root)
- ✅ Scalable for future versions (v0.2/, v0.3/ folders)
- ✅ Timestamped reports create audit trail

---

## Risk Mitigation

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Broken links after move | Medium | Validate all links after moving (see Phase 6 below) |
| Contributors miss docs | Medium | Add prominent `📚 Documentation` section to root README.md |
| Git blame harder to follow | Low | Use `git log --follow` to track renames |
| Too many nested folders | Low | Keep max 3 levels deep: docs/ → category/ → file.md |
| Old URLs bookmarked | Low | Add anchor redirects or update CI/CD if docs are published |

---

## Approval Request

**Before proceeding to implementation, please confirm:**

1. ✅ Directory structure makes sense?
2. ✅ Move/rename table complete?
3. ✅ Precedence rules clear?
4. ✅ Ready to delete files listed in "DELETE" action?
5. ✅ Any concerns about the plan?

**Once approved, I will:**
1. Execute all file moves/renames
2. Update all Markdown links
3. Create navigation hubs (/docs/README.md, etc.)
4. Create archive summary
5. Verify all links work
6. Generate final DOCS-RESTRUCTURE-REPORT.md

---

**Proposed by**: Documentation Restructuring Agent  
**Date**: January 31, 2026  
**Status**: Awaiting Approval
