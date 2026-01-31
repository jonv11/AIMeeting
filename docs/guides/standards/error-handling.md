# Error Handling and Logging Guide

A comprehensive guide to understanding error handling strategies, exception design, logging best practices, and debugging techniques. This guide explains how to handle errors gracefully, log effectively, and design robust error recovery mechanisms.

---

## Why Error Handling Matters

### Impact on Development
- **Reliability**: System continues despite failures
- **Debugging**: Errors show what went wrong
- **User Experience**: Clear error messages help users
- **Security**: Prevents information leakage
- **Monitoring**: Track system health
- **Recovery**: Graceful degradation instead of crashes
- **Compliance**: Meet regulatory requirements

### Cost of Poor Error Handling
- Silent failures that cause data corruption
- Hours spent debugging unclear errors
- Users confused by cryptic error messages
- Security breaches from exposed error details
- Unrecoverable application state
- Lost transactions or data
- Customer support overload

---

## Error Handling Principles

### **1. Fail Fast**
Detect and report errors as early as possible.

```python
# Bad: Silent failure, wrong result produced
def process_order(order):
    items = order.get('items', [])  # Empty list if missing
    total = calculate_total(items)  # Returns 0 for empty
    charge_card(total)  # Charges $0!
    return total

# Good: Fail immediately if required data missing
def process_order(order):
    if 'items' not in order:
        raise ValueError("Order must contain items")
    if not order['items']:
        raise ValueError("Order cannot be empty")
    total = calculate_total(order['items'])
    charge_card(total)
    return total
```

### **2. Handle at Right Level**
Handle errors where you can take action, propagate when you can't.

```python
# Bad: Catching at wrong level
try:
    data = requests.get(url)
except RequestException:
    # What can we do here? Log and ignore?
    pass  # Silent failure!

# Good: Handle at appropriate level
def fetch_user_data(user_id):
    """Fetch user data from API.
    
    Raises:
        UserNotFoundError: If user doesn't exist
        APITimeoutError: If API doesn't respond
    """
    try:
        response = requests.get(f'/api/users/{user_id}', timeout=5)
        response.raise_for_status()
        return response.json()
    except requests.Timeout:
        raise APITimeoutError(f"API timeout fetching user {user_id}")
    except requests.HTTPError as e:
        if e.response.status_code == 404:
            raise UserNotFoundError(f"User {user_id} not found")
        raise APIError(f"API returned {e.response.status_code}")

# Handle at call site where decision can be made
try:
    user = fetch_user_data(user_id)
except UserNotFoundError:
    return render_404_page()
except APITimeoutError:
    return render_error_page("Service temporarily unavailable")
```

### **3. Be Specific**
Use specific exceptions so caller knows what happened.

```python
# Bad: Generic exception
try:
    process_payment(amount)
except Exception as e:
    logger.error(f"Error: {e}")  # Could be anything!

# Good: Specific exceptions
try:
    process_payment(amount)
except InsufficientFundsError:
    charge_backup_payment_method()
except PaymentTimeoutError:
    retry_with_backoff()
except CardDeclinedError:
    notify_user("Your card was declined")
except PaymentServiceError:
    escalate_to_support()
```

### **4. Provide Context**
Errors should include information for debugging.

```python
# Bad: No context
raise ValueError("Invalid amount")

# Good: Clear context
raise ValueError(
    f"Invalid amount: {amount}. "
    f"Must be positive number, got {type(amount).__name__}"
)

# Better: Custom exception with all context
class InvalidAmountError(ValueError):
    def __init__(self, amount, reason):
        self.amount = amount
        self.reason = reason
        super().__init__(
            f"Invalid amount {amount}: {reason}"
        )

raise InvalidAmountError(amount=-10, reason="Amount cannot be negative")
```

### **5. Recovery When Possible**
Try to recover from errors gracefully.

