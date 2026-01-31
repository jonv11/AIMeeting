# API Design Conventions Guide

A comprehensive guide to designing APIs that are consistent, intuitive, and maintainable. This covers REST principles, endpoint design, HTTP semantics, versioning strategies, security, and documentation standards.

---

## Why API Design Matters

### Impact on Development
- **Developer Experience**: Consistent, predictable APIs are easy to use
- **Adoption**: Clear APIs attract users and integrators
- **Maintenance**: Well-designed APIs are easier to evolve
- **Debugging**: Clear semantics help identify issues
- **Scalability**: Good design patterns support growth
- **Security**: Consistent patterns prevent vulnerabilities
- **Documentation**: Good design self-documents

### Cost of Poor API Design
- Developers waste time learning inconsistent patterns
- Confusion leads to misuse and security issues
- Breaking changes force all clients to update
- Inconsistent error formats difficult to handle
- Hard to extend without breaking existing clients
- Support burden from confused users

---

## REST Principles

### **1. Client-Server Architecture**
API and client are independent, communicate via standard protocols.

```
Client                          Server
  |                              |
  |------ HTTP Request -------->  |
  |                              |
  |<----- HTTP Response ---------|
  |                              |

Benefits:
- Server can evolve independently
- Multiple client types (web, mobile, CLI)
- Horizontal scaling possible
```

### **2. Resource-Oriented Design**
Model API around resources (nouns), not actions (verbs).

```python
# Bad: Action-oriented (RPC-style)
POST /createUser
POST /getUser
POST /updateUser
POST /deleteUser

# Good: Resource-oriented (REST-style)
POST   /users              # Create user
GET    /users/123          # Get user 123
PUT    /users/123          # Replace user 123
PATCH  /users/123          # Update user 123
DELETE /users/123          # Delete user 123
```

### **3. Standard HTTP Methods**

| Method | Semantics | Safe | Idempotent | Example |
|--------|-----------|------|------------|---------|
| **GET** | Retrieve | ✅ | ✅ | Get user details |
| **POST** | Create | ❌ | ❌ | Create new user |
| **PUT** | Replace entire resource | ❌ | ✅ | Replace user |
| **PATCH** | Partial update | ❌ | ❌* | Update user email only |
| **DELETE** | Remove | ❌ | ✅ | Delete user |
| **HEAD** | Like GET but no body | ✅ | ✅ | Check if resource exists |
| **OPTIONS** | Describe communication options | ✅ | ✅ | CORS preflight |

*PATCH can be idempotent depending on implementation

### **4. Status Code Semantics**

Use correct HTTP status codes for clarity:

```
2xx Success
  200 OK - Request succeeded
  201 Created - Resource created
  202 Accepted - Request accepted, processing
  204 No Content - Success, no response body

3xx Redirection
  301 Moved Permanently - Resource moved
  302 Found - Temporary redirect
  304 Not Modified - Use cached version

4xx Client Error
  400 Bad Request - Invalid parameters
  401 Unauthorized - Authentication required
  403 Forbidden - Authenticated but not allowed
  404 Not Found - Resource doesn't exist
  409 Conflict - State conflict
  422 Unprocessable Entity - Validation error
  429 Too Many Requests - Rate limited

5xx Server Error
  500 Internal Server Error - Server error
  502 Bad Gateway - Upstream problem
  503 Service Unavailable - Temporarily down
```

### **5. Statelessness**
Each request contains all information needed; server doesn't rely on stored context.

```python
# Bad: Server maintains session state
@app.route('/users/<int:user_id>/update', methods=['POST'])
def update_user(user_id):
    # Assumes session['user_id'] was set by login
    if session.get('user_id') != user_id:
        return "Unauthorized", 401
    # ...

# Good: Client provides credentials each request
@app.route('/api/users/<int:user_id>', methods=['PUT'])
@require_auth()  # Token in Authorization header
def update_user(user_id):
    current_user = get_user_from_token(request.headers)
    if current_user.id != user_id:
        return {"error": "Forbidden"}, 403
    # ...
```

---

## Endpoint Design

### **Naming Conventions**

```
✅ Good:
/api/users                          # Collection
/api/users/123                      # Specific resource
/api/users/123/orders               # Nested resource
/api/users/123/orders/456           # Nested specific resource
/api/v2/users                       # Versioned

❌ Bad:
/api/user                           # Inconsistent singular/plural
/api/users/getById/123              # Action in URL
/api/users/123/order_details        # Inconsistent naming
/api/USERS                          # Inconsistent casing
/api/users-list                     # Mixed conventions
```

### **Collections vs Resources**

```python
# Collection endpoints (plural, operate on multiple)
GET    /api/products                # List all products
POST   /api/products                # Create product
DELETE /api/products                # Delete all (rare)

# Resource endpoints (specific item)
GET    /api/products/123            # Get specific product
PUT    /api/products/123            # Replace specific product
PATCH  /api/products/123            # Update specific product
DELETE /api/products/123            # Delete specific product
```

