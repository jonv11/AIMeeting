# Phase 5 Testing Guide: Real Copilot SDK Integration

This guide provides a structured approach to testing the orchestrator with real GitHub Copilot SDK for .NET integration.

## Prerequisites

### Required Setup
1. **GitHub Copilot SDK for .NET installed**
   - Already integrated in v0.1.1 via NuGet package
   - Verify in `src/AIMeeting.Copilot/AIMeeting.Copilot.csproj`

2. **AIMeeting built in Release mode**
   ```bash
   dotnet build -c Release
   ```

3. **Remove stub mode environment variable**
   ```bash
   # PowerShell
   Remove-Item Env:AIMEETING_AGENT_MODE
   
   # Bash
   unset AIMEETING_AGENT_MODE
   ```

### Verification
Ensure the orchestrator will call real Copilot:
```bash
# This should NOT be set or should NOT equal "stub"
echo $env:AIMEETING_AGENT_MODE  # PowerShell
echo $AIMEETING_AGENT_MODE      # Bash
```

## Test Scenarios

### Scenario 1: Simple 2-Agent Discussion
**Objective**: Verify basic orchestrator decision-making

**Setup**:
```bash
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Should we use microservices for our new product?" \
    --agents "config/agents/orchestrator.txt" \
             "config/agents/solutions-architect.txt" \
             "config/agents/principal-engineer.txt" \
    --max-duration 5 \
    --max-messages 10
```

**Expected Behaviors**:
- ✅ Orchestrator alternates between agents thoughtfully
- ✅ JSON responses are valid and parse correctly
- ✅ Meeting progresses through phases if appropriate
- ✅ Meeting ends with clear decision or summary
- ❌ No fallback to stub mode (check console logs)
- ❌ No excessive retries (should succeed on first or second attempt)

**Validation Checks**:
- [ ] Meeting completed successfully
- [ ] No "falling back to stub mode" messages
- [ ] Agents spoke in logical order
- [ ] Phase changes occurred at appropriate times
- [ ] Meeting ended with proper rationale

---

### Scenario 2: Multi-Agent Debate (4+ Agents)
**Objective**: Test orchestrator's ability to manage complex discussions

**Setup**:
```bash
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Security vs. Developer Experience: Finding the right balance" \
    --agents "config/agents/orchestrator.txt" \
             "config/agents/security-guardian.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/experience-designer.txt" \
             "config/agents/project-manager.txt" \
    --max-duration 10 \
    --max-messages 20
```

**Expected Behaviors**:
- ✅ Orchestrator ensures all agents contribute
- ✅ Agents with relevant expertise speak more in their domain
- ✅ Meeting moves through phases: Problem → Options → Evaluation → Decision
- ✅ Orchestrator prevents circular discussions
- ✅ Meeting ends when consensus or deadlock is detected

**Validation Checks**:
- [ ] All agents participated at least once
- [ ] Security concerns were addressed
- [ ] Developer experience was considered
- [ ] Phase transitions were logical
- [ ] Final decision reflected multiple perspectives

---

### Scenario 3: Technical Deep Dive (Long Discussion)
**Objective**: Test orchestrator's sustained performance and phase management

**Setup**:
```bash
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Design a scalable event-driven architecture for real-time notifications" \
    --agents "config/agents/orchestrator.txt" \
             "config/agents/solutions-architect.txt" \
             "config/agents/platform-engineer.txt" \
             "config/agents/reliability-specialist.txt" \
             "config/agents/software-craftsperson.txt" \
    --max-duration 15 \
    --max-messages 30
```

**Expected Behaviors**:
- ✅ Orchestrator maintains focus on the topic
- ✅ Technical details are explored systematically
- ✅ Multiple phase transitions occur naturally
- ✅ Meeting doesn't end prematurely
- ✅ Orchestrator tracks progress toward goal

**Validation Checks**:
- [ ] Discussion covered key technical aspects
- [ ] Phase progression: Clarification → Options → Evaluation → Planning
- [ ] No premature termination
- [ ] Execution plan created (if phase reached)
- [ ] Consensus on architecture approach

---

### Scenario 4: Early Consensus
**Objective**: Verify orchestrator can detect and end meetings appropriately

**Setup**:
```bash
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Should we upgrade to .NET 9 for this project?" \
    --agents "config/agents/orchestrator.txt" \
             "config/agents/principal-engineer.txt" \
             "config/agents/software-craftsperson.txt" \
    --max-duration 5 \
    --max-messages 15
```

**Expected Behaviors**:
- ✅ Meeting ends before max limits if clear consensus reached
- ✅ Orchestrator's end_reason explains why
- ✅ All key points were discussed before ending

**Validation Checks**:
- [ ] Meeting ended naturally (not by limits)
- [ ] End reason was appropriate
- [ ] Both agents expressed their views
- [ ] Clear decision was documented

---

### Scenario 5: Deadlock Detection
**Objective**: Test orchestrator's ability to handle unresolvable disagreements

