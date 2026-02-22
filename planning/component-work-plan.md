# Component Work Plan

## Purpose
This plan orders implementation work based on design component dependencies in `design\software-design.md`.

## Planning Scope
- This directory is for internal planning only (not a product deliverable).
- Execution outcomes are tracked under `execution\`.
- Asset planning source is `design\asset-needs-by-requirement.md`.

## Asset Acquisition and Creation Workstream
- Follow a **marketplace-first** strategy: acquire high-quality free assets first, then create/customize only uncovered gaps.
- Track each required asset with status: `needed -> acquired/created -> integrated -> verified`.
- Keep one asset backlog grouped by component phase so implementation is never blocked by missing content.
- Record executed acquisition/creation work in `execution\execution-reports.md`.

## Dependency-Aware Implementation Order

### Phase 0 - Process and traceability foundation (first)
1. **DC-21 Requirements Traceability Tooling**

**Why first:** Establishes the requirements-based, traceable development process and reporting backbone before feature implementation (`UR-022`, `UR-018`, `UR-015`) via DC-21.
**Asset focus:** no major gameplay assets; create templates/schemas for trace links, report formats, and tooling configuration artifacts.

### Phase 1 - Core runtime foundations
3. **DC-04 Engagement Rules**
4. **DC-01 Squad Control**
5. **DC-02 Camera & View**
6. **DC-20 Gameplay State Manager**
7. **DC-03 Timeline/Rewind**
8. **DC-07 HUD & State Feedback**

**Why first:** Enables core squad loop, authoritative gameplay state transitions, combat state rules, rewind/replay behavior, and essential player feedback (`UR-001`..`UR-008`, `UR-010`, `UR-021`).
**Asset focus:** acquire/create baseline squad characters, camera rigs, and core HUD/UI/VFX cues plus save-state indicators (UR-001, UR-002, UR-003, UR-004, UR-005, UR-007, UR-008, UR-010, UR-021).

### Phase 2 - Combat and behavior depth
9. **DC-05 Squad Survival AI**
10. **DC-06 Combat & Weapons**
11. **DC-09 Prototype Scenario Pack**

**Why second:** Builds meaningful gameplay and validates the main loop in a fixed prototype context (`UR-009`, `UR-013`, `UR-017`).
**Asset focus:** acquire/create weapon sets, combat effects, defensive animations, fixed prototype level kit, and stationary enemy assets (UR-009, UR-013, UR-017).

### Phase 3 - Platform feature extension
12. **DC-08 VR Runtime**

**Why here:** Depends on stable camera/HUD/core loop and is best integrated after prototype stability (`UR-014`).
**Asset focus:** acquire/create VR hand rigs, controller interaction assets, and VR HUD variants (UR-014).

### Phase 4 - Observability and tooling base
13. **DC-13 Diagnostics API & Logging**
14. **DC-10 Level Designer**
15. **DC-11 Mission Designer**
16. **DC-12 Asset/Content Validator**

**Why here:** Diagnostics become the backbone for tool validation; level/mission tools then produce data validated by dedicated checks (`UR-011`, `UR-012`, `UR-015`, `UR-016`, `UR-018`).
**Asset focus:** acquire/create level/mission editor icon sets, template event assets, test fixtures, and diagnostics visualization assets (UR-011, UR-012, UR-015, UR-016).

### Phase 5 - Automation, delivery, and performance closure
17. **DC-14 Telemetry & Dashboard**
18. **DC-15 Automated Verification Pipeline**
19. **DC-16 Platform Packaging**
20. **DC-17 Performance Baseline Runner**

**Why last:** Finalize CI evidence, target platform outputs, and performance verification against recommended hardware (`UR-015`, `UR-016`, `UR-018`, `UR-019`, `UR-020`).
**Asset focus:** acquire/create platform packaging assets (icons/templates), benchmark scenes, and performance test content (UR-019, UR-020).

## Parallelization Opportunities
- **DC-10** and **DC-11** can run in parallel once **DC-13** is available.
- **DC-14** can start as soon as stable diagnostics events exist from **DC-13**.
- **DC-03** and **DC-07** should start after **DC-20** gameplay state contracts are stable.
- Test authoring for **DC-15** should run continuously from Phase 1 onward, then consolidated in Phase 5.

## Done Criteria per Phase
- Each completed component has requirement-linked verification evidence.
- Component status and outcomes are recorded in `execution\execution-reports.md`.
- Required assets for in-scope requirements are either integrated or explicitly deferred with rationale.
