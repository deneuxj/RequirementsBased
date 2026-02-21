# Technology Candidates for User Requirements

## Context
This document identifies relevant potential technologies to satisfy `UR-001` through `UR-016` in `user-requirements\user-requirements.md`.

## Candidate Product Stacks

### Option A: Unreal Engine 5 Ecosystem
- **Engine/runtime:** Unreal Engine 5 (C++ + Blueprints)
- **Why relevant:** Strong FPS/third-person foundations (`UR-001`, `UR-002`), mature AI/behavior tooling for defensive non-controlled squad behavior (`UR-009`), broad VR support (`UR-014`), and high-fidelity world tooling.
- **Designer support:** Built-in level tools + Data Assets + Sequencer/Gameplay Ability System extensions for mission/event authoring (`UR-011`, `UR-012`).
- **Testing/diagnostics:** Unreal Automation Framework + Functional Tests (`UR-015`), Unreal Insights + structured logs and telemetry bridges (`UR-016`).

### Option B: Unity + DOTS/Netcode-Compatible Architecture
- **Engine/runtime:** Unity (C#), URP/HDRP depending on target hardware
- **Why relevant:** Fast iteration for gameplay prototypes, flexible custom timeline/rewind implementation (`UR-003` to `UR-008`), mature package ecosystem.
- **Designer support:** Scene tools + ProBuilder for geometry (`UR-011`), custom mission editor windows/ScriptableObjects for objectives/triggers/events (`UR-012`).
- **Testing/diagnostics:** Unity Test Framework + PlayMode tests (`UR-015`), Unity Profiler + structured logging/OpenTelemetry exporters (`UR-016`).

### Option C: Godot 4 (Open-Source Path)
- **Engine/runtime:** Godot 4 (GDScript/C#)
- **Why relevant:** Lower licensing friction and rapid tooling customization; suitable for smaller-scope tactical shooter prototypes.
- **Designer support:** Native scene editing for geometry and triggers with custom editor plugins (`UR-011`, `UR-012`).
- **Testing/diagnostics:** GUT/WAT testing frameworks and custom trace/log APIs (`UR-015`, `UR-016`).
- **Constraint:** Higher engineering investment for AAA-style shooter fidelity and VR polish compared to Unreal/Unity.

## Cross-Cutting Technology Building Blocks

- **Timeline rewind/replay (`UR-004`, `UR-005`, `UR-007`, `UR-008`):**
  - Deterministic action recording (command/event log)
  - State snapshot checkpoints + delta replay
  - Conflict resolution rules when switching controlled soldiers
- **Combat engagement gating (`UR-007`):**
  - Explicit engagement state model in gameplay state machine
- **Non-controlled soldier survival behavior (`UR-009`):**
  - Behavior Trees / Utility AI for cover-seeking and self-preservation
- **Mission tooling (`UR-012`):**
  - Data-driven schemas for spawn points, save points, objectives, events, triggers
  - In-editor validation rules (broken trigger links, invalid spawn geometry)
- **Weapon content (`UR-013`):**
  - Modular weapon data definitions (rifles, pistols, machine guns, shock/frag grenades)
- **Automated conformance (`UR-015`):**
  - Requirement-to-test trace matrix in verification artifacts
  - Scenario integration tests for rewind/switching/combat constraints
- **Diagnostics (`UR-016`):**
  - Structured logging standard (JSON/event schema)
  - Runtime diagnostics API (state snapshots, trigger/event traces, AI decision traces)

## Initial Recommendation
Use **Option A (Unreal Engine 5)** as primary candidate due to fit for tactical shooter fidelity, VR support, mature testing/profiling, and strong built-in editor tooling that can be extended into the required level and mission designers.

## Traceability Notes
- `UR-001`..`UR-010`: Core gameplay runtime, timeline, AI behavior, and UX state communication
- `UR-011`..`UR-012`: Editor/tooling capabilities for content creation
- `UR-013`: Weapon system and data-driven loadout content
- `UR-014`: VR runtime and input stack
- `UR-015`..`UR-016`: Verification and observability infrastructure
