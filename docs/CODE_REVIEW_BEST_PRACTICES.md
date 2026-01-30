# Code Review Best Practices Guide

A comprehensive guide to conducting effective code reviews that improve code quality, share knowledge, maintain standards, and build team culture. This covers what to review, how to give constructive feedback, automating reviews, and establishing healthy review practices.

---

## Why Code Reviews Matter

### Impact on Development
- **Quality**: Catch bugs before production
- **Knowledge Sharing**: Team learns from each other
- **Consistency**: Maintain coding standards
- **Ownership**: Distributed responsibility for quality
- **Learning**: Reviewers learn from new approaches
- **Security**: Additional eyes catch vulnerabilities
- **Culture**: Build collaborative, supportive team

### Cost of Poor Code Reviews
- Important issues slip through to production
- Inconsistent code standards across codebase
- Knowledge silos; only one person understands each area
- Demoralized developers from harsh feedback
- Slow feedback loop delays shipping
- Missed learning opportunities
- Security vulnerabilities not caught

---

## Code Review Goals

### **Primary Goals**
1. **Prevent Bugs**: Find defects before production
2. **Share Knowledge**: Spread understanding across team
3. **Enforce Standards**: Maintain consistent quality bar
4. **Improve Design**: Discuss better approaches
5. **Build Culture**: Create safe, supportive environment

### **Secondary Benefits**
- New developers learn codebase
- Junior developers mentored
- Best practices spread
- Technical debt identified
- Documentation gaps found

---

## What to Review

### **Always Check**

```python
✅ Logic Correctness
- Does the code do what it claims to do?
- Are there edge cases not handled?
- Is the algorithm correct?

Example: User's implementation
def calculate_discount(price, discount_percent):
    return price - price * discount_percent

# Issues:
# 1. Doesn't handle discount_percent > 100
# 2. Doesn't validate inputs
# 3. Negative discount_percent could increase price

# Better:
def calculate_discount(price, discount_percent):
    if not isinstance(price, (int, float)) or price < 0:
        raise ValueError(f"Price must be non-negative number, got {price}")
    if not isinstance(discount_percent, (int, float)):
        raise ValueError(f"Discount must be number, got {discount_percent}")
    if not 0 <= discount_percent <= 100:
        raise ValueError(f"Discount must be 0-100%, got {discount_percent}")
    return price * (1 - discount_percent / 100)
```

```python
✅ Error Handling
- Are errors handled appropriately?
- Are exceptions specific?
- Is error context provided?

Example: Missing error handling
def process_payment(user_id, amount):
    user = database.get_user(user_id)
    balance = user.account_balance
    user.account_balance = balance - amount
    database.save(user)
    notify_payment_service(user_id, amount)

# Issues:
# 1. No check if user exists
# 2. No insufficient funds check
# 3. No error recovery if notify fails
# 4. Silent failures

# Better:
def process_payment(user_id, amount):
    user = database.get_user(user_id)
    if not user:
        raise UserNotFoundError(f"User {user_id} not found")
    
    if amount <= 0:
        raise ValidationError("Amount must be positive")
    
    if user.account_balance < amount:
        raise InsufficientFundsError(
            f"Balance {user.account_balance} < amount {amount}"
        )
    
    try:
        user.account_balance -= amount
        database.save(user)
        notify_payment_service(user_id, amount)
    except NotificationError as e:
        # Rollback transaction
        user.account_balance += amount
        database.save(user)
        raise PaymentNotificationError(str(e))
```

```python
✅ Security Issues
- Are inputs validated?
- Could SQL injection occur?
- Is sensitive data exposed?
- Are permissions checked?

Example: Security issues
def get_user_data():
    user_id = request.args.get('user_id')
    query = f"SELECT * FROM users WHERE id = {user_id}"  # SQL injection!
    return database.execute(query)

def get_sensitive_info():
    return {
        "ssn": user.ssn,            # Expose PII!
        "password": user.password,  # Never expose!
        "api_key": user.api_key     # Never expose!
    }

# Better:
def get_user_data():
    user_id = request.args.get('user_id')
    if not isinstance(user_id, int) or user_id <= 0:
        raise ValueError("Invalid user_id")
    
    current_user = get_authenticated_user()
    if current_user.id != user_id and not current_user.is_admin:
        raise PermissionError("Cannot access other user's data")
    
    return database.get_user(user_id)

def get_safe_user_info(user):
    return {
        "id": user.id,
        "name": user.name,
        "email": user.email
    }
```

