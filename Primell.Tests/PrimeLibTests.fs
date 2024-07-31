// note: Tests not entirely comprehensive
module PPrimeLibTests

open Xunit
open dpenner1.Primell

let TestRange(range, primes) =
  range |> List.iter(fun i ->
    Assert.Equal(List.contains i primes, PPrimeLib.IsPrime(BigRational(i, 1I) |> Rational |> PNumber))
  )

[<Fact>]
let ``Test Is Prime 80-100`` () =
  TestRange([80I..100I], [83I;89I;97I])

[<Fact>]
let ``Test Is Prime 9900-9930`` () =
  TestRange([9900I..9930I], [9901I;9907I;9923I;9929I])

[<Fact>]
let ``Test Is Prime 999950-1000000`` () =
  TestRange([999950I..1000000I], [999953I;999959I;999961I;999979I;999983I])

[<Fact>]
let ``Test Is Prime Special Cases`` () =
  Assert.False <| PPrimeLib.IsPrime(NaN |> PNumber)
  Assert.False <| PPrimeLib.IsPrime(Infinity Positive |> PNumber)
  Assert.False <| PPrimeLib.IsPrime(Infinity Negative |> PNumber)
  
  Assert.False <| PPrimeLib.IsPrime(BigRational(1, -1) |> Rational |> PNumber)
  Assert.False <| PPrimeLib.IsPrime(BigRational(0, 1) |> Rational |> PNumber)
  Assert.False <| PPrimeLib.IsPrime(BigRational(1, 1) |> Rational |> PNumber)

  Assert.True <| PPrimeLib.IsPrime(BigRational(2, 1) |> Rational |> PNumber)

  Assert.False <| PPrimeLib.IsPrime(BigRational(5, 2) |> Rational |> PNumber)
  Assert.False <| PPrimeLib.IsPrime(BigRational(5, -2) |> Rational |> PNumber)