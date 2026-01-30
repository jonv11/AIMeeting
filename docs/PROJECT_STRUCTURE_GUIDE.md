# Project Structure and Directory Organization Guide

A comprehensive guide to understanding effective project structure and directory organization. This guide explains the principles behind well-organized projects, common patterns used in successful repositories, and best practices for different project types. Includes real-world examples and anti-patterns to avoid.

---

## Why Project Structure Matters

### Impact on Development
- **Onboarding**: New developers find code faster
- **Scalability**: Easy to add features without chaos
- **Maintainability**: Clear organization prevents technical debt
- **Collaboration**: Team members work without conflicts
- **Testing**: Tests co-located with code or properly organized
- **Build & Deployment**: Clear separation of concerns speeds automation

### The Cost of Bad Structure
- Developers spend time navigating confusing directories
- Code duplication occurs due to unclear organization
- Testing becomes difficult and incomplete
- Deployment processes are fragile
- Technical debt accumulates rapidly

---

## Core Principles of Good Project Structure

### 1. **Clarity**
The structure should be immediately understandable to someone new to the project.

**Good:**
```
project/
├── src/          # Source code
├── tests/        # Tests
├── docs/         # Documentation
└── config/       # Configuration
```

**Bad:**
```
project/
├── code/
├── stuff/
├── old_files/
└── final_version/
```

### 2. **Separation of Concerns**
Related code stays together; unrelated code is separated.

**Good:**
```
src/
├── features/
│   ├── auth/
│   ├── users/
│   └── products/
└── shared/
    ├── utils/
    ├── hooks/
    └── components/
```

**Bad:**
```
src/
├── utils.js
├── auth.js
├── users.js
├── products.js
├── components.js
├── services.js
├── helpers.js
```

### 3. **Scalability**
Structure should grow with your project without major reorganization.

**Scales Well:**
- Feature-based organization (easy to add new features)
- Layered architecture (easy to add layers)
- Domain-driven design (easy to add domains)

**Doesn't Scale:**
- Flat structure (100+ files in one directory)
- By-type organization (all components in one folder)
- No clear organization scheme

### 4. **Consistency**
Rules should apply uniformly throughout the project.

**Consistent:**
- All features follow same internal structure
- Same naming conventions throughout
- Same depth of nesting everywhere

**Inconsistent:**
- Some features have tests, others don't
- Variable depths of folders
- Different naming patterns in different areas

### 5. **Convention Over Configuration**
Use community-standard patterns rather than inventing new ones.

**Good:**
- Node.js projects use `src/`, `tests/`, `dist/`
- Python projects use `src/`, `tests/`, `docs/`
- Java projects follow Maven structure

**Bad:**
- Unique structure that only your team understands
- Reinventing standard patterns
- Non-obvious directory purposes

---

## Universal Directory Structure

This structure works for almost any project:

```
project/
├── README.md                 # Project overview
├── LICENSE                   # License file
├── .gitignore               # Git ignore rules
├── CONTRIBUTING.md          # Contribution guidelines
├── CODE_OF_CONDUCT.md       # Code of conduct
├── CHANGELOG.md             # Version history
├── AUTHORS.md               # Project authors
├── SECURITY.md              # Security policy
├── .editorconfig            # Editor configuration
├── .gitattributes           # Git attributes
│
├── src/                      # Source code
│   ├── index.js             # Entry point
│   └── (language-specific structure)
│
├── tests/ (or __tests__)    # Test files
│   ├── unit/
│   ├── integration/
│   └── e2e/
│
├── docs/                     # Documentation
│   ├── README.md
│   ├── ARCHITECTURE.md
│   ├── API.md
│   └── guides/
│
├── config/                   # Configuration files
│   ├── development.env
│   ├── production.env
│   └── app.config.js
│
├── .github/                  # GitHub-specific files
│   ├── workflows/           # CI/CD workflows
│   ├── ISSUE_TEMPLATE/
│   └── PULL_REQUEST_TEMPLATE.md
│
├── build/                    # Build output (not in git)
├── dist/                     # Distribution output (not in git)
└── node_modules/            # Dependencies (not in git)
```