```python
def fetch_config():
    """Fetch configuration with fallback."""
    try:
        # Try main source
        config = fetch_from_primary_server()
    except ConnectionError:
        logger.warning("Primary config server unavailable, using backup")
        config = fetch_from_backup_server()
    except Exception:
        logger.error("Both config servers failed, using defaults")
        config = DEFAULT_CONFIG
    
    return config
```

---

## Exception Design

### **Exception Hierarchy**

Design custom exceptions for your domain:

```python
# Bad: Using generic exceptions
class UserService:
    def get_user(self, user_id):
        if not user_id:
            raise ValueError("Invalid user_id")  # Too generic
        # ...

# Good: Custom exception hierarchy
class AppError(Exception):
    """Base exception for application."""
    pass

class ValidationError(AppError):
    """Raised when input validation fails."""
    pass

class UserError(AppError):
    """Base for user-related errors."""
    pass

class UserNotFoundError(UserError):
    """User does not exist."""
    pass

class DuplicateUserError(UserError):
    """User already exists."""
    pass

class PaymentError(AppError):
    """Base for payment errors."""
    pass

class InsufficientFundsError(PaymentError):
    """Account has insufficient funds."""
    pass

class CardDeclinedError(PaymentError):
    """Payment card was declined."""
    pass

# Usage
class UserService:
    def get_user(self, user_id):
        if not user_id:
            raise ValidationError("user_id is required")
        user = self.db.find(user_id)
        if not user:
            raise UserNotFoundError(f"User {user_id} not found")
        return user
```

### **Exception Attributes**

Include useful debugging information:

```python
class APIError(Exception):
    """Error communicating with external API."""
    
    def __init__(self, message, status_code=None, url=None, 
                 response=None, retry_after=None):
        super().__init__(message)
        self.status_code = status_code
        self.url = url
        self.response = response  # Full response for debugging
        self.retry_after = retry_after  # For rate limiting
        self.timestamp = datetime.now()

# Usage
try:
    response = requests.get(url)
    response.raise_for_status()
except requests.HTTPError as e:
    raise APIError(
        f"API request failed: {e.response.reason}",
        status_code=e.response.status_code,
        url=url,
        response=e.response.text,
        retry_after=e.response.headers.get('Retry-After')
    )
```

### **Exception Naming**

- End with "Error" or "Exception"
- Indicate the problem clearly
- Match language conventions

```python
# Good exception names
UserNotFoundError
InvalidEmailFormatError
DatabaseConnectionError
PaymentProcessingError
ConfigurationMissingError
RateLimitExceededError
```

---

## Error Recovery Patterns

### **Pattern 1: Retry with Exponential Backoff**

For transient failures:

```python
import time
from random import random

def retry_with_backoff(func, max_retries=5, base_delay=1):
    """Retry with exponential backoff and jitter."""
    for attempt in range(max_retries):
        try:
            return func()
        except TransientError as e:
            if attempt == max_retries - 1:
                raise  # Last attempt failed
            
            # Exponential backoff: 1s, 2s, 4s, 8s, 16s
            delay = base_delay * (2 ** attempt)
            # Add jitter to prevent thundering herd
            jitter = delay * random()
            wait_time = delay + jitter
            
            logger.warning(
                f"Attempt {attempt+1} failed: {e}. "
                f"Retrying in {wait_time:.1f}s"
            )
            time.sleep(wait_time)

# Usage
def fetch_data():
    return requests.get(url, timeout=5)

data = retry_with_backoff(fetch_data)
```

### **Pattern 2: Circuit Breaker**

Prevent cascading failures:

