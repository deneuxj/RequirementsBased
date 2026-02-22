// REQ:UR-015 REQ:UR-018 REQ:UR-022 REQ:DC-21 REQ:TTR-001 REQ:TTR-002 REQ:TTR-003 REQ:TTR-004 REQ:TTR-005
namespace TraceabilityTool.Core

open System
open System.IO
open System.Text.Json
open System.Text.RegularExpressions

type Layer =
    | User
    | Design
    | Implementation
    | Test
    | Unknown

type Severity =
    | Info
    | Warning
    | Error

type RequirementSource =
    | Definition
    | Marker

type RequirementRef =
    { Id: string
      RequirementLayer: Layer
      SourceLayer: Layer
      SourceKind: RequirementSource
      FilePath: string
      Line: int }

type TraceLink =
    { FromId: string
      ToId: string
      Relation: string
      EvidenceFile: string
      EvidenceLine: int
      EvidenceLayer: Layer }

type UnmappedFinding =
    { Id: string
      ExpectedDownstreamLayer: Layer
      Reason: string
      SuggestedAction: string
      Severity: Severity }

type Diagnostic =
    { Severity: Severity
      Message: string
      FilePath: string option
      Line: int option }

type TransitionCoverage =
    { Transition: string
      UpperCount: int
      MappedCount: int
      UnmappedCount: int }

type TraceabilityReport =
    { Requirements: RequirementRef list
      Links: TraceLink list
      Findings: UnmappedFinding list
      Diagnostics: Diagnostic list
      Coverage: TransitionCoverage list }

module private Layering =
    let private idPrefixRegex = Regex("^([A-Za-z]+)-\\d+$", RegexOptions.Compiled)

    let fromId (id: string) =
        let m = idPrefixRegex.Match(id)

        if not m.Success then
            Unknown
        else
            match m.Groups.[1].Value.ToUpperInvariant() with
            | "UR" -> User
            | "DR"
            | "DC"
            | "TTR" -> Design
            | "TC" -> Test
            | _ -> Unknown

    let fromPath (path: string) =
        let normalized = path.Replace('\\', '/').ToLowerInvariant()

        if normalized.Contains("/user-requirements/") then
            User
        elif normalized.Contains("/design/") || normalized.Contains("/architecture/") then
            Design
        elif normalized.Contains("/implementation/") then
            Implementation
        elif normalized.Contains("/verification/") then
            Test
        else
            Unknown

    let toText =
        function
        | User -> "user"
        | Design -> "design"
        | Implementation -> "implementation"
        | Test -> "test"
        | Unknown -> "unknown"

module private RequirementSourcing =
    let toText =
        function
        | Definition -> "definition"
        | Marker -> "marker"

module SourceDiscovery =
    let private excludedDirs = Set.ofList [ ".git"; ".vs"; "bin"; "obj"; "node_modules" ]

    let private supportedExtensions =
        Set.ofList
            [ ".h"
              ".hpp"
              ".cpp"
              ".uasset"
              ".py"
              ".ps1"
              ".sh"
              ".ts"
              ".tsx"
              ".js"
              ".jsx"
              ".fs"
              ".fsi"
              ".cs"
              ".md" ]

    let private isExcludedPath (filePath: string) =
        let normalized = filePath.Replace('\\', '/')
        normalized.Split('/')
        |> Array.exists (fun segment -> excludedDirs.Contains(segment.ToLowerInvariant()))

    let discoverFiles (roots: string list) =
        roots
        |> List.collect (fun root ->
            if Directory.Exists(root) then
                Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                |> Seq.filter (isExcludedPath >> not)
                |> Seq.toList
            else
                [])
        |> List.sort

    let filterSupportedFiles (files: string list) =
        files
        |> List.filter (fun file ->
            let ext = Path.GetExtension(file).ToLowerInvariant()
            supportedExtensions.Contains(ext))

