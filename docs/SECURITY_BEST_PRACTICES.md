# Security Best Practices Guide

A comprehensive guide to building secure applications across the full stack. This covers OWASP Top 10 vulnerabilities, authentication and authorization, secrets management, secure coding practices, dependency management, and security testing.

---

## Why Security Matters

### Impact on Business
- **Compliance**: Regulatory requirements (GDPR, HIPAA, PCI-DSS, SOC 2)
- **Reputation**: Breaches destroy customer trust
- **Financial**: Fines, incident response, legal costs
- **Operations**: Downtime from attacks, recovery costs
- **Customer Data**: Responsibility to protect user information

### Real-World Impact
- Equifax breach: 147M records, $700M settlement
- Target breach: 40M credit card records, $18.5M settlement
- Facebook/Cambridge Analytica: 87M records misused
- Colonial Pipeline ransomware: $5M ransom, supplies disrupted

---

## OWASP Top 10 Vulnerabilities

### **1. Injection (SQL, NoSQL, Command)**

Untrusted input becomes code.

```python
# Bad: SQL Injection
user_id = request.args.get('id')
query = f"SELECT * FROM users WHERE id = {user_id}"
# If user_id = "1 OR 1=1" — returns all users!
# If user_id = "1; DROP TABLE users;" — deletes table!

# Good: Parameterized queries
query = "SELECT * FROM users WHERE id = ?"
cursor.execute(query, (user_id,))  # ID treated as data, not code

# Bad: NoSQL Injection
db.users.find({"username": username, "password": password})
# If username = {"$ne": ""} — finds any non-empty username!

# Good: Type checking
if not isinstance(username, str) or not isinstance(password, str):
    raise ValueError("Invalid input")
db.users.find({"username": username, "password": password})

# Bad: Command Injection
filename = request.args.get('file')
os.system(f"rm {filename}")  # If filename = "*.txt; rm -rf /" — disaster!

# Good: Avoid shell execution
import shutil
shutil.rmtree(filename)  # Direct API, not shell
```

### **2. Broken Authentication**

Weak or missing authentication controls.

```python
# Bad: Hardcoded credentials
DB_PASSWORD = "admin123"  # In source code!
api_key = "sk_live_abcdef123456"  # Exposed!

# Good: Environment variables
import os
DB_PASSWORD = os.getenv('DATABASE_PASSWORD')
API_KEY = os.getenv('API_KEY')

# Bad: Plain text passwords
password = request.form['password']
user.password = password
db.save(user)

# Good: Hash passwords
import bcrypt
password = request.form['password']
password_hash = bcrypt.hashpw(password.encode(), bcrypt.gensalt())
user.password_hash = password_hash
db.save(user)

# Bad: No session timeout
session['logged_in'] = True  # Never expires

# Good: Session expiration
session['logged_in'] = True
session.set_expiry(3600)  # 1 hour timeout

# Bad: Simple password requirements
# "Must be at least 4 characters"

# Good: Strong password policy
# Must be 12+ characters with mix of:
# - Uppercase (A-Z)
# - Lowercase (a-z)
# - Numbers (0-9)
# - Special characters (!@#$%^&*)
```

### **3. Sensitive Data Exposure**

Protecting data in transit and at rest.

```python
# Bad: HTTP instead of HTTPS
response = requests.get('http://api.example.com/users/123')

# Good: HTTPS enforced
response = requests.get('https://api.example.com/users/123', verify=True)

# Bad: Sensitive data in logs
logger.info(f"Processing payment with card {card_number}")
# Card number appears in logs!

# Good: Mask sensitive data
masked_card = card_number[-4:].rjust(len(card_number), '*')
logger.info(f"Processing payment with card {masked_card}")

# Bad: Unencrypted database storage
user.ssn = "123-45-6789"
db.save(user)

# Good: Encrypt sensitive fields
from cryptography.fernet import Fernet
cipher = Fernet(ENCRYPTION_KEY)
encrypted_ssn = cipher.encrypt(ssn.encode())
user.ssn = encrypted_ssn
db.save(user)

# Bad: API keys in responses
return {"api_key": user.api_key, "data": data}

# Good: Never return sensitive data
return {"user_id": user.id, "data": data}
```

