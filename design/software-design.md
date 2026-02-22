# Software Design Document

## Document Metadata
- Document ID: SDD-001
- Version: 1.0
- Status: Draft
- Inputs: `architecture\architecture-breakdown.md`, `architecture\adr-001-unreal-engine.md`, `architecture\adr-002-tooling-language-fsharp.md`, `user-requirements\user-requirements.md`

## Purpose
Define software design components (modules, tools, and services) that realize `UR-001` through `UR-022`.

## Design Components

| ID | Component | Type | Primary technologies | Responsibilities | Depends on | Requirement mapping |
|---|---|---|---|---|---|---|
| DC-01 | Squad Control | Runtime module | Unreal C++ | Soldier possession, live switching, squad ownership/state. | DC-02, DC-04 | UR-001, UR-003, UR-006 |
| DC-02 | Camera & View | Runtime module | Unreal C++/Blueprint | First-person and close third-person camera control/transitions. | DC-01 | UR-002 |
| DC-03 | Timeline/Rewind | Runtime module | Unreal C++ | Action recording, snapshots, rewind navigation, replay execution. | DC-01, DC-04, DC-20 | UR-004, UR-005 |
| DC-04 | Engagement Rules | Runtime module | Unreal C++ | Engagement detection, rewind lock, combat-state policy, switch allowance in combat. | DC-01 | UR-007, UR-008 |
| DC-20 | Gameplay State Manager | Runtime module | Unreal C++ | Maintain authoritative gameplay state machine (normal play, engagement, rewind, replay, paused/save) and publish state transitions for dependent systems. | DC-01, DC-03, DC-04 | UR-006, UR-007, UR-008, UR-010, UR-021 |
| DC-05 | Squad Survival AI | Runtime module | Unreal C++ + Behavior Trees/EQS | Non-controlled soldier self-protection and cover behavior. | DC-04 | UR-009 |
| DC-06 | Combat & Weapons | Runtime module | Unreal C++ + Data Assets | Weapon behavior/data for rifles, pistols, machine guns, shock/frag grenades. | DC-01 | UR-013 |
| DC-07 | HUD & State Feedback | Runtime module | UMG/Blueprint | Show controlled soldier, replaying entities, engagement/rewind lock states. | DC-01, DC-03, DC-04, DC-20 | UR-010 |
| DC-08 | VR Runtime | Runtime module | Unreal C++ + XR plugins | VR mode enablement, VR input profile, VR HUD adaptations. | DC-02, DC-07 | UR-014 |
| DC-09 | Prototype Scenario Pack | Runtime module/content | Unreal level + C++ data setup | Fixed level, hard-coded mission, fixed squad/loadout, stationary enemies firing on discovery. | DC-01, DC-03, DC-04, DC-06 | UR-017 |
| DC-10 | Level Designer | Tool (editor) | Unreal Editor extensions | Author/edit 3D level geometry and associated editor workflows. | DC-13 | UR-011 |
| DC-11 | Mission Designer | Tool (editor) | Unreal Editor extensions + data schemas | Author spawn/save/objective/event/trigger mission data and behavior links. | DC-13 | UR-012 |
| DC-12 | Asset/Content Validator | Tool (external) | F# (.NET), optional C# host | Validate mission links, spawn validity, schema conformance, content integrity. | DC-10, DC-11 | UR-011, UR-012, UR-015, UR-018 |
| DC-21 | Requirements Traceability Tooling | Tool (external) | F# (.NET), optional C# host + SQLite/JSON | Maintain requirement-design-implementation-verification link records and provide query/report generation for conformance status. | None (foundational) | UR-015, UR-018, UR-022 |
| DC-22 | Planning & Execution Tracking Tooling | Tool (external/process support) | F#/.NET + SQL + Markdown automation | Maintain actionable work backlog, dependency tracking, and execution report synchronization with traceability links. | DC-21 | UR-018, UR-022 |
| DC-13 | Diagnostics API & Logging | Service/runtime support | Unreal C++, structured logs | Expose runtime diagnostics via logs/API, component traces, AI decision traces. | DC-01..DC-11 | UR-016, UR-018 |
| DC-14 | Telemetry & Dashboard | Service | TS/JS or .NET service + storage | Ingest diagnostics, provide dashboards for iteration/debug loops. | DC-13 | UR-016, UR-018 |
| DC-15 | Automated Verification Pipeline | Service/automation | CI + PowerShell/Bash + F# tooling | Build/test orchestration, requirement-mapped test evidence publishing. | DC-12, DC-13, DC-19, DC-21 | UR-015, UR-018, UR-021, UR-022 |
| DC-16 | Platform Packaging | Service/automation | Unreal build tooling + CI scripts | Build/package/validate Windows and Linux outputs. | DC-15 | UR-019 |
| DC-17 | Performance Baseline Runner | Service/tool | Automated benchmark scenarios | Run benchmark scenes/scenarios aligned to recommended hardware baseline. | DC-09, DC-16 | UR-020 |
| DC-18 | Save/Load Persistence | Runtime module | Unreal C++ + SaveGame serialization | Save at arbitrary points, load saved gameplay for players/developers, enforce compatibility-gated old-save loading. | DC-01, DC-03, DC-04, DC-06, DC-20 | UR-021 |
| DC-19 | Save Compatibility Validator | Tool (external) | F# (.NET), optional C# host | Maintain save compatibility matrix and run regression load tests against archived compatible-version saves. | DC-18 | UR-021 |

