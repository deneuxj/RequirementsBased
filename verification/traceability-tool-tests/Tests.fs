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

[<Fact>]
let ``extractFromFile parses REQ and TRACE markers`` () =
    let root = createTempRoot ()
    let filePath = Path.Combine(root, "implementation", "sample.fs")
    writeText filePath "// REQ:DC-100\n// TRACE:DC-100->TC-100"

    let result = MarkerExtraction.extractFromFile filePath

    Assert.Single(result.Requirements) |> ignore
    Assert.Single(result.Links) |> ignore
    Assert.Equal("DC-100", result.Requirements.Head.Id)
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

[<Fact>]
let ``analyzeRoot reports unmapped user requirement`` () =
    let root = createTempRoot ()
    writeText (Path.Combine(root, "user-requirements", "ur.md")) "REQ:UR-001"

    let report = ToolRunner.analyzeRoot root
    let unmapped = report.Findings |> List.filter (fun f -> f.Id = "UR-001")
    Assert.NotEmpty(unmapped)

[<Fact>]
let ``analyzeRoot maps user to design and design to implementation and test`` () =
    let root = createTempRoot ()
    writeText (Path.Combine(root, "user-requirements", "ur.md")) "REQ:UR-010"
    writeText (Path.Combine(root, "design", "design.md")) "REQ:UR-010\nREQ:DC-010"
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
                FilePath = "b.md"
                Line = 1 }
              { Id = "UR-1"
                RequirementLayer = User
                SourceLayer = User
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
    writeText (Path.Combine(root, "user-requirements", "ur.md")) "REQ:UR-200"

    let result = Cli.run [| "analyze"; "--root"; root; "--output-dir"; outputDir |]

    Assert.Equal(1, result.ExitCode)
    Assert.True(File.Exists(Path.Combine(outputDir, "traceability-report.json")))

[<Fact>]
let ``cli export jsonl writes ai consumable file`` () =
    let root = createTempRoot ()
    let outputDir = Path.Combine(root, "out")
    writeText (Path.Combine(root, "user-requirements", "ur.md")) "REQ:UR-300"
    writeText (Path.Combine(root, "design", "design.md")) "REQ:UR-300\nREQ:DC-300"
    writeText (Path.Combine(root, "implementation", "impl.fs")) "// REQ:DC-300"
    writeText (Path.Combine(root, "verification", "test.fs")) "// REQ:DC-300"

    let result = Cli.run [| "export"; "--format"; "jsonl"; "--root"; root; "--output-dir"; outputDir |]
    let jsonlPath = Path.Combine(outputDir, "traceability-report.jsonl")

    Assert.Equal(0, result.ExitCode)
    Assert.True(File.Exists(jsonlPath))
    Assert.NotEmpty(File.ReadAllLines(jsonlPath))
