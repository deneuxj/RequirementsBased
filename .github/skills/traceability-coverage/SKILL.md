---
name: traceability-coverage
description: Verify requirement coverage with the F# traceability tool. Use this when asked to check mapping completeness across user->design, design->implementation, and design->test.
---

# Traceability Coverage Skill

Use this skill to verify requirement coverage and report unmapped requirements.

## When to use
- User asks to verify requirement coverage.
- User asks for requirement traceability status.
- User asks for unmapped requirements.

## Required workflow
1. Run analysis from repository root:
   - `dotnet run --project implementation\traceability-tool\src\TraceabilityTool.Cli\TraceabilityTool.Cli.fsproj -- analyze --root . --output-dir .\verification\traceability-output`
2. Read:
   - `verification\traceability-output\traceability-report.json`
   - `verification\traceability-output\traceability-report.jsonl`
   - `verification\traceability-output\traceability-summary.md`
3. Report:
   - Transition coverage for `user->design`, `design->implementation`, `design->test`
   - Unmapped requirements and suggested actions
   - Diagnostics with file/line evidence
4. If requested, apply focused fixes by adding `REQ:<ID>` or `TRACE:<FROM_ID>-><TO_ID>` markers in the appropriate downstream artifacts.

## Marker conventions
- Requirement marker: `REQ:<ID>` (example: `REQ:UR-022`)
- Explicit trace marker: `TRACE:<FROM_ID>-><TO_ID>`

## Output expectations (AI-consumable)
- Prefer JSON/JSONL findings in responses when precision is required.
- Preserve severity values (`info`, `warning`, `error`) and recommendation text from tool output.
- Keep remediation proposals concrete and file-specific.

## Validation checks
- Non-zero exit on `analyze` indicates policy violations (typically unmapped requirements).
- Verify that new/changed trace markers reduce unmapped findings on re-run.
