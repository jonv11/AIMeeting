# Naming Conventions Guide

A comprehensive guide to understand different naming conventions, their use cases, and best practices for applying them across various contexts. This guide helps developers and teams choose the most appropriate naming convention for their specific use case, language, or project structure.

---

## Quick Reference Table

| Convention | Example | Best For | Languages |
|------------|---------|----------|-----------|
| **snake_case** | `user_profile` | Functions, variables, databases | Python, Ruby, Rust, PHP |
| **camelCase** | `userProfile` | Variables, parameters, methods | JavaScript, Java, C# |
| **PascalCase** | `UserProfile` | Classes, interfaces, types | Java, C#, JavaScript, Python |
| **kebab-case** | `user-profile` | URLs, filenames, CSS classes | URLs, HTML/CSS, Markdown |
| **SCREAMING_SNAKE_CASE** | `MAX_TIMEOUT` | Constants, environment variables | All languages |
| **dot.case** | `user.profile` | Package names, namespaces | Java, Python packages |
| **flatcase** | `userprofile` | Domain names, usernames | URLs, usernames, DNS |
| **camelCase_With_Underscores** | `user_ProfileData` | Legacy code, special cases | Rare, mixed systems |

---

## Naming Conventions Deep Dive

### 1. **snake_case**

#### Definition
Words are separated by underscores, with all letters in lowercase.

#### Pattern
```
word_word_another_word
```

#### Examples
```
first_name
user_profile
calculate_total_sum
max_retry_attempts
database_connection_timeout
fetch_user_data_from_api
```

#### Use Cases
- **Python variables and functions** - Python's official style guide (PEP 8) recommends snake_case
- **Database schemas** - Column names and table names
- **File naming** - Unix/Linux convention
- **Shell scripts and environment variables** - Traditional convention
- **Ruby code** - Ruby style guide recommends snake_case
- **Rust code** - Official Rust style guide uses snake_case for functions and variables

#### Language Recommendations
- **Primary**: Python, Ruby, Rust, PHP, PostgreSQL
- **Secondary**: Bash scripting, SQL (many databases)
- **Avoid**: JavaScript (use camelCase instead), Java (use camelCase instead)

#### Advantages
- Very readable, especially for long identifiers
- Widely supported in all languages
- Works well in URLs and command-line contexts
- Clear word boundaries

#### Disadvantages
- Slower to type than camelCase
- Can make identifiers longer
- Not conventional in camelCase-first languages (JavaScript, Java)

#### Best Practices
- Use consistently across all variables and functions
- Keep words meaningful and not abbreviated
- Use lowercase exclusively (never `Snake_Case`)
- Avoid double underscores except for special Python cases (like `__init__`)

#### Tools & Linters
- **Python**: Pylint, Flake8, Black (enforces snake_case)
- **Rust**: Rustfmt, Clippy
- **Ruby**: RuboCop

---

### 2. **camelCase**

#### Definition
First word is lowercase, subsequent words capitalized, no separators between words.

#### Pattern
```
wordWordAnotherWord
```

#### Examples
```
firstName
userProfile
calculateTotalSum
maxRetryAttempts
databaseConnectionTimeout
fetchUserDataFromAPI
```

#### Use Cases
- **JavaScript variables and functions** - Standard convention
- **Java methods and variables** - Official Java naming conventions
- **C# properties and methods** - Microsoft C# guidelines
- **TypeScript variables** - Follows JavaScript conventions
- **CSS classes and IDs** (in some contexts) - Modern conventions
- **API parameter names** - Common in REST APIs and JSON

#### Language Recommendations
- **Primary**: JavaScript, Java, C#, TypeScript, Swift, Kotlin
- **Secondary**: JSON keys, API parameters, CSS (modern approach)
- **Avoid**: Python (use snake_case), Rust (use snake_case)

#### Advantages
- Fast to type (fewer keystrokes than snake_case)
- Dominant in JavaScript/Web development
- Compact while remaining readable
- Works well in camelCase-heavy languages