module MarkerExtraction =
    type ExtractionResult =
        { Requirements: RequirementRef list
          Links: TraceLink list
          Diagnostics: Diagnostic list }

    let private reqRegex =
        Regex(@"REQ:([A-Za-z][A-Za-z0-9_-]*-\d+)", RegexOptions.Compiled)

    let private traceRegex =
        Regex(@"TRACE:([A-Za-z][A-Za-z0-9_-]*-\d+)->([A-Za-z][A-Za-z0-9_-]*-\d+)", RegexOptions.Compiled)

    let private readLinesSafely (filePath: string) =
        try
            Result.Ok(File.ReadAllLines(filePath))
        with _ ->
            Result.Error
                { Severity = Warning
                  Message = "Unable to read file as text; skipped marker extraction."
                  FilePath = Some filePath
                  Line = None }

    let extractFromFile (filePath: string) =
        let sourceLayer = Layering.fromPath filePath

        match readLinesSafely filePath with
        | Result.Error diag ->
            { Requirements = []
              Links = []
              Diagnostics = [ diag ] }
        | Result.Ok lines ->
            let reqs =
                lines
                |> Array.mapi (fun index line ->
                    reqRegex.Matches(line)
                    |> Seq.cast<Match>
                    |> Seq.map (fun m ->
                        let id = m.Groups.[1].Value
                        let inferredLayer = Layering.fromId id

                        { Id = id
                          RequirementLayer =
                            if inferredLayer = Unknown then
                                sourceLayer
                            else
                                inferredLayer
                          SourceLayer = sourceLayer
                          SourceKind = Marker
                          FilePath = filePath
                          Line = index + 1 }))
                |> Seq.concat
                |> Seq.toList

            let links =
                lines
                |> Array.mapi (fun index line ->
                    traceRegex.Matches(line)
                    |> Seq.cast<Match>
                    |> Seq.map (fun m ->
                        { FromId = m.Groups.[1].Value
                          ToId = m.Groups.[2].Value
                          Relation = "trace"
                          EvidenceFile = filePath
                          EvidenceLine = index + 1
                          EvidenceLayer = sourceLayer }))
                |> Seq.concat
                |> Seq.toList

            { Requirements = reqs
              Links = links
              Diagnostics = [] }

    let extractFromFiles (files: string list) =
        files
        |> List.map extractFromFile
        |> List.fold
            (fun acc next ->
                { Requirements = acc.Requirements @ next.Requirements
                  Links = acc.Links @ next.Links
                  Diagnostics = acc.Diagnostics @ next.Diagnostics })
            { Requirements = []
              Links = []
              Diagnostics = [] }

