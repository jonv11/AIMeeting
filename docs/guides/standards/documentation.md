# Code Comments and Documentation Guide

A comprehensive guide to understanding effective code comments, documentation practices, docstrings, and inline documentation. This guide explains when to comment, what to document, how to write clear documentation, and best practices across different programming languages.

---

## Why Code Comments Matter

### Impact on Development
- **Knowledge Transfer**: New developers understand code quickly
- **Maintenance**: Future you remembers why decisions were made
- **Debugging**: Comments help identify problem areas
- **API Usage**: Clear docs enable proper usage
- **Onboarding**: Reduces time to productivity
- **Code Review**: Reviewers understand intent

### Cost of Bad Comments
- Outdated comments confuse more than help
- Over-commenting obscures code logic
- Misleading comments introduce bugs
- No documentation leaves knowledge in developer's head
- Time wasted figuring out unclear code

---

## Changelog Standards

The repository uses a single changelog file: [CHANGELOG.md](../../../CHANGELOG.md).

### ✅ Required
- **Single source of truth**: Do not create `RELEASE_NOTES_*` files.
- **Structure**: Each release must include headings: Added, Changed, Fixed, Deprecated, Removed, Security (use only what applies).
- **Dates**: Use ISO format $YYYY-MM-DD$.
- **Scope**: Record user-visible behavior changes, dependency/CI updates, and documentation changes that affect usage.
- **Order**: New releases go at the top. Do not reorder or rewrite past releases.

### ❌ Avoid
- Internal-only refactors with no behavioral impact
- Duplicating entries in other files
- Rewriting historical entries (only fix factual errors)

---

## Core Principle: Good Code Explains Itself

```python
# Bad: Over-commented, obvious code
x = 5  # Set x to 5
y = x * 2  # Multiply x by 2

# Better: Clear variable names, no comment needed
initial_count = 5
doubled_count = initial_count * 2

# Good: Comments explain why, not what
# We double the initial count because the API requires batch operations
# in pairs for performance optimization
batch_size = initial_count * 2
```

---

## When to Write Comments

### ✅ DO Comment

**1. Complex Logic**
```python
# Sieve of Eratosthenes: removes non-primes by marking multiples
# of each prime as composite, resulting in O(n log log n) time
def get_primes(n):
    is_prime = [True] * (n + 1)
    is_prime[0] = is_prime[1] = False
    for i in range(2, int(n**0.5) + 1):
        if is_prime[i]:
            for j in range(i*i, n + 1, i):
                is_prime[j] = False
    return [i for i in range(n + 1) if is_prime[i]]
```

**2. Non-obvious Decisions**
```javascript
// We use Map instead of Object because we need iteration order
// preserved and symbol keys as property names
const configMap = new Map();

// Retry with exponential backoff: 100ms, 200ms, 400ms, etc.
// Max 5 retries prevents infinite loops
const maxRetries = 5;
```

**3. Business Logic and Rules**
```python
# According to GDPR compliance, we must delete user data
# after 30 days of account deletion request
GDPR_DELETION_DAYS = 30

# Calculate discount: tiered pricing based on annual order value
# < $1000: 0%, $1000-5000: 5%, > $5000: 10%
discount_rate = calculate_tier_discount(annual_value)
```

**4. Warnings and Gotchas**
```javascript
// WARNING: This function has side effects - it modifies the
// original array. Call .slice() first if you need to preserve it.
function sortUsers(users) {
    return users.sort((a, b) => a.name.localeCompare(b.name));
}

// NOTE: Do not use Math.random() for cryptographic purposes.
// Use crypto.getRandomValues() instead for security-sensitive code.
```

**5. External Dependencies and References**
```python
# Implementation based on RFC 3339 timestamp format
# See: https://tools.ietf.org/html/rfc3339
def parse_iso_timestamp(timestamp_str):
    return datetime.fromisoformat(timestamp_str)

# Database indexes required for performance:
# - users(email) for login lookups
# - orders(user_id, created_at) for dashboard queries
```

**6. Temporary Workarounds**
```javascript
// HACK: Temporary fix for race condition in data loader.
// TODO: Refactor to use proper async queue (ticket #456)
// Remove this workaround after migration to Bull queue
setTimeout(() => processQueue(), 100);
```

### ❌ DON'T Comment

**1. Obvious Code**
```python
# Bad: Comment explains what code obviously does
# Loop through users and print their names
for user in users:
    print(user.name)

# Good: No comment needed, code is clear
for user in users:
    print(user.name)
```