#### Disadvantages
- Less readable for very long identifiers
- Not standard in snake_case languages
- Harder to distinguish word boundaries at a glance
- URL-unfriendly (different from kebab-case)

#### Best Practices
- Start with lowercase first letter
- Capitalize first letter of each subsequent word
- Acronyms: treat as single word or capitalize each letter (`xmlParser` or `XMLParser`)
- Never use consecutive capitals except for established acronyms
- Keep consistent with your language's ecosystem

#### Tools & Linters
- **JavaScript**: ESLint, Prettier
- **Java**: Checkstyle, SpotBugs
- **C#**: StyleCop, Roslyn analyzers
- **TypeScript**: TSLint, ESLint

---

### 3. **PascalCase** (Upper Camel Case)

#### Definition
Like camelCase, but the first letter is also capitalized.

#### Pattern
```
WordWordAnotherWord
```

#### Examples
```
FirstName
UserProfile
CalculateTotalSum
MaxRetryAttempts
DatabaseConnectionTimeout
FetchUserDataFromAPI
```

#### Use Cases
- **Class names** - All object-oriented languages
- **Interface names** - Java, C#, TypeScript, Go
- **Type definitions** - TypeScript, Go, Rust (though Rust also uses snake_case for constructors)
- **Enum values** - Most languages
- **React/Vue component names** - Component files and exports
- **Namespaces** - Java, C#, Python packages

#### Language Recommendations
- **Primary**: Java, C#, TypeScript, Go, C++
- **Secondary**: Python (for classes), Ruby (for classes), JavaScript (React components)
- **Avoid**: Using for functions/methods (use camelCase instead)

#### Advantages
- Visually distinctive from variables/functions
- Industry standard for classes and types
- Clear hierarchical distinction in code
- Enables quick type identification

#### Disadvantages
- Not suitable for functions/methods in most languages
- Makes lowercase identifiers stand out
- Not URL-friendly or command-line friendly
- Harder to use in database schemas

#### Best Practices
- Use exclusively for classes, interfaces, and types
- Never use for functions, methods, or variables
- Match your language's standard conventions
- Use consistently across entire codebase
- Acronyms: either `HTTPServer` or `HttpServer` (be consistent within project)

#### Tools & Linters
- **Java**: Checkstyle
- **C#**: StyleCop, .NET Code Style
- **TypeScript/JavaScript**: ESLint, Prettier
- **Python**: Black (for class names)

---

### 4. **kebab-case**

#### Definition
Words are separated by hyphens, all lowercase.

#### Pattern
```
word-word-another-word
```

#### Examples
```
user-profile
calculate-total-sum
max-retry-attempts
database-connection-timeout
fetch-user-data-from-api
my-awesome-project
```

#### Use Cases
- **URLs and web routes** - Web standard
- **HTML attributes and CSS classes** - Web standard (w3c recommendation)
- **NPM package names** - Node package manager convention
- **Docker image tags** - Container naming
- **Kubernetes resources** - K8s manifests
- **File naming** (in some contexts) - Web-friendly files
- **Markdown file names** - Documentation files
- **Git branch names** - Modern Git workflow convention
- **CLI command names** - Command-line tools

#### Language Recommendations
- **Primary**: HTML/CSS/Web, NPM packages, URLs
- **Secondary**: CLI tools, Kubernetes, Docker
- **Avoid**: Variable/function names in code (use language-specific conventions)

#### Advantages
- Standard for URLs and web (SEO-friendly)
- Improves readability in URLs
- Works well with domain names
- Standard in CSS and HTML
- Easy to type with hyphen key

#### Disadvantages
- Not valid in most programming languages as identifier
- Cannot be used for variable/function names in code
- Requires special handling in many contexts
- Less compact than camelCase

#### Best Practices
- Use exclusively for URLs, file names, and web-based identifiers
- Never use hyphens in variable/function names within code
- Combine with other conventions as needed (e.g., `app-user-profile.js`)
- Use consistently across all web assets
- Consider SEO implications for public URLs