---

## Language and Framework-Specific Structures

### **Node.js / JavaScript / TypeScript**

#### Recommended Structure (Feature-Based)
```
project/
├── src/
│   ├── features/
│   │   ├── auth/
│   │   │   ├── auth.controller.ts
│   │   │   ├── auth.service.ts
│   │   │   ├── auth.route.ts
│   │   │   ├── auth.model.ts
│   │   │   └── __tests__/
│   │   ├── users/
│   │   ├── products/
│   │   └── orders/
│   ├── shared/
│   │   ├── middleware/
│   │   ├── utils/
│   │   ├── helpers/
│   │   └── constants/
│   ├── config/
│   ├── database/
│   └── index.ts
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── docs/
├── .env.example
├── package.json
└── tsconfig.json
```

#### Alternative: By-Type Structure (For Small Projects)
```
project/
├── src/
│   ├── controllers/
│   ├── services/
│   ├── models/
│   ├── routes/
│   ├── middleware/
│   ├── utils/
│   └── index.js
├── tests/
├── docs/
└── package.json
```

#### React Application Structure
```
project/
├── public/
│   ├── index.html
│   └── favicon.ico
├── src/
│   ├── components/
│   │   ├── Header/
│   │   │   ├── Header.jsx
│   │   │   ├── Header.css
│   │   │   └── __tests__/
│   │   ├── Footer/
│   │   └── Common/
│   ├── pages/
│   │   ├── Home/
│   │   ├── About/
│   │   └── NotFound/
│   ├── hooks/
│   │   ├── useAuth.js
│   │   ├── useFetch.js
│   │   └── useLocalStorage.js
│   ├── services/
│   │   ├── api.js
│   │   ├── auth.js
│   │   └── storage.js
│   ├── utils/
│   ├── styles/
│   │   ├── globals.css
│   │   └── variables.css
│   ├── App.jsx
│   └── index.jsx
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── public/
├── package.json
└── vite.config.js
```

### **Python**

#### Standard Structure (Package-Based)
```
project/
├── src/
│   └── myproject/
│       ├── __init__.py
│       ├── main.py
│       ├── core/
│       │   ├── __init__.py
│       │   ├── models.py
│       │   └── services.py
│       ├── api/
│       │   ├── __init__.py
│       │   └── routes.py
│       ├── database/
│       │   ├── __init__.py
│       │   └── connection.py
│       └── utils/
│           ├── __init__.py
│           └── helpers.py
├── tests/
│   ├── __init__.py
│   ├── conftest.py
│   ├── unit/
│   │   ├── test_models.py
│   │   └── test_services.py
│   └── integration/
│       └── test_api.py
├── docs/
│   ├── conf.py
│   └── index.rst
├── requirements.txt
├── requirements-dev.txt
├── setup.py
├── pyproject.toml
└── README.md
```

#### Django Project Structure
```
project/
├── myproject/
│   ├── manage.py
│   ├── myproject/
│   │   ├── __init__.py
│   │   ├── settings.py
│   │   ├── urls.py
│   │   └── wsgi.py
│   └── apps/
│       ├── users/
│       │   ├── migrations/
│       │   ├── models.py
│       │   ├── views.py
│       │   ├── urls.py
│       │   ├── tests.py
│       │   └── admin.py
│       ├── products/
│       └── orders/
├── tests/
├── static/
├── media/
├── docs/
├── requirements.txt
└── README.md
```

### **Java**