### **Query Parameters**

```python
# Filtering
GET /api/products?category=electronics&brand=sony

# Pagination
GET /api/products?page=2&limit=20
GET /api/products?offset=40&limit=20  # Offset-based
GET /api/products?after=eyJpZCI6IDEyM30  # Cursor-based (more efficient)

# Sorting
GET /api/products?sort=price,-date_created  # Plus=asc, minus=desc

# Searching
GET /api/products?search=laptop

# Including related data
GET /api/users/123?include=orders,profile

# Specific fields
GET /api/users/123?fields=name,email  # Reduce bandwidth

# Comprehensive example
GET /api/products?category=electronics&sort=-price&page=1&limit=20&fields=id,name,price
```

### **Nested Resources**

```python
# One-to-many relationships
GET    /api/users/123/orders                # Get user's orders
POST   /api/users/123/orders                # Create order for user
GET    /api/users/123/orders/456            # Get specific order
DELETE /api/users/123/orders/456            # Delete order

# Many-to-many relationships
GET    /api/users/123/groups                # Get user's groups
POST   /api/users/123/groups/456            # Add user to group
DELETE /api/users/123/groups/456            # Remove user from group
```

### **Actions on Resources**

```python
# Bad: Non-RESTful actions
POST /api/users/123/activate
POST /api/users/123/deactivate
POST /api/orders/456/send

# Acceptable alternatives:
# 1. Use status field in resource
PUT /api/users/123
{
    "status": "active"
}

# 2. Use special sub-resources
POST /api/users/123/resend-verification
# (Okay for complex workflows that don't map to CRUD)

# 3. Use query parameter for action
POST /api/orders/456?action=send
```

---

## Request/Response Format

### **Consistent Structure**

```python
# Success response
{
    "data": {
        "id": 123,
        "name": "John Doe",
        "email": "john@example.com"
    },
    "meta": {
        "timestamp": "2024-01-29T10:30:00Z",
        "version": "1.0"
    }
}

# List response with pagination
{
    "data": [
        {"id": 1, "name": "Product 1"},
        {"id": 2, "name": "Product 2"}
    ],
    "pagination": {
        "page": 1,
        "limit": 20,
        "total": 42,
        "total_pages": 3,
        "next": "/api/products?page=2&limit=20"
    },
    "meta": {
        "timestamp": "2024-01-29T10:30:00Z"
    }
}

# Error response
{
    "error": {
        "code": "INVALID_EMAIL",
        "message": "Email format is invalid",
        "details": {
            "field": "email",
            "value": "not-an-email"
        }
    },
    "meta": {
        "timestamp": "2024-01-29T10:30:00Z"
    }
}
```

### **Request Format**

```python
# POST: Create with JSON body
POST /api/users
Content-Type: application/json

{
    "name": "John Doe",
    "email": "john@example.com",
    "age": 30
}

# PUT: Replace entire resource
PUT /api/users/123
{
    "name": "John Doe",
    "email": "john.new@example.com",
    "age": 31
}

# PATCH: Partial update
PATCH /api/users/123
{
    "email": "john.new@example.com"
}

# GET: Query parameters for filtering
GET /api/users?name=John&status=active&sort=-created_at
```

### **Null/Empty Values**

```python
# Bad: Inconsistent representation
{
    "email": null,
    "phone": "",
    "address": undefined
}

# Good: Consistent representation
{
    "email": null,              # Explicitly null if not provided
    "phone": null,              # Null instead of empty string
    "optional_field": null      # Even if optional, explicit null
}

# Alternative: Omit optional fields
{
    "id": 123,
    "name": "John",
    "email": null               # Only include if relevant
}
```

---

## Error Handling

### **Consistent Error Format**

```python
{
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "The request contains validation errors",
        "status": 422,
        "details": {
            "errors": [
                {
                    "field": "email",
                    "message": "Email format is invalid",
                    "value": "not-an-email"
                },
                {
                    "field": "age",
                    "message": "Age must be at least 18",
                    "value": 15
                }
            ]
        },
        "timestamp": "2024-01-29T10:30:00Z"
    }
}
```

### **Error Status Codes**

| Scenario | Status | Error |
|----------|--------|-------|
| Missing email field | 422 | `VALIDATION_ERROR` |
| Email already exists | 409 | `RESOURCE_CONFLICT` |
| User not found | 404 | `NOT_FOUND` |
| Token invalid | 401 | `AUTHENTICATION_ERROR` |
| No permission | 403 | `FORBIDDEN` |
| Rate limited | 429 | `RATE_LIMITED` |
| Server error | 500 | `INTERNAL_ERROR` |