### **4. XML External Entities (XXE)**

Parsing untrusted XML enables attacks.

```python
# Bad: Unsafe XML parsing
import xml.etree.ElementTree as ET
root = ET.parse(untrusted_xml)  # Vulnerable to XXE

# Good: Disable external entities
import defusedxml.ElementTree as ET
root = ET.parse(untrusted_xml)  # Safe

# Or configure explicitly
parser = ET.XMLParser()
parser.entity = {}  # No external entities
root = ET.parse(untrusted_xml, parser)
```

### **5. Broken Access Control**

Users can access unauthorized resources.

```python
# Bad: No authorization check
@app.route('/api/users/<int:user_id>')
def get_user(user_id):
    user = User.find(user_id)
    return user.to_dict()

# Anyone can access any user's profile!

# Good: Authorization check
@app.route('/api/users/<int:user_id>')
@login_required
def get_user(user_id):
    current_user = get_authenticated_user()
    if current_user.id != user_id and not current_user.is_admin:
        return {"error": "Forbidden"}, 403
    user = User.find(user_id)
    return user.to_dict()

# Bad: Client-side authorization
if user.role == 'admin':
    show_delete_button()  # Easy to bypass!

# Good: Server-side authorization
@app.route('/api/users/<int:user_id>', methods=['DELETE'])
@login_required
def delete_user(user_id):
    current_user = get_authenticated_user()
    if not current_user.is_admin:
        return {"error": "Forbidden"}, 403
    User.delete(user_id)
    return {}, 204
```

### **6. Security Misconfiguration**

Insecure defaults, unnecessary features, unpatched systems.

```python
# Bad: Debug mode enabled in production
DEBUG = True
if request.args.get('__debug__'):
    # Shows full error details, code
    raise Exception("Debug info exposed!")

# Good: Debug mode disabled in production
DEBUG = os.getenv('FLASK_ENV') != 'production'

# Bad: Default credentials
database_user = "admin"
database_password = "admin"

# Good: Change defaults
database_user = os.getenv('DATABASE_USER')
database_password = os.getenv('DATABASE_PASSWORD')

# Bad: All HTTP methods enabled
@app.route('/api/data', methods=['GET', 'POST', 'PUT', 'DELETE', 'PATCH'])
def handle_data():
    return data

# Good: Only necessary methods
@app.route('/api/data', methods=['GET', 'POST'])
def handle_data():
    return data

# Bad: Security headers missing
response = flask.Response()
# No security headers!

# Good: Include security headers
@app.after_request
def set_security_headers(response):
    response.headers['Strict-Transport-Security'] = 'max-age=31536000'
    response.headers['X-Content-Type-Options'] = 'nosniff'
    response.headers['X-Frame-Options'] = 'DENY'
    response.headers['X-XSS-Protection'] = '1; mode=block'
    response.headers['Referrer-Policy'] = 'strict-origin-when-cross-origin'
    return response
```

### **7. Cross-Site Scripting (XSS)**

Injected scripts execute in users' browsers.

```python
# Bad: Unescaped user input in HTML
user_input = request.args.get('comment')
html = f"<p>Comment: {user_input}</p>"
# If input = "<script>alert('xss')</script>" — executes!

# Good: Escape user input
from markupsafe import escape
user_input = request.args.get('comment')
html = f"<p>Comment: {escape(user_input)}</p>"

# Bad: Unescaped JSON in script
user_data = request.args.get('data')
html = f"<script>var data = {user_data}</script>"
# If data = "'); alert('xss'); ('" — injects code!

# Good: Use json.dumps for safe serialization
import json
user_data = json.loads(request.args.get('data'))
html = f"<script>var data = {json.dumps(user_data)}</script>"

# Bad: Setting innerHTML with user content
element.innerHTML = user_comment  # Executes scripts!

# Good: Use textContent for user data
element.textContent = user_comment  # Safe
# Or use templating engine that escapes
```

