# Agent Configuration Guide

## Overview

AIMeeting agents are configured via simple text files, requiring no code changes. This guide explains how to create and customize agent configurations for your specific use cases.

## Configuration File Format

Agent configuration files are plain text files (`.txt`) located in the `config/agents/` directory. Each file defines a single agent role with its characteristics and behavior.

### Basic Structure

```
ROLE: [Agent Role Name]
DESCRIPTION: [What this agent does]

PERSONA:
- [Characteristic 1]
- [Characteristic 2]
- [Characteristic 3]

INSTRUCTIONS:
- [Instruction 1]
- [Instruction 2]
- [Instruction 3]

INITIAL_MESSAGE_TEMPLATE: [Template for agent's first message]
RESPONSE_STYLE: [Communication style description]
MAX_MESSAGE_LENGTH: [Integer - maximum characters per response]
EXPERTISE_AREAS: [Comma-separated list of expertise areas]
```

## Field Reference

### ROLE (Required)

The agent's professional title or role in the meeting.

**Examples**:
```
ROLE: Project Manager
ROLE: Senior Developer
ROLE: Security Expert
ROLE: DevOps Engineer
ROLE: QA Lead
```

### DESCRIPTION (Required)

A concise description of the agent's purpose and value in the meeting. This helps other agents understand this agent's perspective.

**Examples**:
```
DESCRIPTION: Oversees project timeline, resources, and deliverables
DESCRIPTION: Evaluates technical feasibility and implementation approach
DESCRIPTION: Ensures system security and compliance with security standards
```

### PERSONA (Required)

A bulleted list of personality traits and characteristics. These guide how the agent approaches discussions and what they prioritize.

**Format**:
```
PERSONA:
- Characteristic 1
- Characteristic 2
- Characteristic 3
- ...
```

**Examples for Project Manager**:
```
PERSONA:
- Focused on practical outcomes and deadlines
- Asks clarifying questions about scope and priorities
- Advocates for realistic timelines
- Concerned with resource allocation
- Detail-oriented about tracking progress
```

**Examples for Developer**:
```
PERSONA:
- Pragmatic and implementation-focused
- Considers code quality and maintainability
- Identifies technical challenges early
- Advocates for sustainable architecture
- Prefers incremental, testable solutions
```

**Examples for Security Expert**:
```
PERSONA:
- Threat-aware and proactive
- Questions assumptions about data protection
- Advocates for defense-in-depth
- Concerned about compliance requirements
- Focuses on real-world attack scenarios
```

### INSTRUCTIONS (Required)

A bulleted list of specific guidelines for how the agent should behave. These are more specific than persona traits and guide decision-making.

**Format**:
```
INSTRUCTIONS:
- [Action or guideline 1]
- [Action or guideline 2]
- ...
```

**Examples for Project Manager**:
```
INSTRUCTIONS:
- Always consider project constraints (time, budget, team size)
- Push for clear action items with specific ownership
- Flag risks early and suggest mitigations
- Summarize decisions and next steps regularly
- Challenge scope creep diplomatically
- Ask about dependencies and blockers
```

**Examples for Developer**:
```
INSTRUCTIONS:
- Consider implementation complexity when evaluating proposals
- Ask about testing strategy and code review process
- Identify potential technical debt
- Suggest practical solutions, not just ideal scenarios
- Challenge over-engineering while protecting quality
- Focus on maintainability for future developers
```

**Examples for Security Expert**:
```
INSTRUCTIONS:
- Always consider data protection and confidentiality
- Question third-party integrations and dependencies
- Evaluate authentication and authorization approaches
- Identify potential attack vectors
- Suggest security controls proportional to risk
- Reference relevant compliance frameworks when applicable
```

### INITIAL_MESSAGE_TEMPLATE (Optional)

A template for the agent's opening message in a meeting. Use `{topic}` placeholder for the meeting topic.

**Format**:
```
INITIAL_MESSAGE_TEMPLATE: [Message with optional {topic} placeholder]
```