### **Meaningful Error Codes**

```python
# Bad: Generic error codes
{"error": "Error", "status": 400}
{"error": "Invalid request", "status": 400}

# Good: Specific error codes
{"error": "INVALID_EMAIL_FORMAT", "status": 422}
{"error": "EMAIL_ALREADY_REGISTERED", "status": 409}
{"error": "USER_NOT_FOUND", "status": 404}
{"error": "INSUFFICIENT_PERMISSIONS", "status": 403}

# Usage: Clients can check error.code
if response.error.code == "EMAIL_ALREADY_REGISTERED":
    show_user("Email already registered, please log in")
elif response.error.code == "INVALID_EMAIL_FORMAT":
    show_user("Please enter valid email")
```

---

## API Versioning

### **Strategy 1: URL Path Versioning**

```
GET /api/v1/users/123
GET /api/v2/users/123

Pros: Clear version in URL
Cons: Multiple code paths, URLs change
```

### **Strategy 2: Query Parameter Versioning**

```
GET /api/users/123?api-version=1
GET /api/users/123?api-version=2

Pros: Single URL, flexible routing
Cons: Less explicit, easy to forget
```

### **Strategy 3: Header Versioning**

```
GET /api/users/123
Accept: application/vnd.myapi.v1+json

GET /api/users/123
Accept: application/vnd.myapi.v2+json

Pros: Clean URLs, standard HTTP
Cons: Less visible, harder to test
```

### **Strategy 4: No Versioning (Best Practice)**

Design API to be backward compatible, rarely need versions:

```python
# v1
GET /api/users/123
{
    "id": 123,
    "name": "John",
    "email": "john@example.com"
}

# Add new field without breaking v1
GET /api/users/123
{
    "id": 123,
    "name": "John",
    "email": "john@example.com",
    "created_at": "2024-01-29T00:00:00Z"  # New field, v1 clients ignore
}

# Rename field: keep both for compatibility
{
    "id": 123,
    "name": "John",
    "email": "john@example.com",
    "email_address": "john@example.com",  # Renamed, but keep old
    "created_at": "2024-01-29T00:00:00Z"
}
```

### **Deprecation Strategy**

```python
# Response header signals deprecation
HTTP/1.1 200 OK
Deprecation: true
Sunset: Mon, 01 Jul 2025 00:00:00 GMT
Link: </api/v2/users/123>; rel="successor-version"

# Deprecation notice in documentation
Endpoint: GET /api/v1/users
Deprecated: true
Sunset Date: July 1, 2025
Migration Guide: Use GET /api/v2/users instead
Reason: Extended fields available in v2
```

---

## Security

### **Authentication**

```python
# API Key (simple, less secure)
GET /api/users/123
Authorization: Bearer sk_live_abcdef123456

# JWT Token (recommended, stateless)
GET /api/users/123
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

# OAuth 2.0 (for third-party access)
GET /api/users/123
Authorization: Bearer access_token_from_oauth_server
```

### **Authorization**

```python
# Only users can access their own data
GET /api/users/123        # Current user accessing own profile: 200 OK
GET /api/users/456        # Current user accessing others: 403 Forbidden

# Role-based access
DELETE /api/users/123     # Admin can delete, regular user cannot: 403

# Resource-specific permissions
PATCH /api/orders/456     # User who created order can edit: 200 OK
PATCH /api/orders/789     # User who didn't create: 403 Forbidden
```

### **Rate Limiting**

```
Response headers:
X-RateLimit-Limit: 1000          # Requests per hour
X-RateLimit-Remaining: 234       # Requests left
X-RateLimit-Reset: 1234567890    # Unix timestamp when limit resets

When limit exceeded: 429 Too Many Requests
Retry-After: 3600                # Retry after 1 hour
```

### **CORS (Cross-Origin Resource Sharing)**

```
OPTIONS /api/users
Access-Control-Allow-Origin: https://app.example.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
Access-Control-Allow-Headers: Content-Type, Authorization
Access-Control-Max-Age: 3600
```

### **Sensitive Data**

```python
# Bad: Exposing sensitive data
GET /api/users/123
{
    "id": 123,
    "name": "John",
    "ssn": "123-45-6789",          # Never expose!
    "password_hash": "abc123xyz",  # Never expose!
    "api_key": "secret_key"        # Never expose!
}

# Good: Only return necessary data
GET /api/users/123
{
    "id": 123,
    "name": "John",
    "email": "john@example.com"
}

# Sensitive operations use dedicated endpoints
POST /api/users/123/change-password
{
    "current_password": "oldpass",
    "new_password": "newpass"
}
```

---

## Documentation

### **OpenAPI/Swagger Specification**