**2. Redundant Docstrings**
```python
# Bad: Docstring repeats the function name
def get_user_by_id(user_id):
    """Get user by ID."""
    return users[user_id]

# Good: Explains parameters, return value, exceptions
def get_user_by_id(user_id):
    """
    Retrieve a user from cache or database.
    
    Args:
        user_id: Integer ID of user to fetch
        
    Returns:
        User object if found
        
    Raises:
        UserNotFoundError: If user with given ID doesn't exist
        DatabaseError: If database connection fails
    """
    return users[user_id]
```

**3. Commented-Out Code**
```javascript
// Bad: Leaves confusing code in repository
// function calculateOldWay() {
//     return x + y;
// }
function calculate() {
    return x * y;
}

// Good: Delete old code, Git history preserves it
function calculate() {
    return x * y;
}
```

**4. Lies and Misleading Comments**
```python
# Bad: Comment is wrong, misleading developers
# Returns user object
def get_user(user_id):
    return user_id  # Actually returns an integer!

# Good: Comment matches reality
def get_user(user_id):
    return users_cache.get(user_id)
```

---

## Types of Comments

### **1. Implementation Comments** (How)

Explain the implementation details and algorithms:

```python
# Calculate Levenshtein distance (edit distance) between two strings
# using dynamic programming to find minimum edits needed
def levenshtein_distance(s1, s2):
    # Initialize matrix with lengths
    m, n = len(s1), len(s2)
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    
    # Base cases: converting from/to empty string
    for i in range(m + 1):
        dp[i][0] = i
    for j in range(n + 1):
        dp[0][j] = j
    
    # Fill matrix using recurrence relation
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                dp[i][j] = dp[i-1][j-1]
            else:
                dp[i][j] = 1 + min(dp[i-1][j], dp[i][j-1], dp[i-1][j-1])
    
    return dp[m][n]
```

### **2. Intent Comments** (Why)

Explain business logic and design decisions:

```javascript
// Users must be at least 18 to access adult content per COPPA
if (user.age < 18) {
    return showParentalControls();
}

// Cache user's last search result to minimize database queries
// during rapid refinement of search criteria
this.lastSearchCache = results;

// Use session tokens instead of cookies for API requests
// to support cross-domain requests and mobile apps
const token = generateSessionToken(user);
```

### **3. Warning Comments** (Watch out!)

Alert developers to potential issues:

```python
# WARNING: This endpoint returns different data based on user role.
# Make sure to test with different user types (admin, user, guest)
@app.route('/api/users')
def get_users():
    pass

# DANGER: Deleting this cache will break dependent services
# Coordinate with team before removing
redis_cache.delete('critical_config')

# NOTE: Timezone handling is timezone-aware UTC internally.
# Always use UTC when working with timestamps
timestamp = datetime.now(tz=UTC)
```

### **4. Summary Comments** (Overview)

Provide high-level summary of what code does:

```java
/**
 * Processes payment transactions from the payment queue.
 * Handles retries for transient failures and logs all outcomes.
 * 
 * This is called periodically by the scheduler and processes
 * batches of 100 transactions for performance reasons.
 */
public void processPaymentQueue() {
    // Implementation
}
```

### **5. Clarification Comments** (Explanation)

Clarify non-obvious but necessary code:

```javascript
// The API returns prices as integers representing cents, not dollars
// So 2999 means $29.99. Convert to decimal for display.
const displayPrice = apiPrice / 100;

// We can't use === for equality because NaN !== NaN
const isNaN = (value) => value !== value;

// Regex explanation: match email following RFC 5322 (simplified)
// ^[^@]+: one or more non-@ chars (local part)
// @: literal @
// [^@]+\.[^@]+$: domain with at least one dot
const emailRegex = /^[^@]+@[^@]+\.[^@]+$/;
```

---

## Docstrings and API Documentation

### **Python Docstrings**

#### Google Style
```python
def fetch_user_data(user_id, include_profile=False):
    """
    Fetch user data from database or cache.
    
    Attempts to retrieve from cache first for performance.
    Falls back to database if cache miss occurs.
    
    Args:
        user_id (int): The user ID to fetch
        include_profile (bool): Whether to include user profile data.
            Default is False for performance.
    
    Returns:
        dict: User data containing id, name, email. If include_profile
              is True, also includes profile photo and bio.
    
    Raises:
        UserNotFoundError: If user with given ID doesn't exist in
                          database after cache miss.
        DatabaseError: If database connection fails.
        
    Example:
        >>> user = fetch_user_data(123)
        >>> user['name']
        'John Doe'
        >>> user_with_profile = fetch_user_data(123, include_profile=True)
    """
    pass
```