#### Maven/Gradle Standard Structure
```
project/
├── src/
│   ├── main/
│   │   ├── java/
│   │   │   └── com/
│   │   │       └── mycompany/
│   │   │           └── myapp/
│   │   │               ├── controller/
│   │   │               ├── service/
│   │   │               ├── repository/
│   │   │               ├── model/
│   │   │               ├── exception/
│   │   │               └── util/
│   │   └── resources/
│   │       ├── application.properties
│   │       └── logback.xml
│   └── test/
│       ├── java/
│       │   └── com/mycompany/myapp/
│       │       ├── controller/
│       │       ├── service/
│       │       └── repository/
│       └── resources/
├── pom.xml (Maven) or build.gradle (Gradle)
├── docs/
└── README.md
```

#### Spring Boot Structure
```
project/
├── src/
│   ├── main/
│   │   ├── java/
│   │   │   └── com/mycompany/
│   │   │       └── myapp/
│   │   │           ├── MyAppApplication.java
│   │   │           ├── config/
│   │   │           ├── controller/
│   │   │           ├── service/
│   │   │           ├── repository/
│   │   │           ├── entity/
│   │   │           ├── dto/
│   │   │           ├── exception/
│   │   │           └── util/
│   │   └── resources/
│   │       ├── application.yml
│   │       ├── application-dev.yml
│   │       └── application-prod.yml
│   └── test/
│       ├── java/
│       └── resources/
├── pom.xml
└── README.md
```

### **Go**

#### Standard Go Project Structure
```
project/
├── cmd/
│   └── myapp/
│       └── main.go
├── internal/
│   ├── app/
│   │   ├── app.go
│   │   ├── handlers/
│   │   ├── services/
│   │   └── models/
│   ├── config/
│   ├── database/
│   └── logger/
├── pkg/
│   ├── auth/
│   └── utils/
├── api/
│   └── openapi.yaml
├── tests/
│   ├── integration/
│   └── e2e/
├── go.mod
├── go.sum
├── Makefile
└── README.md
```

### **Rust**

#### Cargo Project Structure
```
project/
├── src/
│   ├── main.rs (if binary)
│   ├── lib.rs (if library)
│   ├── bin/
│   │   └── binary_name.rs
│   ├── modules/
│   │   ├── auth.rs
│   │   ├── database.rs
│   │   └── api.rs
│   └── utils/
│       ├── mod.rs
│       ├── helpers.rs
│       └── constants.rs
├── tests/
│   └── integration_test.rs
├── benches/
│   └── benchmarks.rs
├── examples/
│   └── example.rs
├── Cargo.toml
└── README.md
```

### **C/C++**

#### Standard C/C++ Project Structure
```
project/
├── include/
│   ├── mylib/
│   │   ├── header.h
│   │   └── utils.h
│   └── config.h
├── src/
│   ├── CMakeLists.txt
│   ├── main.cpp
│   ├── core/
│   │   ├── CMakeLists.txt
│   │   ├── implementation.cpp
│   │   └── implementation.h
│   └── utils/
├── tests/
│   ├── CMakeLists.txt
│   ├── unit/
│   └── integration/
├── docs/
├── build/
│   ├── Debug/
│   └── Release/
├── CMakeLists.txt
├── Makefile
└── README.md
```

### **Web Frontend (HTML/CSS/JavaScript)**

#### Static Website Structure
```
project/
├── index.html
├── pages/
│   ├── about.html
│   ├── contact.html
│   └── services.html
├── css/
│   ├── styles.css
│   ├── variables.css
│   └── responsive.css
├── js/
│   ├── main.js
│   ├── utils.js
│   └── api.js
├── images/
│   ├── hero/
│   ├── icons/
│   └── products/
├── assets/
│   ├── fonts/
│   └── data/
└── README.md
```

---

## Organizing Large Projects

### **Monorepo Structure**

For projects with multiple packages/services in one repository:

