# Software Design Document

## Document Metadata
- Document ID: SDD-001
- Version: 1.0
- Status: Draft
- Inputs: `architecture\architecture-breakdown.md`, `architecture\adr-001-unreal-engine.md`, `architecture\adr-002-tooling-language-fsharp.md`, `user-requirements\user-requirements.md`

## Purpose
Define software design components (modules, tools, and services) that realize `UR-001` through `UR-020`.

## Design Components

| ID | Component | Type | Primary technologies | Responsibilities | Depends on | Requirement mapping |
|---|---|---|---|---|---|---|
| DC-01 | Squad Control | Runtime module | Unreal C++ | Soldier possession, live switching, squad ownership/state. | DC-02, DC-04 | UR-001, UR-003, UR-006 |
| DC-02 | Camera & View | Runtime module | Unreal C++/Blueprint | First-person and close third-person camera control/transitions. | DC-01 | UR-002 |
| DC-03 | Timeline/Rewind | Runtime module | Unreal C++ | Action recording, snapshots, rewind navigation, replay execution. | DC-01, DC-04 | UR-004, UR-005 |
| DC-04 | Engagement Rules | Runtime module | Unreal C++ | Engagement detection, rewind lock, combat-state policy, switch allowance in combat. | DC-01 | UR-007, UR-008 |
| DC-05 | Squad Survival AI | Runtime module | Unreal C++ + Behavior Trees/EQS | Non-controlled soldier self-protection and cover behavior. | DC-04 | UR-009 |
| DC-06 | Combat & Weapons | Runtime module | Unreal C++ + Data Assets | Weapon behavior/data for rifles, pistols, machine guns, shock/frag grenades. | DC-01 | UR-013 |
| DC-07 | HUD & State Feedback | Runtime module | UMG/Blueprint | Show controlled soldier, replaying entities, engagement/rewind lock states. | DC-01, DC-03, DC-04 | UR-010 |
| DC-08 | VR Runtime | Runtime module | Unreal C++ + XR plugins | VR mode enablement, VR input profile, VR HUD adaptations. | DC-02, DC-07 | UR-014 |
| DC-09 | Prototype Scenario Pack | Runtime module/content | Unreal level + C++ data setup | Fixed level, hard-coded mission, fixed squad/loadout, stationary enemies firing on discovery. | DC-01, DC-03, DC-04, DC-06 | UR-017 |
| DC-10 | Level Designer | Tool (editor) | Unreal Editor extensions | Author/edit 3D level geometry and associated editor workflows. | DC-13 | UR-011 |
| DC-11 | Mission Designer | Tool (editor) | Unreal Editor extensions + data schemas | Author spawn/save/objective/event/trigger mission data and behavior links. | DC-13 | UR-012 |
| DC-12 | Asset/Content Validator | Tool (external) | F# (.NET), optional C# host | Validate mission links, spawn validity, schema conformance, content integrity. | DC-10, DC-11 | UR-011, UR-012, UR-015, UR-018 |
| DC-13 | Diagnostics API & Logging | Service/runtime support | Unreal C++, structured logs | Expose runtime diagnostics via logs/API, component traces, AI decision traces. | DC-01..DC-11 | UR-016, UR-018 |
| DC-14 | Telemetry & Dashboard | Service | TS/JS or .NET service + storage | Ingest diagnostics, provide dashboards for iteration/debug loops. | DC-13 | UR-016, UR-018 |
| DC-15 | Automated Verification Pipeline | Service/automation | CI + PowerShell/Bash + F# tooling | Build/test orchestration, requirement-mapped test evidence publishing. | DC-12, DC-13 | UR-015, UR-018 |
| DC-16 | Platform Packaging | Service/automation | Unreal build tooling + CI scripts | Build/package/validate Windows and Linux outputs. | DC-15 | UR-019 |
| DC-17 | Performance Baseline Runner | Service/tool | Automated benchmark scenarios | Run benchmark scenes/scenarios aligned to recommended hardware baseline. | DC-09, DC-16 | UR-020 |

## Integration and Data Contracts
- Runtime modules exchange state through explicit interfaces/events (squad state, engagement state, rewind state, UI state).
- Mission and level authoring outputs are versioned data assets consumed by runtime modules.
- Diagnostics events use structured schemas so automated tools can correlate failures to requirement IDs.

## Technology Allocation Rules
- Runtime gameplay authority: **C++**.
- Designer-facing orchestration: **Blueprints**.
- External .NET tooling default: **F#**.
- **C#** allowed only where framework-required (host/entry-point/integration boundaries).

## Traceability Notes
- Full requirement coverage target: `UR-001`..`UR-020`.
- Asset planning reference: `design\asset-needs-by-requirement.md`.
- Architecture references: `architecture\architecture-breakdown.md`, `architecture\technology-candidates.md`.