#### NumPy Style
```python
def calculate_statistics(data):
    """
    Calculate mean, median, and std deviation of dataset.
    
    Parameters
    ----------
    data : array_like
        Input data array
        
    Returns
    -------
    stats : dict
        Dictionary with keys 'mean', 'median', 'std'
        
    Notes
    -----
    Uses pandas for performance on large datasets.
    Handles NaN values by removing them before calculation.
    
    See Also
    --------
    numpy.mean : Compute mean
    numpy.median : Compute median
    """
    pass
```

### **JavaScript/TypeScript JSDoc**

```javascript
/**
 * Calculates the total price including tax and shipping.
 * 
 * @param {number} subtotal - Base price before tax and shipping
 * @param {number} [taxRate=0.08] - Tax rate as decimal (0.08 = 8%)
 * @param {number} [shipping=5.99] - Shipping cost, default $5.99
 * @returns {number} Total price with tax and shipping
 * 
 * @throws {Error} If subtotal is negative
 * 
 * @example
 * // Calculate total for $100 purchase with defaults
 * const total = calculateTotal(100);
 * // returns 113.99
 * 
 * @example
 * // Custom tax rate
 * const total = calculateTotal(100, 0.05);
 * // returns 110.99
 * 
 * @deprecated Use {@link PricingCalculator.getTotal} instead
 * @see {@link PricingCalculator} for more options
 */
function calculateTotal(subtotal, taxRate = 0.08, shipping = 5.99) {
    if (subtotal < 0) throw new Error('Subtotal cannot be negative');
    return subtotal * (1 + taxRate) + shipping;
}
```

### **Java Documentation**

```java
/**
 * Validates email address format according to RFC 5322 standard.
 * 
 * <p>This method performs both format validation and DNS MX record
 * checks if the {@code checkDNS} parameter is true.</p>
 * 
 * @param email the email address to validate, must not be null
 * @param checkDNS whether to perform DNS MX record validation,
 *                  which adds 100-500ms latency
 * @return {@code true} if email is valid, {@code false} otherwise
 * @throws NullPointerException if email is null
 * @throws DNSException if DNS lookup fails when checkDNS is true
 * 
 * @since 1.2.0
 * @see #isValidEmailFormat(String)
 * 
 * @example
 * <pre>
 * boolean isValid = EmailValidator.validate("user@example.com", true);
 * </pre>
 */
public static boolean validate(String email, boolean checkDNS) {
    // Implementation
}
```

### **Go Comments**

```go
// Package payment provides payment processing functionality.
// It handles transaction authorization, settlement, and refunds
// through the Stripe API.
package payment

// Transaction represents a payment transaction.
type Transaction struct {
    ID        string    // Unique transaction ID
    Amount    int       // Amount in cents
    Currency  string    // ISO 4217 currency code (e.g., "USD")
    Status    string    // Status: "pending", "completed", "failed"
    CreatedAt time.Time // Transaction creation time
}

// ProcessTransaction processes a payment transaction.
//
// It validates the transaction data, communicates with Stripe API,
// and updates the transaction status. If the API call fails,
// it automatically retries up to 3 times with exponential backoff.
//
// Args:
//   ctx: Context for cancellation and timeout
//   tx: Transaction to process
//
// Returns:
//   Error if processing fails after all retries, nil on success
//
// The function panics if ctx or tx is nil.
func ProcessTransaction(ctx context.Context, tx *Transaction) error {
    if ctx == nil || tx == nil {
        panic("context and transaction must not be nil")
    }
    // Implementation
}
```

### **Rust Comments**

```rust
/// Calculates the greatest common divisor using Euclidean algorithm.
///
/// # Arguments
///
/// * `a` - First positive integer
/// * `b` - Second positive integer
///
/// # Returns
///
/// The greatest common divisor of `a` and `b`
///
/// # Panics
///
/// Panics if either `a` or `b` is zero
///
/// # Examples
///
/// ```
/// assert_eq!(gcd(48, 18), 6);
/// assert_eq!(gcd(100, 50), 50);
/// ```
pub fn gcd(mut a: u32, mut b: u32) -> u32 {
    if a == 0 || b == 0 {
        panic!("Arguments must be positive");
    }
    while b != 0 {
        let temp = b;
        b = a % b;
        a = temp;
    }
    a
}
```

---

## Module and File Documentation

### **Module Header**

**Python:**
```python
"""
user.py - User authentication and profile management module.

This module handles user registration, authentication, password
management, and profile operations. It integrates with the database
layer and provides a public API for user-related operations.

Key Functions:
    - authenticate(): Validates user credentials
    - create_user(): Creates new user account
    - update_profile(): Updates user profile information

Dependencies:
    - database module for persistence
    - encryption module for password hashing
    
Example:
    >>> user = authenticate("user@example.com", "password")
    >>> user.update_profile(name="John Doe")
"""
```

**JavaScript:**
```javascript
/**
 * @fileoverview Payment processing module.
 * 
 * Handles all payment-related operations including:
 * - Credit card authorization and charging
 * - Refund processing
 * - Payment status tracking
 * 
 * This module integrates with Stripe API and maintains
 * internal transaction logs for audit purposes.
 * 
 * @module payment
 * @requires stripe
 * @requires logger
 */