**Setup**:
```bash
dotnet run --project src/AIMeeting.CLI -- start-meeting \
    --topic "Tabs vs. Spaces: Settle this once and for all" \
    --agents "config/agents/orchestrator.txt" \
             "config/agents/senior-developer.txt" \
             "config/agents/software-craftsperson.txt" \
    --max-duration 10 \
    --max-messages 20
```

**Expected Behaviors**:
- ✅ Orchestrator detects circular arguments
- ✅ Meeting ends with acknowledgment of deadlock
- ✅ Both perspectives are documented

**Validation Checks**:
- [ ] Orchestrator recognized the deadlock
- [ ] Meeting didn't cycle endlessly
- [ ] End reason mentioned disagreement or deadlock
- [ ] Both viewpoints were captured

---

## Performance Metrics to Track

During each test, monitor:

### Response Time
- Time from turn request to decision event
- **Target**: < 5 seconds per decision
- **Acceptable**: < 10 seconds
- **Action Required**: > 15 seconds

### Retry Rate
- Number of retry attempts per decision
- **Target**: 0 retries (success on first attempt)
- **Acceptable**: 1-2 retries occasionally
- **Action Required**: 3 retries (fallback to stub) frequently

### Decision Quality
- Agent selection appropriateness (1-5 scale)
- Phase transition logic (1-5 scale)
- Meeting termination appropriateness (1-5 scale)

### Fallback Rate
- Percentage of decisions using stub fallback
- **Target**: 0%
- **Acceptable**: < 5%
- **Action Required**: > 10%

## Debugging Failed Tests

### Problem: Orchestrator falls back to stub mode
**Symptoms**: Console logs show "Falling back to stub mode"

**Diagnosis**:
1. Check Copilot SDK is integrated: Verify `ICopilotClient` is implemented in `src/AIMeeting.Copilot/`
2. Verify authentication: `gh auth status`
3. Check network connectivity
4. Review exception message in logs

**Solutions**:
- Reinstall/update GitHub CLI
- Re-authenticate: `gh auth login`
- Check corporate proxy/firewall settings
- Review API rate limits

---

### Problem: Invalid JSON responses
**Symptoms**: Parse errors, retry attempts, eventually stub fallback

**Diagnosis**:
1. Check response in logs (before "Failed to parse")
2. Look for malformed JSON or extra text
3. Check if LLM ignored format instructions

**Solutions**:
- Refine prompt to emphasize JSON-only responses
- Add more explicit examples to prompt
- Consider adding prompt engineering techniques (zero-shot, few-shot)
- Review orchestrator configuration instructions

---

### Problem: Poor agent selection
**Symptoms**: Wrong agents speaking, irrelevant contributions

**Diagnosis**:
1. Review orchestrator's rationale in decisions
2. Check if agent roles are clear in prompt
3. Verify meeting context includes agent information

**Solutions**:
- Enhance agent descriptions in configurations
- Add expertise areas to prompt
- Include recent speaker history to avoid repetition
- Provide more decision-making guidance

---

### Problem: Premature meeting termination
**Symptoms**: Meeting ends before meaningful progress

**Diagnosis**:
1. Check end_reason in final decision
2. Review meeting transcript for actual progress
3. Look for misinterpretation of discussion state

**Solutions**:
- Refine "End meeting when..." guidance in prompt
- Add examples of what constitutes goal achievement
- Consider tracking open questions in meeting context
- Adjust orchestrator configuration instructions

---

## Prompt Tuning Checklist

Based on test results, consider adjusting:

### Context Information
- [ ] Add/remove agent expertise details
- [ ] Increase/decrease message history (currently 5)
- [ ] Include/exclude meeting statistics
- [ ] Add phase descriptions or goals

### Decision Guidance
- [ ] Clarify when to change phases
- [ ] Provide examples of good agent selection
- [ ] Add guidance for detecting deadlock
- [ ] Specify when consensus is achieved

### Response Format
- [ ] Add examples of valid responses
- [ ] Emphasize JSON-only requirement
- [ ] Provide templates for each decision type
- [ ] Add validation notes

## Success Criteria

Phase 5 is considered complete when:

- ✅ At least 5 test scenarios pass successfully
- ✅ Fallback rate < 5%
- ✅ Average response time < 10 seconds
- ✅ Decision quality rated 4+ / 5 consistently
- ✅ No critical bugs or crashes
- ✅ Retry logic works as designed
- ✅ Error messages are clear and actionable

## Next Steps After Phase 5

Once manual testing is complete:

1. **Document findings**: Add results to CHANGELOG
2. **Tune prompts**: Implement improvements based on observations
3. **Add metrics**: Implement telemetry for production monitoring
4. **Phase 6**: Iterative prompt tuning based on outcomes
5. **Phase 7**: State tracking implementation
6. **Release v0.1.2**: Deploy orchestrator to production

## Related Documentation

- [Orchestrator Guide](orchestrator-guide.md)
- [Orchestrator Response Format](../reference/orchestrator-response-format.md)
- [CLI Guide](cli.md)
- [Agent Configuration Reference](../reference/agent-configuration.md)