### **8. Insecure Deserialization**

Untrusted data executed during deserialization.

```python
# Bad: Pickle untrusted data
import pickle
user_data = request.args.get('data')
user = pickle.loads(user_data)  # Can execute arbitrary code!

# Good: Use JSON for untrusted data
import json
user_data = request.args.get('data')
user = json.loads(user_data)  # Only creates data structures

# If using pickle, sign the data
import pickle
import hmac
import hashlib

def sign_pickle(obj, secret):
    pickled = pickle.dumps(obj)
    signature = hmac.new(secret.encode(), pickled, hashlib.sha256).digest()
    return pickle.dumps((pickled, signature))

def verify_pickle(data, secret):
    pickled, signature = pickle.loads(data)
    expected = hmac.new(secret.encode(), pickled, hashlib.sha256).digest()
    if not hmac.compare_digest(signature, expected):
        raise ValueError("Invalid signature")
    return pickle.loads(pickled)
```

### **9. Using Components with Known Vulnerabilities**

Outdated dependencies with security issues.

```bash
# Bad: Ignoring security warnings
$ pip list
Django 3.0.0  # Security vulnerability in versions < 3.2.0!

# Good: Regular updates
$ pip install --upgrade django
$ pip audit  # Check for known vulnerabilities

# Use specific versions
requirements.txt:
# Bad:
requests  # Gets any version, might be vulnerable
django   # Gets any version

# Good:
requests==2.28.1
django==4.2.0  # Specific, verified version

# Regularly update
pip install --upgrade -r requirements.txt
pip audit  # Reports vulnerabilities
```

### **10. Insufficient Logging and Monitoring**

Can't detect or investigate attacks.

```python
# Bad: No logging
def process_payment(amount):
    charge_card(amount)
    update_balance(amount)
    # No record of what happened!

# Good: Comprehensive logging
def process_payment(amount, user_id):
    logger.info(f"Payment initiated: user={user_id}, amount=${amount}")
    try:
        charge_card(amount)
        logger.info(f"Charge succeeded: user={user_id}, amount=${amount}")
    except Exception as e:
        logger.error(f"Charge failed: user={user_id}, error={e}")
        raise
    
    # Monitor for anomalies
    alert_if(payment_failure_rate > 10%)
    alert_if(unusual_amount(amount))
```

---

## Authentication & Authorization

### **Authentication Methods**

| Method | Security | Use Case |
|--------|----------|----------|
| **Username/Password** | Moderate | Internal systems, websites |
| **API Key** | Low-moderate | Service-to-service, trusted clients |
| **OAuth 2.0** | High | Third-party access |
| **SAML 2.0** | High | Enterprise SSO |
| **Multi-Factor** | Very High | High-security accounts |

### **Best Practices**

```python
# Multi-factor authentication
def verify_mfa(user, code):
    stored_secret = user.mfa_secret
    # TOTP: Time-based One-Time Password
    import pyotp
    totp = pyotp.TOTP(stored_secret)
    return totp.verify(code)

# Password reset securely
def request_password_reset(email):
    user = User.find_by_email(email)
    if not user:
        return  # Don't reveal if email exists
    
    # Generate short-lived token
    token = secrets.token_urlsafe(32)
    expires_at = datetime.now() + timedelta(hours=1)
    
    store_reset_token(user.id, hash_token(token), expires_at)
    send_email(email, f"Click to reset: {url_with_token}")

# Validate token
def reset_password(token, new_password):
    stored_hash = lookup_reset_token(token)
    if not stored_hash or stored_hash.expires_at < datetime.now():
        raise InvalidTokenError("Token expired or invalid")
    
    if not hash_token(token) == stored_hash.hash:
        raise InvalidTokenError("Invalid token")
    
    user = stored_hash.user
    user.password = hash_password(new_password)
    user.save()
```

### **Authorization Patterns**