```python
✅ Performance Issues
- Are there N+1 queries?
- Unnecessary loops over large datasets?
- Memory leaks?
- Inefficient algorithms?

Example: Performance issues
def get_user_orders():
    users = database.get_all_users()
    result = []
    for user in users:
        orders = database.get_orders(user.id)  # N+1 query problem!
        result.append({
            "user": user,
            "orders": orders,
            "count": len(orders)
        })
    return result

# Better: Use join or batch query
def get_user_orders():
    users = database.get_users_with_orders()  # Single query with join
    return [{
        "user": user,
        "orders": user.orders,
        "count": len(user.orders)
    } for user in users]
```

```python
✅ Testing
- Are tests included?
- Do tests cover the change?
- Are edge cases tested?
- Do tests verify behavior, not implementation?

Example: Insufficient tests
def test_calculate_discount():
    # Only tests happy path
    assert calculate_discount(100, 10) == 90

# Better:
def test_calculate_discount():
    # Happy path
    assert calculate_discount(100, 10) == 90
    
    # Edge cases
    assert calculate_discount(100, 0) == 100
    assert calculate_discount(100, 100) == 0
    assert calculate_discount(0, 50) == 0
    
    # Invalid inputs
    with pytest.raises(ValueError):
        calculate_discount(-100, 10)
    with pytest.raises(ValueError):
        calculate_discount(100, 150)
    with pytest.raises(TypeError):
        calculate_discount("100", 10)
```

### **Often Check**

```python
✅ Code Style & Consistency
- Follows project conventions?
- Properly formatted?
- Good naming?
- Readable?

✅ Documentation
- Docstrings for functions?
- Complex logic explained?
- Examples provided?
- API changes documented?

✅ Design & Architecture
- Does solution fit architecture?
- Unnecessary coupling?
- Proper abstractions?
- Could be simpler?

✅ Dependencies
- New dependencies necessary?
- Version constraints reasonable?
- License compatible?
```

### **Sometimes Check**

```python
⚠️ Performance Optimization
- Only if clear bottleneck
- Profile before optimizing
- Balance with readability

⚠️ Style Preference
- Avoid bikeshedding
- Focus on consistency, not preference
- Use automated formatters

⚠️ Alternative Approaches
- Only if significantly better
- Respect author's choices
- Consider team growth
```

### **Don't Check**

```python
❌ Excessive Nitpicking
- Typos in comments (can fix later)
- Minor style variations
- Personal preferences

❌ Can Be Automated
- Formatting (use linters)
- Import ordering (use tools)
- Type checking (use type checkers)

❌ Outside Scope
- Implementation of other parts
- Existing bugs unrelated to PR
- Personal coding style
```

---

## Giving Effective Feedback

### **Principles for Constructive Feedback**

```python
✅ Specific
# Bad: "This is inefficient"
# Good: "This query fetches all rows then filters in Python. 
#        Consider filtering in the database with WHERE clause to reduce data transfer."

✅ Actionable
# Bad: "Bad design"
# Good: "Consider extracting this validation logic to a separate function 
#        since it's used in three places and would benefit from centralization."

✅ Kind
# Bad: "This is obviously wrong"
# Good: "I missed this initially too—remember we need to handle the None case."

✅ Explain Reasoning
# Bad: "This violates DRY"
# Good: "This payment logic appears in two places. If validation rules change 
#        later, we'd need to update both. Consider extracting to shared function."

✅ Praise Good Work
# Bad: [Only mentions issues]
# Good: "Great refactoring—the error handling is much clearer now. 
#        One suggestion on performance..."
```

### **Comment Types and Tones**

```python
# BLOCKER: Must fix before merge
# 🚨 BLOCKER: This will cause data loss if user updates their email twice.
#    The race condition happens because we don't use a transaction.
#    Suggest using database transaction or optimistic locking.

# MAJOR: Should fix before merge
# 🔴 MAJOR: Missing validation for negative amounts. This could cause
#    accounting errors. Suggest: if amount < 0: raise ValueError(...)

# SUGGESTION: Nice to have, not required
# 💡 SUGGESTION: This could be simpler with a built-in function.
#    Instead of: [list comprehension], consider: result = list(set(items))

# QUESTION: Clarify intent
# ❓ QUESTION: Why do we need to call refresh_cache() here? 
#    I don't see where the cache gets invalidated.

# PRAISE: Acknowledge good work
# ✅ GREAT: Really liked how you broke down the complex logic into 
#    named functions. Makes it much easier to follow.
```

### **Example Code Review**

```markdown
## Review for PR #123: Add payment retry logic

### General Comments
Nice implementation of the retry pattern! The circuit breaker protection 
is particularly good. A few suggestions below.

### 🚨 BLOCKER: Sensitive data in logs
```python
logger.error(f"Payment failed with amount {amount} and card {credit_card}")
```
This logs the full credit card number. Use a masked version instead:
```python
masked_card = credit_card[-4:].rjust(len(credit_card), '*')
logger.error(f"Payment failed for card {masked_card}")
```

### 🔴 MAJOR: Missing edge case
The retry logic restarts if timeout occurs after processing charge.
If charge succeeded before timeout, we'll charge twice. Suggest:
```python
try:
    charge_result = charge_card(amount)  # Returns transaction_id
    # Even if notify fails, we log the transaction
    notify_success(charge_result)
    return charge_result
