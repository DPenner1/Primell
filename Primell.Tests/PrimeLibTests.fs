// note: Tests not entirely comprehensive
module PPrimeLibTests

open Xunit
open dpenner1.Primell

let AssertEqualSequences<'T>(expected: seq<'T>, actual: seq<'T>) =
  Assert.Equal(Seq.length expected, Seq.length actual)
  Seq.zip expected actual |> Seq.iter(fun pair -> Assert.Equal(fst pair, snd pair))

let TestRange(range, knownPrimes) =
  let calculatedPrimes = range |> Seq.filter (fun x -> PPrimeLib.IsPrime(BigRational(x, 1I) |> Rational |> PNumber))  
  AssertEqualSequences(knownPrimes, calculatedPrimes)

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


[<Fact>]
let ``Test PrimeFactorization``() = 
  let calculated = PPrimeLib.PrimeFactorization(BigRational(-20, 77) |> Rational |> PNumber)
  let expected = 
    [BigRational -1; BigRational(1, 11); BigRational(1, 7); BigRational 2; BigRational 2; BigRational 5] 
    |> Seq.map(fun r -> r |> Rational |> PNumber :> PObject) |> PList :> PObject
  Assert.Equal(expected, calculated)

  let calculated = PPrimeLib.PrimeFactorization(BigRational -5 |> Rational |> PNumber)
  let expected = 
    [BigRational -1; BigRational 5] 
    |> Seq.map(fun r -> r |> Rational |> PNumber :> PObject) |> PList :> PObject
  Assert.Equal(expected, calculated)


[<Fact>]
let ``Test IsSquare``() = 
  let squares = [1I..400I] |> List.map(fun x -> x*x)
  for i in 1I..160000I do
    Assert.Equal(squares |> List.contains i, PrimeLib.IsSquare i)  
  