```python
from enum import Enum
from datetime import datetime, timedelta

class CircuitState(Enum):
    CLOSED = "closed"      # Normal operation
    OPEN = "open"          # Failing, reject requests
    HALF_OPEN = "half_open"  # Testing if recovered

class CircuitBreaker:
    def __init__(self, failure_threshold=5, timeout=60):
        self.failure_threshold = failure_threshold
        self.timeout = timeout  # Seconds before retry
        self.state = CircuitState.CLOSED
        self.failure_count = 0
        self.last_failure_time = None
    
    def call(self, func, *args, **kwargs):
        """Execute function with circuit breaker protection."""
        if self.state == CircuitState.OPEN:
            if datetime.now() - self.last_failure_time > timedelta(seconds=self.timeout):
                self.state = CircuitState.HALF_OPEN
                logger.info("Circuit breaker: entering half-open state")
            else:
                raise CircuitBreakerOpenError("Service unavailable")
        
        try:
            result = func(*args, **kwargs)
            self._on_success()
            return result
        except Exception as e:
            self._on_failure()
            raise
    
    def _on_success(self):
        self.failure_count = 0
        if self.state != CircuitState.CLOSED:
            self.state = CircuitState.CLOSED
            logger.info("Circuit breaker: closed")
    
    def _on_failure(self):
        self.failure_count += 1
        self.last_failure_time = datetime.now()
        if self.failure_count >= self.failure_threshold:
            self.state = CircuitState.OPEN
            logger.error(f"Circuit breaker: opened after {self.failure_count} failures")

# Usage
breaker = CircuitBreaker(failure_threshold=5, timeout=30)

def call_external_api():
    return requests.get("https://api.example.com/data")

try:
    data = breaker.call(call_external_api)
except CircuitBreakerOpenError:
    logger.error("External API unavailable, using cached data")
    data = get_cached_data()
```

### **Pattern 3: Fallback**

Use alternative when primary fails:

```python
def get_user_profile(user_id):
    """Get user profile with fallbacks."""
    try:
        # Try cache first (fastest)
        return cache.get(f"user:{user_id}")
    except CacheError:
        logger.debug("Cache miss for user profile")
    
    try:
        # Try database (fast)
        user = database.get_user(user_id)
        cache.set(f"user:{user_id}", user, expire=3600)
        return user
    except DatabaseError as e:
        logger.warning(f"Database error fetching user: {e}")
    
    try:
        # Try backup service (slower)
        user = backup_service.get_user(user_id)
        return user
    except ServiceError as e:
        logger.error(f"Backup service error: {e}")
    
    # Return default or raise
    logger.error(f"Could not fetch user {user_id}")
    raise UserUnavailableError(f"User {user_id} data unavailable")
```

---

## Logging Best Practices

### **Logging Levels**

| Level | When to Use | Example |
|-------|-----------|---------|
| **DEBUG** | Detailed diagnostic info | Variable values, function entry/exit |
| **INFO** | General informational | App startup, major operations |
| **WARNING** | Something unexpected but handled | Retry attempt, cache miss, deprecated API |
| **ERROR** | Error that affects functionality | Failed operation, exception caught |
| **CRITICAL** | System unusable | Database down, out of memory |

### **Examples**

```python
import logging

logger = logging.getLogger(__name__)

def process_payment(user_id, amount):
    logger.info(f"Processing payment: user={user_id}, amount=${amount}")
    
    try:
        # Check funds
        balance = get_balance(user_id)
        logger.debug(f"User balance: ${balance}")
        
        if balance < amount:
            logger.warning(
                f"Insufficient funds: user={user_id}, "
                f"required=${amount}, available=${balance}"
            )
            raise InsufficientFundsError()
        
        # Process charge
        result = charge_card(user_id, amount)
        logger.info(f"Payment successful: transaction_id={result.id}")
        
        return result
        
    except CardDeclinedError as e:
        logger.error(f"Card declined for user {user_id}: {e.reason}")
        raise
    except Exception as e:
        logger.critical(
            f"Unexpected error processing payment for user {user_id}",
            exc_info=True  # Include full traceback
        )
        raise
```

### **What to Log**

