# Contributing to AIMeeting

Thank you for your interest in contributing to AIMeeting! This guide will help you get started.

## Quick Start for Contributors

### Prerequisites

- **.NET 8 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **GitHub Copilot CLI** ([Install](https://github.com/github/copilot-cli))
- **Git** ([Download](https://git-scm.com/))
- **Active GitHub Copilot subscription** (required for running meetings)

### Setup

```bash
# Clone the repository
git clone https://github.com/jonv11/AIMeeting.git
cd AIMeeting

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
AIMEETING_AGENT_MODE=stub dotnet test
```

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes

- Write clean, idiomatic C# code
- Follow the [coding standards](docs/guides/standards/)
- Add tests for new functionality
- Update documentation as needed

### 3. Run Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage (if configured)
dotnet test /p:CollectCoverage=true
```

### 4. Commit Your Changes

```bash
git add .
git commit -m "Brief description of your changes"
```

Follow these commit message guidelines:
- Use present tense ("Add feature" not "Added feature")
- Keep the first line under 50 characters
- Add a detailed description if needed

### 5. Submit a Pull Request

- Push your branch to your fork
- Create a Pull Request on GitHub
- Fill out the PR template completely
- Link any related issues

## Project Standards

All contributions must follow these standards (see [docs/guides/standards/](docs/guides/standards/) for details):

### Must-Read Standards
- **[Security Best Practices](docs/guides/standards/security.md)** - Critical security requirements
- **[Error Handling](docs/guides/standards/error-handling.md)** - Exception handling patterns
- **[Testing Strategy](docs/guides/standards/testing.md)** - Test coverage and quality requirements
- **[Git Workflow](docs/guides/standards/git-workflow.md)** - Branch and commit conventions

### Important Standards
- **[API Design](docs/guides/standards/api-design.md)** - Interface design patterns
- **[Naming Conventions](docs/guides/standards/naming-conventions.md)** - Code naming standards
- **[Code Review](docs/guides/standards/code-review.md)** - PR review expectations

### Reference Standards
- **[Documentation](docs/guides/standards/documentation.md)** - Documentation guidelines
- **[Markdown Guidelines](docs/guides/standards/markdown.md)** - Markdown formatting
- **[Project Structure](docs/guides/standards/project-structure.md)** - Repository organization

## Testing Requirements

All code contributions must include:

- ✅ **Unit tests** for new functionality
- ✅ **Integration tests** for cross-component features
- ✅ **Test coverage** of at least 80% for new code
- ✅ **All tests passing** before submitting PR

Run tests in stub mode to avoid external API dependencies:
```bash
AIMEETING_AGENT_MODE=stub dotnet test
```

## Code Review Process

1. **Automated Checks**: CI/CD pipeline runs tests and linters
2. **Peer Review**: At least one maintainer reviews your code
3. **Feedback**: Address review comments and requested changes
4. **Approval**: Once approved, a maintainer will merge your PR

## Documentation

When contributing code:

- Update relevant documentation in `docs/`
- Add examples to `docs/reference/examples.md` if applicable
- Update API documentation in `docs/reference/api.md` for public interfaces
- Add entries to FAQ if introducing new concepts

## Questions?

- Check the **[FAQ](docs/learning/faq.md)**
- Review **[Getting Started](docs/learning/getting-started.md)**
- Browse **[Documentation Hub](docs/README.md)**
- Open a discussion on GitHub

## Code of Conduct

By participating in this project, you agree to maintain a respectful and professional environment. Be constructive in feedback, patient with newcomers, and collaborative in problem-solving.

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (see [LICENSE](LICENSE)).

---

**Thank you for contributing to AIMeeting!** 🎉