```

---

## README and Guide Documentation

### **README Sections**

```markdown
# Project Name

Brief one-line description.

## Overview
Longer description of what this is and why it exists.

## Quick Start
Get someone productive in 5 minutes:
1. Installation
2. Basic usage
3. Common example

## Features
- Feature 1
- Feature 2
- Feature 3

## Usage
More detailed usage examples and patterns.

## API Reference
Link to detailed API docs.

## Contributing
How to contribute.

## License
License information.
```

### **Architecture Documentation**

```markdown
# Architecture

## Overview
High-level system design and data flow.

## Components
- Component 1: Description
- Component 2: Description

## Data Flow
How data flows through the system.

## Design Decisions
Why key decisions were made and alternatives considered.

## Performance Considerations
Optimization notes and trade-offs.
```

---

## Comment Organization Patterns

### **Section Comments**

Group related code with clear sections:

```python
# ============================================================================
# USER AUTHENTICATION
# ============================================================================

def login(email, password):
    """Authenticate user with email and password."""
    pass

def register(email, password):
    """Register new user."""
    pass


# ============================================================================
# USER PROFILE
# ============================================================================

def get_profile(user_id):
    """Retrieve user profile."""
    pass

def update_profile(user_id, data):
    """Update user profile."""
    pass
```

### **Parameter Documentation**

```javascript
/**
 * Send email notification to user.
 *
 * @param {Object} options - Configuration object
 * @param {string} options.to - Recipient email address
 * @param {string} options.subject - Email subject line
 * @param {string} options.body - Email body content
 * @param {boolean} [options.html=false] - Whether body is HTML
 * @param {number} [options.priority=5] - Delivery priority (1-10)
 * @param {Array<string>} [options.cc] - CC recipients
 * @param {Array<string>} [options.bcc] - BCC recipients
 * @param {Object} [options.metadata] - Custom metadata
 * @returns {Promise<string>} Email ID once sent
 */
async function sendEmail(options) {
    // Implementation
}
```

---

## Documentation Tools

### **Automatic Documentation Generation**

```bash
# Python: Generate docs from docstrings
python -m pydoc mymodule

# JavaScript: Generate docs with JSDoc
jsdoc src/ -d docs/

# Java: Generate Javadoc
javadoc -d docs/ src/Main.java

# Go: Built-in documentation
godoc -http=:6060

# Rust: Built-in documentation
cargo doc --open
```

### **Popular Documentation Generators**

- **Sphinx** (Python)
- **Doxygen** (C++, Java, Python)
- **MkDocs** (General, Markdown-based)
- **Swagger/OpenAPI** (API documentation)
- **GitBook** (Knowledge base)
- **Typedoc** (TypeScript)

---

## Best Practices

### ✅ DO

- **Write for your audience**: Assume readers have domain knowledge but not code knowledge
- **Update comments with code**: Outdated comments are worse than none
- **Use clear language**: Write like you're explaining to a colleague
- **Document assumptions**: What must be true for this code to work?
- **Explain trade-offs**: Why this approach over alternatives?
- **Link to resources**: Reference specifications, tickets, research
- **Include examples**: Show how to use the code
- **Document gotchas**: What can go wrong?
- **Use consistent style**: Follow language conventions
- **Make it searchable**: Use clear keywords in documentation

### ❌ DON'T

- **Over-comment obvious code**: Trust the reader's intelligence
- **Lie or mislead**: Wrong info is worse than no info
- **Leave commented-out code**: Delete it, Git has history
- **Use unclear abbreviations**: `usr_auth` vs `user_authentication`
- **Comment in other languages**: Use English for consistency
- **Write novels**: Keep comments concise
- **Comment every line**: Only comment non-obvious parts
- **Use inside jokes**: New readers won't understand
- **Contradict the code**: If they disagree, fix the code
- **Forget to update docs**: Maintain documentation quality

---

## Anti-Patterns

### ❌ **Anti-Pattern 1: Comment Bloat**

```python
# Bad: Too many obvious comments
# Create empty list
users = []
# Loop through data
for item in data:
    # Check if item has name
    if 'name' in item:
        # Append to users
        users.append(item)