```python
# Role-based access control (RBAC)
def require_role(*roles):
    def decorator(func):
        def wrapper(*args, **kwargs):
            user = get_current_user()
            if user.role not in roles:
                raise PermissionError("Insufficient role")
            return func(*args, **kwargs)
        return wrapper
    return decorator

@app.route('/api/admin/users')
@require_role('admin')
def get_users():
    return User.all()

# Attribute-based access control (ABAC)
def can_edit_order(user, order):
    # User owns order
    if order.user_id == user.id:
        return True
    # Admin can edit
    if user.role == 'admin':
        return True
    # Manager can edit orders in their region
    if user.role == 'manager' and user.region == order.region:
        return True
    return False

@app.route('/api/orders/<int:order_id>', methods=['PUT'])
@login_required
def update_order(order_id):
    order = Order.find(order_id)
    if not can_edit_order(get_current_user(), order):
        return {"error": "Forbidden"}, 403
    # Update order
```

---

## Secrets Management

### **What Are Secrets?**

```python
✓ Passwords, API keys, tokens
✓ Database credentials
✓ Encryption keys
✓ Private signing keys
✓ OAuth client secrets
✓ Payment gateway credentials
```

### **Never Do This**

```python
# ❌ Hardcode in source
api_key = "sk_live_abcdef123456"

# ❌ Commit to git
# .env file with:
# DATABASE_PASSWORD=admin123

# ❌ Plain text in configuration
config = """
db_password: admin123
api_key: secret_key
"""

# ❌ Deploy with defaults
docker run my-app  # Assumes default secrets
```

### **Use Secrets Management**

```python
# ✅ Environment variables
import os

API_KEY = os.getenv('API_KEY')
if not API_KEY:
    raise ValueError("API_KEY environment variable required")

DATABASE_URL = os.getenv('DATABASE_URL')

# ✅ Secrets manager (AWS Secrets Manager, HashiCorp Vault)
import boto3

secrets_client = boto3.client('secretsmanager')
secret = secrets_client.get_secret_value(SecretId='prod/database/password')
db_password = secret['SecretString']

# ✅ .env file (development only, never commit)
# .env (in .gitignore)
# API_KEY=dev_key_12345
# DATABASE_PASSWORD=dev_password

# Load in development
from dotenv import load_dotenv
load_dotenv()

# ✅ Docker secrets (production)
# Pass at runtime:
docker run -e API_KEY=$API_KEY -e DATABASE_PASSWORD=$DB_PASSWORD my-app

# ✅ Kubernetes secrets
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
data:
  api-key: c2tfbGl2ZV9hYmNkZWYxMjM0NTY=  # base64 encoded
  db-password: YWRtaW4xMjM=

# Access in pod
env:
  - name: API_KEY
    valueFrom:
      secretKeyRef:
        name: app-secrets
        key: api-key
```

### **Key Rotation**

```python
# ✅ Plan for rotation
# - Change secrets regularly (monthly, quarterly)
# - Support multiple versions during transition
# - Automated alerts for expiring keys

# ✅ Rolling update
v1_key = old_key
v2_key = new_key

# Accept both during rotation period
if token_matches(v1_key) or token_matches(v2_key):
    authenticate(user)

# After rotation complete, deprecate v1_key
```

---

## Input Validation & Sanitization

### **Always Validate Input**

```python
# Bad: Trust user input
def create_order(quantity):
    price = PRODUCT_PRICE
    total = price * quantity
    return total

# If quantity = -1000: customer gets paid!

# Good: Validate input
def create_order(quantity):
    # Type check
    if not isinstance(quantity, int):
        raise ValueError("Quantity must be integer")
    
    # Range check
    if not 1 <= quantity <= 1000:
        raise ValueError("Quantity must be 1-1000")
    
    price = PRODUCT_PRICE
    total = price * quantity
    return total
```

### **Whitelist Approach**