```python
✅ DO Log:
- Application startup/shutdown
- Configuration used
- User actions (login, logout)
- Error conditions and recoveries
- Performance metrics
- Security events
- State changes
- External API calls

❌ DON'T Log:
- Sensitive data (passwords, tokens, credit cards)
- Excessive detail for every line
- Third-party library debug output
- Internal implementation details
- Same information repeatedly
```

### **Structured Logging**

Use consistent format for easy parsing:

```python
import json
import logging

class JSONFormatter(logging.Formatter):
    def format(self, record):
        log_obj = {
            'timestamp': self.formatTime(record),
            'level': record.levelname,
            'logger': record.name,
            'message': record.getMessage(),
            'module': record.module,
            'function': record.funcName,
            'line': record.lineno,
        }
        
        # Add custom fields if present
        if hasattr(record, 'user_id'):
            log_obj['user_id'] = record.user_id
        if hasattr(record, 'request_id'):
            log_obj['request_id'] = record.request_id
        
        # Include exception if present
        if record.exc_info:
            log_obj['exception'] = self.formatException(record.exc_info)
        
        return json.dumps(log_obj)

# Usage
logger.info("User login", extra={'user_id': 123, 'request_id': 'req-456'})

# Output:
# {"timestamp": "2024-01-29 10:30:45", "level": "INFO", 
#  "message": "User login", "user_id": 123, "request_id": "req-456"}
```

### **Contextual Logging**

Maintain context across log messages:

```python
import contextvars

# Define context variables
request_id = contextvars.ContextVar('request_id', default=None)
user_id = contextvars.ContextVar('user_id', default=None)

class ContextFilter(logging.Filter):
    def filter(self, record):
        record.request_id = request_id.get()
        record.user_id = user_id.get()
        return True

# Setup
handler = logging.StreamHandler()
handler.addFilter(ContextFilter())
logger.addHandler(handler)

# Usage in request handler
def handle_request(request):
    request_id.set(request.id)
    user_id.set(request.user_id)
    
    logger.info("Request started")  # Includes context
    process_request(request)
    logger.info("Request completed")  # Same context
```

---

## Common Error Scenarios

### **Scenario 1: API Call Fails**

```python
def fetch_user_data(user_id):
    """Fetch user from external API with resilience."""
    try:
        # Try with timeout
        response = requests.get(
            f"{API_URL}/users/{user_id}",
            timeout=5,
            headers={'Authorization': f'Bearer {API_KEY}'}
        )
        response.raise_for_status()
        
        logger.info(f"Successfully fetched user {user_id}")
        return response.json()
        
    except requests.Timeout:
        logger.warning(f"API timeout fetching user {user_id}")
        # Try cached version
        cached = cache.get(f"user:{user_id}")
        if cached:
            logger.info("Using cached user data")
            return cached
        raise APITimeoutError("Could not fetch user")
        
    except requests.HTTPError as e:
        if e.response.status_code == 404:
            logger.info(f"User {user_id} not found")
            raise UserNotFoundError(f"User {user_id} doesn't exist")
        elif e.response.status_code == 401:
            logger.error("API authentication failed - check key")
            raise APIAuthenticationError()
        else:
            logger.error(f"API returned {e.response.status_code}: {e.response.text}")
            raise APIError(f"API error: {e.response.status_code}")
            
    except Exception as e:
        logger.critical(f"Unexpected error fetching user: {e}", exc_info=True)
        raise
```

### **Scenario 2: Database Operation Fails**