# Good: Let code speak for itself
users = [item for item in data if 'name' in item]
```

### ❌ **Anti-Pattern 2: Outdated Comments**

```python
# Bad: Comment is outdated and misleading
# This function returns the user ID (ACTUALLY returns User object now)
def get_user():
    return user_object  # Misleading!

# Good: Keep documentation current
# Returns the full User object with all profile data
def get_user():
    return user_object
```

### ❌ **Anti-Pattern 3: Cryptic Comments**

```python
# Bad: Doesn't explain the purpose
# Only works on Tuesdays and Thursdays
if datetime.now().weekday() in (1, 3):
    process_batch()

# Good: Explains why
# Run batch processing on Tuesday/Thursday when load is lowest
# This was determined by analysis of traffic patterns
if datetime.now().weekday() in (1, 3):
    process_batch()
```

### ❌ **Anti-Pattern 4: Commented Code**

```python
# Bad: Confusing commented-out code
def process_payment(amount):
    # validate_payment_amount(amount)
    # check_fraud_score(amount)
    charge_card(amount)

# Good: Delete old code, use Git history if needed
def process_payment(amount):
    validate_payment_amount(amount)
    check_fraud_score(amount)
    charge_card(amount)
```

### ❌ **Anti-Pattern 5: Vague TODOs**

```python
# Bad: Unclear what needs to be done
# TODO: Make this better
def expensive_operation():
    pass

# Good: Specific, actionable TODO
# TODO: Optimize with caching (ticket #456, estimated 2 hours)
# Currently O(n²), should be O(n log n)
def expensive_operation():
    pass
```

---

## Documentation Checklist

### **For Functions/Methods**
- [ ] Purpose clearly stated
- [ ] Parameters documented with types
- [ ] Return value documented
- [ ] Exceptions documented
- [ ] Pre-conditions and post-conditions clear
- [ ] Examples provided
- [ ] Performance characteristics noted (O(n), O(1), etc.)
- [ ] Thread safety documented (if applicable)

### **For Modules/Files**
- [ ] Module purpose clear
- [ ] Key components explained
- [ ] Dependencies listed
- [ ] Usage examples
- [ ] Common patterns
- [ ] Known limitations

### **For Complex Logic**
- [ ] Algorithm explained
- [ ] Why chosen over alternatives
- [ ] References to specifications/papers
- [ ] Complexity analysis
- [ ] Edge cases documented
- [ ] Examples of expected inputs/outputs

---

## Language-Specific Conventions

| Language | Style | Tools | Comments |
|----------|-------|-------|----------|
| Python | Google/NumPy | Sphinx, MkDocs | Module/function docstrings |
| JavaScript | JSDoc | JSDoc, TypeDoc | Function/class comments |
| Java | Javadoc | Javadoc, Maven | Class/method documentation |
| C# | XML Comments | Sandcastle, DocFX | Summary, param, returns |
| Go | GoDoc | godoc | Package and function comments |
| Rust | Doc Comments | Rustdoc | Examples in docs |
| C/C++ | Doxygen | Doxygen | Detailed API docs |
| Ruby | YARD | YARD | Parameter and return docs |

---

## References

- [Google's C++ Style Guide](https://google.github.io/styleguide/cppguide.html#Comments)
- [Python PEP 257 - Docstring Conventions](https://www.python.org/dev/peps/pep-0257/)
- [JSDoc](https://jsdoc.app/)
- [Javadoc Tool](https://www.oracle.com/java/technologies/javase/javadoc-tool.html)
- [Sphinx Documentation](https://www.sphinx-doc.org/)
- [Read the Docs](https://readthedocs.org/)
- [The Art of Code Documentation](https://hackernoon.com/the-art-of-code-documentation-8f4b60dbb1bf)

---

## Quick Reference: When to Comment

```
DECISION TREE
=============

1. Is the code's purpose immediately obvious from names?
   ├─ Yes → No comment needed
   └─ No → Consider comment

2. Would a new developer understand it in 30 seconds?
   ├─ Yes → No comment needed
   └─ No → Add explanation

3. Is there non-obvious business logic?
   ├─ Yes → Add intent comment explaining why
   └─ No → No comment needed

4. Are there potential gotchas or edge cases?
   ├─ Yes → Add warning comment
   └─ No → No comment needed

5. Does the code use external specifications/standards?
   ├─ Yes → Add reference/link comment
   └─ No → No comment needed
```
