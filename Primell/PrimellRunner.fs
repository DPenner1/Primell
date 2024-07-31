namespace dpenner1.Primell

open System


type ConsoleCommand = { Key: string; ArgumentDescription: string; Description: string }

type PrimellRunner() = 

  let runnerVariables = System.Collections.Generic.Dictionary<string, PObject>()
  let consoleCommands = [ { Key = ""; ArgumentDescription = ""; Description = "Help. Which I assume you've already figured out." }
                          { Key = "echo"; ArgumentDescription = ""; Description = "Toggles echo. This provides feedback for some commands to soothe your worries." }
                          { Key = "q"; ArgumentDescription = ""; Description = "Quit Primell. Primell is sad." }
                          { Key = "run"; ArgumentDescription = "<file-path>?"; Description = "Runs the given file. If none specified, defaults to file set by --source-file setting" }
                          { Key = "set"; ArgumentDescription = "<settings-list>?"; Description = "Sets Prime to given settings list. Y/N parameters toggle of not provided. Echoes current settings if none provided. " }
                          { Key = "clear"; ArgumentDescription = ""; Description = "Clears the variable cache" }
                          { Key = "reset"; ArgumentDescription = ""; Description = "Net new: Clears the variable cache and resets the settings to default" }
                        ]

  let consoleSettings = [ "-of   OR  --output-filepath <file-path>?", "Directs Primell to output to given file. If no file is specified, output is to console."
                          "-sf   OR  --source-filepath <file-path>?", "Sets the default source code file path for the ?run command."
                          "-b    OR  --base <integer>", "Sets the base globally to given value."
                          "-ib   OR  --input-base <integer>", "Sets the base that Primell expects the user to write in for the list read (:_) operator."
                          "-ob   OR  --output-base <integer>", "Sets the base that Primell outputs in."
                          "-sb   OR  --source-base <ingeger>", "Sets the base that Primell expects the source code to be in."
                          "-c63  OR  --character-63 <char>", "Sets the 63rd character for base 63/64 (value 62). Must be alphanumeric, but not in [0-9a-zA-Z]."
                          "-c64  OR  --character-64 <char>", "Sets the 64th character for base 64 (value 63). Must be alphanumeric, but not in [0-9a-zA-Z]."
                          "-rs   OR  --restricted-source <Y/N>?", "Sets whether non-prime values are allowed."
                          "-po   OR  --use-prime-operators", "Sets whether some operators are prime-based or boring."
                          "-tp   OR  --truth-prime <Y/N>?", "Sets that primes are the truth for boolean operators."
                          "-ta   OR  --truth-all <Y/N>?", "Sets whether all list items must be truth for boolean operators."
                          "-pd   OR  --primell-default", "Sets configuration to Primell's default. Other simultaneously provided settings override this."
                          "-ld   OR  --listell-default", "Sets configuration to Listell, Primell's non-prime believing cousin. Other simultaneously provided settings override this."
                        ]

  member this.GetResultString (result: PrimellObject) (control: PrimellProgramControl) =
    let temp =
      result.ToString()

    match result with
    | :? PList as l when not l.IsEmpty -> temp.Substring(1, temp.Length - 2)  // trim off outer parentheses
    | _ -> temp
  

  member this.ExecuteLine (lineText: string) (control: PrimellProgramControl) =

    let parser = PrimellVisitor.GetParser lineText
    PrimellVisitor(control).Visit(parser.line())

  member this.Run (program: string) (settings: PrimellConfiguration) =
    let lines = program.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

    let control = PrimellProgramControl(settings, lines)
    runnerVariables |> Seq.iter(fun kvp -> control.Variables[kvp.Key] <- kvp.Value)  // add in saved variables

    // pre-initialized variables
    control.TrySetVariable(",,,", PList.Empty, PList(Seq.initInfinite(fun _ -> PList.Empty :> PObject), Infinity Positive |> PNumber)) |> ignore
    control.TrySetVariable(",,,,,", PList.Empty, PList(Seq.initInfinite(fun _ -> ExtendedBigRational.Zero |> PNumber :> PObject), Infinity Positive |> PNumber)) |> ignore

    // also this has just been cobbled together over time, this could definitely be cleaner
    for i in 0..(control.LineResults.Length - 1) do
      control.CurrentLine <- i
      let result, doOutput = this.ExecuteLine control.LineResults[i].Text control, not control.LastOperationWasAssignment
      control.LastOperationWasAssignment <- false  // reset for next line
      control.LineResults[i] <-
        { control.LineResults[i] with Result = Some result; Output = if doOutput then Some (this.GetResultString result control) else None }
      if doOutput then printfn "%s" <| control.LineResults[i].Output.Value

    control

  // running from file is independent - no runner variables added
  member this.RunFromFile (settings: PrimellConfiguration) =
    let lines = IO.File.ReadAllLines(settings.SourceFilePath)
    this.Run (String.concat "\n" lines) settings
 

  member this.InteractiveMode ?settings =
    runnerVariables.Clear()
    let settings = defaultArg settings PrimellConfiguration.PrimellDefault
    this.PromptForInput settings false "Welcome to Primell. Enter ? for help."

  // prompt parameter is a bit misnamed, it mostly ends up being echo from the previous recursion, but can't think of better name right now
  member private this.PromptForInput (settings: PrimellConfiguration)(echo: bool)(prompt: string) =
    printfn "%s" prompt
    if not <| String.IsNullOrWhiteSpace prompt then 
      printfn ""  // extra line to provide space on console after previous output

    let input = Console.ReadLine().Trim()

    try
      if input.StartsWith("?") then
        let commandKey = if input.Length > 1 then input.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries)[0] else ""
        let argument = input.Substring(input.IndexOf(commandKey) + commandKey.Length).Trim()
        match commandKey.ToLowerInvariant() with
        | "q" | "quit" -> ()
        | "" | "?" | "help" -> 
            this.PromptForInput settings echo (this.HelpSpiel())
        | "run" ->   // TODO - read from settings
            if argument.Length = 0 then
              if String.IsNullOrWhiteSpace settings.SourceFilePath then
                this.PromptForInput settings echo "Run what?"
              else
                (this.RunFromFile(settings)).Variables |> Seq.iter(fun kvp -> runnerVariables[kvp.Key] <- kvp.Value)  
                this.PromptForInput settings echo (if echo then "Program has completed." else "") 
            else
              // not saving filepath in "permanent" settings is intentional
              this.RunFromFile({settings with SourceFilePath = argument}).Variables |> Seq.iter(fun kvp -> runnerVariables[kvp.Key] <- kvp.Value)  
              this.PromptForInput settings echo (if echo then "Program has completed." else "")  
        | "set" ->
            let newSettings = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            if newSettings |> Array.isEmpty then
              this.PromptForInput settings echo (sprintf "%A" settings) // TODO - format nicer
            else
              try
                this.PromptForInput (ParseLib.UpdateSettings settings (newSettings |> Array.toList)) echo (if echo then "Settings updated." else "")
              with
              | :? ArgumentException | :? InvalidCastException -> this.PromptForInput settings echo "Invalid setting"
        | "echo" -> 
            this.PromptForInput settings (not echo) (if echo then "" else "Echo turned on. on. on.")
        | "clear" -> 
            runnerVariables.Clear()
            this.PromptForInput settings echo (if echo then "Variable cache cleared." else "")
        | "reset" ->
            runnerVariables.Clear()
            this.PromptForInput PrimellConfiguration.PrimellDefault false (if echo then "Reset complete." else "")
        | _ -> this.PromptForInput settings echo "Unrecognized command" 
      else 
        (this.Run input settings).Variables |> Seq.iter(fun kvp -> runnerVariables[kvp.Key] <- kvp.Value)  
        this.PromptForInput settings echo (if echo then "Program line executed." else "")
    with
    | :? Antlr4.Runtime.Misc.ParseCanceledException as ex -> this.PromptForInput settings echo $"Invalid syntax: {ex.Message}"
    | :? NotImplementedException as ex -> this.PromptForInput settings echo $"Primell's programmer hasn't implemented that yet: {ex.Message}"
    | :? InvalidCastException as ex -> this.PromptForInput settings echo $"Primell's programmer hasn't implemented that yet: {ex.Message}"
         // invalid case is basically same as not implemented in this solution - they are hacks to temporarly not do the harder datatype
    | PrimellInvalidSyntaxException msg -> this.PromptForInput settings echo $"Invalid Syntax: {msg}"
    | PrimellProgrammerProblemException _ -> this.PromptForInput settings echo "Error in Primell's programmer's brain"
    | NonPrimeDectectionException msg -> printfn $"NON-PRIME DETECTED! ({msg})"  // purposeful exit
    
  
  member private this.HelpSpiel() =
    let line = "----------------------------------------------------------------------------------------------"

    let commandPreface = [""; "Commands  (input not prefixed with '?' engages REPL mode)"; line]

    let padding = (consoleCommands |> List.map(fun x -> x.Key.Length + x.ArgumentDescription.Length) |> List.max) + 4
    let commandList = consoleCommands |> List.map(fun command -> let s = $"?{command.Key} {command.ArgumentDescription}"
                                                                 sprintf "%s- %s" (s.PadRight(padding)) command.Description)

    let settingPreface = [""; ""; "Settings"; line]
    let settingList = consoleSettings |> List.collect(fun setting -> [fst setting; sprintf "    %s\n" (snd setting)])
    
    List.concat [commandPreface; commandList; settingPreface; settingList] |> String.concat "\n"