```
monorepo/
├── packages/
│   ├── common/
│   │   ├── src/
│   │   ├── tests/
│   │   └── package.json
│   ├── frontend/
│   │   ├── src/
│   │   ├── public/
│   │   └── package.json
│   ├── backend/
│   │   ├── src/
│   │   ├── tests/
│   │   └── package.json
│   ├── cli/
│   │   ├── src/
│   │   ├── tests/
│   │   └── package.json
│   └── api/
│       ├── src/
│       ├── docs/
│       └── package.json
├── tools/
│   ├── build-scripts/
│   ├── deploy/
│   └── testing/
├── docs/
├── .github/
├── package.json (root)
├── lerna.json (or pnpm-workspace.yaml)
└── README.md
```

### **Microservices Structure**

For organizations with multiple microservices:

```
services/
├── user-service/
│   ├── src/
│   ├── tests/
│   ├── Dockerfile
│   └── package.json
├── product-service/
│   ├── src/
│   ├── tests/
│   ├── Dockerfile
│   └── package.json
├── order-service/
│   ├── src/
│   ├── tests/
│   ├── Dockerfile
│   └── package.json
├── shared/
│   ├── proto/
│   ├── utils/
│   └── types/
├── infrastructure/
│   ├── docker-compose.yml
│   ├── kubernetes/
│   └── terraform/
├── docs/
└── README.md
```

### **Monolithic Application**

For large single applications:

```
app/
├── src/
│   ├── features/
│   │   ├── auth/
│   │   │   ├── controllers/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   ├── routes/
│   │   │   ├── middlewares/
│   │   │   ├── validators/
│   │   │   └── __tests__/
│   │   ├── users/
│   │   ├── products/
│   │   ├── orders/
│   │   └── reports/
│   ├── shared/
│   │   ├── middleware/
│   │   ├── decorators/
│   │   ├── guards/
│   │   ├── filters/
│   │   ├── pipes/
│   │   ├── utils/
│   │   ├── helpers/
│   │   ├── constants/
│   │   ├── types/
│   │   └── interfaces/
│   ├── config/
│   ├── database/
│   ├── logger/
│   ├── app.module.ts
│   └── main.ts
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── docs/
└── package.json
```

---

## File Organization Patterns

### **Pattern 1: Feature-Based (Recommended for Most Projects)**

Group by feature or domain, with complete feature code together.

```
src/
├── features/
│   ├── auth/
│   │   ├── auth.controller.ts
│   │   ├── auth.service.ts
│   │   ├── auth.model.ts
│   │   ├── __tests__/
│   │   └── README.md
│   └── users/
│       ├── users.controller.ts
│       ├── users.service.ts
│       ├── users.model.ts
│       └── __tests__/
└── shared/
    ├── utils/
    ├── middleware/
    └── types/
```

**When to Use:**
- Medium to large projects
- Multiple developers working in parallel
- Clear feature boundaries
- Easy to find all code for one feature

**Advantages:**
- Easy to locate feature code
- Minimal cross-feature coupling
- Teams can own entire features
- Simple to extract features into separate services

---

### **Pattern 2: By Type (For Simple Projects)**

Group by type of file (controllers, services, models).

```
src/
├── controllers/
├── services/
├── models/
├── middleware/
├── utils/
└── types/
```

**When to Use:**
- Small projects (< 10 files per category)
- Learning projects
- Simple CRUD applications
- Prototypes

**Advantages:**
- Simple to understand initially
- Clear file categorization
- Easy for beginners

**Disadvantages:**
- Doesn't scale beyond ~50 total files
- Hard to find related code
- Risk of circular dependencies
- Difficult for teams

---

### **Pattern 3: Layered Architecture**

Group by technical layer (presentation, business, data).

```
src/
├── presentation/
│   ├── controllers/
│   ├── views/
│   └── dto/
├── business/
│   ├── services/
│   ├── domain/
│   └── rules/
├── data/
│   ├── repositories/
│   ├── models/
│   └── mappers/
└── infrastructure/
    ├── database/
    ├── cache/
    └── logging/
```

**When to Use:**
- Enterprise applications
- Complex business logic
- Need clear separation of concerns
- Multiple data sources

**Advantages:**
- Clear architectural layers
- Easy to test each layer
- Scalable for large teams
- Promotes SOLID principles

