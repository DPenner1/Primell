namespace dpenner1.PrimellF

open Antlr4.Runtime

type PrimellRunner() = 

  //static member ExecuteLine (allLineContexts: list<PrimellParser.LineContext>) (lineNumber: int): PrimellObject = 
  member this.ExecuteLine lineContext control =
    let visitor = PrimellVisitor control
    visitor.Visit(lineContext)

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
        printfn "%O" result