```python
# Bad: Blacklist approach (what you don't allow)
def sanitize_filename(filename):
    return filename.replace('..', '')  # Only remove one bad pattern
    # Still vulnerable: /etc/passwd, ../../etc/passwd, etc.

# Good: Whitelist approach (what you do allow)
import re

def sanitize_filename(filename):
    # Only allow alphanumeric, dash, underscore, dot
    if not re.match(r'^[a-zA-Z0-9._-]+$', filename):
        raise ValueError("Invalid filename")
    
    # Prevent path traversal
    if '/' in filename or '\\' in filename:
        raise ValueError("Path separators not allowed")
    
    return filename

# Better: Use dedicated library
from pathlib import Path

def safe_join_path(base, filename):
    # Ensures filename can't escape base directory
    return (Path(base) / filename).resolve()
```

### **Type Coercion**

```python
# Bad: Weak typing
def process_payment(amount):
    # If amount = "100; DELETE FROM users;" — could inject code
    charge = amount * 1.1  # If string: "100; DELETE FROM users;1.1"
    return charge

# Good: Strict typing
def process_payment(amount: float):
    if not isinstance(amount, (int, float)):
        raise TypeError("Amount must be number")
    if amount <= 0:
        raise ValueError("Amount must be positive")
    charge = amount * 1.1
    return charge
```

---

## Secure Coding Practices

### **Principle of Least Privilege**

```python
# Bad: Unnecessary permissions
def process_data():
    # Runs as root, has access to everything
    return expensive_operation()

# Good: Minimal permissions
def process_data():
    # Runs as unprivileged user
    # Only has access to necessary resources
    return expensive_operation()

# In containers
FROM python:3.11
# Bad
USER root

# Good
RUN useradd -m appuser
USER appuser
```

### **Defense in Depth**

```python
def process_sensitive_operation():
    # Multiple layers of defense
    
    # 1. Authentication: User is logged in
    user = get_authenticated_user()
    if not user:
        raise AuthenticationError("Not authenticated")
    
    # 2. Authorization: User has permission
    if not user.can_perform_operation():
        raise PermissionError("Insufficient permissions")
    
    # 3. Validation: Input is valid
    if not validate_input(data):
        raise ValidationError("Invalid input")
    
    # 4. Rate limiting: Not abused
    if is_rate_limited(user):
        raise RateLimitError("Too many requests")
    
    # 5. Encryption: Data protected
    encrypted_data = encrypt(sensitive_data)
    
    # 6. Logging: Track the action
    logger.info(f"Sensitive operation by {user.id}")
    
    # 7. Monitoring: Detect anomalies
    if is_suspicious(operation):
        alert_security_team()
    
    return perform_operation()
```

### **Fail Securely**

```python
# Bad: Fail open (insecure)
try:
    verify_signature(message)
except Exception:
    # Authentication failed, but proceeding anyway!
    return data

# Good: Fail closed (secure)
try:
    verify_signature(message)
except SignatureError:
    raise AuthenticationError("Invalid signature")
```

---

## Dependency Management

### **Keep Dependencies Updated**

```bash
# Check for vulnerabilities
$ pip audit
Found 2 vulnerabilities in 2 packages
Name: requests
Version: 2.27.0
Severity: LOW
Fix: 2.28.0

Name: django
Version: 3.1.0
Severity: HIGH
Fix: 3.2.11

# Update packages
$ pip install --upgrade requests django

# Automated updates (dependabot, renovate)
# Automatically opens PRs for updates
```

### **Use Dependency Locks**

```
requirements.txt (version ranges)
requests>=2.25.0,<3.0.0
django==4.0.0

requirements.lock (pinned versions)
requests==2.28.1
django==4.2.0
certifi==2023.5.7
...

# Generate lock
pip freeze > requirements.lock

# Install from lock (reproducible)
pip install -r requirements.lock
```

### **Audit Dependencies**

```bash
# List all dependencies
pip tree

# Check for vulnerabilities
pip audit

# Check specific package
pip show requests

# Security Advisories
# - National Vulnerability Database (NVD)
# - GitHub Security Advisories
# - PyPI Security
```