except RequestException:
    # Only retry if charge never happened
    if charge_result is None:
        retry_payment()
```

### 💡 SUGGESTION: Configuration
Magic number `max_retries=3` could be configurable:
```python
def retry_payment(amount, max_retries=None):
    max_retries = max_retries or config.PAYMENT_MAX_RETRIES
```

### ❓ QUESTION: Circuit breaker reset
How does the circuit breaker recover if payment service is actually down?
The `timeout=60` might not be enough if it's a widespread outage.

### ✅ GREAT: Exponential backoff
Love the exponential backoff with jitter implementation. Prevents 
thundering herd well.

### Test Coverage
Tests look good, but suggest adding:
- Test for charging twice scenario
- Test with circuit breaker fully open
- Integration test with actual failure

Looks good overall! Please address blockers and we can merge.
```

---

## Automated Code Review

### **Linting and Formatting**

```yaml
# .pre-commit-config.yaml - Run before committing
repos:
  - repo: https://github.com/psf/black
    rev: 23.3.0
    hooks:
      - id: black
  
  - repo: https://github.com/PyCQA/flake8
    rev: 6.0.0
    hooks:
      - id: flake8
        args: ['--max-line-length=100']
  
  - repo: https://github.com/PyCQA/isort
    rev: 5.12.0
    hooks:
      - id: isort
```

### **Type Checking**

```bash
# mypy: static type checking
mypy src/

# Catches issues before code review
error: Argument 1 to "get_user" has incompatible type "str"; expected "int"
```

### **Security Scanning**

```bash
# bandit: security vulnerability scanner
bandit -r src/

# Issues reported:
# >> Issue: Hardcoded SQL string allows SQL injection
#    File: src/db.py, line 45
#    query = f"SELECT * FROM users WHERE id = {user_id}"
```

### **Test Coverage**

```bash
# pytest with coverage
pytest --cov=src --cov-report=term-missing

# Coverage report
Name                    Stmts   Miss  Cover
src/payment.py            50      3    94%
src/auth.py              30      5    83%
TOTAL                    80      8    90%
```

### **Continuous Integration**

```yaml
# .github/workflows/review.yml
name: Code Review

on: [pull_request]

jobs:
  review:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Format check
        run: black --check src/
      
      - name: Linting
        run: flake8 src/
      
      - name: Type checking
        run: mypy src/
      
      - name: Security
        run: bandit -r src/
      
      - name: Tests
        run: pytest --cov=src
      
      - name: Coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./coverage.xml
          fail_ci_if_error: true
```

### **What to Automate**

```
✅ AUTOMATE (objective, no judgment)
- Code formatting (black, prettier)
- Import ordering (isort)
- Linting rules (flake8, eslint)
- Type checking (mypy, typescript)
- Security scanning (bandit, SAST)
- Test execution
- Coverage measurement
- Dependency updates

⚠️ USE WITH CAUTION (some judgment required)
- Complexity metrics (cyclomatic complexity)
- Maintainability index
- Duplication detection

❌ DON'T AUTOMATE (require human judgment)
- Design decisions
- Architecture fit
- Business logic correctness
- Testing approach
- Documentation quality
```

---

## Code Review Process

### **Pull Request Checklist**

```markdown
## Author's Checklist (before requesting review)
- [ ] Code implements the required feature/fix
- [ ] All tests pass: `pytest`
- [ ] Code is formatted: `black src/`
- [ ] No linting errors: `flake8 src/`
- [ ] Type checking passes: `mypy src/`
- [ ] New functions have docstrings
- [ ] Complex logic is explained
- [ ] Related documentation updated
- [ ] No debugging code left in
- [ ] Secrets not committed
- [ ] PR description explains the change

## Reviewer's Checklist
- [ ] Understand what the change does
- [ ] Run the code locally to verify
- [ ] Check logic correctness
- [ ] Review test coverage
- [ ] Check for security issues
- [ ] Verify no new technical debt
- [ ] Comment is constructive and kind
- [ ] Praise good work
```

### **Review Workflow**

```
1. Author creates PR with description
   ↓
2. Automated checks run (CI/CD pipeline)
   ├─ Tests
   ├─ Linting
   ├─ Type checking
   └─ Security scanning
   ↓
3. Reviewer(s) assigned
   ↓
4. Reviewer reads PR and comments
   - Asks for changes (request changes)
   - Suggests improvements (comment)
   - Approves (approve)
   ↓
5. Author responds to comments
   - Makes code changes
   - Explains decisions
   - Pushes new commits
   ↓
6. Reviewer reviews changes
   ↓
7. If approved: Merge to main
   ↓
8. Deploy to production
```

