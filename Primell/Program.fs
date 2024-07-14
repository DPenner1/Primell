open dpenner1.PrimellF

// slightly regretting the nested types
let weirdList = seq { PList.Empty :> IPrimellObject; NaN |> PNumber :> IPrimellObject } |> PList :> IPrimellObject

let funlist = seq {BigRational(1,2) |> Rational |> PNumber :> IPrimellObject; Infinity Negative |> PNumber :> IPrimellObject; weirdList} |> PList :> IPrimellObject

//PList(PrimellList((seq{ Atom(Number <| BigRational(1I, 2I)); Atom(Infinity Negative); weirdList })))
let runner = new PrimellRunner();
let range = ExtendedBigRational.Range (5 |> BigRational |> Rational) (101 |> BigRational |> Rational) |> Seq.map(fun x -> x |> PNumber)
let namedList = PList(PList.Empty, 0 |> BigRational |> Rational |> PNumber, "Hello", PList.Empty, 0)

//printfn "%O" <| namedList.Name
runner.Run "2..101" PrimellConfiguration.PrimellDefault
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
