open dpenner1.PrimellF


let x: PNumber = Rational <| R(10I, -3I)
let y: PNumber = Rational <| R(10, -3)

let n = 2I
let n' = n - 1I

let sum = x + y
let dif = x - y

//let a: Infinity = Positive
//let numbers = [for i in -1I..20000I do Rational(i, 1I)

//List.iter(fun n -> if PrimeLib.IsPrime n then printf "%O; " n) <| numbers

let obj = PList(seq{ Atom(Rational <| R(1I, 2I)); Atom(Infinity Negative); PList(seq{ PList(Seq.empty); Atom(NaN) }) })

printfn "%O" <| floor(Rational <| R(10I, -3I))

//let c = inputs |> List.concat