#### Tools & Linters
- **HTML/CSS**: Stylelint, HTML validators
- **NPM**: Built-in validation
- **Git**: Pre-commit hooks for branch names

---

### 5. **SCREAMING_SNAKE_CASE**

#### Definition
Words are separated by underscores with all letters in uppercase.

#### Pattern
```
WORD_WORD_ANOTHER_WORD
```

#### Examples
```
MAX_TIMEOUT
DEFAULT_USER_ROLE
API_KEY_LENGTH
DATABASE_HOST
ENVIRONMENT_PRODUCTION
APPLICATION_VERSION
FATAL_ERROR_EXIT_CODE
```

#### Use Cases
- **Constants** - All programming languages
- **Environment variables** - System configuration
- **Configuration file values** - System settings
- **Compile-time constants** - C, C++, Java
- **Global immutable values** - All languages
- **Symbolic constants** - Assembly, C, C++
- **Enum values** (in some languages) - Java, C, C++
- **Configuration keys** - INI files, environment

#### Language Recommendations
- **Primary**: All languages (universal for constants)
- **Special**: C, C++ (macro definitions), Java (static final fields)

#### Advantages
- Immediately identifies constants vs variables
- Prevents accidental modification
- Universal convention across all languages
- Clear intent of immutability
- Familiar to all developers

#### Disadvantages
- All uppercase can be harder to read
- Longer identifiers
- Not suitable for anything except constants
- Can make code look "shouty"

#### Best Practices
- Use ONLY for constants and immutable values
- Never use for variables that can change
- Pair with appropriate visibility modifiers (static final, const, etc.)
- Keep names meaningful, not cryptic
- Use sparingly—only truly constant values
- Document complex constants with comments

#### Tools & Linters
- **Python**: Pylint, Black (enforces for constants)
- **Java**: Checkstyle
- **C/C++**: Compiler warnings
- **JavaScript**: ESLint with constant naming rules

#### Language-Specific Examples

**Python:**
```python
MAX_RETRIES = 3
API_KEY = os.environ.get('API_KEY')
DEFAULT_TIMEOUT = 30
```

**Java:**
```java
public static final int MAX_TIMEOUT = 5000;
public static final String API_KEY = "key-value";
```

**JavaScript:**
```javascript
const MAX_RETRIES = 3;
const DEFAULT_PORT = 8080;
```

---

### 6. **dot.case**

#### Definition
Words are separated by dots (periods), all lowercase.

#### Pattern
```
word.word.another.word
```

#### Examples
```
com.example.application
org.springframework.boot
java.util.collections
user.profile.settings
configuration.database.connection
```

#### Use Cases
- **Java package names** - Official Java naming convention
- **Python module paths** - Package hierarchy
- **Application namespaces** - Organizational structures
- **Configuration hierarchies** - Nested configurations
- **DNS names and domains** - Reverse domain notation
- **Java Properties file keys** - Configuration management
- **Property paths** - Configuration objects

#### Language Recommendations
- **Primary**: Java (packages), Python (modules)
- **Secondary**: Configuration systems, DNS
- **Avoid**: URLs (use kebab-case), programming identifiers

#### Advantages
- Perfect for hierarchical namespaces
- Standard in Java ecosystem
- Clear organizational structure
- Works well for reverse DNS notation
- Separates domains clearly

#### Disadvantages
- Not valid in many programming language identifiers
- Confusing if misused (dots have different meanings)
- Can be verbose
- Difficult to type in some contexts
- Not suitable for non-hierarchical names

#### Best Practices
- Use for hierarchical structures only
- Follow your language's package naming standards
- Use reverse domain notation for uniqueness (Java)
- Keep package hierarchies shallow (3-4 levels max)
- Document package purposes clearly
- Match organizational structure when possible

#### Examples by Language

