open dpenner1.PrimellF


let x: PNumber = Number <| BigRational(10I, -3I)
let y: PNumber = Number <| BigRational(10, -3)

let n = 2I
let n' = n - 1I

let sum = x + y
let dif = x - y

//let a: Infinity = Positive
//let numbers = [for i in -1I..20000I do Rational(i, 1I)

//List.iter(fun n -> if PrimeLib.IsPrime n then printf "%O; " n) <| numbers

let obj = PList(seq{ Atom(Number <| BigRational(1I, 2I)); Atom(Infinity Negative); PList(seq{ PList(Seq.empty); Atom(NaN) }) })

printfn "%O" <| obj

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