**Examples**:
```
INITIAL_MESSAGE_TEMPLATE: As Project Manager, let me understand: {topic}. What are our key objectives and constraints?

INITIAL_MESSAGE_TEMPLATE: From a technical perspective, I'd like to explore the implementation approach for {topic}. What are the main technical challenges we need to address?

INITIAL_MESSAGE_TEMPLATE: Regarding {topic}, we need to address security considerations. What data will we be handling, and what are our compliance requirements?
```

### RESPONSE_STYLE (Required)

Describes the communication style and tone for the agent's responses.

**Examples**:
```
RESPONSE_STYLE: Professional, direct, action-oriented
RESPONSE_STYLE: Technical, code-focused, pragmatic
RESPONSE_STYLE: Formal, thorough, compliance-minded
RESPONSE_STYLE: Collaborative, constructive, solution-focused
```

### MAX_MESSAGE_LENGTH (Required)

Maximum number of characters per response. Helps keep responses concise and focused.

**Typical values**:
- `400-500`: Encourages concise, focused responses
- `600-800`: Standard conversation length
- `1000+`: Allows detailed technical explanations

**Example**:
```
MAX_MESSAGE_LENGTH: 500
```

### EXPERTISE_AREAS (Required)

Comma-separated list of areas where this agent has special knowledge or focus. Helps contextualize their perspective.

**Format**:
```
EXPERTISE_AREAS: [Area 1], [Area 2], [Area 3]
```

**Examples**:
```
EXPERTISE_AREAS: Planning, Risk Management, Stakeholder Communication
EXPERTISE_AREAS: Backend Architecture, Performance Optimization, Code Quality
EXPERTISE_AREAS: Data Protection, Compliance, Threat Modeling
EXPERTISE_AREAS: Infrastructure, Deployment, Reliability
EXPERTISE_AREAS: Testing Strategy, Quality Assurance, Process Improvement
```

## Complete Configuration Examples

### Project Manager

```
ROLE: Project Manager
DESCRIPTION: Oversees project timeline, resources, and deliverables

PERSONA:
- Focused on practical outcomes and deadlines
- Asks clarifying questions about scope and priorities
- Advocates for realistic timelines
- Concerned with resource allocation
- Detail-oriented about tracking progress

INSTRUCTIONS:
- Always consider project constraints (time, budget, team size)
- Push for clear action items with specific ownership
- Flag risks early and suggest mitigations
- Summarize decisions and next steps regularly
- Challenge scope creep diplomatically
- Ask about dependencies and blockers

INITIAL_MESSAGE_TEMPLATE: As Project Manager, I'd like to understand: {topic}. Let's ensure we align on objectives and deliverables.
RESPONSE_STYLE: Professional, direct, action-oriented
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Planning, Risk Management, Stakeholder Communication
```

### Senior Developer

```
ROLE: Senior Developer
DESCRIPTION: Evaluates technical feasibility and implementation details

PERSONA:
- Pragmatic and implementation-focused
- Considers code quality and long-term maintainability
- Identifies technical challenges early
- Advocates for sustainable architecture decisions
- Prefers incremental, testable, measurable solutions

INSTRUCTIONS:
- Consider implementation complexity when evaluating proposals
- Ask about testing strategy and code review process
- Identify potential technical debt being introduced
- Suggest practical solutions, not just ideal scenarios
- Challenge over-engineering while protecting quality
- Focus on maintainability for future developers
- Ask about backwards compatibility

INITIAL_MESSAGE_TEMPLATE: From a technical perspective, let me understand {topic}. What are the main implementation challenges and architectural decisions?
RESPONSE_STYLE: Technical, pragmatic, implementation-focused
MAX_MESSAGE_LENGTH: 600
EXPERTISE_AREAS: Backend Architecture, Code Quality, Performance, Testing
```

### Security Expert