**Java Packages:**
```java
package com.mycompany.project.utils;
package org.apache.commons.lang3;
package javax.persistence.criteria;
```

**Python Modules:**
```python
import django.db.models
from flask.ext.sqlalchemy import SQLAlchemy
```

**Configuration Keys:**
```
server.port=8080
database.url=jdbc:mysql://localhost:3306/db
app.name=MyApplication
```

---

### 7. **flatcase**

#### Definition
All words combined without separators, all lowercase.

#### Pattern
```
wordwordanotherword
```

#### Examples
```
userprofile
calculatetotalsum
maxretryattempts
databaseconnectiontimeout
```

#### Use Cases
- **Domain names** - Web addresses (DNS names)
- **Usernames** - Social media, application accounts
- **Product/brand names** - Company branding
- **Single-word identifiers** - Rarely for multi-word
- **Legacy system names** - Older software conventions
- **Abbreviated commands** - Some CLI tools

#### Language Recommendations
- **Primary**: Domain names, usernames, branded names
- **Secondary**: Rarely used in modern code
- **Avoid**: Multi-word programming identifiers (impossible to parse)

#### Advantages
- Compact
- Works well for single words
- Domain-friendly
- Easy to type

#### Disadvantages
- Very difficult to read with multiple words
- Impossible to distinguish word boundaries
- Not recommended for new projects
- Poor for code maintainability

#### Best Practices
- Limit to single-word or pre-established names
- Only use for domains or usernames
- Avoid for new multi-word identifiers
- If forced to use, document word boundaries clearly
- Consider user experience with typos

#### When NOT to Use
```javascript
// Bad - impossible to parse
const userprofiledatabaseconnectiontimeout = 5000;

// Good - use appropriate convention
const userProfileDatabaseConnectionTimeout = 5000;
```

---

### 8. **Mixed and Hybrid Conventions**

#### SCREAMING_camelCase
Rarely used. Usually indicates a mistake or legacy code.
```
API_ResponseCode
MAX_userConnections
```

#### snake_Case_With_Capitals
Not recommended. Mixing conventions causes confusion.
```
User_Profile  // Bad
user_Profile  // Bad
user_profile  // Good
```

#### When Might You Encounter These?
- Legacy code migrations
- Cross-platform integrations
- Third-party API inconsistencies
- Team transition periods

#### Best Practice
Standardize on one convention per context. Avoid mixing unless absolutely necessary.

---

## Language-Specific Naming Convention Standards

### **Python (PEP 8 Standard)**
```python
# Functions and variables: snake_case
def calculate_total_sum(items):
    total_amount = 0
    return total_amount

# Classes: PascalCase
class UserProfile:
    pass

# Constants: SCREAMING_SNAKE_CASE
MAX_RETRIES = 3
DEFAULT_TIMEOUT = 30

# Private methods: _snake_case (single underscore)
def _internal_helper():
    pass

# Name-mangled methods: __snake_case (double underscore)
def __private_method():
    pass
```

### **JavaScript/TypeScript**
```javascript
// Variables and functions: camelCase
const firstName = "John";
function calculateTotalSum(items) {
    return items.reduce((sum, item) => sum + item.price, 0);
}

// Classes: PascalCase
class UserProfile {
    constructor(name) {
        this.name = name;
    }
}

// Constants: SCREAMING_SNAKE_CASE
const MAX_RETRIES = 3;
const API_KEY = process.env.API_KEY;

// React components: PascalCase (file and export)
// File: UserProfile.jsx
export function UserProfile({ user }) {
    return <div>{user.name}</div>;
}
```

### **Java**
```java
// Variables and methods: camelCase
private String firstName;
public void calculateTotalSum(List<Item> items) {
    int totalAmount = 0;
}

// Classes: PascalCase
public class UserProfile {
    // Nested classes also PascalCase
    public static class NestedClass {
    }
}

// Constants: SCREAMING_SNAKE_CASE
public static final int MAX_RETRIES = 3;
private static final String API_KEY = "key";

// Packages: dot.case, lowercase
package com.mycompany.project.utils;

// Interfaces: PascalCase (optionally with 'I' prefix)
public interface UserProvider {
}
// or
public interface IUserProvider {
}
```

