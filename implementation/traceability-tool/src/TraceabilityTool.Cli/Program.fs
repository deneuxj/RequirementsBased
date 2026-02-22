// REQ:UR-018 REQ:UR-022 REQ:DC-21 REQ:TTR-004
open TraceabilityTool.Core

[<EntryPoint>]
let main argv =
    let result = Cli.run argv
    printfn "%s" result.Message
    result.ExitCode
