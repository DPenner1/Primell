open dpenner1.PrimellF

// slightly regretting the nested types
let weirdList = seq { PList.Empty :> PObject; NaN |> PNumber :> PObject } |> PList :> PObject

let funlist = seq {BigRational(1,2) |> Rational |> PNumber :> PObject; Infinity Negative |> PNumber :> PObject; weirdList} |> PList :> PObject

//PList(PrimellList((seq{ Atom(Number <| BigRational(1I, 2I)); Atom(Infinity Negative); weirdList })))
let runner = new PrimellRunner();
let range = ExtendedBigRational.Range (5 |> BigRational |> Rational) (101 |> BigRational |> Rational) |> Seq.map(fun x -> x |> PNumber)

let program = """
, =$ 2*2*5*5
, -= 2
; =^$ 2/2
; =^ (;_< + 3 - 2 ;)
"""

let program2 = ",@(2 3) =$ 2-2 2-2\n,"

runner.Run program2 PrimellConfiguration.PrimellDefault |> ignore
//runner.InteractiveMode()

//let program2 = ", =$ 2*2*5*5\n, -= 2\n,"
//let variableProgram = "(, ;) = (3)\n,\n;"
//let variableIndexProgram = ", = (2 3)\n,\n,@1 = 5\n,"
//let variableProgram = ", = ;\n; = 3\n,\n;"
//let testList = seq {ExtendedBigRational.Zero |> PNumber :> PObject; ExtendedBigRational.One |> PNumber :> PObject; ExtendedBigRational.Two |> PNumber :> PObject} |> PList
    
//let testIndex = testList.Index(0 |> BigRational |> Rational |> PNumber)

//printfn "%O" <| testIndex

//printfn "%O" <| namedList.Name
//runner.Run "2..101" PrimellConfiguration.PrimellDefault
//let programControl = PrimellProgramControl PrimellConfiguration.Listell
//runner.Run program2 programControl |> ignore

let control2 = {PrimellConfiguration.PrimellDefault with SourceFilePath = "../../../../Examples/NumbersTo100.pll"}

runner.RunFromFile  control2 |> ignore

//let x = PrimellConfiguration.Listell
//printf "%A " <| {x with SourceBase=13}
(*
for x in BigRational.Range(BigRational(3, 1), BigRational(13, 2), BigRational(1, 3), true, false) do 
    printf "%O " <| x
printfn ""

for x in BigRational.Range(BigRational(3, 1), BigRational(13, 2), BigRational(1, 2), true, true) do 
    printf "%O " <| x
printfn ""

for x in BigRational.Range(BigRational(3, 1), BigRational(8, 2), BigRational(1, 1), true, true) do 
    printf "%O " <| x
printfn ""

//let c = inputs |> List.concat


*)