### **C#**
```csharp
// Methods and properties: PascalCase
public class UserProfile
{
    public string FirstName { get; set; }
    public void CalculateTotalSum(List<Item> items)
    {
        // Method body
    }
}

// Local variables: camelCase
string firstName = "John";
int totalAmount = 0;

// Constants: PascalCase (or SCREAMING_SNAKE_CASE for macros)
public const int MaxRetries = 3;
public const string ApiKey = "key";

// Private fields: _camelCase (with leading underscore)
private string _internalName;
```

### **Go**
```go
// Functions and variables: camelCase (unexported: lowercase, exported: PascalCase)
func calculateTotalSum(items []Item) int {
    totalAmount := 0
    return totalAmount
}

// Exported function: PascalCase
func CalculateTotalSum(items []Item) int {
}

// Types: PascalCase
type UserProfile struct {
    FirstName string
    LastName  string
}

// Constants: camelCase (unexported) or PascalCase (exported)
const maxRetries = 3
const MaxRetries = 3
```

### **Rust**
```rust
// Functions and variables: snake_case
fn calculate_total_sum(items: &[Item]) -> u32 {
    let total_amount = 0;
    total_amount
}

// Types and structs: PascalCase
struct UserProfile {
    first_name: String,
}

// Constants: SCREAMING_SNAKE_CASE
const MAX_RETRIES: u32 = 3;

// Modules: snake_case
mod user_profile {
}
```

### **Ruby**
```ruby
# Variables and methods: snake_case
def calculate_total_sum(items)
  total_amount = 0
  total_amount
end

# Classes: PascalCase
class UserProfile
end

# Constants: SCREAMING_SNAKE_CASE
MAX_RETRIES = 3

# Private methods: snake_case (convention with private keyword)
private def internal_helper
end
```

### **SQL/Databases**
```sql
-- Tables and columns: snake_case (most common)
CREATE TABLE user_profiles (
    id INT PRIMARY KEY,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    email_address VARCHAR(255)
);

-- Or PascalCase (less common but used in some teams)
CREATE TABLE UserProfiles (
    ID INT PRIMARY KEY,
    FirstName VARCHAR(100)
);

-- Constants/defaults: SCREAMING_SNAKE_CASE
DEFAULT_USER_ROLE = 'USER';
MAX_TIMEOUT = 30000;
```

---

## Choosing the Right Convention

### Decision Tree

```
1. What are you naming?
   ├─ Class/Type/Interface?
   │  └─ Use PascalCase
   ├─ Function/Method?
   │  ├─ Python/Ruby/Rust? → Use snake_case
   │  └─ JS/Java/C#/Go? → Use camelCase
   ├─ Variable?
   │  ├─ Constant? → Use SCREAMING_SNAKE_CASE
   │  ├─ Python/Ruby/Rust? → Use snake_case
   │  └─ JS/Java/C#/Go? → Use camelCase
   ├─ URL/Web path?
   │  └─ Use kebab-case
   ├─ File name?
   │  ├─ Source code? → Match language convention
   │  ├─ Web asset? → Use kebab-case
   │  └─ Configuration? → Match context
   ├─ Package/Namespace?
   │  ├─ Java? → Use dot.case
   │  ├─ Python? → Use dot.case or snake_case
   │  └─ Other? → Match language standard
   └─ Environment variable?
      └─ Use SCREAMING_SNAKE_CASE
```

### Quick Selection Guide

