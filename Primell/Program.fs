open dpenner1.PrimellF

// slightly regretting the nested types
let weirdList = seq { Seq.empty |> PrimellList |> PList ; NaN |> Atom } |> PrimellList |> PList

let obj = PList(PrimellList((seq{ Atom(Number <| BigRational(1I, 2I)); Atom(Infinity Negative); weirdList })))
let runner = new PrimellRunner();
let range = PNumber.Range (5 |> BigRational |> Number) (101 |> BigRational |> Number)

//printfn "%O" <| (range |> Seq.map (fun x -> x |> Atom) |> PrimellList |> PList)
printfn "%O" <| runner.Run "2..101" PrimellConfiguration.PrimellDefault
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