```yaml
openapi: 3.0.0
info:
  title: User API
  version: 1.0.0
  description: API for managing users

servers:
  - url: https://api.example.com
    description: Production

paths:
  /users:
    get:
      summary: List users
      parameters:
        - name: page
          in: query
          schema:
            type: integer
            default: 1
        - name: limit
          in: query
          schema:
            type: integer
            default: 20
      responses:
        '200':
          description: Success
          content:
            application/json:
              schema:
                type: object
                properties:
                  data:
                    type: array
                    items:
                      $ref: '#/components/schemas/User'
    
    post:
      summary: Create user
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/UserCreate'
      responses:
        '201':
          description: Created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'

components:
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
        email:
          type: string
          format: email
        created_at:
          type: string
          format: date-time

    UserCreate:
      type: object
      required:
        - name
        - email
      properties:
        name:
          type: string
        email:
          type: string
          format: email
```

### **Documentation Best Practices**

```
✅ DO:
- Document every endpoint
- Show example requests/responses
- Document error responses
- List required/optional parameters
- Explain authentication method
- Document rate limiting
- Provide code examples
- Document deprecation timeline

❌ DON'T:
- Assume developers know your domain
- Document only happy path
- Leave parameters unexplained
- Provide outdated examples
- Require separate tools for exploration
- Hide error codes/meanings
```

---

## Common Scenarios

### **Scenario 1: Pagination**

```python
# Request
GET /api/products?page=2&limit=20

# Response
{
    "data": [
        {"id": 21, "name": "Product 21"},
        {"id": 22, "name": "Product 22"}
    ],
    "pagination": {
        "page": 2,
        "limit": 20,
        "total": 1000,
        "total_pages": 50,
        "has_next": true,
        "has_prev": true,
        "next_page": "/api/products?page=3&limit=20",
        "prev_page": "/api/products?page=1&limit=20"
    }
}
```

### **Scenario 2: Partial Updates**

```python
# User exists with full profile
GET /api/users/123
{
    "id": 123,
    "name": "John",
    "email": "john@example.com",
    "phone": "555-1234",
    "address": "123 Main St"
}

# Client sends PATCH with partial data
PATCH /api/users/123
{
    "email": "john.new@example.com"
}

# Server updates only email, preserves other fields
GET /api/users/123
{
    "id": 123,
    "name": "John",                    # Unchanged
    "email": "john.new@example.com",   # Updated
    "phone": "555-1234",               # Unchanged
    "address": "123 Main St"           # Unchanged
}
```

### **Scenario 3: Bulk Operations**

```python
# Bad: Make multiple requests
POST /api/users/1/activate
POST /api/users/2/activate
POST /api/users/3/activate

# Good: Single bulk request
POST /api/users/bulk-activate
{
    "user_ids": [1, 2, 3]
}

# Or with bulk endpoint
POST /api/bulk
{
    "operations": [
        {"method": "PATCH", "path": "/users/1", "body": {"status": "active"}},
        {"method": "PATCH", "path": "/users/2", "body": {"status": "active"}},
        {"method": "PATCH", "path": "/users/3", "body": {"status": "active"}}
    ]
}
```

### **Scenario 4: Filtering with Relations**

```python
# Get user's orders with specific status
GET /api/users/123/orders?status=pending&sort=-created_at

# Get users matching criteria with their total orders
GET /api/users?email_domain=example.com&include=order_count

# Deep filtering (use cautiously)
GET /api/users?filter[orders.status]=pending&filter[created_at][$gte]=2024-01-01
```

---

## Checklist

- [ ] Resource-oriented design (nouns, not verbs)
- [ ] Standard HTTP methods used correctly
- [ ] Correct status codes returned
- [ ] Consistent naming conventions
- [ ] Consistent error format
- [ ] Error codes meaningful and documented
- [ ] Pagination for collection endpoints
- [ ] Filtering capabilities documented
- [ ] Authentication method chosen and documented
- [ ] Rate limiting implemented
- [ ] CORS properly configured
- [ ] Sensitive data not exposed
- [ ] Versioning strategy chosen
- [ ] Deprecation process documented
- [ ] API documented (OpenAPI/Swagger)
- [ ] Example requests/responses provided
- [ ] Error scenarios documented
- [ ] Rate limit headers included
- [ ] Idempotent operations properly designed
- [ ] Monitoring and logging in place

---

## References

- [REST API Design Guidelines](https://restfulapi.net/)
- [OpenAPI Specification](https://spec.openapis.org/oas/v3.0.0)
- [HTTP Status Codes](https://httpwg.org/specs/rfc7231.html#status.codes)
- [RFC 7231: HTTP Semantics](https://tools.ietf.org/html/rfc7231)
- [JSON API Specification](https://jsonapi.org/)
- [GraphQL Specification](https://spec.graphql.org/)
- [Building Web APIs: Acceptance Test Driven Development with Examples in Ruby](https://pragprog.com/titles/bdgrap/building-web-apis/)