---

### **Pattern 4: Domain-Driven Design (DDD)**

Group by business domain with nested layers.

```
src/
├── domains/
│   ├── user/
│   │   ├── application/
│   │   ├── domain/
│   │   ├── infrastructure/
│   │   └── presentation/
│   ├── order/
│   │   ├── application/
│   │   ├── domain/
│   │   ├── infrastructure/
│   │   └── presentation/
│   └── payment/
├── shared/
│   ├── domain/
│   ├── application/
│   └── infrastructure/
└── main.ts
```

**When to Use:**
- Large, complex business domains
- Multiple bounded contexts
- Long-term projects
- Enterprise systems

**Advantages:**
- Aligns with business structure
- Clear domain boundaries
- Easy to scale teams
- Facilitates microservices transition

---

## Test File Organization

### **Option 1: Co-located Tests (Recommended)**

Tests live alongside source code:

```
src/
├── features/
│   ├── auth/
│   │   ├── auth.service.ts
│   │   ├── auth.service.test.ts
│   │   ├── auth.controller.ts
│   │   └── auth.controller.test.ts
│   └── users/
│       ├── users.service.ts
│       ├── users.service.test.ts
│       └── users.controller.ts
```

**Advantages:**
- Tests stay with code during refactoring
- Easy to find related tests
- Encourages test writing
- Clear test-to-source ratio

---

### **Option 2: Separate Test Directory**

Tests in separate `tests/` directory:

```
src/
├── features/
│   ├── auth/
│   │   ├── auth.service.ts
│   │   └── auth.controller.ts
│   └── users/
tests/
├── unit/
│   ├── auth/
│   │   ├── auth.service.test.ts
│   │   └── auth.controller.test.ts
│   └── users/
├── integration/
│   ├── auth/
│   └── users/
└── e2e/
```

**Advantages:**
- Clean source directory
- Flexible test organization
- Easy to exclude from builds
- Good for large test suites

---

### **Option 3: Hybrid (For Complex Projects)**

Unit tests co-located, integration/e2e tests separate:

```
src/
├── features/
│   ├── auth/
│   │   ├── auth.service.ts
│   │   ├── auth.service.unit.test.ts
│   │   └── auth.controller.ts
tests/
├── integration/
│   └── auth/
└── e2e/
```

---

## Best Practices for Scaling

### **1. Keep Directories Shallow (2-4 Levels Deep)**

**Good:**
```
src/
├── features/
│   └── auth/
```

**Bad:**
```
src/
├── features/
│   └── auth/
│       └── domain/
│           └── entities/
│               └── user/
```

### **2. Avoid Very Large Directories (> 20 Files)**

If a directory has > 20 files, it's time to subdivide:

**Before:**
```
src/
└── utils/
    ├── 30+ utility files...
```

**After:**
```
src/
└── utils/
    ├── string/
    ├── array/
    ├── date/
    ├── validation/
    └── format/
```

### **3. Use Index Files (index.ts, __init__.py)**

Make imports cleaner:

**Before:**
```typescript
import { UserService } from '../users/user.service';
import { AuthService } from '../auth/auth.service';
import { UserModel } from '../users/user.model';
```

**After (with index files):**
```typescript
import { UserService, UserModel } from '../users';
import { AuthService } from '../auth';
```

### **4. Document Structure**

Create `ARCHITECTURE.md` explaining:

```markdown
# Project Architecture

## Directory Structure

- `src/` - All source code
  - `features/` - Feature modules
  - `shared/` - Shared utilities and types
  - `config/` - Configuration files

## Feature Structure

Each feature in `src/features/` contains:
- Controller - Request handling
- Service - Business logic
- Model - Data models
- Tests - Unit tests
```

### **5. Enforce Structure with Linting**

Use ESLint, Pylint, or similar tools:

