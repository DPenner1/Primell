namespace dpenner1.PrimellF

open Antlr4.Runtime
open System


type ConsoleCommand = { Key: string; ArgumentDescription: string; Description: string }

type PrimellRunner() = 

  let consoleCommands = [ { Key = ""; ArgumentDescription = ""; Description = "Help. Which I assume you've already figured out." }
                          { Key = "echo"; ArgumentDescription = ""; Description = "Toggles echo. This provides feedback for some commands to soothe your worries." }
                          { Key = "q"; ArgumentDescription = ""; Description = "Quit Primell. Primell is sad." }
                          { Key = "run"; ArgumentDescription = "<file-path>?"; Description = "Runs the given file. If none specified, defaults to file set by --source-file setting" }
                          { Key = "set"; ArgumentDescription = "<settings-list>?"; Description = "Sets Prime to given settings list. Echoes current settings if none provided." }
                        ]

  let consoleSettings = [ "-of   OR  --output-filepath <file-path>?", "Directs Primell to output to given file. If no file is specified, output is to console."
                          "-sf   OR  --source-filepath <file-path>?", "Sets the default source code file path for the ?run command."
                          "-b    OR  --base <base>", "Sets the base globally to given value."
                          "-ib   OR  --input-base <base>", "Sets the base that Primell expects the user to write in for the list read (:_) operator."
                          "-ob   OR  --output-base <base>", "Sets the base that Primell outputs in."
                          "-sb   OR  --source-base <base>", "Sets the base that Primell expects the source code to be in."
                          "-rs   OR  --restricted-source <Y or N>?", "Sets whether non-prime values are allowed. If value not provided, toggles the current value."
                          "-t    OR  --truth <0 to 7>", "Sets what truth means for the boolean (?) operators (setting unimplemented)"
                          "-pd   OR  --primell-default", "Sets configuration to Primell's default. Other simultaneously provided settings override this."
                          "-ld   OR  --listell-default", "Sets configuration to Listell, Primell's non-prime believing cousin. Other simultaneously provided settings override this."
                        ]

  member this.GetResultString (result: PrimellObject) (control: PrimellProgramControl) =
    let temp =
      result.ToString()

    match result with
    | :? PList as l when not l.IsEmpty -> temp.Substring(1, temp.Length - 2)  // trim off outer parentheses
    | :? PVariable as v ->
        match v.CapturedValue with
        | :? PList as l when not l.IsEmpty -> temp.Substring(1, temp.Length - 2)  // trim off outer parentheses
        | _ -> temp
    | :? PReference as ref ->
        match ref.CapturedValue with
        | :? PList as l when not l.IsEmpty -> temp.Substring(1, temp.Length - 2)  // trim off outer parentheses
        | _ -> temp
    | _ -> temp
  

  member this.ExecuteLine lineContext control =
    let visitor = PrimellVisitor control
    visitor.Visit(lineContext)

  // TODO - for now I'm leaving grammar intact due to needing to compare results to original C#
  //      - But Primell executes line by line, I don't need the program + line stuff,
  //      - I can just create a new parser for each line (passing in an ever changing control)

// TODO also this method now has a silly return type
  member this.Run (program: string) (settings: PrimellConfiguration) =
    let stream = AntlrInputStream program
    let lexer = PrimellLexer stream
    let tokens = CommonTokenStream lexer
    let parser = PrimellParser tokens
    parser.BuildParseTree <- true
    
    let allLineContexts = parser.program().line() |> Array.toList
    let control = PrimellProgramControl(settings, allLineContexts)

    // resetting LastOperationWasAssignment here is a temporary hack
    allLineContexts |> List.map(fun line ->  let result, doOutput = this.ExecuteLine line control, not control.LastOperationWasAssignment
                                             control.LastOperationWasAssignment <- false
                                             if doOutput then
                                               printfn "%s" <| (this.GetResultString result control)
                                             result, doOutput), control

  member this.RunFromFile (settings: PrimellConfiguration) =
    let lines = IO.File.ReadAllLines(settings.SourceFilePath)
    this.Run (String.concat "\n" lines) settings
 

  member this.InteractiveMode ?settings =
    let settings = defaultArg settings PrimellConfiguration.PrimellDefault
    this.PromptForInput settings false "Welcome to Primell. Enter ? for help."

  // prompt parameter is a bit misnamed, it mostly ends up being echo from the previous recursion, but can't think of better name right now
  member private this.PromptForInput (settings: PrimellConfiguration)(echo: bool)(prompt: string) =
    printfn "%s" prompt
    printfn "%s" ""
    let input = Console.ReadLine().Trim()
 
     // TODO - exception handle

    if input.StartsWith("?") then
      let commandKey = if input.Length > 1 then input.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries)[0] else ""
      let argument = input.Substring(input.IndexOf(commandKey) + commandKey.Length).Trim()
      match commandKey.ToLowerInvariant() with
      | "q" | "quit" -> ()
      | "" | "?" | "help" -> 
          //this.HelpSpiel()  // TODO generate string for recursive print
          this.PromptForInput settings echo (this.HelpSpiel())
      | "run" ->   // TODO - read from settings
          if argument.Length = 0 then
            this.PromptForInput settings echo "Run what?"
          else        
            this.RunFromFile({settings with SourceFilePath = argument}) |> ignore
            this.PromptForInput settings echo (if echo then "Program has completed." else "")  
          // not saving filepatch in "permanent" settings is intentional
      | "set" ->
          let newSettings = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries)
          if newSettings |> Array.isEmpty then
            this.PromptForInput settings echo (sprintf "%A" settings) // TODO - format nicer
          else
            this.PromptForInput(ParseLib.UpdateSettings settings (newSettings |> Array.toList)) echo (if echo then "Settings updated." else "")
      | "echo" -> 
          this.PromptForInput settings (not echo) (if echo then "" else "Echo turned on. on. on.")
      | _ -> this.PromptForInput settings echo "Unrecognized command" 
    else 
      this.Run input settings |> ignore  
      this.PromptForInput settings echo (if echo then "Program line executed." else "")
    
  
  member private this.HelpSpiel() =
    let line = "----------------------------------------------------------------------------------------------"

    let commandPreface = [""; "Commands  (input not prefixed with '?' engages REPL mode - currently limited to single lines)"; line]

    let padding = (consoleCommands |> List.map(fun x -> x.Key.Length + x.ArgumentDescription.Length) |> List.max) + 4
    let commandList = consoleCommands |> List.map(fun command -> let s = $"?{command.Key} {command.ArgumentDescription}"
                                                                 sprintf "%s- %s" (s.PadRight(padding)) command.Description)

    let settingPreface = [""; ""; "Settings"; line]
    let settingList = consoleSettings |> List.collect(fun setting -> [fst setting; sprintf "    %s" (snd setting)])
    
    List.concat [commandPreface; commandList; settingPreface; settingList] |> String.concat "\n"