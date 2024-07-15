namespace dpenner1.PrimellF

open Antlr4.Runtime

type PrimellRunner() = 

  let rec GetResultString (result: PrimellObject) (control: PrimellProgramControl) =
    let temp =
      result.ToString(control.GetVariableDictionary())

    match result with
    | :? PReference as ref -> GetResultString(control.GetVariable(ref.Name)) control
    | :? PList as l when not l.IsEmpty -> temp.Substring(1, Seq.length l.Value - 2)  // trim off outer parentheses
    | _ -> temp
  

  member this.ExecuteLine lineContext control =
    let visitor = PrimellVisitor control
    visitor.Visit(lineContext)

  // TODO - for now I'm leaving grammar intact due to needing to compare results to original C#
  //      - But Primell executes line by line, I don't need the program + line stuff,
  //      - I can just create a new parser for each line (passing in an ever changing control)

  member this.Run (program: string) (settings: PrimellConfiguration) =
    let stream = AntlrInputStream program
    let lexer = PrimellLexer stream
    let tokens = CommonTokenStream lexer
    let parser = PrimellParser tokens
    parser.BuildParseTree <- true
    
    let programControl = PrimellProgramControl settings
    let allLineContexts = parser.program().line() |> Array.toList

    // TODO - more functional way?
    for line in allLineContexts do
      let result = this.ExecuteLine line programControl
      if not programControl.LastOperationWasAssignment then
        printfn "%O" <| GetResultString result programControl

  member this.RunFromFile (filepath: string) (settings: PrimellConfiguration) =
    let lines = System.IO.File.ReadAllLines(filepath)
    this.Run (String.concat "\n" lines) settings