module DefinitionExtraction =
    type ExtractionResult =
        { Requirements: RequirementRef list
          Links: TraceLink list
          Diagnostics: Diagnostic list }

    let private headingRegex = Regex(@"^(#{1,6})\s+(.+?)\s*$", RegexOptions.Compiled)

    let private definitionHeaderRegex =
        Regex(@"^#{1,6}\s+([A-Za-z][A-Za-z0-9_-]*-\d+)\s*-\s+.+$", RegexOptions.Compiled)

    let private definitionIdRegex =
        Regex(@"^#{1,6}\s+([A-Za-z][A-Za-z0-9_-]*-\d+)\s*-\s+.+$", RegexOptions.Compiled)

    let private higherLevelRegex =
        Regex(@"^\s*-\s*Higher-level requirements\s*:\s*(.+)\s*$", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private referenceIdRegex =
        Regex(@"^(?:REQ:)?([A-Za-z][A-Za-z0-9_-]*-\d+)$", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private isAuthoritativeMarkdownPath (filePath: string) =
        let normalized = filePath.Replace('\\', '/').ToLowerInvariant()
        let isMd = normalized.EndsWith(".md")
        isMd && (normalized.Contains("/user-requirements/") || normalized.Contains("/design/"))

    let private parseHigherLevelIds (rawValue: string) =
        let value = rawValue.Trim()

        if value.Equals("None", StringComparison.OrdinalIgnoreCase) then
            []
        else
            value.Split(',', StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun token -> token.Trim())
            |> Array.choose (fun token ->
                let m = referenceIdRegex.Match(token)
                if m.Success then Some m.Groups.[1].Value else None)
            |> Array.toList

    let private readLinesSafely (filePath: string) =
        try
            Result.Ok(File.ReadAllLines(filePath))
        with _ ->
            Result.Error
                { Severity = Warning
                  Message = "Unable to read markdown file for requirement definition extraction."
                  FilePath = Some filePath
                  Line = None }

    let extractFromFile (filePath: string) =
        if not (isAuthoritativeMarkdownPath filePath) then
            { Requirements = []
              Links = []
              Diagnostics = [] }
        else
            match readLinesSafely filePath with
            | Result.Error diag ->
                { Requirements = []
                  Links = []
                  Diagnostics = [ diag ] }
            | Result.Ok lines ->
                let sourceLayer = Layering.fromPath filePath
                let mutable currentContainerLevel: int option = None
                let mutable currentDefinition: (string * int) option = None
                let refs = ResizeArray<RequirementRef>()
                let links = ResizeArray<TraceLink>()

                for index in 0 .. lines.Length - 1 do
                    let line = lines.[index]
                    let headingMatch = headingRegex.Match(line)

                    if headingMatch.Success then
                        let headingLevel = headingMatch.Groups.[1].Value.Length
                        let headingTitle = headingMatch.Groups.[2].Value.Trim()

                        match currentDefinition with
                        | Some(_, definitionLevel) when headingLevel <= definitionLevel ->
                            currentDefinition <- None
                        | _ -> ()

                        match currentContainerLevel with
                        | Some containerLevel when headingLevel <= containerLevel ->
                            currentContainerLevel <- None
                            currentDefinition <- None
                        | _ -> ()

                        if headingTitle.IndexOf("Requirement Definitions", StringComparison.OrdinalIgnoreCase) >= 0 then
                            currentContainerLevel <- Some headingLevel
                        else
                            match currentContainerLevel with
                            | Some containerLevel when headingLevel > containerLevel && definitionHeaderRegex.IsMatch(line) ->
                                let idMatch = definitionIdRegex.Match(line)

                                if idMatch.Success then
                                    let id = idMatch.Groups.[1].Value
                                    let inferredLayer = Layering.fromId id
                                    currentDefinition <- Some(id, headingLevel)

                                    refs.Add(
                                        { Id = id
                                          RequirementLayer =
                                            if inferredLayer = Unknown then
                                                sourceLayer
                                            else
                                                inferredLayer
                                          SourceLayer = sourceLayer
                                          SourceKind = Definition
                                          FilePath = filePath
                                          Line = index + 1 }
                                    )
                            | _ -> ()
                    else
                        match currentDefinition with
                        | Some(currentId, _) ->
                            let higherLevelMatch = higherLevelRegex.Match(line)

                            if higherLevelMatch.Success then
                                let upstreamIds = parseHigherLevelIds higherLevelMatch.Groups.[1].Value

                                upstreamIds
                                |> List.iter (fun upstreamId ->
                                    links.Add(
                                        { FromId = upstreamId
                                          ToId = currentId
                                          Relation = "higher-level"
                                          EvidenceFile = filePath
                                          EvidenceLine = index + 1
                                          EvidenceLayer = sourceLayer }
                                    ))
                        | None -> ()

                let distinctLinks =
                    links
                    |> Seq.distinctBy (fun l -> l.FromId, l.ToId, l.Relation, l.EvidenceFile, l.EvidenceLine)
                    |> Seq.toList

                { Requirements = refs |> Seq.toList
                  Links = distinctLinks
                  Diagnostics = [] }

    let extractFromFiles (files: string list) =
        files
        |> List.map extractFromFile
        |> List.fold
            (fun acc next ->
                { Requirements = acc.Requirements @ next.Requirements
                  Links = acc.Links @ next.Links
                  Diagnostics = acc.Diagnostics @ next.Diagnostics })
            { Requirements = []
              Links = []
              Diagnostics = [] }

module MappingAnalysis =
    let private uniqueIdsBy predicate (requirements: RequirementRef list) =
        requirements
        |> List.filter predicate
        |> List.map (fun r -> r.Id)
        |> Set.ofList

    let private hasReferenceInLayer id layer (requirements: RequirementRef list) =
        requirements
        |> List.exists (fun r -> r.Id = id && r.SourceLayer = layer)

    let private hasTraceInLayer id layer (links: TraceLink list) =
        links
        |> List.exists (fun l -> l.FromId = id && l.EvidenceLayer = layer)

    let private evaluateTransition transitionName upperLayer lowerLayer requirements links =
        let upperIds =
            uniqueIdsBy
                (fun r ->
                    r.RequirementLayer = upperLayer
                    && r.SourceLayer = upperLayer)
                requirements
            |> Set.filter (fun id ->
                requirements
                |> List.exists (fun r ->
                    r.Id = id
                    && r.SourceLayer = upperLayer
                    && r.SourceKind = Definition))

        let mappedIds =
            upperIds
            |> Seq.filter (fun id -> hasReferenceInLayer id lowerLayer requirements || hasTraceInLayer id lowerLayer links)
            |> Set.ofSeq

        let unmappedIds = Set.difference upperIds mappedIds

        let findings =
            unmappedIds
            |> Seq.map (fun id ->
                { Id = id
                  ExpectedDownstreamLayer = lowerLayer
                  Reason = $"No trace mapping from {Layering.toText upperLayer} to {Layering.toText lowerLayer}."
                  SuggestedAction = $"Add REQ:{id} or TRACE:{id}-><target> in {Layering.toText lowerLayer} artifacts."
                  Severity = Error })
            |> Seq.toList

        let coverage =
            { Transition = transitionName
              UpperCount = upperIds.Count
              MappedCount = mappedIds.Count
              UnmappedCount = unmappedIds.Count }

        findings, coverage

    let analyze (requirements: RequirementRef list) (links: TraceLink list) (inputDiagnostics: Diagnostic list) =
        let userDesignFindings, userDesignCoverage =
            evaluateTransition "user->design" User Design requirements links

        let designImplementationFindings, designImplementationCoverage =
            evaluateTransition "design->implementation" Design Implementation requirements links

        let designTestFindings, designTestCoverage =
            evaluateTransition "design->test" Design Test requirements links

        let knownDefinitionIds =
            requirements
            |> List.filter (fun r -> r.SourceKind = Definition)
            |> List.map (fun r -> r.Id)
            |> Set.ofList

        let danglingDiagnostics =
            links
            |> List.collect (fun l ->
                [ if not (knownDefinitionIds.Contains(l.FromId)) then
                       { Severity = Warning
                         Message = $"TRACE source '{l.FromId}' is not declared in authoritative requirement definitions."
                         FilePath = Some l.EvidenceFile
                         Line = Some l.EvidenceLine }
                  if not (knownDefinitionIds.Contains(l.ToId)) then
                      { Severity = Warning
                        Message = $"TRACE target '{l.ToId}' is not declared in authoritative requirement definitions."
                        FilePath = Some l.EvidenceFile
                        Line = Some l.EvidenceLine } ])

        { Requirements = requirements
          Links = links
          Findings = userDesignFindings @ designImplementationFindings @ designTestFindings
          Diagnostics = inputDiagnostics @ danglingDiagnostics
          Coverage = [ userDesignCoverage; designImplementationCoverage; designTestCoverage ] }

module OutputFormatting =
    let private severityToText =
        function
        | Info -> "info"
        | Warning -> "warning"
        | Error -> "error"

    let private toSerializable (report: TraceabilityReport) =
        let requirements =
            report.Requirements
            |> List.sortBy (fun r -> r.Id, r.FilePath, r.Line)
            |> List.map (fun r ->
                {| id = r.Id
                   requirementLayer = Layering.toText r.RequirementLayer
                   sourceLayer = Layering.toText r.SourceLayer
                   sourceKind = RequirementSourcing.toText r.SourceKind
                   file = r.FilePath
                   line = r.Line |})

        let links =
            report.Links
            |> List.sortBy (fun l -> l.FromId, l.ToId, l.EvidenceFile, l.EvidenceLine)
            |> List.map (fun l ->
                {| fromId = l.FromId
                   toId = l.ToId
                   relation = l.Relation
                   evidenceFile = l.EvidenceFile
                   evidenceLine = l.EvidenceLine
                   evidenceLayer = Layering.toText l.EvidenceLayer |})

        let findings =
            report.Findings
            |> List.sortBy (fun f -> f.Id, f.ExpectedDownstreamLayer)
            |> List.map (fun f ->
                {| id = f.Id
                   expectedDownstreamLayer = Layering.toText f.ExpectedDownstreamLayer
                   reason = f.Reason
                   suggestedAction = f.SuggestedAction
                   severity = severityToText f.Severity |})

        let diagnostics =
            report.Diagnostics
            |> List.sortBy (fun d -> d.FilePath, d.Line, d.Message)
            |> List.map (fun d ->
                {| severity = severityToText d.Severity
                   message = d.Message
                   filePath = d.FilePath
                   line = d.Line |})

        let coverage =
            report.Coverage
            |> List.sortBy (fun c -> c.Transition)
            |> List.map (fun c ->
                {| transition = c.Transition
                   upperCount = c.UpperCount
                   mappedCount = c.MappedCount
                   unmappedCount = c.UnmappedCount |})

        {| requirements = requirements
           links = links
           findings = findings
           diagnostics = diagnostics
           coverage = coverage |}

    let toJson (report: TraceabilityReport) =
        let payload = toSerializable report
        JsonSerializer.Serialize(payload, JsonSerializerOptions(WriteIndented = true))

    let toJsonl (report: TraceabilityReport) =
        let payload = toSerializable report

        seq {
            for requirement in payload.requirements do
                yield JsonSerializer.Serialize({| recordType = "requirement"; value = requirement |})

            for link in payload.links do
                yield JsonSerializer.Serialize({| recordType = "link"; value = link |})

            for finding in payload.findings do
                yield JsonSerializer.Serialize({| recordType = "finding"; value = finding |})

            for diagnostic in payload.diagnostics do
                yield JsonSerializer.Serialize({| recordType = "diagnostic"; value = diagnostic |})

            for coverage in payload.coverage do
                yield JsonSerializer.Serialize({| recordType = "coverage"; value = coverage |})
        }
        |> Seq.toList

    let toSummaryMarkdown (report: TraceabilityReport) =
        let header =
            [ "# Traceability Summary"
              ""
              $"Requirements extracted: {report.Requirements.Length}"
              $"Links extracted: {report.Links.Length}"
              $"Findings: {report.Findings.Length}"
              $"Diagnostics: {report.Diagnostics.Length}"
              "" ]

        let coverageLines =
            report.Coverage
            |> List.sortBy (fun c -> c.Transition)
            |> List.collect (fun c ->
                [ $"## {c.Transition}"
                  $"- Upper requirements: {c.UpperCount}"
                  $"- Mapped: {c.MappedCount}"
                  $"- Unmapped: {c.UnmappedCount}"
                  "" ])

        let findingLines =
            if report.Findings.IsEmpty then
                [ "## Unmapped Findings"; "- None" ]
            else
                [ "## Unmapped Findings" ]
                @ (report.Findings
                   |> List.sortBy (fun f -> f.Id, f.ExpectedDownstreamLayer)
                   |> List.map (fun f ->
                       $"- {f.Id} -> {Layering.toText f.ExpectedDownstreamLayer}: {f.Reason}"))

        String.concat Environment.NewLine (header @ coverageLines @ findingLines)

    let writeJson path report =
        File.WriteAllText(path, toJson report)

    let writeJsonl path report =
        File.WriteAllLines(path, toJsonl report)

    let writeSummary path report =
        File.WriteAllText(path, toSummaryMarkdown report)

module ToolRunner =
    type RunMode =
        | Scan
        | Analyze
        | Export of string

    type RunResult =
        { Report: TraceabilityReport
          WrittenFiles: string list
          ExitCode: int }

    let analyzeRoot root =
        let files =
            SourceDiscovery.discoverFiles [ root ]
            |> SourceDiscovery.filterSupportedFiles

        let markerExtraction = MarkerExtraction.extractFromFiles files
        let definitionExtraction = DefinitionExtraction.extractFromFiles files

        let allRequirements = markerExtraction.Requirements @ definitionExtraction.Requirements
        let allLinks = markerExtraction.Links @ definitionExtraction.Links
        let allDiagnostics = markerExtraction.Diagnostics @ definitionExtraction.Diagnostics

        MappingAnalysis.analyze allRequirements allLinks allDiagnostics

    let private hasPolicyViolations report =
        report.Findings |> List.exists (fun f -> f.Severity = Error)

    let run mode root outputDir =
        let report = analyzeRoot root
        Directory.CreateDirectory(outputDir) |> ignore

        let writtenFiles =
            match mode with
            | Scan ->
                let path = Path.Combine(outputDir, "traceability-report.json")
                OutputFormatting.writeJson path report
                [ path ]
            | Analyze ->
                let jsonPath = Path.Combine(outputDir, "traceability-report.json")
                let jsonlPath = Path.Combine(outputDir, "traceability-report.jsonl")
                let summaryPath = Path.Combine(outputDir, "traceability-summary.md")
                OutputFormatting.writeJson jsonPath report
                OutputFormatting.writeJsonl jsonlPath report
                OutputFormatting.writeSummary summaryPath report
                [ jsonPath; jsonlPath; summaryPath ]
            | Export format ->
                match format.Trim().ToLowerInvariant() with
                | "json" ->
                    let path = Path.Combine(outputDir, "traceability-report.json")
                    OutputFormatting.writeJson path report
                    [ path ]
                | "jsonl" ->
                    let path = Path.Combine(outputDir, "traceability-report.jsonl")
                    OutputFormatting.writeJsonl path report
                    [ path ]
                | "summary" ->
                    let path = Path.Combine(outputDir, "traceability-summary.md")
                    OutputFormatting.writeSummary path report
                    [ path ]
                | _ -> []

        let exitCode =
            match mode with
            | Scan -> 0
            | _ ->
                if hasPolicyViolations report then 1 else 0

        { Report = report
          WrittenFiles = writtenFiles
          ExitCode = exitCode }

module Cli =
    type CliResult =
        { ExitCode: int
          Message: string
          WrittenFiles: string list
          Report: TraceabilityReport option }

    let private usage =
        "Usage: trace-tool <scan|analyze|export> [--root <path>] [--output-dir <path>] [--format <json|jsonl|summary>]"

    let private parseArgs (argv: string array) =
        if argv.Length = 0 then
            Result.Error "Missing command."
        else
            let command = argv.[0].Trim().ToLowerInvariant()
            let mutable root = Directory.GetCurrentDirectory()
            let mutable outputDir = Directory.GetCurrentDirectory()
            let mutable format = "jsonl"
            let mutable idx = 1
            let mutable parseError: string option = None

            while idx < argv.Length && parseError.IsNone do
                match argv.[idx] with
                | "--root" when idx + 1 < argv.Length ->
                    root <- argv.[idx + 1]
                    idx <- idx + 2
                | "--output-dir" when idx + 1 < argv.Length ->
                    outputDir <- argv.[idx + 1]
                    idx <- idx + 2
                | "--format" when idx + 1 < argv.Length ->
                    format <- argv.[idx + 1]
                    idx <- idx + 2
                | arg ->
                    parseError <- Some($"Unknown or incomplete argument: {arg}")

            match parseError with
            | Some err -> Result.Error err
            | None ->
                let mode =
                    match command with
                    | "scan" -> Some ToolRunner.Scan
                    | "analyze" -> Some ToolRunner.Analyze
                    | "export" -> Some(ToolRunner.Export format)
                    | _ -> None

                match mode with
                | Some m -> Result.Ok(m, root, outputDir)
                | None -> Result.Error $"Unknown command: {command}"

    let run (argv: string array) =
        match parseArgs argv with
        | Result.Error err ->
            { ExitCode = 2
              Message = $"{err}{Environment.NewLine}{usage}"
              WrittenFiles = []
              Report = None }
        | Result.Ok(mode, root, outputDir) ->
            let result = ToolRunner.run mode root outputDir

            let fileSummary =
                if result.WrittenFiles.IsEmpty then
                    "No files written."
                else
                    "Written files: " + String.concat ", " result.WrittenFiles

            { ExitCode = result.ExitCode
              Message = $"Traceability analysis complete. {fileSummary}"
              WrittenFiles = result.WrittenFiles
              Report = Some result.Report }
