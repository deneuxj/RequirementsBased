# Architecture Breakdown (Modules, Tools, Services)

## 1) Runtime Product Modules (Unreal Engine)
- **Squad Control Module (C++):** soldier possession, live switching, squad state ownership (`UR-001`, `UR-003`, `UR-006`).
- **Camera & Presentation Module (C++/Blueprint):** first-person/close-third-person camera modes and transitions (`UR-002`).
- **Timeline/Rewind Module (C++):** command/event recording, snapshots, rewind cursor, replay execution (`UR-004`, `UR-005`).
- **Engagement Rules Module (C++):** combat engagement detection, rewind lock policy, switch-while-engaged allowance (`UR-007`, `UR-008`).
- **Squad Survival AI Module (C++/Behavior Trees):** non-controlled soldier cover/self-preservation behavior (`UR-009`).
- **HUD/UI State Module (UMG/Blueprint):** controlled soldier, replaying soldiers, rewind lock indicators (`UR-010`).
- **Weapons & Combat Module (C++/Data Assets):** rifles, pistols, machine guns, shock/frag grenades (`UR-013`).
- **VR Interaction Module (C++/XR plugins):** VR mode bootstrap, input bindings, VR HUD variants (`UR-014`).
- **Prototype Scenario Module (C++/Data):** fixed level, hard-coded mission, fixed squad/loadout, stationary reactive enemies (`UR-017`).
- **Save/Load Persistence Module (C++):** save at arbitrary gameplay points, load for player/developer workflows, and compatibility-gated loading of older-version saves (`UR-021`).

## 2) Authoring Tools
- **Level Designer Toolchain (Editor extensions):** 3D geometry authoring, asset placement, collision/material presets (`UR-011`).
- **Mission Designer Toolchain (Editor + data schemas):** spawn points, save points, objectives, events, triggers and behavior effects (`UR-012`).
- **Asset Validation Tool:** static checks for missing links, invalid trigger/event references, spawn validity (`UR-011`, `UR-012`, `UR-015`).

## 3) External Services and Support Components
- **Diagnostics Service/API:** structured logs, traces, runtime state endpoints, AI decision/event traces (`UR-016`).
- **Telemetry/Observability Backend:** ingestion + dashboards for gameplay and tool diagnostics (`UR-016`).
- **Build/Test Orchestration Service (CI):** automated build, test execution, result publication, traceability evidence (`UR-015`, `UR-018`).
- **Platform Packaging Service:** Windows/Linux build artifacts and release validation (`UR-019`).
- **Performance Baseline Runner:** benchmark scenarios aligned with recommended hardware profile (`UR-020`).
- **Save Compatibility Regression Service:** automated replay/load checks for archived saves from compatible historical versions (`UR-021`).

## 4) Language Allocation
- **C++ + Blueprints:** runtime gameplay and editor integration.
- **F# (default external .NET tooling):** validators, traceability analyzers, diagnostics processors.
- **C# (boundary-only):** host/entry-point integration when framework-required.
- **Python + PowerShell/Bash:** content pipeline and automation scripting.

## 5) Verification Structure
- **Unit tests:** module-level logic (timeline rules, engagement gating, data validators).
- **Integration tests:** mission flow, rewind/replay consistency, switching behavior.
- **Scenario tests:** prototype main loop and combat behavior checks.
- **Compatibility tests:** save/load across versions declared compatible by the compatibility policy.
- **Traceability tests:** requirement-ID mapped pass/fail outputs for `UR-001`..`UR-021`.

## 6) Suggested Delivery Sequence
1. Prototype Scenario + Squad Control + Timeline/Rewind + Save/Load persistence core.
2. Engagement Rules + Survival AI + HUD state clarity.
3. Weapons module + fixed content completion.
4. Level/Mission designer tooling and validators.
5. Diagnostics service + automated conformance + platform packaging/perf baselining.
