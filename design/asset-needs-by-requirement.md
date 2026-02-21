# Design Asset Needs by User Requirement

## Scope
This design artifact lists assets needed per user requirement where assetization is relevant.  
Source requirements: `user-requirements\user-requirements.md` (`UR-001`..`UR-020`).

## Asset Mapping

| Requirement | Asset needs (when relevant) |
|---|---|
| UR-001 | Squad soldier character assets (3+ variants), tactical environment kit, squad HUD elements. |
| UR-002 | First-person arms/weapons animation sets, close third-person character rig/animations, camera profiles. |
| UR-003 | Soldier identity/UI switch indicators, control-transfer VFX/SFX cue assets. |
| UR-004 | Timeline rewind UI widgets, rewind VFX/SFX package, timeline marker visuals. |
| UR-005 | Replay-state indicators, ghost/replay visualization assets (subtle overlays/icons), synchronization cue SFX. |
| UR-006 | Shared tactical sandbox assets for multi-soldier coordination scenarios (cover props, chokepoints, elevation features). |
| UR-007 | Rewind-locked combat indicator UI assets and warning audio cues. |
| UR-008 | Combat-safe switching UI state assets (highlighted selectable squad members while engaged). |
| UR-009 | Cover node marker assets (editor/debug), defensive animation set (crouch, lean, blind-fire), hit reaction animations. |
| UR-010 | Full control-state HUD set (active soldier, replaying soldiers, engagement/rewind lock status). |
| UR-011 | Level designer asset packs: modular geometry kits, materials, terrain textures, collision presets, editor gizmo icons. |
| UR-012 | Mission designer authoring assets: spawn/save/objective/trigger/event icons, mission flow graph widgets, event template library (e.g., explosion, enemy spawn). |
| UR-013 | Weapon assets: rifles, pistols, machine guns, shock grenade, fragmentation grenade models/animations/SFX/VFX. |
| UR-014 | VR interaction assets: VR hand rigs, controller bindings/icons, VR HUD variants, comfort vignette assets. |
| UR-015 | Test content assets: deterministic test levels, scripted scenario fixtures, golden-reference outputs for regression. |
| UR-016 | Diagnostics assets: log schema templates, debug overlays, trace visualization widgets, telemetry dashboard templates. |
| UR-017 | Prototype-only asset bundle: one fixed level kit, fixed squad characters, fixed loadout set, stationary enemy character + muzzle/fire effects. |
| UR-018 | AI-agent-friendly development assets: coding templates, prompt/playbook assets, example scenes with known-good references. |
| UR-019 | Platform delivery assets: Windows/Linux packaging configs, launcher icons, platform-specific startup/config templates. |
| UR-020 | Performance baseline assets: benchmark replay scene, stress-test encounter setup, profiling capture presets targeting recommended hardware. |

## Notes
- Asset selection should prioritize high-quality free marketplace assets first, then custom content for uncovered gaps.
- Prototype phase (`UR-017`) should use a minimal, fixed subset of the full asset plan.