```json
// eslintrc
{
  "rules": {
    "no-restricted-imports": [
      "error",
      {
        "patterns": ["features/*"]  // Force feature imports from index
      }
    ]
  }
}
```

---

## Anti-Patterns to Avoid

### ❌ **Anti-Pattern 1: Flat Structure**

```
src/
├── 50+ .ts files directly
└── No organization
```

**Problem:** Unnavigable, impossible to find anything
**Solution:** Group into features or by type

---

### ❌ **Anti-Pattern 2: God Folder**

```
src/
├── utils/
│   ├── 100+ utility files
│   └── No subcategories
```

**Problem:** Massive folder hard to navigate
**Solution:** Split into utils/string, utils/array, utils/date, etc.

---

### ❌ **Anti-Pattern 3: Circular Dependencies**

```
src/
├── user/
│   └── user.ts (imports from product)
├── product/
│   └── product.ts (imports from user)
```

**Problem:** Code is fragile, hard to test, hard to refactor
**Solution:** Create `shared/` module that both can import

---

### ❌ **Anti-Pattern 4: Random Directory Names**

```
src/
├── stuff/
├── temp/
├── old_code/
├── new_features/
└── testing/
```

**Problem:** Unclear purpose, maintenance nightmare
**Solution:** Use standard names (features, shared, config, etc.)

---

### ❌ **Anti-Pattern 5: Inconsistent Structure**

```
src/
├── auth/
│   ├── auth.service.ts
│   ├── auth.controller.ts
│   └── tests/
├── users/
│   ├── user.service.ts
│   └── __tests__/ (different name)
```

**Problem:** Team confusion, hard to predict structure
**Solution:** Enforce consistency through documentation and linting

---

### ❌ **Anti-Pattern 6: Unnecessary Nesting**

```
src/
├── domain/
│   └── models/
│       └── entities/
│           └── user/
│               └── User.ts (5 levels to reach file)
```

**Problem:** Too many directories, hard to navigate
**Solution:** Limit depth to 2-3 levels

---

## Migration and Refactoring

### **When to Reorganize**

- Project structure no longer makes sense
- Adding new features requires confusing decisions
- Developers regularly can't find files
- Dependencies are becoming circular
- Project has grown 3-5x in size

### **How to Reorganize**

1. **Plan** - Document target structure
2. **Create** - Set up new directory structure
3. **Move** - Gradually move files (or all at once if small)
4. **Update** - Fix all imports
5. **Test** - Ensure everything still works
6. **Communicate** - Tell team about structure changes
7. **Document** - Update ARCHITECTURE.md

### **Tools to Help**

- **IDE Refactoring** - Most IDEs can move files and update imports
- **Scripts** - Write scripts to update imports
- **Linters** - Use linters to catch import issues
- **Tests** - Run comprehensive tests after moving

---

## Real-World Examples

### **Example 1: Small Express.js API**

```
project/
├── src/
│   ├── index.js (entry point)
│   ├── config.js
│   ├── routes.js
│   ├── middleware.js
│   ├── models.js
│   ├── services.js
│   └── utils.js
├── tests/
│   └── api.test.js
├── docs/
├── .env.example
├── package.json
└── README.md
```

### **Example 2: Medium React Application**

```
project/
├── src/
│   ├── components/
│   │   ├── Header/
│   │   ├── Footer/
│   │   ├── UserProfile/
│   │   └── Common/
│   ├── pages/
│   │   ├── Home/
│   │   ├── About/
│   │   └── Dashboard/
│   ├── hooks/
│   ├── services/
│   ├── utils/
│   ├── types/
│   └── App.jsx
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── public/
├── docs/
└── package.json
```

### **Example 3: Large NestJS Backend**

