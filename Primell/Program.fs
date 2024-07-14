open dpenner1.PrimellF

// slightly regretting the nested types
let weirdList = seq { PList.Empty :> PObject; NaN |> PNumber :> PObject } |> PList :> PObject

let funlist = seq {BigRational(1,2) |> Rational |> PNumber :> PObject; Infinity Negative |> PNumber :> PObject; weirdList} |> PList :> PObject

//PList(PrimellList((seq{ Atom(Number <| BigRational(1I, 2I)); Atom(Infinity Negative); weirdList })))
let runner = new PrimellRunner();
let range = ExtendedBigRational.Range (5 |> BigRational |> Rational) (101 |> BigRational |> Rational) |> Seq.map(fun x -> x |> PNumber)

let program = """
, = (2 3 5 7)
,
,@2 = 13
,
,@(2 3) =$ 7 11
,"""

//let indexProgram = "(2 3 5)@2"  //works
let indexProgram = "(2 3 5 7)@$3~" // works

//let indexProgram = "(2 3 5 7)@(2 3)" //works
//let indexProgram = "(2 3 5 7)@(3~ 2)" //works 
//let indexProgram = "(2 3 5 7)@(2 3~)"

let testNegate = 5 |> BigRational |> Rational
let negate = -testNegate

let negateParseTest = "2 3~"

let variableProgram = ",\n, = 3\n,"
//let testList = seq {ExtendedBigRational.Zero |> PNumber :> PObject; ExtendedBigRational.One |> PNumber :> PObject; ExtendedBigRational.Two |> PNumber :> PObject} |> PList
    
//let testIndex = testList.Index(0 |> BigRational |> Rational |> PNumber)

//printfn "%O" <| testIndex

//printfn "%O" <| namedList.Name
//runner.Run "2..101" PrimellConfiguration.PrimellDefault

runner.Run variableProgram PrimellConfiguration.PrimellDefault
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
