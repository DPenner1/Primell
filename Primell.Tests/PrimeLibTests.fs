// note: Tests not entirely comprehensive
module PrimeLibTests

open Xunit
open dpenner1.PrimellF

let TestRange(range, primes) =
  range |> List.iter(fun i ->
    Assert.Equal(List.contains i primes, PrimeLib.IsPrime(Rational <| R(i, 1I)))
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
  Assert.False <| PrimeLib.IsPrime NaN
  Assert.False <| PrimeLib.IsPrime(Infinity Positive)
  Assert.False <| PrimeLib.IsPrime(Infinity Negative)
  
  Assert.False <| PrimeLib.IsPrime(Rational <| R(1, -1))
  Assert.False <| PrimeLib.IsPrime(Rational <| R(0, 1))
  Assert.False <| PrimeLib.IsPrime(Rational <| R(1, 1))

  Assert.True <| PrimeLib.IsPrime(Rational <| R(2, 1))

  Assert.False <| PrimeLib.IsPrime(Rational <| R(5, 2))
  Assert.False <| PrimeLib.IsPrime(Rational <| R(5, -2))