## Design Requirement Definitions

### DC-01 - Squad Control
- Higher-level requirements: UR-001, UR-003, UR-006
- Definition: Runtime module responsible for soldier possession, live control switching, and squad ownership/state.

### DC-02 - Camera and View
- Higher-level requirements: UR-002
- Definition: Runtime module for first-person and close third-person camera control and transitions.

### DC-03 - Timeline and Rewind
- Higher-level requirements: UR-004, UR-005
- Definition: Runtime module for action recording, snapshots, rewind navigation, and replay execution.

### DC-04 - Engagement Rules
- Higher-level requirements: UR-007, UR-008
- Definition: Runtime module for engagement detection, rewind lock policy, and combat-state switching rules.

### DC-05 - Squad Survival AI
- Higher-level requirements: UR-009
- Definition: Runtime module implementing non-controlled soldier self-protection and cover behavior.

### DC-06 - Combat and Weapons
- Higher-level requirements: UR-013
- Definition: Runtime module defining weapon behavior/data for rifles, pistols, machine guns, and grenade variants.

### DC-07 - HUD and State Feedback
- Higher-level requirements: UR-010
- Definition: Runtime module presenting controlled soldier, replaying entities, and engagement/rewind lock state.

### DC-08 - VR Runtime
- Higher-level requirements: UR-014
- Definition: Runtime module enabling VR mode, VR input profile support, and VR HUD adaptations.

### DC-09 - Prototype Scenario Pack
- Higher-level requirements: UR-017
- Definition: Fixed prototype runtime/content package for main-loop validation (fixed level, mission, squad/loadout, stationary enemies).

### DC-10 - Level Designer
- Higher-level requirements: UR-011
- Definition: Editor tooling for authoring and editing 3D level geometry and associated workflows.

### DC-11 - Mission Designer
- Higher-level requirements: UR-012
- Definition: Editor tooling for authoring mission data (spawn/save/objective/event/trigger definitions and behavior links).

### DC-12 - Asset and Content Validator
- Higher-level requirements: UR-011, UR-012, UR-015, UR-018
- Definition: External tooling validating mission links, spawn validity, schema conformance, and content integrity.

### DC-13 - Diagnostics API and Logging
- Higher-level requirements: UR-016, UR-018
- Definition: Runtime/service support component exposing structured logs, runtime diagnostics, and component traces.

### DC-14 - Telemetry and Dashboard
- Higher-level requirements: UR-016, UR-018
- Definition: Service component ingesting diagnostics and providing analysis dashboards for iterative improvement.

### DC-15 - Automated Verification Pipeline
- Higher-level requirements: UR-015, UR-018, UR-021, UR-022
- Definition: Automation component orchestrating build/tests and publishing requirement-mapped verification evidence.

### DC-16 - Platform Packaging
- Higher-level requirements: UR-019
- Definition: Automation component building, packaging, and validating Windows/Linux outputs.

### DC-17 - Performance Baseline Runner
- Higher-level requirements: UR-020
- Definition: Service/tool component running benchmark scenarios aligned to recommended hardware baseline.

### DC-18 - Save and Load Persistence
- Higher-level requirements: UR-021
- Definition: Runtime module supporting save-at-any-point, load for player/developer workflows, and compatibility-gated old-save loading.

### DC-19 - Save Compatibility Validator
- Higher-level requirements: UR-021
- Definition: External tooling maintaining save compatibility matrix and regression checks for archived compatible-version saves.

### DC-20 - Gameplay State Manager
- Higher-level requirements: UR-006, UR-007, UR-008, UR-010, UR-021
- Definition: Runtime module maintaining authoritative gameplay state machine (normal play, engagement, rewind, replay, paused/save) and publishing state transitions.

### DC-21 - Requirements Traceability Tooling
- Higher-level requirements: UR-015, UR-018, UR-022
- Definition: Foundational external tooling maintaining requirement-design-implementation-verification links and generating conformance trace reports.

### DC-22 - Planning and Execution Tracking Tooling
- Higher-level requirements: UR-018, UR-022
- Definition: External process-support tooling maintaining work backlog/dependencies and synchronizing execution reporting with trace links.

## Integration and Data Contracts
- Runtime modules exchange state through explicit interfaces/events, with DC-20 as authoritative gameplay state source (squad state, engagement state, rewind state, UI state).
- Mission and level authoring outputs are versioned data assets consumed by runtime modules.
- Diagnostics events use structured schemas so automated tools can correlate failures to requirement IDs.
- Save data uses explicit versioned schemas with compatibility checks and migration/adapter paths where required.
- Traceability records use stable IDs (requirement IDs, component IDs, work item IDs, verification IDs) to support end-to-end impact analysis.

## Technology Allocation Rules
- Runtime gameplay authority: **C++**.
- Designer-facing orchestration: **Blueprints**.
- External .NET tooling default: **F#**.
- **C#** allowed only where framework-required (host/entry-point/integration boundaries).

## Traceability Notes
- Full requirement coverage target: `UR-001`..`UR-022`.
- Asset planning reference: `design\asset-needs-by-requirement.md`.
- Architecture references: `architecture\architecture-breakdown.md`, `architecture\technology-candidates.md`.
