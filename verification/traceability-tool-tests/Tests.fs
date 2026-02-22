// REQ:UR-015 REQ:UR-022 REQ:DC-21 REQ:TTR-006
module Tests

open System
open System.IO
open TraceabilityTool.Core
open Xunit

let private createTempRoot () =
    let root = Path.Combine(Path.GetTempPath(), $"trace-tool-tests-{Guid.NewGuid():N}")
    Directory.CreateDirectory(root) |> ignore
    root

let private writeText (path: string) (content: string) =
    let dir = Path.GetDirectoryName(path)

    if not (String.IsNullOrWhiteSpace(dir)) then
        Directory.CreateDirectory(dir) |> ignore

    File.WriteAllText(path, content)

let private userDefinitions (sections: string list) =
    String.concat
        "\n"
        ([ "# User Requirements"
           "## User Requirement Definitions" ]
         @ sections)

let private designDefinitions (sections: string list) =
    String.concat
        "\n"
        ([ "# Design Requirements"
           "## Design Requirement Definitions" ]
         @ sections)

[<Fact>]
let ``extractFromFile parses REQ and TRACE markers`` () =
    let root = createTempRoot ()
    let filePath = Path.Combine(root, "implementation", "sample.fs")
    writeText filePath "// REQ:DC-100\n// TRACE:DC-100->TC-100"

    let result = MarkerExtraction.extractFromFile filePath

    Assert.Single(result.Requirements) |> ignore
    Assert.Single(result.Links) |> ignore
    Assert.Equal("DC-100", result.Requirements.Head.Id)
    Assert.Equal(Marker, result.Requirements.Head.SourceKind)
    Assert.Equal("DC-100", result.Links.Head.FromId)
    Assert.Equal("TC-100", result.Links.Head.ToId)

[<Fact>]
let ``extractFromFile fallback works for unknown extension`` () =
    let root = createTempRoot ()
    let filePath = Path.Combine(root, "implementation", "notes.custom")
    writeText filePath "REQ:DC-101"

    let result = MarkerExtraction.extractFromFile filePath
    Assert.Single(result.Requirements) |> ignore
    Assert.Equal("DC-101", result.Requirements.Head.Id)
    Assert.Equal(Marker, result.Requirements.Head.SourceKind)

[<Fact>]
let ``definition extraction parses recognized requirement definition section`` () =
    let root = createTempRoot ()
    let filePath = Path.Combine(root, "design", "definitions.md")

    let content =
        designDefinitions
            [ "### DC-010 - Gameplay State Manager"
              "- Higher-level requirements: UR-010"
              "Definition: Manages gameplay state transitions."
              "REQ:UR-010" ]

    writeText filePath content
    let result = DefinitionExtraction.extractFromFile filePath

    Assert.Single(result.Requirements) |> ignore
    Assert.Equal("DC-010", result.Requirements.Head.Id)
    Assert.Equal(Definition, result.Requirements.Head.SourceKind)
    Assert.Single(result.Links) |> ignore
    Assert.Equal("UR-010", result.Links.Head.FromId)
    Assert.Equal("DC-010", result.Links.Head.ToId)
    Assert.Equal("higher-level", result.Links.Head.Relation)

[<Fact>]
let ``definition extraction ignores IDs outside recognized definition section`` () =
    let root = createTempRoot ()
    let filePath = Path.Combine(root, "design", "examples.md")

    let content =
        String.concat
            "\n"
            [ "# Design Examples"
              "## Example Table"
              "| ID | Value |"
              "|---|---|"
              "| DC-999 | Example only |"
              "### DC-888 - Looks like header but outside recognized container"
              "Definition: Should not count." ]

    writeText filePath content
    let result = DefinitionExtraction.extractFromFile filePath
    Assert.Empty(result.Requirements)
    Assert.Empty(result.Links)

[<Fact>]
let ``definition extraction does not create links for Higher-level requirements None`` () =
    let root = createTempRoot ()
    let filePath = Path.Combine(root, "design", "definitions.md")

    let content =
        designDefinitions
            [ "### DC-011 - Standalone Requirement"
              "- Higher-level requirements: None"
              "Definition: No upstream links." ]

    writeText filePath content
    let result = DefinitionExtraction.extractFromFile filePath

    Assert.Single(result.Requirements) |> ignore
    Assert.Empty(result.Links)

