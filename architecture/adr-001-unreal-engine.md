# ADR-001: Use Unreal Engine 5 as the Product Engine

## Status
Accepted

## Date
2026-02-21

## Context
The product requires:
- Tactical squad shooter gameplay with first-person/close third-person control and soldier switching (`UR-001` to `UR-003`)
- Timeline rewind/replay mechanics with combat constraints (`UR-004` to `UR-008`)
- Defensive behavior for non-controlled soldiers (`UR-009`)
- Built-in tooling for level and mission authoring (`UR-011`, `UR-012`)
- Diverse weapon systems (`UR-013`)
- VR support (`UR-014`)
- Strong automated verification and diagnostics support (`UR-015`, `UR-016`)
- Rapid prototype delivery for main loop validation (`UR-017`)

Technology candidates were evaluated in `architecture\technology-candidates.md`.

## Decision
Use **Unreal Engine 5** (C++ with Blueprint integration) as the primary engine platform.

## Rationale
- Strong native fit for FPS/third-person tactical gameplay and high-fidelity combat spaces.
- Mature AI tooling (Behavior Trees, EQS, perception systems) for non-controlled soldier defensive behavior.
- Robust editor ecosystem suitable for extending into required level and mission designer workflows.
- Broad VR platform support and proven performance/tooling for immersive interaction.
- Built-in profiling, automation, and diagnostics capabilities aligned with requirement conformance and iterative improvement.
- Vast availability of free marketplace assets that can accelerate prototype and early content development.

## Consequences
### Positive
- Faster delivery of a credible prototype for the main loop (`UR-017`).
- Lower implementation risk for advanced shooter mechanics and VR (`UR-001`..`UR-005`, `UR-014`).
- Clear path to automated testing and rich observability (`UR-015`, `UR-016`).

### Trade-offs
- Team must maintain C++/Blueprint development expertise.
- Unreal project footprint and build times are typically larger than lighter-weight engines.

## Traceability
- Primary user requirements addressed: `UR-001` through `UR-017`.
- Related architecture document: `architecture\technology-candidates.md`.
