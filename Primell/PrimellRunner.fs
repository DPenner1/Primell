namespace dpenner1.PrimellF

open Antlr4.Runtime

type PrimellRunner = 

  static member ExecuteLine (allLineContexts: ResizeArray<PrimellParser.LineContext>) (lineNumber: int): int = 
    allLineContexts[lineNumber] |> ignore //temp!
    lineNumber

  static member Run (program: string) (settings: PrimellConfiguration) =
    let stream = AntlrInputStream program
    let lexer = PrimellLexer stream
    let tokens = CommonTokenStream lexer
    let parser = PrimellParser tokens
    parser.BuildParseTree <- true
    
    // TODO - This bit was direct port from C# to get things going, see if there's an immutable way to do this
    let allLineContexts = new ResizeArray<PrimellParser.LineContext>();
    parser.program().line() |> Seq.iter(fun line -> allLineContexts.Add(line))

    let settings = 
      {
        FreeSource = true // for my sanity while testing
        TruthDefinition = { PrimesAreTruth = true; EmptyIsTruth = false; RequireAllTruth = true }
        InputBase = 10
        OutputBase = 10
        SourceBase = 10
        InputEncoding = System.Text.Encoding.UTF8
        OutputEncoding = System.Text.Encoding.UTF8
        SourceEncoding = System.Text.Encoding.UTF8
        OutputFilePath = "./Testing/testout.txt"
        SourceFilePath = "../Examples/PrimesTo100.pll"
      }
    
    // TODO - again, more familiar C# style to get things going
    let mutable currentLine = 0
    
    while currentLine >= 0 && currentLine < allLineContexts.Count do 
      currentLine <- PrimellRunner.ExecuteLine allLineContexts currentLine

  //
(*public void Run(string program, PLProgramSettings settings)
        {
            AntlrInputStream stream = new AntlrInputStream(program);
            ITokenSource lexer = new PrimellLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            PrimellParser parser = new PrimellParser(tokens);
            parser.BuildParseTree = true;

            var allLineContexts = new List<PrimellParser.LineContext>();
            foreach (var lineContext in parser.program().line())
            {
                allLineContexts.Add(lineContext);
            }

            var programContext = new PrimeProgramControl(allLineContexts, settings);

            while (programContext.ExecuteCurrentLine()) ;
        }*)