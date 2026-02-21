# Language and Scripting Guidance for Architecture

## Purpose
Define applicable programming/scripting languages for the selected Unreal-based architecture and provide guidance for AI coding efficiency and reliability.

## Applicable Languages

### Primary product implementation
- **C++ (primary):** Core gameplay systems, performance-critical logic, engine integrations, networking/serialization, deterministic replay components.
- **Blueprints (visual scripting):** Rapid gameplay iteration, designer-authored behavior composition, mission/interaction glue logic, UI flow prototyping.

### Tooling and automation
- **Python:** Editor scripting, asset pipeline automation, data import/export, batch content validation, build/test orchestration helpers.
- **PowerShell/Bash:** CI scripts, local developer automation, packaging and verification command orchestration on Windows/Linux.

### Optional supporting languages
- **TypeScript/JavaScript:** Web-based diagnostics dashboards, telemetry viewers, lightweight service tooling.
- **F# (preferred for .NET tooling):** Domain-heavy validation/analysis tooling, traceability processors, deterministic transformation pipelines.
- **C# (boundary-only):** Use only where required by framework entry points/host integration in .NET stacks.

## Recommendation on Static Typing for AI Coding
Yes. To maximize reliability with AI coding tools, prefer **statically typed languages for core code paths**.

### Why static typing helps AI workflows
- Surfaces errors early through compilation/type-checking, reducing hidden runtime defects.
- Gives AI agents stronger constraints and clearer contracts, improving edit precision.
- Enables faster automated feedback loops in CI, which is critical for iterative agent-driven development.
- Improves safe refactoring across large codebases through type-aware tooling.

### Practical policy for this product
- **Use C++ for authoritative gameplay/runtime systems** (squad control, rewind/replay, combat gating, AI behavior state, diagnostics APIs).
- **Use Blueprints for controlled high-level orchestration**, but keep interfaces typed and narrow.
- **Use Python mainly for tooling**, with typed schemas/contracts at boundaries (JSON schema, strict data models, validation rules).
- **Use F# as the default for new external .NET tooling**, with C# constrained to required host/entry-point layers.
- Require every AI-generated change to pass compile/type checks and automated tests before acceptance.

## Traceability to User Requirements
- **UR-018:** Technology choices support AI coding-agent workflows.
- **UR-015 / UR-016:** Typed core systems + automation improve testability and diagnosability.
- **UR-019 / UR-020:** Language/tool choices support Windows/Linux delivery and performance-focused development.
