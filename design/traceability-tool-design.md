# Traceability Tool Design

## Document Metadata
- Document ID: TTD-001
- Version: 1.0
- Status: Draft
- Related component: `DC-21 Requirements Traceability Tooling` (`design\software-design.md`)

## Purpose
Define the design of a traceability tool that supports requirements-based delivery and cross-layer mapping from requirements to implementation and verification artifacts.

## Tool Requirements

| ID | Requirement |
|---|---|
| TTR-001 | Map requirements from upper layer to lower layer for: user->design, design->implementation, and design->test. |
| TTR-002 | Identify and report unmapped requirements at each layer transition. |
| TTR-003 | Extract traceability markers and requirement identifiers from any source file type used in this architecture. |
| TTR-004 | Produce results in a format easily consumable by AI tools. |
| TTR-005 | Implement the tool in F#. |
| TTR-006 | Provide unit tests using xUnit. |
| TTR-007 | Identify requirement definitions only from recognized requirement-definition sections in Markdown requirement/design artifacts to avoid accidental matches from examples. |

## Authoritative Requirement Definition Rules
- A **requirement definition** is authoritative only when found in a `.md` file under:
  - `user-requirements/`
  - `design/`
- Requirement definitions shall be inside a recognizable container section whose title includes **`Requirement Definitions`** (for example: `## User Requirement Definitions`, `## Design Requirement Definitions`, `## Design Component Definitions`).
- Inside that container, each requirement definition is a subsection header in this form:
  - `### <REQ-ID> - <Short title>` (examples: `### UR-021 - Save/Load Compatibility`, `### DC-20 - Gameplay State Manager`)
- The subsection body shall include:
  - higher-level requirement references (if any)
  - the requirement definition content.
- IDs appearing in examples, code snippets, tables, or plain text **outside recognized definition sections** are not counted as requirement definitions.
- Existing requirement/design tables should be expanded into these sections; tables are reference views, not authoritative definition sources for extraction.

## Applicable Source Languages and File Types
Based on `architecture\language-and-scripting-guidance.md`, extraction support shall include:
- C++ (`.h`, `.hpp`, `.cpp`)
- Blueprint-exported or metadata-based artifacts (`.uasset` metadata exports / text exports)
- Python (`.py`)
- PowerShell (`.ps1`)
- Bash/Shell (`.sh`)
- TypeScript/JavaScript (`.ts`, `.tsx`, `.js`, `.jsx`)
- F# (`.fs`, `.fsi`)
- C# (`.cs`)
- Markdown/spec docs (`.md`) for requirements/design linkage
- Test files across the same extensions above

## Trace Marker Strategy
- Canonical marker format: `REQ:<ID>` (example: `REQ:UR-021`, `REQ:DC-20`, `REQ:TC-004`).
- Optional link marker: `TRACE:<FROM_ID>-><TO_ID>`.
- Marker parser is extension-aware for comment syntaxes but also supports fallback raw-text scanning for robustness.

## Component Design

### 1) Source Discovery Module
- Recursively enumerate files in configured roots (`user-requirements`, `design`, `implementation`, `verification`, and optional tooling dirs).
- Apply include/exclude patterns and extension filters.

### 2) Marker Extraction Module
- Language profile registry: comment syntax + regex patterns per extension.
- Extract trace markers:
  - requirement IDs (`UR-*`, `DR-*`, `DC-*`, `TC-*`, etc.)
  - explicit trace links (`TRACE:*`)
  - file/line provenance.
- Fallback extractor runs for unknown extensions to still detect marker patterns.
- Markdown definition extraction is a separate step with strict section/header rules (see **Authoritative Requirement Definition Rules**); marker extraction alone does not establish requirement definitions.

### 3) Trace Graph Builder
- Normalize extracted IDs.
- Build typed graph:
  - Node: `{ id, layer, sourceFile, sourceLine }`
  - Edge: `{ fromId, toId, relationType, evidence }`
- Layer model: `User`, `Design`, `Implementation`, `Test`.

### 4) Mapping Analyzer
- Validate required transition coverage:
  - user->design
  - design->implementation
  - design->test
- Detect unmapped nodes:
  - no downstream links where required
  - dangling references to missing IDs.

### 5) AI Output Formatter
- Emit machine-friendly artifacts:
  - `traceability-report.json` (full graph + diagnostics)
  - `traceability-report.jsonl` (one record per finding/link)
  - concise `traceability-summary.md`.
- JSON schema is stable and normalized for AI consumption:
  - deterministic keys
  - explicit severity (`info`, `warning`, `error`)
  - explicit recommendation field.

### 6) CLI/Automation Interface
- Example commands:
  - `trace-tool scan`
  - `trace-tool analyze`
  - `trace-tool export --format jsonl`
- Non-zero exit code on policy violations (e.g., unmapped design requirement).

## Data Contracts
- `RequirementRef`: `{ id, layer, file, line }`
- `TraceLink`: `{ fromId, toId, relation, evidenceFile, evidenceLine }`
- `UnmappedFinding`: `{ id, expectedDownstreamLayer, reason, suggestedAction }`

## Testing Design (xUnit)
- Unit test suite in xUnit for:
  1. Marker parsing per language profile.
  2. Fallback parsing for unknown file types.
  3. Requirement-definition section detection and `<ID> - <title>` header parsing in Markdown.
  4. Rejection of accidental IDs found only in examples/tables/outside recognized definition sections.
  5. Graph building and layer classification.
  6. Unmapped requirement detection rules.
  7. JSON/JSONL serialization contract and deterministic output ordering.
  8. CLI exit-code behavior for success/failure scenarios.

## Non-Functional Design Notes
- Deterministic output ordering for repeatable CI and AI diffing.
- Fast incremental scanning via file hash cache.
- Clear error messages with file/line evidence for agent-driven remediation loops.

## Traceability Alignment
- Supports `UR-022` directly.
- Supports `UR-015` by producing verifiable requirement coverage outputs.
- Supports `UR-018` through AI-consumable output and automation-friendly behavior.