---

## Security Testing

### **Vulnerability Testing**

```bash
# Static analysis (code without running)
$ bandit -r src/
>> Issue: Hardcoded SQL string allows SQL injection
   Severity: HIGH
   Location: src/db.py:45

# Dynamic analysis (running code)
$ owasp-zap run -t https://localhost:8000
>> XSS vulnerability found
>> SQL injection possible
>> Missing security headers

# Dependency scanning
$ safety check
>> Vulnerability in requests 2.27.0
   ID: 42599
   Safe version: 2.28.0
```

### **Testing Checklist**

```
Authentication
- ✓ Can log in with valid credentials
- ✓ Cannot log in with invalid credentials
- ✓ Session expires after inactivity
- ✓ Cannot reuse expired token
- ✓ Password hashed, not stored as plain text

Authorization
- ✓ User can't access other user's data
- ✓ User can't perform admin actions
- ✓ API enforces permissions, not just frontend

Input Validation
- ✓ SQL injection attempts blocked
- ✓ XSS attempts blocked
- ✓ Invalid data types rejected
- ✓ Out-of-range values rejected
- ✓ File uploads validated

Data Protection
- ✓ HTTPS enforced
- ✓ Sensitive data encrypted
- ✓ Sensitive data not in logs
- ✓ Secrets not in source code
- ✓ Database access restricted

Error Handling
- ✓ Errors don't expose sensitive info
- ✓ Errors don't show stack traces to users
- ✓ Errors logged for investigation
```

---

## Incident Response

### **Preparation**

```
- Identify security contacts
- Document response procedures
- Test incident response plan
- Have incident response checklist
- Know how to gather evidence
```

### **Stages**

```
1. DETECT
   - Alert received
   - Confirm it's real incident
   - Activate incident response team

2. CONTAIN
   - Isolate affected systems
   - Stop active attack
   - Preserve evidence
   - Notify stakeholders

3. INVESTIGATE
   - Determine scope
   - Find root cause
   - Identify affected data
   - Gather evidence for legal

4. ERADICATE
   - Remove attacker access
   - Patch vulnerabilities
   - Fix misconfigurations
   - Verify cleanup

5. RECOVER
   - Restore systems
   - Verify functionality
   - Monitor for re-entry

6. IMPROVE
   - Post-mortem analysis
   - Fix root causes
   - Update defenses
   - Improve detection
```

---

## Checklist

- [ ] OWASP Top 10 understood and mitigated
- [ ] Input validation on all user input
- [ ] SQL/NoSQL injection protection (parameterized queries)
- [ ] XSS protection (output encoding)
- [ ] CSRF tokens on state-changing operations
- [ ] Strong password requirements enforced
- [ ] Passwords hashed with bcrypt/scrypt/argon2
- [ ] Multi-factor authentication available
- [ ] HTTPS enforced everywhere
- [ ] Security headers set (HSTS, CSP, X-Frame-Options)
- [ ] Secrets managed via environment or secrets manager
- [ ] Secrets never in source code or logs
- [ ] API authentication and authorization enforced
- [ ] Rate limiting implemented
- [ ] Sensitive data encrypted
- [ ] Dependencies kept up to date
- [ ] Security tests in CI/CD
- [ ] Logging for security events
- [ ] Monitoring and alerting for attacks
- [ ] Incident response plan documented
- [ ] Data backups and disaster recovery tested
- [ ] Security training provided to team

---

## References

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP Testing Guide](https://owasp.org/www-project-web-security-testing-guide/)
- [Security Guide for Developers](https://cheatsheetseries.owasp.org/)
- [CWE: Common Weakness Enumeration](https://cwe.mitre.org/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [The Web Security Testing Guide](https://owasp.org/www-project-web-security-testing-guide/)
- [Secure Coding Guidelines](https://www.securecoding.cert.org/)
- [API Security Top 10](https://owasp.org/www-project-api-security/)
- [Cloud Security Best Practices](https://www.cisecurity.org/)
