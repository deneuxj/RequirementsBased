# Traceability Tool Implementation

This directory contains the implementation projects for the requirements coverage tool:
- `src\TraceabilityTool.Core`
- `src\TraceabilityTool.Cli`

## Traceability note on TTR-006

Trace marker: `REQ:TTR-006`

`TTR-006` (xUnit unit tests) is intentionally not an implementation-side runtime requirement.
It is realized in the verification test project at:

- `verification\traceability-tool-tests\Tests.fs`

The implementation projects in this directory focus on runtime/tool behavior (including TTR-007 and TTR-008 logic).