### **Response Time**

```
Target review times:
- Urgent (production bug): 1-2 hours
- High priority: 4-8 hours
- Regular: 24 hours
- Low priority: 2-3 days

To improve:
- Smaller PRs reviewed faster
- Clearly describe what needs review
- @mention specific reviewers
- Use priority labels
```

---

## Building a Healthy Review Culture

### **For Reviewers**

```python
✅ DO:
- Be respectful and kind
- Explain the "why" not just "do this"
- Ask questions to understand
- Praise good work
- Review promptly
- Focus on important issues
- Trust team members
- Suggest, don't demand
- Learn from feedback

❌ DON'T:
- Be condescending
- Comment on everything
- Demand specific implementation
- Ignore good work
- Delay reviews
- Nit-pick style
- Rewrite code without discussion
- Take feedback personally
- Block on subjective style
```

### **For Authors**

```python
✅ DO:
- Take feedback constructively
- Ask clarifying questions
- Explain your reasoning
- Small, focused PRs
- Good PR descriptions
- Respond to all comments
- Thank reviewers

❌ DON'T:
- Get defensive
- Dismiss feedback
- Large unwieldy PRs
- Vague descriptions
- Ignore comments
- Force merge
- Push back on all feedback
```

### **Anti-patterns**

```
❌ Drive-by reviews
Problem: Reviewer approves without careful reading
Solution: Use checklist, require local testing

❌ Bikeshedding
Problem: Endless debate on minor style
Solution: Use automated formatting, focus on logic

❌ Blame culture
Problem: Reviews used to assign fault
Solution: Focus on improvement, not blame

❌ Performance reviews based on review comments
Problem: Creates incentive to approve everything
Solution: Separate code review from performance

❌ Approval from unqualified reviewer
Problem: Review is just rubber-stamp
Solution: Require expert reviewer for complex areas
```

---

## Common Scenarios

### **Scenario 1: Disagreement**

```
Reviewer: "This algorithm is inefficient, use sorting instead"
Author: "My approach is more intuitive, efficiency isn't critical here"

Better approach:
- Measure actual performance
- Consider readability vs optimization
- Make data-driven decision
- Compromise if possible
- Escalate to tech lead if needed
```

### **Scenario 2: Feedback Rejected**

```
Reviewer suggests: "Extract this to a function"
Author: "It's only used once"

Better response:
- Acknowledge the suggestion
- Explain reasoning
- Ask if it's blocking or optional
- If optional, respect their choice
- Document the decision
```

### **Scenario 3: Late Discovery**

```
Issue found in production that review should have caught:

Learning:
- Review the review process
- What check was missed?
- Could it be automated?
- Update PR template
- Add to review checklist
```

---

## Metrics and Monitoring

### **Review Metrics**

```
✅ Track:
- Review time (target: 24 hours)
- Bugs found in review
- Bugs found in production (post-review)
- PR size (smaller is better)
- Test coverage
- Number of reviewers per PR

❌ Don't track:
- Number of comments (quality > quantity)
- Approvals per person (creates bad incentives)
- Time to approval (encourages rubber-stamp)
```

### **Improving Code Review**

```
1. Measure current state
   - Review time
   - Bug escape rate
   - Team satisfaction

2. Identify bottlenecks
   - Too few reviewers?
   - PR too large?
   - Unclear requirements?
   - Missing automation?

3. Address root causes
   - Add reviewers or rotate
   - Enforce smaller PRs
   - Clarify requirements
   - Automate checks

4. Measure improvement
   - Same metrics monthly
   - Team feedback
   - Quality improvements
```

---

## Checklist

- [ ] Automated linting, formatting, type checking in CI
- [ ] Code review checklist created and documented
- [ ] Review time target established (24 hours)
- [ ] Minimum number of reviewers defined
- [ ] Complex areas require expert reviewer
- [ ] PR templates include review notes
- [ ] Team trained on constructive feedback
- [ ] Feedback is specific and actionable
- [ ] Blockers clearly marked
- [ ] Code style automated, not manual
- [ ] Review process documented
- [ ] Response times tracked
- [ ] Regular retros on review process
- [ ] New team members trained on culture
- [ ] Bug escapes analyzed for root cause

---

## References

- [Google's Code Review Standards](https://google.github.io/eng-practices/review/)
- [Best Practices for Code Review](https://smartbear.com/learn/code-review/best-practices-for-peer-code-review/)
- [Code Review Culture](https://mtlynch.github.io/code-review-love/)
- [Effective Code Review](https://pragprog.com/titles/tread/the-technical-recruiter/)
- [A Practical Guide to Code Review](https://github.blog/2015-12-16-how-to-write-the-perfect-pull-request/)
