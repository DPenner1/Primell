open dpenner1.PrimellF

// slightly regretting the nested types
let weirdList = seq { PList.Empty :> PObject; NaN |> PNumber :> PObject } |> PList :> PObject

let funlist = seq {BigRational(1,2) |> Rational |> PNumber :> PObject; Infinity Negative |> PNumber :> PObject; weirdList} |> PList :> PObject

//PList(PrimellList((seq{ Atom(Number <| BigRational(1I, 2I)); Atom(Infinity Negative); weirdList })))
let runner = new PrimellRunner();
let range = ExtendedBigRational.Range (5 |> BigRational |> Rational) (101 |> BigRational |> Rational) |> Seq.map(fun x -> x |> PNumber)

let program = ", = (; 3 5)\n; = 2\n,\n;"

runner.Run program PrimellConfiguration.PrimellDefault |> ignore
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

//let control2 = {PrimellConfiguration.PrimellDefault with SourceFilePath = "../../../../Examples/NumbersTo100.pll"}

//runner.RunFromFile  control2 |> ignore
(*
let x = ref 0
let y = ref 1
let z = ref 2

let refSeq = seq {ref 0; ref 1; ref 2}
let refList = [ref 0; ref 1; ref 2]
let refLetSeq = seq {x; y; z}
let refInitSeq = Seq.init 3 (fun i -> ref i)
let refInfSeq = Seq.initInfinite (fun i -> ref i)

refInitSeq |> Seq.iter(fun x -> x.Value <- 33; printfn "%d" x.Value)


refInitSeq |> Seq.iter(fun x -> printfn "%d" x.Value)

(Seq.head refSeq).Value <- 42
(Seq.head refList).Value <- 42
(Seq.head refLetSeq).Value <- 42
(Seq.head refInitSeq).Value <- 42
(Seq.head refInfSeq).Value <- 42


printfn "Ref seq:      %d" (Seq.head refSeq).Value
printfn "Ref list:     %d" (Seq.head refList).Value
printfn "Ref let seq:  %d" (Seq.head refLetSeq).Value
printfn "Ref init seq: %d" (Seq.head refInitSeq).Value
printfn "Ref inf seq:  %d" (Seq.head refInfSeq).Value

//let x = PrimellConfiguration.Listell
//printf "%A " <| {x with SourceBase=13}

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
