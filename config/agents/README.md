# Agent Configuration Summary

This document lists all agent configurations created based on the roles defined in ROLES.md, following the AI Prompt Engineering Guide principles.

## Created Agent Configurations

### v0.1 Sample Configurations

These configurations align with the v0.1 CLI examples and documentation.

- **project-manager.txt**
   - Role: Project Manager
   - Focus: Scope, timeline, stakeholder alignment

- **senior-developer.txt**
   - Role: Senior Developer
   - Focus: Implementation feasibility, technical risks

- **security-expert.txt**
   - Role: Security Expert
   - Focus: Threat modeling and security risks

- **moderator.txt**
   - Role: Moderator
   - Focus: Facilitation, summaries, decision tracking

### Executive Leadership

1. **product-strategist.txt**
   - Role: Product Strategist (formerly Product Manager/Product Owner)
   - Focus: Product scope, feature prioritization, business decisions
   - Key Strengths: Strategic thinking, stakeholder management, scope control
   - Expertise: Product vision, requirements gathering, prioritization

2. **delivery-coordinator.txt**
   - Role: Delivery Coordinator (formerly Project Manager/Scrum Master)
   - Focus: Team coordination, timeline tracking, blocker removal
   - Key Strengths: Process facilitation, risk management, communication
   - Expertise: Agile/Scrum, timeline estimation, stakeholder communication

### Technical Leadership

3. **solutions-architect.txt**
   - Role: Solutions Architect (formerly Technical Lead/Architecture Owner)
   - Focus: System architecture, technical decisions, code review
   - Key Strengths: System design, mentoring, quality standards
   - Expertise: C# and .NET 8, event-driven architecture, SOLID principles

### Development Team

4. **principal-engineer.txt**
   - Role: Principal Engineer (formerly Senior Developer/Principal Engineer)
   - Focus: Core component implementation, code quality, mentoring
   - Key Strengths: TDD, complex problem-solving, technical leadership
   - Expertise: Advanced C#, .NET 8, event-driven patterns, testing

5. **software-craftsperson.txt**
   - Role: Software Craftsperson (formerly Developer/Software Engineer)
   - Focus: Feature implementation, testing, pattern following
   - Key Strengths: Collaboration, learning, implementation quality
   - Expertise: C# fundamentals, .NET 8, unit testing, Git workflow

### Quality & Testing

6. **quality-architect.txt**
   - Role: Quality Architect (formerly QA Lead/Test Architect)
   - Focus: Testing strategy, test fixtures, quality standards
   - Key Strengths: Risk assessment, framework expertise, metrics tracking
   - Expertise: xUnit, Moq, test strategy, acceptance criteria

7. **test-engineer.txt**
   - Role: Test Engineer (formerly QA Engineer/Tester)
   - Focus: Test execution, quality validation, defect reporting
   - Key Strengths: Detail orientation, thoroughness, edge case testing
   - Expertise: Test execution, defect reporting, exploratory testing

### Infrastructure & Operations

8. **platform-engineer.txt**
   - Role: Platform Engineer (formerly DevOps Engineer/Infrastructure Owner)
   - Focus: CI/CD setup, infrastructure management, deployment automation
   - Key Strengths: Automation, monitoring, reliability
   - Expertise: GitHub Actions, .NET build, logging, deployment

9. **reliability-specialist.txt**
   - Role: Reliability Specialist (formerly SRE/Site Reliability Engineer)
   - Focus: Production monitoring, incident response, performance
   - Key Strengths: Operations expertise, troubleshooting, post-mortems
   - Expertise: Monitoring, incident response, performance optimization

### Security & Compliance

10. **security-guardian.txt**
    - Role: Security Guardian (formerly Security Lead/Security Officer)
    - Focus: Security design review, threat modeling, secure coding
    - Key Strengths: Threat analysis, vulnerability assessment, risk mitigation
    - Expertise: Application security, OWASP, threat modeling, compliance

