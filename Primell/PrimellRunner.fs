namespace dpenner1.PrimellF

open Antlr4.Runtime
open System

(*

        static List<Tuple<string, string>> ConsoleSettings = new List<Tuple<string, string>>
        {
            { Tuple.Create("-o <file-path>?  OR  --output <file-path>?", "Directs Prime to output to given file. If no file is specified, output is to console.") },
            { Tuple.Create("-b <base>  OR  --base <base>", "Sets the base globally to given value.") },
            { Tuple.Create("-ib <base>  OR  --input-base <base>", "Sets the base that Prime expects the user to write in for the list read (:_) operator.") },
            { Tuple.Create("-ob <base>  OR  --output-base <base>", "Sets the base that Prime outputs in.") },
            { Tuple.Create("-sb <base>  OR  --source-base <base>", "Sets the base that Prime expects the source code to be in.") },
            { Tuple.Create("-fs <Y or N>?  OR  --free-source <Y or N>?", "Sets whether non-prime values are allowed. If value not provided, toggles the current value.")},
            { Tuple.Create("-t <0 to 7>  OR  --truth <0 to 7>", "Sets what truth means for the boolean (?) operators") },
        };*)
type ConsoleCommand = { Key: string; ArgumentDescription: string; Description: string }

type PrimellRunner() = 

  let consoleCommands = [ { Key = ""; ArgumentDescription = ""; Description = "Help. Which I assume you've already figured out." }
                          { Key = "echo"; ArgumentDescription = ""; Description = "Toggles echo. This provides feedback for some commands to soothe your worries." }
                          { Key = "q"; ArgumentDescription = ""; Description = "Quit Prime. Prime is sad." }
                          { Key = "run"; ArgumentDescription = "<file-path>"; Description = "Runs the given file." }
                          { Key = "set"; ArgumentDescription = "<settings-list>?"; Description = "Sets Prime to given settings list. Echoes current settings if none provided." }
                        ]

  let consoleSettings = [ "-o    OR  --output <file-path>?", "Directs Prime to output to given file. If no file is specified, output is to console."
                          "-b    OR  --base <base>", "Sets the base globally to given value."
                          "-ib   OR  --input-base <base>", "Sets the base that Prime expects the user to write in for the list read (:_) operator."
                          "-ob   OR  --output-base <base>", "Sets the base that Prime outputs in."
                          "-sb   OR  --source-base <base>", "Sets the base that Prime expects the source code to be in."
                          "-rs   OR  --restricted-source <Y or N>?", "Sets whether non-prime values are allowed. If value not provided, toggles the current value."
                          "-t    OR  --truth <0 to 7>", "Sets what truth means for the boolean (?) operators"
                        ]

  member this.GetResultString (result: PrimellObject) (control: PrimellProgramControl) =
    let temp =
      result.ToString(control.GetVariableDictionary())

    match result with
    | :? PReference as ref -> this.GetResultString(control.GetVariable(ref.Name)) control
    | :? PList as l when not l.IsEmpty -> temp.Substring(1, temp.Length - 2)  // trim off outer parentheses
    | _ -> temp
  

  member this.ExecuteLine lineContext control =
    let visitor = PrimellVisitor control
    visitor.Visit(lineContext)

  // TODO - for now I'm leaving grammar intact due to needing to compare results to original C#
  //      - But Primell executes line by line, I don't need the program + line stuff,
  //      - I can just create a new parser for each line (passing in an ever changing control)

  member this.Run (program: string) (control: PrimellProgramControl) =
    let stream = AntlrInputStream program
    let lexer = PrimellLexer stream
    let tokens = CommonTokenStream lexer
    let parser = PrimellParser tokens
    parser.BuildParseTree <- true
    
    let allLineContexts = parser.program().line() |> Array.toList

    // resetting LastOperationWasAssignment here is a temporary hack
    allLineContexts |> List.map(fun line ->  let result, doOutput = this.ExecuteLine line control, not control.LastOperationWasAssignment
                                             control.LastOperationWasAssignment <- false
                                             if doOutput then
                                               printfn "%O" <| (this.GetResultString result control)
                                             result, doOutput)

  member this.RunFromFile (control: PrimellProgramControl) =
    let lines = IO.File.ReadAllLines(control.Settings.SourceFilePath)
    this.Run (String.concat "\n" lines) control

  

  member this.InteractiveMode ?settings =
    printfn "%s" "Welcome to Prime. Enter ? for help."
    this.PromptForInput (?settings=settings)

  member private this.PromptForInput (?settings: PrimellConfiguration) =
    let input = Console.ReadLine().Trim()
    // lots of mutable stuff from direct C# conversion
    let mutable keepPrompting = true
    let mutable settings = defaultArg settings PrimellConfiguration.PrimellDefault
    let mutable echo = false

    if input.StartsWith("?") then
      let commandKey = if input.Length > 1 then input.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries)[0] else ""
      let argument = input.Substring(input.IndexOf(commandKey) + commandKey.Length).Trim()
      match commandKey.ToLowerInvariant() with
      | "" | "?" | "help" -> 
          this.HelpSpiel()
      | "q" | "quit" -> 
          keepPrompting <- false
      | "run" ->           
          this.RunFromFile(PrimellProgramControl{settings with SourceFilePath = argument}) |> ignore
          if echo then printfn "%s" "Program has completed."
      | "set" ->
          let newSettings = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries)
          if newSettings |> Array.isEmpty then
            printfn "%A" settings  // TODO - format nicer
          else
            settings <- ParseLib.UpdateSettings settings (newSettings |> Array.toList)
            if echo then printfn "%s" "Settings updated."
      | "echo" -> 
          echo <- not echo
          if echo then printfn "%s" "Echo turned on. on. on."
      | _ -> printfn "%s" "Unrecognized command" 
    else 
      this.Run input (PrimellProgramControl settings) |> ignore
    
    printfn "%s" ""
    if keepPrompting then this.PromptForInput()
      

  
  member private this.HelpSpiel() =
    printfn "%s" ""
    printfn "%s" "Commands  (input not prefixed with '?' engages REPL mode - currenty limited)"
    printfn "%s" "-------------------------------------------------------------------------------------------"

    let padding = (consoleCommands |> List.map(fun x -> x.Key.Length + x.ArgumentDescription.Length) |> List.max) + 4
    consoleCommands |> List.iter(fun command -> let s = $"?{command.Key} {command.ArgumentDescription}"
                                                printfn "%s- %s" (s.PadRight(padding)) command.Description)

    printfn "%s" ""
    printfn "%s" ""
    printfn "%s" "Settings"
    printfn "%s" "-------------------------------------------------------------------------------------------"

    consoleSettings |> List.iter(fun setting -> printfn "%s" (fst setting); printfn "    %s" (snd setting))

