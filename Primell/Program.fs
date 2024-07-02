open dpenner1.PrimellF

let x: PNumber = Rational(10, 3)
let y: PNumber = Rational(10, -3)

let n = 2I
let n' = n - 1I

let sum = x + y
let dif = x - y

//let a: Infinity = Positive
let numbers = [for i in -1I..20000I do Rational(i, 1I)]

List.iter(fun n -> if PrimeLib.IsPrime n then printf "%O; " n) <| numbers

//printfn "%O" <| PNumber.Round(x)

//printfn "%O" <| PNumber.Round(y)