```
project/
├── src/
│   ├── features/
│   │   ├── auth/
│   │   │   ├── auth.controller.ts
│   │   │   ├── auth.service.ts
│   │   │   ├── strategies/
│   │   │   ├── guards/
│   │   │   ├── dto/
│   │   │   └── __tests__/
│   │   ├── users/
│   │   ├── products/
│   │   └── orders/
│   ├── shared/
│   │   ├── middleware/
│   │   ├── decorators/
│   │   ├── filters/
│   │   ├── pipes/
│   │   ├── guards/
│   │   ├── utils/
│   │   └── constants/
│   ├── config/
│   ├── database/
│   ├── logger/
│   ├── app.module.ts
│   └── main.ts
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── docs/
├── src/
└── package.json
```

### **Example 4: Python Django Project**

```
project/
├── manage.py
├── myproject/
│   ├── __init__.py
│   ├── settings.py
│   ├── urls.py
│   └── wsgi.py
├── apps/
│   ├── auth/
│   │   ├── models.py
│   │   ├── views.py
│   │   ├── serializers.py
│   │   ├── urls.py
│   │   ├── tests/
│   │   └── admin.py
│   ├── users/
│   └── products/
├── tests/
├── static/
├── media/
├── docs/
├── requirements.txt
└── README.md
```

---

## Checklist for Project Structure

### ✅ **Structure Audit**

- [ ] Root directory is clean (only essential files)
- [ ] Source code in `src/` or language-standard location
- [ ] Tests organized logically
- [ ] Documentation in `docs/` or root README
- [ ] No "temp", "old", or "testing" folders
- [ ] Directories have clear purposes
- [ ] Related code is grouped together
- [ ] No directory has > 20 files (subdivide if needed)
- [ ] Directory depth is 2-4 levels max
- [ ] Consistent naming throughout
- [ ] `__init__.py` or `index.ts` files for imports
- [ ] `ARCHITECTURE.md` documents structure
- [ ] Large directories have README explaining contents
- [ ] Tests are easy to locate
- [ ] No circular dependencies

---

## Tools for Structure Management

### **Visualization**

- **tree** command - Show directory structure
- **VS Code Extensions** - File tree organizers
- **PlantUML** - Diagram architecture
- **GraphQL Explorer** - Visualize dependencies

### **Enforcement**

- **ESLint** - Enforce import paths and restrictions
- **Pylint** - Python import validation
- **Checkstyle** - Java structure rules
- **Pre-commit hooks** - Validate on commit
- **CI/CD checks** - Validate on pull requests

### **Documentation**

- **README** - Root directory explanation
- **ARCHITECTURE.md** - Overall structure
- **Module READMEs** - Large module documentation
- **Diagrams** - Visual representations

---

## References and Further Reading

- [Project Structure Best Practices](https://github.com/goldbergyoni/nodebestpractices)
- [Golang Code Layout](https://github.com/golang-standards/project-layout)
- [Maven Standard Directory Layout](https://maven.apache.org/guides/introduction/introduction-to-the-standard-directory-layout.html)
- [Python Packaging Guide](https://packaging.python.org/tutorials/packaging-projects/)
- [NestJS Documentation - Modules](https://docs.nestjs.com/modules)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design (DDD)](https://en.wikipedia.org/wiki/Domain-driven_design)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

## Quick Decision Guide

```
STRUCTURE SELECTION FLOWCHART
=============================

1. What's your project type?
   ├─ Web API/Backend?
   │  ├─ Small (< 500 lines)? → By-type pattern
   │  ├─ Medium (< 5K lines)? → Feature-based pattern
   │  └─ Large? → Layered or DDD pattern
   ├─ Frontend/UI?
   │  ├─ Small (< 10 components)? → By-type
   │  ├─ Medium? → Feature-based (pages + components)
   │  └─ Large? → Feature-based with shared utils
   ├─ Library?
   │  └─ Follow language-standard structure
   ├─ Multiple services?
   │  └─ Monorepo or separate repositories
   └─ Complex domain?
      └─ Consider DDD

2. How many files do you have?
   ├─ < 50? → By-type is fine
   ├─ 50-500? → Feature-based recommended
   └─ > 500? → Layered or DDD, consider splitting
```