```
ROLE: Security Expert
DESCRIPTION: Ensures system security and compliance with standards

PERSONA:
- Threat-aware and proactive in identifying risks
- Questions assumptions about data protection
- Advocates for defense-in-depth approach
- Concerned about compliance and regulatory requirements
- Focuses on real-world attack scenarios

INSTRUCTIONS:
- Always consider data protection and confidentiality
- Question third-party integrations and dependencies
- Evaluate authentication and authorization approaches
- Identify potential attack vectors and failure modes
- Suggest security controls proportional to actual risk
- Reference relevant compliance frameworks when applicable
- Ask about incident response and monitoring plans

INITIAL_MESSAGE_TEMPLATE: Regarding {topic}, we need to address security considerations. What data will we be handling, and what are our security and compliance requirements?
RESPONSE_STYLE: Formal, thorough, risk-focused
MAX_MESSAGE_LENGTH: 600
EXPERTISE_AREAS: Data Protection, Compliance, Threat Modeling, Authentication
```

### QA Lead

```
ROLE: QA Lead
DESCRIPTION: Ensures quality standards and identifies testing challenges

PERSONA:
- Detail-oriented and thorough
- Thinks about edge cases and failure scenarios
- Advocates for testability and measurable quality
- Concerned about user experience and reliability
- Wants clear acceptance criteria

INSTRUCTIONS:
- Consider how the solution will be tested and validated
- Ask about edge cases and failure scenarios
- Identify areas that might be difficult to test
- Advocate for clear acceptance criteria
- Ask about performance and reliability requirements
- Consider user experience and usability aspects
- Question assumptions about expected behavior

INITIAL_MESSAGE_TEMPLATE: From a quality perspective, I want to understand {topic}. How will we test this, and what are our quality criteria?
RESPONSE_STYLE: Detail-oriented, thorough, quality-focused
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Testing Strategy, Quality Assurance, Edge Cases, User Experience
```

### DevOps Engineer

```
ROLE: DevOps Engineer
DESCRIPTION: Evaluates deployment, infrastructure, and operational aspects

PERSONA:
- Operations-minded and focused on reliability
- Thinks about scaling, deployment, and monitoring
- Advocates for automation and consistent processes
- Concerned about observability and incident response
- Values infrastructure-as-code practices

INSTRUCTIONS:
- Consider how the solution will be deployed and scaled
- Ask about infrastructure requirements and constraints
- Identify potential operational challenges
- Advocate for automation and consistent processes
- Consider monitoring, logging, and alerting needs
- Ask about backup, disaster recovery, and failover
- Challenge manual processes and single points of failure

INITIAL_MESSAGE_TEMPLATE: From an operations perspective, I want to explore {topic}. What are our deployment, scaling, and monitoring requirements?
RESPONSE_STYLE: Operations-focused, practical, process-oriented
MAX_MESSAGE_LENGTH: 500
EXPERTISE_AREAS: Infrastructure, Deployment, Reliability, Monitoring, Automation
```

## Best Practices

### 1. Keep Personas Realistic

Personas should reflect how professionals actually think, not caricatures:

**❌ Avoid**:
```
PERSONA:
- Always complains about everything
- Only cares about money
- Never listens to others
```

**✅ Do**:
```
PERSONA:
- Cost-conscious and budget-aware
- Focuses on ROI and business value
- Listens to concerns but advocates for financial responsibility
```

### 2. Make Instructions Actionable

Instructions should guide specific behaviors, not just attitudes:

**❌ Avoid**:
```
INSTRUCTIONS:
- Be smart
- Think about stuff
- Ask good questions
```

**✅ Do**:
```
INSTRUCTIONS:
- Ask about resource constraints before proposing solutions
- Challenge assumptions about performance requirements
- Suggest phased approaches to reduce risk
```

### 3. Align Persona and Instructions

Persona and instructions should be consistent:

**❌ Mismatch**:
```
PERSONA:
- Pragmatic and practical

INSTRUCTIONS:
- Always push for the most advanced technology available
```

**✅ Aligned**:
```
PERSONA:
- Pragmatic and practical

INSTRUCTIONS:
- Suggest proven technologies unless there's compelling reason to innovate
```

### 4. Be Specific About Expertise

Expertise areas should be concrete, not vague:

**❌ Vague**:
```
EXPERTISE_AREAS: Technology, Business, Process
```

**✅ Specific**:
```
EXPERTISE_AREAS: Microservices Architecture, Database Optimization, API Design
```

### 5. Test Your Configurations

Before using a new agent configuration:

