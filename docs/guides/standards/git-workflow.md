# Git Workflow and Version Control Guide

A comprehensive guide to understanding Git workflows, version control strategies, branching models, commit conventions, and collaboration patterns. This guide explains different approaches, their use cases, trade-offs, and best practices for teams of all sizes.

---

## Why Git Workflow Matters

### Impact on Development
- **Team Coordination**: Clear workflow prevents conflicts and confusion
- **Release Management**: Predictable process for deploying code
- **History**: Clean commits enable easy debugging and auditing
- **Scalability**: Workflow scales with team size
- **Collaboration**: Multiple developers work efficiently
- **Rollback**: Easy to revert problematic changes

### Common Problems from Bad Workflows
- Untracked changes in production
- Difficulty identifying which commit broke things
- Long-running branches with massive conflicts
- Unclear what's deployed where
- Merge nightmares
- Loss of code or commits

---

## Core Git Concepts

### **Branches**
Independent lines of development. Allow parallel work without affecting others.

```
main
├── feature-1
├── feature-2
└── hotfix

```

### **Commits**
Snapshots of code at a point in time with a message describing changes.

```
Good: "Add user authentication with JWT tokens"
Bad:  "Fix stuff", "wip", "asdf"
```

### **Tags**
Named references to specific commits, usually for releases.

```
v1.0.0 → commit abc123
v1.1.0 → commit def456
```

### **Remotes**
Versions of the repository on other servers (GitHub, GitLab, etc.).

```
origin   → your remote repository
upstream → original repository (if forked)
```

---

## Git Branching Strategies

### **Strategy 1: Git Flow**

A complete branching model with dedicated branches for features, releases, and hotfixes.

#### Structure
```
main (production-ready code)
├── develop (integration branch)
│   ├── feature/user-authentication
│   ├── feature/payment-integration
│   └── release/v1.2.0
└── hotfix/security-patch
```