| I'm writing... | Language | Convention |
|---|---|---|
| Function | Python | snake_case |
| Function | JavaScript | camelCase |
| Variable | Python | snake_case |
| Variable | JavaScript | camelCase |
| Constant | Any | SCREAMING_SNAKE_CASE |
| Class | Any | PascalCase |
| URL | N/A | kebab-case |
| File name | Markdown/Web | kebab-case |
| Package | Java | com.example.package |
| Database column | SQL | snake_case |
| Environment var | N/A | SCREAMING_SNAKE_CASE |
| CSS class | CSS | kebab-case |

---

## Best Practices and Anti-Patterns

### ✅ DO

- **Be consistent** within the same codebase and context
- **Follow language standards** - use PEP 8 for Python, follow Java conventions for Java
- **Match existing code** - new code should follow established patterns
- **Use clear, descriptive names** - convention is secondary to clarity
- **Document exceptions** - if you deviate from standard, explain why
- **Use linters** - automate convention enforcement
- **Review naming in code reviews** - maintain consistency across PRs

### ❌ DON'T

- **Mix conventions** in the same file or module
- **Use abbreviations excessively** - `usr_prfl` instead of `user_profile`
- **Ignore language standards** - don't use camelCase for Python, snake_case for Java
- **Use single-letter names** (except loop counters: `i`, `j`, `x`, `y`)
- **Use numbers as word substitutes** - `user4me` instead of `user_for_me`
- **Over-specify with prefixes/suffixes** - `m_member` is outdated, use `member`
- **Use reserved keywords** - check your language's reserved word list

### Anti-Patterns to Avoid

```python
# Bad: Inconsistent naming
class UserProfile:
    def __init__(self):
        self.FirstName = "John"  # Should be first_name
        self.last_name = "Doe"   # Inconsistent with FirstName
        self.eMail = "john@example.com"  # Should be email

# Bad: Ambiguous abbreviations
def calc(x, y):  # What does this calculate?
    return x + y

# Bad: Unnecessary prefixes
def get_getName():  # Redundant 'get' in method name
    return self.name

# Bad: Over-abbreviated
usr_pfl_db_con_tmout = 5000  # Hard to read

# Good: Consistent, clear naming
class UserProfile:
    def __init__(self):
        self.first_name = "John"
        self.last_name = "Doe"
        self.email = "john@example.com"
    
    def get_full_name(self):
        return f"{self.first_name} {self.last_name}"

MAX_USER_PROFILE_DB_TIMEOUT = 5000
```

---

## Tools and Automation

### Linters and Formatters

| Tool | Language | Enforces |
|------|----------|----------|
| Black | Python | snake_case, SCREAMING_SNAKE_CASE |
| Pylint | Python | PEP 8 conventions |
| Flake8 | Python | PEP 8 conventions |
| ESLint | JavaScript | camelCase, PascalCase |
| Prettier | JavaScript/TypeScript | Formatting consistency |
| StyleCop | C# | C# naming conventions |
| Checkstyle | Java | Java conventions |
| RuboCop | Ruby | Ruby conventions |
| Clippy | Rust | Rust conventions |
| Stylelint | CSS | kebab-case for classes |

### Configuration Examples

**ESLint (JavaScript):**
```json
{
  "rules": {
    "camelcase": ["error", { "properties": "always" }],
    "id-pattern": ["error", "^[a-z_]*$"],
    "naming-convention": [
      "error",
      { "selector": "default", "format": ["camelCase"] },
      { "selector": "typeLike", "format": ["PascalCase"] },
      { "selector": "variable", "modifiers": ["const"], "format": ["UPPER_CASE"] }
    ]
  ]
}
```

**Pylint (Python):**
```ini
[BASIC]
# Regular expression for function names
function-rgx=[a-z_][a-z0-9_]*$

# Regular expression for variable names
variable-rgx=[a-z_][a-z0-9_]*$

# Regular expression for constant names
const-rgx=(([A-Z_][A-Z0-9_]*)|(__.*__))$

# Regular expression for class names
class-rgx=[A-Z_][a-zA-Z0-9]*$
```

---

## Real-World Examples

### E-commerce Platform