11. **compliance-advisor.txt**
    - Role: Compliance Advisor (formerly Compliance Officer)
    - Focus: Licensing, legal requirements, regulatory compliance
    - Key Strengths: Legal knowledge, license compatibility, risk identification
    - Expertise: Software licensing, GDPR, HIPAA, intellectual property

### Documentation & Support

12. **documentation-specialist.txt**
    - Role: Documentation Specialist (formerly Technical Writer/Documentation Lead)
    - Focus: User documentation, API docs, troubleshooting guides
    - Key Strengths: Clear communication, user focus, technical translation
    - Expertise: Technical writing, API documentation, Markdown

13. **experience-designer.txt**
    - Role: Experience Designer (formerly UX Designer/UX Lead)
    - Focus: CLI user experience, error messages, help text
    - Key Strengths: User research, interaction design, usability testing
    - Expertise: CLI UX design, error message design, user research

### Special Meeting Agents

14. **meeting-facilitator.txt**
    - Role: Meeting Facilitator
    - Focus: Meeting orchestration, discussion flow, decision facilitation
    - Key Strengths: Neutral facilitation, time management, consensus building
    - Expertise: Meeting facilitation, conflict resolution, decision making
    - Special: Can signal when meetings should conclude

15. **meeting-scribe.txt**
    - Role: Meeting Scribe
    - Focus: Transcript maintenance, decision capture, action item tracking
    - Key Strengths: Detail capture, objective recording, organization
    - Expertise: Note-taking, documentation, information organization
    - Special: Creates meeting artifacts (transcript, decisions, action items)

## Agent Naming Conventions

All agent names follow a consistent pattern:
- Descriptive role-based names (not generic titles)
- Hyphenated lowercase format for file names
- Clear indication of primary function
- Professional and specific (not vague)

## Prompt Engineering Principles Applied

Each agent configuration follows the AI_PROMPT_ENGINEERING_GUIDE.md principles:

1. **Clear Role Definition**: Specific role name and primary responsibility
2. **Detailed Persona**: Characteristics, mindset, and approach
3. **Explicit Instructions**: Concrete, actionable guidance on what to do
4. **Response Style Guidelines**: How to communicate and structure responses
5. **Length Constraints**: Max message length to keep contributions focused
6. **Expertise Areas**: Specific domains of knowledge and skill
7. **Communication Approach**: Structured guidance on how to interact

## Usage

To use these agents in a meeting:

```bash
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Your meeting topic" \
    --agents "config/agents/product-strategist.txt" \
             "config/agents/solutions-architect.txt" \
             "config/agents/principal-engineer.txt" \
             "config/agents/meeting-facilitator.txt" \
    --max-duration 30 \
    --max-messages 50
```

## Recommended Meeting Compositions

### Architecture Review Meeting
- solutions-architect.txt
- principal-engineer.txt
- security-guardian.txt
- meeting-facilitator.txt

### Sprint Planning Meeting
- product-strategist.txt
- delivery-coordinator.txt
- solutions-architect.txt
- principal-engineer.txt
- meeting-facilitator.txt

### Technical Design Discussion
- solutions-architect.txt
- principal-engineer.txt
- software-craftsperson.txt
- quality-architect.txt
- meeting-facilitator.txt
- meeting-scribe.txt

### Security Review Meeting
- security-guardian.txt
- solutions-architect.txt
- compliance-advisor.txt
- meeting-facilitator.txt

### Release Planning Meeting
- product-strategist.txt
- delivery-coordinator.txt
- platform-engineer.txt
- quality-architect.txt
- meeting-facilitator.txt

## Notes

- All agents are designed to work together in multi-agent meetings
- The meeting-facilitator should typically be included in every meeting
- The meeting-scribe is optional but recommended for important meetings
- Agents can be mixed and matched based on meeting needs
- Each agent has specific expertise but can contribute to broader discussions

---

**Created**: January 30, 2026  
**Based on**: ROLES.md and AI_PROMPT_ENGINEERING_GUIDE.md  
**Total Agents**: 15 unique role configurations