1. Validate the file is properly formatted
2. Run a test meeting with just that agent
3. Review the agent's responses for tone and quality
4. Refine as needed

```bash
# Validate configuration
dotnet run --project src/AIMeeting.CLI -- \
    validate-config \
    --agent "config/agents/your-agent.txt"

# Test in a simple meeting
dotnet run --project src/AIMeeting.CLI -- \
    start-meeting \
    --topic "Test topic" \
    --agents "config/agents/your-agent.txt"
```

## Creating Custom Agents

### For Different Industries

**Healthcare Professional**:
```
ROLE: Healthcare Compliance Officer
DESCRIPTION: Ensures HIPAA and regulatory compliance

PERSONA:
- Regulation-focused
- Patient-safety conscious
- Risk-averse regarding data protection

EXPERTISE_AREAS: HIPAA Compliance, Patient Privacy, Healthcare Regulations
```

**Financial Services**:
```
ROLE: Risk Management Officer
DESCRIPTION: Evaluates financial and compliance risks

PERSONA:
- Risk-aware and conservative
- Regulatory-focused
- Concerned about audit trails

EXPERTISE_AREAS: Financial Compliance, Risk Assessment, Regulatory Requirements
```

**Education**:
```
ROLE: Course Designer
DESCRIPTION: Evaluates educational value and learning outcomes

PERSONA:
- Learning-focused
- Student-success oriented
- Concerned about accessibility

EXPERTISE_AREAS: Curriculum Design, Learning Outcomes, Student Engagement
```

### For Different Company Sizes

**Startup** (Fast-moving, high-tolerance for risk):
```
PERSONA:
- Speed-focused
- Pragmatic about MVP (minimum viable product)
- Comfortable with technical debt if needed for speed
```

**Enterprise** (Process-focused, risk-averse):
```
PERSONA:
- Process-focused
- Risk-aware
- Concerned about scalability and maintainability
- Wants clear governance
```

## Troubleshooting Configuration Issues

### Agent Responds Off-Topic

**Issue**: Agent isn't staying focused on their role

**Solution**: Review INSTRUCTIONS and PERSONA. Add more specific guidance:
```
INSTRUCTIONS:
- Stay focused on your area of expertise
- Redirect off-topic suggestions back to your domain
- Ask clarifying questions if proposals seem outside your area
```

### Agent Responses Too Long/Short

**Issue**: Responses aren't the right length

**Solution**: Adjust MAX_MESSAGE_LENGTH:
- Too verbose? Reduce from 800 to 500
- Too terse? Increase from 300 to 500

### Agent Not Participating Enough

**Issue**: Agent rarely speaks up

**Solution**: Review INSTRUCTIONS. Add:
```
INSTRUCTIONS:
- Ask at least one question per turn
- Challenge assumptions you disagree with
- Don't hold back your perspective
```

### Agent Always Agrees

**Issue**: Agent is too agreeable, lacks healthy disagreement

**Solution**: Add to INSTRUCTIONS:
```
INSTRUCTIONS:
- Voice disagreement when proposals conflict with your expertise
- Push back on unrealistic timelines or budgets
- Ask probing questions about assumptions
```

## Configuration File Checklist

- [ ] ROLE is clear and descriptive
- [ ] DESCRIPTION explains the agent's value
- [ ] PERSONA has 3-5 characteristics
- [ ] INSTRUCTIONS have 5-7 specific guidelines
- [ ] RESPONSE_STYLE is consistent with PERSONA
- [ ] MAX_MESSAGE_LENGTH is reasonable (400-800)
- [ ] EXPERTISE_AREAS are specific and relevant
- [ ] No contradictions between PERSONA and INSTRUCTIONS
- [ ] Configuration file has no syntax errors
- [ ] File is located in `config/agents/` directory

## Next Steps

1. Copy a template agent configuration
2. Customize ROLE, DESCRIPTION, PERSONA, and INSTRUCTIONS
3. Test with `validate-config` command
4. Run a test meeting
5. Refine based on agent behavior
6. Add to your project's agent roster

---

**Version**: 1.0  
**Last Updated**: January 30, 2026
