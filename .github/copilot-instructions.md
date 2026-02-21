# Copilot Instructions

## Build, test, and lint commands

No build, test, or lint configuration files were found in this repository at the moment (no package manifests, project files, or test runner configs).  
When adding implementation code, add the canonical build/test/lint commands here, including a single-test invocation.

## High-level architecture

This repository is organized as a requirements-based delivery flow with top-level phases:
- `user-requirements/` for user-level requirement capture
- `architecture/` for documentation of system architecture decisions
- `design/` for design-level requirements
- `implementation/` for source code
- `verification/` for verification of implementation, typically automated tests

Work should preserve traceability across these phases (requirements -> design -> implementation and verification).

Architecture decisions describe the organization of the product, and links to user-requirements.

Design requirements are refinements of user requirements, and take into account architecture decisions.

Implementation and verification can be traced back to design requirements. Verification contains tests that verify that their corresponding implementation complies to the design requirements.

## Key conventions

- Keep artifacts in the phase-specific top-level directory rather than mixing phase outputs.
- Treat this repository as process-first: architecture/design/verification artifacts are first-class deliverables alongside implementation.
- Maintain forward traceability when adding new artifacts by linking related items across phase directories.