```python
# Models (Python/Django)
class ProductCategory:  # PascalCase for class
    def __init__(self):
        self.category_name = ""  # snake_case for variables
        self.is_active = True

# Constants
MAX_PRODUCT_PRICE = 999999
DEFAULT_DISCOUNT_PERCENT = 0

# Functions
def calculate_product_discount(original_price, discount_percent):
    return original_price * (1 - discount_percent / 100)
```

```javascript
// Frontend (JavaScript/React)
export function ProductCategory({ product }) {  // PascalCase for component
    const [categoryName, setCategoryName] = useState("");  // camelCase for state
    
    const calculateDiscount = (price, percent) => {  // camelCase for function
        return price * (1 - percent / 100);
    };
    
    return <div>{categoryName}</div>;
}
```

```html
<!-- HTML/CSS -->
<div class="product-category">  <!-- kebab-case for CSS classes -->
    <h1 class="product-title"></h1>
    <p class="product-description"></p>
</div>

<style>
    .product-category { }       /* kebab-case for class names */
    #product-details { }        /* kebab-case for ID names */
    --product-bg-color: #fff;   /* kebab-case for CSS variables */
</style>
```

```sql
-- Database
CREATE TABLE product_categories (  -- snake_case for tables
    category_id INT PRIMARY KEY,
    category_name VARCHAR(100),
    is_active BOOLEAN DEFAULT true
);

-- Constants
SET @MAX_PRODUCT_PRICE = 999999;
```

---

## Migration and Refactoring

### When You Need to Change Conventions

1. **Use automated tools first** - Linters and IDE refactoring can handle most cases
2. **Test thoroughly** - Especially for public APIs
3. **Update documentation** - Explain why and when you changed
4. **Plan in phases** - Don't change everything at once
5. **Communicate** - Notify team before major changes

### Migration Checklist
- [ ] Run linter/formatter on entire codebase
- [ ] Review and test high-impact changes (APIs, public classes)
- [ ] Update documentation and examples
- [ ] Communicate changes to team
- [ ] Create pull requests for review
- [ ] Update coding standards documentation
- [ ] Add pre-commit hooks to enforce new conventions

---

## References and Resources

- [PEP 8 - Python Style Guide](https://www.python.org/dev/peps/pep-0008/)
- [Google Python Style Guide](https://google.github.io/styleguide/pyguide.html)
- [Google JavaScript Style Guide](https://google.github.io/styleguide/jsguide.html)
- [Oracle Java Code Conventions](https://www.oracle.com/java/technologies/javase/codeconventions-136091.html)
- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Go Code Review Comments](https://github.com/golang/go/wiki/CodeReviewComments)
- [Rust API Guidelines](https://rust-lang.github.io/api-guidelines/naming.html)
- [W3C CSS Naming Conventions](https://www.w3.org/Style/CSS/conventions.html)
- [NPM Naming Package Best Practices](https://docs.npmjs.com/package-json)

---

## Quick Reference Card

Keep this handy:

```
QUICK NAMING REFERENCE
======================

Python:          snake_case (vars/funcs), PascalCase (classes), SCREAMING_SNAKE_CASE (constants)
JavaScript:      camelCase (vars/funcs), PascalCase (classes), SCREAMING_SNAKE_CASE (constants)
Java:            camelCase (vars/funcs), PascalCase (classes), SCREAMING_SNAKE_CASE (constants)
C#:              PascalCase (props), camelCase (locals), _camelCase (private fields)
Go:              camelCase (exported), lowercase (unexported), snake_case (in rare cases)
Rust:            snake_case (vars/funcs), PascalCase (types/structs), SCREAMING_SNAKE_CASE (constants)
Ruby:            snake_case (everything), PascalCase (classes), SCREAMING_SNAKE_CASE (constants)

URLs:            kebab-case
Databases:       snake_case
Environment:     SCREAMING_SNAKE_CASE
Config keys:     dot.case or camelCase
Packages:        dot.case (Java), snake_case (Python)
CSS:             kebab-case
```
