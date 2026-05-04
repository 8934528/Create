# Contributing to Create

First off, thank you for considering contributing to Create! It's people like you that make this tool better for everyone.

## Code of Conduct

By participating in this project, you agree to maintain a professional and respectful environment.

## How Can I Contribute?

### Reporting Bugs

- Use the GitHub Issues tracker.
- Describe the bug in detail and provide steps to reproduce.
- Mention your environment (.NET version, Python version, OS).

### Suggesting Enhancements

- Open an issue with the "enhancement" label.
- Explain why this feature would be useful and how it should work.

### Pull Requests

1. **Fork the repository**.
2. **Create a branch**: `feat/your-feature-name` or `fix/bug-name`.
3. **Write clean code**:
    - Follow C# Coding Conventions for the .NET parts.
    - Follow PEP 8 for the Python parts.
4. **Document your changes**: Update the `ARCHITECTURE.md` or `README.md` if necessary.
5. **Submit a PR**: Provide a clear description of what you've changed.

## Coding Standards

### .NET (C#)

- Use **PascalCase** for classes, methods, and properties.
- Use **camelCase** for private fields and local variables.
- Always add **XML Documentation** (`///`) for public API members.
- Follow Clean Architecture: Keep logic in `Create.Application`, keep entities in `Create.Domain`.

### Python

- Follow **PEP 8** style guide.
- Use **Google-style docstrings**.
- Use **Type Hints** for all function signatures.
- Keep the `ai-service` stateless.

## Branching Strategy

- `main`: Production-ready code.
- `develop`: Integration branch for new features.
- `feat/*`: Feature branches.
- `fix/*`: Bug fix branches.

---

Thank you for your help!
