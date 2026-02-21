# User Requirements Specification

## Document Metadata
- Document ID: URS-001
- Product: Tactical Squad Shooter (Time-Replay Control)
- Version: 1.0
- Status: Draft

## Purpose
Define user-level requirements for a tactical shooter where one player coordinates a small squad by switching control between soldiers and using timeline rewind/replay.

## Scope
These requirements describe player-visible behavior and constraints. Design and implementation details are intentionally excluded.

## User Requirements

| ID | Requirement Statement | Rationale | Acceptance Criteria |
|---|---|---|---|
| UR-001 | The game shall provide a player experience centered on controlling a small tactical squad rather than a single soldier. | Defines the core product value. | A play session requires use of multiple squad members to complete tactical objectives. |
| UR-002 | The game shall present gameplay from first-person view or close third-person view when a soldier is controlled. | Preserves immersive tactical control. | While controlling any soldier, camera mode is first-person or close third-person. |
| UR-003 | The player shall be able to switch live control between available squad soldiers during gameplay. | Enables direct control of the full squad. | Player can select another available soldier and control is transferred without loading screens. |
| UR-004 | The player shall be able to rewind to an earlier moment and take control of a different squad soldier. | Enables timeline-based squad coordination. | Player activates rewind, selects a past point, and resumes as another soldier. |
| UR-005 | Actions performed before rewind by a previously controlled soldier shall replay after timeline resume. | Supports coordinated multi-soldier outcomes without manual command scripting. | After rewind and resume, prior soldier reproduces previously recorded actions in sync with the timeline. |
| UR-006 | Core squad coordination shall be achievable through switching and rewind/replay mechanics without requiring command-heavy squad AI control by the player. | Protects the intended interaction model. | A representative mission can be completed using switching/rewind mechanics as the primary squad-control method. |
| UR-007 | Rewinding shall be unavailable while any squad member is actively engaged by enemies. | Prevents exploitation during immediate threat. | During active engagement state, rewind input is blocked and cannot start rewind mode. |
| UR-008 | Live switching between soldiers shall remain available while engaged by enemies. | Maintains tactical responsiveness under pressure. | During active engagement state, player can still switch to another available soldier. |
| UR-009 | While engaged, non-controlled soldiers shall perform immediate self-preservation behavior, including taking cover when feasible. | Keeps the squad viable when not directly controlled. | In engagements, non-controlled soldiers react defensively within a short, observable response window. |
| UR-010 | The game shall clearly communicate current control state and rewind availability to the player. | Reduces confusion in timeline management. | UI indicates controlled soldier, replaying soldiers, and explicit rewind-locked state when engagement blocks rewind. |
| UR-011 | The product shall include a level designer that allows users to design 3D geometry for playable levels. | Enables user/content-creator authored tactical spaces. | A user can create, edit, and save level 3D geometry using the included level designer. |
| UR-012 | The product shall include a mission designer that allows users to place and configure spawn points, save points, objectives, events (including explosions and enemy spawns), and trigger zones that can invoke events or alter enemy behavior. | Enables creation of mission flow and encounter logic without code changes. | A user can place, edit, and save all listed mission elements, and trigger zones can successfully fire linked events or modify enemy behavior in playable testing. |
| UR-013 | Weapons available to soldiers shall include multiple types of rifles, pistols, machine guns, and grenades (shock and fragmentation). | Ensures expected tactical loadout variety for squad roles and encounters. | In playable loadout/equipment selection, soldiers can be assigned weapons from all listed categories, including both shock and fragmentation grenades. |
| UR-014 | The product shall support gameplay in VR (virtual reality). | Extends immersion and platform accessibility for intended users. | A user with supported VR hardware can start and play missions in VR mode with core squad-control features available. |
| UR-015 | It shall be possible to assess implementation conformance to requirements using automated testing. | Ensures objective and repeatable validation of requirement fulfillment. | Automated tests can be executed to produce pass/fail evidence mapped to requirement IDs. |
| UR-016 | All product components shall expose diagnostic information through logging and/or APIs to support iterative implementation improvement by coding agents against the requirements. | Enables fast, evidence-based debugging and requirement conformance refinement. | For each major component, accessible diagnostics (logs or API endpoints) provide actionable runtime/state information sufficient to identify requirement gaps during automated or manual test runs. |
| UR-017 | A prototype shall be developed to validate the main game loop using a fixed level, a hard-coded mission, a fixed squad with fixed loadout, and simple stationary enemies that fire at discovered player soldiers. | De-risks core loop feasibility before full content/tooling scale-up. | A playable prototype mission exists with all listed fixed constraints and enemy behavior, and demonstrates the intended main game loop end-to-end. |
| UR-018 | Code development shall be performed mostly using AI coding agents, and selected technologies shall be suitable for AI-assisted development workflows. | Aligns delivery execution model with toolchain and ecosystem capabilities that maximize AI agent effectiveness. | The delivery stack provides AI-friendly documentation, automation interfaces, diagnostics, and testability enabling coding agents to implement and iterate on requirements with limited manual intervention. |
| UR-019 | The targeted deployment platform shall be gaming PCs running Windows or Linux. | Constrains runtime compatibility and technology choices to intended player environments. | Builds are available and runnable on supported Windows and Linux gaming PC configurations. |
| UR-020 | Recommended player system specifications shall include AMD Ryzen 7 5800X CPU, 32 GB RAM, and NVIDIA RTX 3080 GPU. | Defines a concrete performance target for development, testing, and player expectations. | Product documentation and test environments include the stated recommended specification baseline. |

## Traceability Seeds
- Design requirements should reference IDs `UR-001` through `UR-020`.
- Verification artifacts should map test cases to these same IDs.
