# ADR-002: Use F# as Primary .NET Language for External Tooling

## Status
Accepted

## Date
2026-02-21

## Context
The architecture requires external tooling for validation, diagnostics processing, and automation support aligned with AI-agent-driven development (`UR-018`) and automated conformance workflows (`UR-015`, `UR-016`).

## Decision
- Use **F#** as the default language for new external .NET tooling.
- Use **C# only when required** by framework constraints or integration boundaries, typically at entry points/host layers in .NET frameworks.

## Rationale
- F# provides strong static typing and concise expression of domain rules, improving correctness for validation and analysis tooling.
- Functional-first design helps reduce incidental state complexity in test/diagnostic pipelines.
- Retaining C# for required host integration preserves compatibility with framework expectations and ecosystem tooling.

## Consequences
### Positive
- More reliable rule-heavy tooling (mission checks, traceability checks, diagnostics analysis).
- Better fit for AI-assisted generation/refinement of deterministic, testable helper services.

### Trade-offs
- Team needs F# proficiency for tooling code reviews and maintenance.
- Mixed-language .NET tooling requires clear project boundaries and conventions.

## Guardrails
- Keep C# usage minimal and boundary-focused (application host, framework glue, required SDK entry points).
- Implement domain logic, transformation pipelines, and validation engines in F#.
- Enforce automated tests and static analysis in CI for both F# and boundary C# projects.

## Traceability
- Supports `UR-018` (AI-suitable technologies), `UR-015` (automated conformance), and `UR-016` (diagnostics support).