#### Branches
- **main**: Production code only, tagged with version numbers
- **develop**: Integration branch, staging environment
- **feature/**: Individual features (branch from develop)
- **release/**: Release preparation (branch from develop)
- **hotfix/**: Critical fixes for production (branch from main)

#### Workflow
```
1. Create feature branch from develop
   git checkout develop
   git checkout -b feature/login-system

2. Work on feature (multiple commits)
   git commit -m "Add login validation"
   git commit -m "Add password reset"

3. Create Pull Request to develop
   (Code review and discussion)

4. Merge to develop
   git checkout develop
   git merge feature/login-system

5. When ready for release, create release branch
   git checkout -b release/v1.2.0

6. Fix release issues, update version numbers
   git commit -m "Bump version to 1.2.0"

7. Merge to main and tag
   git checkout main
   git merge release/v1.2.0
   git tag -a v1.2.0

8. Merge back to develop
   git checkout develop
   git merge release/v1.2.0
```

#### Use Cases
- **Large teams** (10+ developers)
- **Multiple environments** (dev, staging, production)
- **Scheduled releases** (planned release dates)
- **Long development cycles**
- **Enterprise projects**

#### Advantages
- Clear separation of concerns
- Supports multiple parallel development efforts
- Well-defined release process
- Hotfixes don't disrupt development
- Good for managing versions

#### Disadvantages
- Complex for small teams
- Many branches to manage
- Steeper learning curve
- Overhead for continuous deployment
- Risk of develop branch becoming unstable

#### Tools & Implementations
- **git-flow**: Extension that automates Git Flow
- **Gitflow AVH Edition**: Community-maintained version
- **GitHub/GitLab**: Native branch protection

---

### **Strategy 2: GitHub Flow**

Simplified, continuous deployment-friendly workflow with feature branches and main branch.

#### Structure
```
main (always deployable)
├── feature/dark-mode
├── feature/user-profiles
├── feature/api-pagination
└── fix/database-migration
```

#### Branches
- **main**: Production code, always deployable
- **feature/***: New features and fixes (branch from main)
- Typically short-lived (1-3 days)

#### Workflow
```
1. Create descriptive feature branch from main
   git checkout main
   git pull origin main
   git checkout -b feature/dark-mode

2. Make commits with clear messages
   git commit -m "Add dark mode toggle to settings"
   git commit -m "Update CSS variables for dark theme"
   git commit -m "Add dark mode preference to user profile"

3. Push branch and create Pull Request
   git push origin feature/dark-mode

4. Discuss, review, and test in PR
   (Team reviews, CI/CD tests automatically)

5. Merge when ready
   git checkout main
   git merge feature/dark-mode
   git push origin main

6. Delete feature branch
   git branch -d feature/dark-mode
   git push origin --delete feature/dark-mode

7. Deploy to production (usually automatic with CI/CD)
```

#### Use Cases
- **Small to medium teams**
- **Continuous deployment/SaaS products**
- **Fast iteration cycles**
- **Web applications**
- **Simple codebases**
- **Startups**

#### Advantages
- Simple and easy to understand
- Ideal for continuous deployment
- Short-lived branches reduce conflicts
- Encourages frequent integration
- Fast feedback loops
- Low overhead

#### Disadvantages
- No staging environment concept
- Doesn't handle multiple production versions well
- Requires strong CI/CD pipeline
- All code goes directly to production
- Not ideal for scheduled releases

#### Tools & Implementations
- **GitHub**: Native support (default for new projects)
- **GitHub Actions**: Built-in CI/CD
- **Netlify/Vercel**: Automatic deployments from branches

---

### **Strategy 3: Trunk-Based Development**

Everyone works on a single main branch with short-lived feature branches (< 1 day).

#### Structure
```
main (always releasable)
├── feature-1 (1-2 hours)
├── feature-2 (3-4 hours)
└── feature-3 (1 day)
```

#### Branches
- **main**: Single source of truth, always deployable
- **feature/***: Very short-lived (hours, not days)
- Minimal branching, maximum integration

#### Workflow
```
1. Start small feature from main
   git checkout main
   git pull origin main
   git checkout -b feature/add-validation

2. Commit frequently (30 minutes max)
   git commit -m "Add email validation"
   git push origin feature/add-validation

3. Create PR immediately
   (Even before finished, mark as draft if needed)

4. Continuous integration and review
   (Tests run automatically)

5. Merge quickly (within hours)
   git checkout main
   git merge feature/add-validation
   git push origin main

6. Delete branch
   git branch -d feature/add-validation

7. Deploy continuously or at scheduled intervals
```

#### Use Cases
- **High-velocity teams** (10-50+ developers)
- **Monorepo structures**
- **Extreme programming teams**
- **Continuous deployment environments**
- **Projects needing frequent releases**

#### Advantages
- Minimizes merge conflicts
- Forces small, reviewable changes
- Encourages continuous integration
- Scales well with large teams
- Fastest feedback loops
- Simplest branch structure

#### Disadvantages
- Requires excellent CI/CD infrastructure
- Demands discipline and team buy-in
- Hard for teams with slow review cycles
- Difficult with long-running features
- Not ideal for multiple versions in production

#### Requirements
- Strong automated testing
- Fast CI/CD pipeline
- Experienced team
- Feature flags for incomplete features

---

### **Strategy 4: Environment Branches**

Branches mirror your environments (dev, staging, production).

#### Structure
```
production (live code)
staging (testing environment)
development (dev environment)
└── feature/new-feature (merged when ready)
```

#### Workflow
```
1. Create feature from development
   git checkout development
   git checkout -b feature/api-pagination

2. Merge to development when ready
   git checkout development
   git merge feature/api-pagination

3. Test in development environment
   (Automatic deployment to dev server)

4. When stable, merge to staging
   git checkout staging
   git merge feature/api-pagination

5. Test in staging environment
   (Automatic deployment to staging server)

6. When approved, merge to production
   git checkout production
   git merge feature/api-pagination
   git tag v1.5.0

7. Deploy to production
   (Automatic or manual deployment)
```

#### Use Cases
- **Organizations with strict environments**
- **Regulated industries** (banking, healthcare)
- **Projects requiring thorough testing**
- **Multiple approval stages**
- **Traditional enterprise**

#### Advantages
- Clear separation of environments
- Thorough testing before production
- Good audit trail
- Multiple approval stages possible
- Matches organizational structure

#### Disadvantages
- Complexity managing multiple branches
- Merging can be error-prone
- Slow time to production
- Merge conflicts between environment branches
- Not ideal for frequent releases

---

### **Strategy 5: Release Branches**

Focus on stable main branch with release branches for version management.

#### Structure
```
main (development)
├── 1.0.x (maintenance branch for 1.0)
├── 1.1.x (maintenance branch for 1.1)
└── 2.0.x (maintenance branch for 2.0)
```

#### Workflow
```
1. Develop on main normally
   git checkout main
   git checkout -b feature/new-feature

2. When ready to release, create release branch
   git checkout main
   git checkout -b 1.2.x

3. On 1.2.x, only bug fixes and version bumps
   git commit -m "Fix critical bug in payment processing"
   git tag v1.2.1

4. Backport critical fixes to main if needed
   git cherry-pick <commit-hash>

5. Main continues with 1.3.0 development
   git checkout main
   git commit -m "Add new feature for 1.3.0"
```

#### Use Cases
- **Libraries and frameworks**
- **Projects with long-term version support**
- **Software with maintenance versions**
- **Open source projects**

#### Advantages
- Support multiple versions simultaneously
- Long-term maintenance capability
- Clear version tracking
- Easy backporting
- Good for libraries

#### Disadvantages
- Complexity with many branches
- Backporting burden
- Release branch management overhead
- Not ideal for simple projects

---

## Commit Message Conventions

### **Conventional Commits**

Standard format: `type(scope): subject`

#### Types
- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style (formatting, missing semicolons)
- **refactor**: Code refactoring without feature/fix
- **perf**: Performance improvements
- **test**: Adding or updating tests
- **chore**: Dependency updates, tool changes
- **ci**: CI/CD changes
- **revert**: Reverting a previous commit

#### Format
```
<type>(<scope>): <subject>
<blank line>
<body>
<blank line>
<footer>
```

#### Examples
```
feat(auth): add JWT token refresh mechanism

- Implement automatic token refresh
- Add token expiration validation
- Update authentication middleware

Fixes #123
```

```
fix(database): resolve connection pool leak

The database connection pool was not properly
releasing idle connections, causing exhaustion
under sustained load.

Closes #456
```

```
docs: update API documentation for v2.0

- Add new endpoints
- Update examples
- Fix typos in error responses
```

#### Benefits
- Automated changelog generation
- Clear commit history
- Easier to find related commits
- CI/CD can parse commits (trigger actions)
- Better for blame and bisect

### **Alternative: Angular Commit Format**

Similar to Conventional Commits, widely used:

```
type(scope): subject line
body
footer
```

### **Simple Format (For Small Teams)**

```
category: brief description

More detailed explanation if needed.

Related issues: #123, #456
```

#### Examples
```
Backend: Add caching layer for user queries

Implements Redis caching for frequently
accessed user data to improve response times.

Performance improvement: ~40% faster for cached queries
```

```
Frontend: Fix responsive layout on mobile

CSS media queries were missing for screens < 480px.
Added proper breakpoints and tested on various devices.
```

### **Best Practices**

- **Use present tense**: "Add feature" not "Added feature"
- **Keep subject < 50 characters**
- **Capitalize first letter**
- **No period at end of subject**
- **Body wraps at 72 characters**
- **One logical change per commit**
- **Reference issues**: "Fixes #123"
- **Explain why, not what**: "The code explains what, commit should explain why"

### **Anti-Patterns to Avoid**

```
❌ "Fix bug"
✅ "Fix authentication bug in login endpoint"

❌ "WIP", "asdf", "temp"
✅ "Work in progress: implement payment processor"

❌ "Updated files"
✅ "Refactor: extract utility functions into separate module"

❌ "Fixed everything"
✅ Break into separate commits:
   - Fix: validation logic
   - Add: error handling
   - Docs: update API docs
```

---

## Pull Request Best Practices

### **PR Title**

Clear, descriptive title following commit conventions:

```
✅ feat(auth): implement OAuth2 integration
✅ fix(api): resolve race condition in concurrent requests
✅ docs: update setup instructions for Windows

❌ Fix stuff
❌ Update
❌ WIP
```

### **PR Description Template**

```markdown
## Description
Brief description of changes and why they're needed.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## How to Test
Steps to reproduce/test the changes:
1. ...
2. ...
3. ...

## Screenshots (if applicable)

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No breaking changes (or documented)

## Related Issues
Fixes #123
Related to #456
```

### **PR Size**

**Keep PRs small and focused:**

```
Good:
- Single feature
- Single bug fix
- Refactoring of one module
- Size: 200-400 lines changed

Bad:
- Multiple unrelated features
- Large refactoring + features
- Size: 2000+ lines changed
```

### **Review Expectations**

- **Simple PRs** (< 100 lines): 1-2 hour turnaround
- **Medium PRs** (100-500 lines): 1-2 day turnaround
- **Large PRs** (> 500 lines): 2-3 day turnaround
- **Complex PRs**: Review in parts

### **Merge Strategies**

#### **1. Squash and Merge** (Recommended for most)

All commits squashed into one:
```
Before: 10 commits (fix: typo, fix: linting, feat: feature)
After:  1 commit (feat: implement feature)
```

**When to use:**
- Feature branches with work-in-progress commits
- Keeping main history clean
- Feature-based development

**When not to use:**
- Need to preserve individual commits
- Using Conventional Commits for changelog

#### **2. Create a Merge Commit** (For complex histories)

Preserves all commits with merge commit:
```
Before: Main + 5 commits on feature branch
After:  All 5 commits merged with merge commit
```

**When to use:**
- Want to preserve feature branch history
- Complex features with logical steps
- Want to know which commits are in feature

**When not to use:**
- Want clean linear history
- Have many work-in-progress commits

#### **3. Rebase and Merge** (Advanced)

Replays commits on main, no merge commit:
```
Before: Main (A, B) + Feature (C, D, E)
After:  Main (A, B, C, D, E) linear history
```

**When to use:**
- Want linear history
- Clean commit history important
- Using Conventional Commits

**When not to use:**
- Collaborative feature branches
- Already pushed to shared branch
- Team not familiar with rebase

---

## Tagging and Versioning

### **Semantic Versioning (SemVer)**

Format: `MAJOR.MINOR.PATCH`

```
v1.2.3
├── 1 = MAJOR (breaking changes)
├── 2 = MINOR (new features, backward compatible)
└── 3 = PATCH (bug fixes)
```

#### Examples
```
1.0.0 → Initial release
1.1.0 → Added new feature
1.1.1 → Bug fix
2.0.0 → Breaking changes
2.0.1 → Bug fix in 2.0
2.1.0 → New feature in 2.x
```

#### Rules
- MAJOR: 0+ increments, reset MINOR and PATCH
- MINOR: 0+ increments, reset PATCH
- PATCH: 0+ increments

### **Pre-release and Build Metadata**

```
1.0.0-alpha           # Pre-release
1.0.0-beta.1          # Beta version 1
1.0.0-rc.1            # Release candidate
1.0.0+build.123       # Build metadata
```

### **Git Tagging**

```bash
# Lightweight tag (just a pointer)
git tag v1.2.3

# Annotated tag (full object with metadata)
git tag -a v1.2.3 -m "Release version 1.2.3"

# Tag specific commit
git tag v1.2.3 abc123

# Push tags
git push origin v1.2.3      # Single tag
git push origin --tags      # All tags

# Delete tag
git tag -d v1.2.3           # Local
git push origin --delete v1.2.3  # Remote
```

### **Calendar Versioning**

Alternative for time-based projects:

```
2025.01.29      (year.month.day)
2025.01         (year.month)
```

---

## Collaboration Patterns

### **Centralied Workflow**

Everyone pushes to main branch:

```
Developer 1 ← → Repository ← → Developer 2
Developer 3 → ↗              ↖ ← Developer 4
```

**Use for:**
- Small teams (2-3 people)
- Single feature development
- Learning Git

**Issues:**
- Conflicts common
- No code review
- Hard to coordinate
- No branching for safety

### **Feature Branch Workflow**

Everyone creates feature branches, review before merge:

```
Developer 1 (feature-1) ┐
Developer 2 (feature-2) ├─→ Main Repository ← Pull Requests
Developer 3 (feature-3) ┘     ↓ (Review & Test)
```

**Use for:**
- Most teams
- Code review important
- Multiple concurrent features
- Any significant project

### **Forking Workflow**

Each developer has personal fork, contributions via PR:

```
Original Repository ← Pull Request ← Developer Fork 1
                    ← Pull Request ← Developer Fork 2
                    ← Pull Request ← Developer Fork 3
```

**Use for:**
- Open source projects
- External contributors
- Distributed teams
- Large communities

### **Gitflow Workflow** (Covered in branching strategies)

---

## Remote Management

### **Adding Remotes**

```bash
# View remotes
git remote -v

# Add remote
git remote add upstream https://github.com/original/repo.git

# Remove remote
git remote remove upstream

# Rename remote
git remote rename origin destination
```

### **Typical Remote Setup**

```bash
# Forked project setup
git remote add origin https://github.com/myusername/project.git
git remote add upstream https://github.com/original/project.git

# Sync with original
git fetch upstream
git rebase upstream/main
git push origin main
```

### **Multiple Remotes**

```bash
# Work with multiple repositories
git remote add backup https://github.com/backup/repo.git
git push origin main
git push backup main

# Fetch from all
git fetch --all
```

---

## Merge Conflicts Resolution

### **Understanding Conflicts**

When the same lines changed differently:

```
<<<<<<< HEAD (your changes)
Display name
=======
Full name
>>>>>>> feature/profile-update (incoming changes)
```

### **Resolving Conflicts**

**Manual Resolution:**
```
1. Open conflicted file
2. Find conflict markers (<<<, ===, >>>)
3. Choose version(s) to keep
4. Delete conflict markers
5. Save file
6. Stage and commit
```

**Using Tools:**
```bash
# Abort merge
git merge --abort

# Use merge tool
git mergetool

# Accept current branch
git checkout --ours file.txt

# Accept incoming branch
git checkout --theirs file.txt
```

### **Preventing Conflicts**

- Keep branches short-lived
- Avoid long-running branches
- Frequent merges/rebases
- Communicate about shared code
- Use feature flags for long features

---

## Undoing Changes

### **Before Committing**

```bash
# View unstaged changes
git diff

# View staged changes
git diff --staged

# Unstage file
git reset HEAD file.txt

# Discard changes in working directory
git checkout -- file.txt
git restore file.txt (newer Git)

# Discard all changes
git reset --hard HEAD
```

### **After Committing**

```bash
# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1

# Amend last commit
git commit --amend

# Create new commit that undoes changes
git revert abc123
```

### **Recovering Deleted Commits**

```bash
# Find deleted commits
git reflog

# Restore branch
git checkout -b restored <commit-hash>
```

---

## Best Practices

### ✅ DO

- **Use branches** for all work - Direct commits to main are blocked by branch protection
- **Create Pull Requests** - Required for all changes to main (1 approval needed)
- **Wait for CI checks** - All tests must pass on Ubuntu, Windows, and macOS
- **Keep branch up-to-date** - Sync with main before merging (enforced)
- **Resolve conversations** - Address all review comments (enforced)
- **Write clear commit messages** explaining why, not what
- **Keep commits small** - one logical change per commit
- **Pull before push** - `git pull origin main` before starting work
- **Test before merging** - run tests locally with `$env:AIMEETING_AGENT_MODE="stub"; dotnet test`
- **Keep history clean** - use squash for work-in-progress commits
- **Tag releases** - use semantic versioning
- **Document process** - CONTRIBUTING.md explains workflow
- **Communicate** - discuss major changes with team

### ❌ DON'T

- **Push directly to main** - Blocked by branch protection (even for admins)
- **Merge without approval** - At least one review required (enforced)
- **Skip CI checks** - All platforms must pass (enforced)
- **Ignore review comments** - Conversations must be resolved (enforced)
- **Write vague commits** - "Fix", "Update", "WIP" unhelpful
- **Large commits** - hard to review, hard to debug (keep under 500 lines)
- **Force push to shared branches** - `git push --force` dangers
- **Commit secrets** - use .gitignore and environment variables
- **Long-lived branches** - merge within days, not weeks
- **Duplicate work** - communicate about who's doing what
- **Rewrite history** - don't rebase pushed commits

---

## Git Workflow Decision Guide

```
CHOOSE YOUR WORKFLOW
====================

1. Team size?
   ├─ 1-2 people → Centralized or simple feature branch
   ├─ 3-10 people → GitHub Flow or simple Git Flow
   └─ 10+ people → Trunk-based, Git Flow, or environment branches

2. Release frequency?
   ├─ Multiple times per day → Trunk-based development
   ├─ Daily/weekly → GitHub Flow
   ├─ Monthly/quarterly → Git Flow
   └─ Multiple versions supported → Release branches

3. Environment complexity?
   ├─ Single environment → GitHub Flow
   ├─ Dev + Production → Environment branches
   └─ Dev + Staging + Production → Full Git Flow

4. Open source project?
   ├─ Yes → Forking workflow + GitHub Flow
   └─ No → Feature branch + Git Flow or GitHub Flow

5. Project maturity?
   ├─ Early stage → GitHub Flow (simplicity)
   ├─ Growing → Git Flow (organization)
   └─ Mature → Trunk-based or hybrid (efficiency)
```

---

## Workflow Comparison Table

| Aspect | Centralized | Feature Branch | GitHub Flow | Trunk-Based | Git Flow |
|--------|------------|-----------------|-------------|------------|----------|
| **Team Size** | 1-2 | 3-5 | 3-10 | 10+ | 5-20 |
| **Complexity** | Very Low | Low | Low | Medium | High |
| **Branch Count** | 1 | Few | Few | 1-2 | Many |
| **Release Cycle** | Continuous | Continuous | Continuous | Continuous | Scheduled |
| **Conflict Risk** | High | Medium | Medium | Low | Medium |
| **Learning Curve** | Flat | Gentle | Gentle | Steep | Steep |
| **CI/CD Required** | No | Optional | Yes | Essential | Optional |
| **Scaling** | Poor | Good | Excellent | Excellent | Good |

---

## Implementation Checklist

- [ ] Team agrees on workflow
- [ ] Document in CONTRIBUTING.md
- [ ] Set branch protection rules
- [ ] Establish commit message conventions
- [ ] Setup CI/CD pipeline
- [ ] Define PR review process
- [ ] Decide on merge strategy
- [ ] Setup automated checks (linting, tests)
- [ ] Tag releases
- [ ] Create branching diagram for documentation
- [ ] Train team members
- [ ] Enforce via hooks or CI

---

## Tools and Automation

### **Git Hooks (Prevent Bad Commits)**

Pre-commit hook:
```bash
#!/bin/bash
# Check for large files
git diff --cached --diff-filter=d | grep "^Binary" && \
  echo "Error: Binary files detected" && exit 1

# Lint staged files
npm run lint --staged
```

### **CI/CD Integration**

```yaml
# GitHub Actions example
on: [pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - run: npm install
      - run: npm test
```

### **Commit Linting**

```json
// commitlint.config.js
module.exports = {
  extends: ['@commitlint/config-conventional']
};
```

### **Branch Protection Rules**

- Require PR reviews
- Dismiss stale PR approvals
- Require status checks to pass
- Require linear history
- Automatically delete head branches
- Require updated branches before merge

---

## References

- [Git Documentation](https://git-scm.com/doc)
- [GitHub Flow Guide](https://guides.github.com/introduction/flow/)
- [Atlassian Git Tutorials](https://www.atlassian.com/git/tutorials)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Git Flow Cheatsheet](https://danielkummer.github.io/git-flow-cheatsheet/)
- [Pro Git Book](https://git-scm.com/book/en/v2)
- [GitHub Best Practices](https://docs.github.com/en/github/administering-a-repository)

---

## Quick Reference

```
COMMON COMMANDS
===============

Setup
git clone <url>
git config user.name "Name"
git config user.email "email@example.com"

Branching
git branch                    # List branches
git branch feature/new        # Create branch
git checkout feature/new      # Switch branch
git checkout -b feature/new   # Create and switch

Commits
git add file.txt              # Stage file
git add .                     # Stage all
git commit -m "message"       # Commit
git push origin main          # Push to remote

Updates
git pull origin main          # Fetch and merge
git fetch origin              # Fetch only
git merge feature/new         # Merge branch

Undoing
git revert abc123             # Undo commit
git reset --soft HEAD~1       # Undo, keep changes
git reset --hard HEAD~1       # Undo, lose changes

Tags
git tag v1.0.0                # Create tag
git push origin v1.0.0        # Push tag
git tag -l                    # List tags
```