```python
def update_user_email(user_id, new_email):
    """Update user email with validation."""
    try:
        # Validate input
        if not is_valid_email(new_email):
            raise ValidationError(f"Invalid email: {new_email}")
        
        logger.debug(f"Updating email for user {user_id}")
        
        # Check for duplicates
        existing = User.find_by_email(new_email)
        if existing and existing.id != user_id:
            logger.warning(
                f"Email {new_email} already exists for user {existing.id}"
            )
            raise DuplicateEmailError(f"Email {new_email} is already in use")
        
        # Update
        user = User.find(user_id)
        if not user:
            logger.error(f"User {user_id} not found")
            raise UserNotFoundError(f"User {user_id} not found")
        
        user.email = new_email
        user.save()
        
        # Clear cache
        cache.delete(f"user:{user_id}")
        
        logger.info(f"Updated email for user {user_id}")
        return user
        
    except DatabaseConnectionError as e:
        logger.error(f"Database connection failed: {e}")
        raise ServiceUnavailableError("Database unavailable")
        
    except IntegrityError as e:
        # Database-level constraint violation
        logger.error(f"Integrity violation: {e}")
        raise DuplicateEmailError("Email already exists")
        
    except Exception as e:
        logger.critical(f"Unexpected error updating user: {e}", exc_info=True)
        raise
```

### **Scenario 3: Parsing/Validation Fails**

```python
def parse_order(order_json):
    """Parse and validate order from JSON."""
    try:
        # Parse JSON
        order = json.loads(order_json)
        logger.debug(f"Parsed order JSON: {order}")
        
        # Validate structure
        required_fields = ['user_id', 'items', 'total']
        missing = [f for f in required_fields if f not in order]
        if missing:
            raise ValidationError(f"Missing fields: {missing}")
        
        # Validate user
        if not isinstance(order['user_id'], int) or order['user_id'] <= 0:
            raise ValidationError(f"Invalid user_id: {order['user_id']}")
        
        # Validate items
        if not isinstance(order['items'], list) or len(order['items']) == 0:
            raise ValidationError("Order must contain items")
        
        for idx, item in enumerate(order['items']):
            if 'product_id' not in item or 'quantity' not in item:
                raise ValidationError(
                    f"Item {idx} missing required fields"
                )
        
        logger.info(f"Validated order with {len(order['items'])} items")
        return order
        
    except json.JSONDecodeError as e:
        logger.error(f"Invalid JSON: {e}")
        raise ValidationError(f"Invalid JSON format: {e}")
        
    except ValidationError as e:
        logger.warning(f"Order validation failed: {e}")
        raise  # Re-raise for caller to handle
        
    except Exception as e:
        logger.critical(f"Unexpected error parsing order: {e}", exc_info=True)
        raise
```

---

## Debugging Techniques

### **1. Use Logging for Debugging**

```python
def complex_calculation(values):
    logger.debug(f"Input values: {values}")
    
    filtered = [v for v in values if v > 0]
    logger.debug(f"After filtering: {filtered}")
    
    result = sum(filtered)
    logger.debug(f"Final result: {result}")
    
    return result
```

### **2. Add Diagnostic Context**

```python
def process_with_diagnostics(data):
    diagnostics = {
        'input_size': len(data),
        'start_time': time.time(),
    }
    
    try:
        result = process(data)
        diagnostics['success'] = True
        diagnostics['result_size'] = len(result)
    except Exception as e:
        diagnostics['success'] = False
        diagnostics['error'] = str(e)
        raise
    finally:
        diagnostics['duration'] = time.time() - diagnostics['start_time']
        logger.info(f"Processing diagnostics: {diagnostics}")
```

### **3. Add Trace IDs**

Track requests through entire system:

```python
import uuid

class RequestTracer:
    def __init__(self):
        self.trace_id = None
    
    def set_trace_id(self, trace_id=None):
        self.trace_id = trace_id or str(uuid.uuid4())
        contextvars.trace_id.set(self.trace_id)
        return self.trace_id
    
    def log(self, message, level=logging.INFO):
        logger.log(level, f"[{self.trace_id}] {message}")

# Usage in web request
tracer = RequestTracer()
tracer.set_trace_id(request.headers.get('X-Trace-ID'))
tracer.log("Processing request")
# External service call will include trace_id in headers
# Log files can be searched by trace_id
```

---

## Error Handling Anti-Patterns

### ❌ **Anti-Pattern 1: Silent Failure**

