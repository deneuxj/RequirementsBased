# Copilot Instructions

## Build, test, and lint commands

Traceability tool (.NET/F#) canonical commands:
- Restore: `dotnet restore implementation\\traceability-tool\\TraceabilityTool.sln`
- Build: `dotnet build implementation\\traceability-tool\\TraceabilityTool.sln --nologo`
- Test (all): `dotnet test implementation\\traceability-tool\\TraceabilityTool.sln --nologo`
- Test (single): `dotnet test verification\\traceability-tool-tests\\TraceabilityTool.Tests.fsproj --filter \"FullyQualifiedName~Tests.cli analyze returns non-zero for policy violations\" --nologo`

No linter is configured yet for this repository.

## High-level architecture

This repository is organized as a requirements-based delivery flow with top-level phases:
- `user-requirements/` for user-level requirement capture
- `architecture/` for documentation of system architecture decisions
- `design/` for design-level requirements
- `implementation/` for source code
- `verification/` for verification of implementation, typically automated tests
- `planning/` for internal work planning and sequencing (non-deliverable tracking aid)
- `execution/` for reports of executed plan parts (non-deliverable tracking aid)

Work should preserve traceability across these phases (requirements -> design -> implementation and verification).

Architecture decisions describe the organization of the product, and links to user-requirements.

Design requirements are refinements of user requirements, and take into account architecture decisions.

Implementation and verification can be traced back to design requirements. Verification contains tests that verify that their corresponding implementation complies to the design requirements.

## Key conventions

- Keep artifacts in the phase-specific top-level directory rather than mixing phase outputs.
- Treat this repository as process-first: architecture/design/verification artifacts are first-class deliverables alongside implementation.
- Maintain forward traceability when adding new artifacts by linking related items across phase directories.
- Use `planning/` and `execution/` only for internal progress tracking artifacts, not product deliverables.
