# SOLID-Practice

A collection of C# code examples that are progressively refactored to follow **SOLID** design principles. Each example starts with a "before" version that violates one or more principles and ends with an "after" version that demonstrates the clean, SOLID-compliant solution.

---

## What is SOLID?

SOLID is an acronym for five object-oriented design principles that help developers write code that is easier to maintain, extend, and understand:

| Letter | Principle | Short Description |
|--------|-----------|-------------------|
| [**S**](----S) | **S**ingle Responsibility Principle (SRP) | A class should have only one reason to change. |
| [**O**](---O) | **O**pen/Closed Principle (OCP) | Software entities should be open for extension but closed for modification. |
| [**L**](--L) | **L**iskov Substitution Principle (LSP) | Subtypes must be substitutable for their base types without altering the correctness of the program. |
| [**I**](-I) | **I**nterface Segregation Principle (ISP) | Clients should not be forced to depend on interfaces they do not use. |
| [**D**](D) | **D**ependency Inversion Principle (DIP) | High-level modules should not depend on low-level modules; both should depend on abstractions. |
| [**SOLID**](SOLID) | **SOLID** | Applying every SOLID principle. |
---

## Repository Structure

Each SOLID principle will have its own folder containing:

```
<PrincipleName>/
├── Before/   # Original code that violates the principle
└── After/    # Refactored code that follows the principle
```

---

## Language

All code examples are written in **C#**.

---

## Goals

- Understand each SOLID principle through practical examples.
- Practice identifying design smells in existing code.
- Refactor code step-by-step toward a clean, maintainable architecture.

---

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/mathyc0de/SOLID-Practice.git
   ```
2. Open the desired principle folder in your preferred C# IDE (e.g., Visual Studio, Visual Studio Code with C# Dev Kit, or Rider).
3. Compare the `Before` and `After` projects to see the refactoring changes.

---