```python
# Bad: Exception is hidden
try:
    process_payment()
except Exception:
    pass  # What?!

# Good: Handle appropriately
try:
    process_payment()
except InsufficientFundsError:
    notify_user("Insufficient funds")
except PaymentError as e:
    logger.error(f"Payment failed: {e}")
    notify_user("Payment processing error")
```

### ❌ **Anti-Pattern 2: Catching Too Broadly**

```python
# Bad: Catches everything including programming errors
try:
    data = parse_response(response)
    user = database.get_user(data['id'])
    user.email = data['email']
except Exception:
    logger.error("Error processing user")  # Could be anything!

# Good: Catch specific exceptions
try:
    data = parse_response(response)
except ValidationError as e:
    logger.error(f"Invalid response: {e}")
    
try:
    user = database.get_user(data['id'])
except UserNotFoundError:
    logger.error(f"User not found: {data['id']}")
```

### ❌ **Anti-Pattern 3: Swallowing Stack Traces**

```python
# Bad: Loses debugging information
except Exception as e:
    logger.error(str(e))  # Stack trace lost!

# Good: Preserve stack trace
except Exception as e:
    logger.error("Error occurred", exc_info=True)  # Includes traceback
    # or
    logger.exception("Error occurred")  # Same as exc_info=True
```

### ❌ **Anti-Pattern 4: Excessive Logging**

```python
# Bad: Logs on every iteration, too verbose
for item in items:
    logger.debug(f"Processing {item}")
    try:
        result = process(item)
        logger.debug(f"Result: {result}")
    except Exception as e:
        logger.debug(f"Error: {e}")

# Good: Log summary instead
logger.debug(f"Processing {len(items)} items")
errors = []
for item in items:
    try:
        result = process(item)
    except Exception as e:
        errors.append((item, e))

if errors:
    logger.warning(f"Failed to process {len(errors)}/{len(items)} items")
```

---

## Monitoring and Alerting

### **Key Metrics to Monitor**

```python
# Error rate
errors_per_minute = count(ERROR logs) / minutes

# Response times
p95_response_time = percentile(response_times, 95)
p99_response_time = percentile(response_times, 99)

# Availability
uptime_percentage = successful_requests / total_requests

# Resource usage
memory_usage = get_memory_usage()
cpu_usage = get_cpu_usage()

# Business metrics
failed_payments = count("Payment failed")
rate_limited = count("Rate limit exceeded")
```

### **Alert Rules**

```yaml
alerts:
  - name: high_error_rate
    condition: error_rate > 5%
    severity: critical
    
  - name: slow_response
    condition: p95_response_time > 1000ms
    severity: warning
    
  - name: service_down
    condition: uptime < 99%
    severity: critical
    
  - name: payment_failures
    condition: failed_payments > 10 in 5_minutes
    severity: high
```

---

## Checklist

- [ ] Custom exception hierarchy defined
- [ ] Specific exceptions used, not generic
- [ ] Errors logged with appropriate level
- [ ] Sensitive data not logged
- [ ] Stack traces preserved for debugging
- [ ] Contextual information included in logs
- [ ] Retry logic with backoff for transient failures
- [ ] Circuit breaker for failing services
- [ ] Fallback strategies implemented
- [ ] Errors provide actionable information
- [ ] Monitoring and alerting in place
- [ ] No silent failures
- [ ] Error messages clear for users
- [ ] Developer errors vs user errors distinguished
- [ ] Graceful degradation when possible

---

## References

- [Python Logging Documentation](https://docs.python.org/3/library/logging.html)
- [The Twelve-Factor App - Logs](https://12factor.net/logs)
- [Structured Logging](https://www.kartar.net/2015/12/structured-logging/)
- [Release It! - Michael Nygard](https://pragprog.com/titles/mnee2/release-it-second-edition/)
- [Error Handling Best Practices](https://www.baeldung.com/java-exceptions)
- [Circuit Breaker Pattern](https://martinfowler.com/bliki/CircuitBreaker.html)