[<Fact>]
let ``analyzeRoot reports unmapped user requirement`` () =
    let root = createTempRoot ()

    writeText
        (Path.Combine(root, "user-requirements", "ur.md"))
        (userDefinitions
            [ "### UR-001 - Tactical Squad Focus"
              "Definition: Core user requirement." ])

    let report = ToolRunner.analyzeRoot root
    let unmapped = report.Findings |> List.filter (fun f -> f.Id = "UR-001")
    Assert.NotEmpty(unmapped)

[<Fact>]
let ``analyzeRoot maps user to design and design to implementation and test`` () =
    let root = createTempRoot ()

    writeText
        (Path.Combine(root, "user-requirements", "ur.md"))
        (userDefinitions
            [ "### UR-010 - State Clarity"
              "Definition: UI indicates state." ])

    writeText
        (Path.Combine(root, "design", "design.md"))
        (designDefinitions
            [ "### DC-010 - HUD State Component"
              "- Higher-level requirements: UR-010"
              "Definition: Displays state." ])

    writeText (Path.Combine(root, "implementation", "impl.fs")) "// REQ:DC-010"
    writeText (Path.Combine(root, "verification", "test.fs")) "// REQ:DC-010"

    let report = ToolRunner.analyzeRoot root
    let userFindings = report.Findings |> List.filter (fun f -> f.Id = "UR-010")
    let designFindings = report.Findings |> List.filter (fun f -> f.Id = "DC-010")

    Assert.Empty(userFindings)
    Assert.Empty(designFindings)

[<Fact>]
let ``toJson output ordering is deterministic by id`` () =
    let report =
        { Requirements =
            [ { Id = "UR-2"
                RequirementLayer = User
                SourceLayer = User
                SourceKind = Definition
                FilePath = "b.md"
                Line = 1 }
              { Id = "UR-1"
                RequirementLayer = User
                SourceLayer = User
                SourceKind = Definition
                FilePath = "a.md"
                Line = 1 } ]
          Links = []
          Findings = []
          Diagnostics = []
          Coverage = [] }

    let json = OutputFormatting.toJson report
    let firstIndex = json.IndexOf("\"id\": \"UR-1\"", StringComparison.Ordinal)
    let secondIndex = json.IndexOf("\"id\": \"UR-2\"", StringComparison.Ordinal)
    Assert.True(firstIndex >= 0 && secondIndex > firstIndex)

[<Fact>]
let ``cli analyze returns non-zero for policy violations`` () =
    let root = createTempRoot ()
    let outputDir = Path.Combine(root, "out")

    writeText
        (Path.Combine(root, "user-requirements", "ur.md"))
        (userDefinitions
            [ "### UR-200 - Coverage Policy"
              "Definition: Must be traced." ])

    let result = Cli.run [| "analyze"; "--root"; root; "--output-dir"; outputDir |]

    Assert.Equal(1, result.ExitCode)
    Assert.True(File.Exists(Path.Combine(outputDir, "traceability-report.json")))

[<Fact>]
let ``cli export jsonl writes ai consumable file`` () =
    let root = createTempRoot ()
    let outputDir = Path.Combine(root, "out")

    writeText
        (Path.Combine(root, "user-requirements", "ur.md"))
        (userDefinitions
            [ "### UR-300 - Export Baseline"
              "Definition: export should produce machine-readable output." ])

    writeText
        (Path.Combine(root, "design", "design.md"))
        (designDefinitions
            [ "### DC-300 - Export Support Component"
              "- Higher-level requirements: UR-300"
              "Definition: supports export behavior." ])

    writeText (Path.Combine(root, "implementation", "impl.fs")) "// REQ:DC-300"
    writeText (Path.Combine(root, "verification", "test.fs")) "// REQ:DC-300"

    let result = Cli.run [| "export"; "--format"; "jsonl"; "--root"; root; "--output-dir"; outputDir |]
    let jsonlPath = Path.Combine(outputDir, "traceability-report.jsonl")

    Assert.Equal(0, result.ExitCode)
    Assert.True(File.Exists(jsonlPath))
    Assert.NotEmpty(File.ReadAllLines(jsonlPath))
