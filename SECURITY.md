# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 0.1.x   | :white_check_mark: |
| < 0.1   | :x:                |

## Reporting a Vulnerability

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please report them via email to: [Your security contact email]

You should receive a response within 48 hours. If for some reason you do not, please follow up via email to ensure we received your original message.

Please include the following information in your report:

- Type of vulnerability
- Full paths of source file(s) related to the vulnerability
- Location of the affected source code (tag/branch/commit or direct URL)
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the vulnerability, including how an attacker might exploit it

This information will help us triage your report more quickly.

## Preferred Languages

We prefer all communications to be in English.

## Security Best Practices

When contributing to AIMeeting, please follow these security guidelines:

### Code Security
- Never commit secrets, API keys, or credentials
- Use environment variables for sensitive configuration
- Validate all file paths to prevent path traversal attacks
- Sanitize user input in agent configurations
- Follow the [Security Best Practices](docs/guides/standards/security.md) guide

### Agent Configuration Security
- Agent config files are limited to 64KB to prevent resource exhaustion
- All file paths are validated against the allowed directories
- Meeting rooms are isolated with path traversal protection
- No code execution from agent configuration files

### Dependencies
- Keep dependencies up to date
- Review security advisories for .NET and NuGet packages
- Use Dependabot for automated dependency updates (coming soon)

## Known Security Considerations

### v0.1.0 (Current)

1. **GitHub Copilot CLI Dependency**
   - AIMeeting relies on GitHub Copilot CLI for AI responses
   - Requires active GitHub Copilot subscription
   - API calls are made to external services

2. **Meeting Room Isolation**
   - Each meeting operates in isolated directory
   - Path traversal protection implemented
   - File locking prevents concurrent corruption

3. **Configuration Validation**
   - Agent configs are validated before loading
   - Maximum file size: 64KB
   - UTF-8 encoding enforced

## Security Update Policy

Security updates are released as soon as possible after a vulnerability is confirmed and a fix is developed. We aim to:

- Acknowledge receipt of vulnerability reports within 48 hours
- Provide an initial assessment within 5 business days
- Release security patches within 30 days for critical vulnerabilities
- Publish security advisories on GitHub

## Recognition

We appreciate the security research community's efforts in responsibly disclosing vulnerabilities. Contributors who report valid security issues may be acknowledged in our release notes (with their permission).

---

**Last Updated**: January 31, 2026  
**Contact**: [Your email or security contact]
