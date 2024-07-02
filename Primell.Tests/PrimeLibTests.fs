module PrimeLibTests

open System
open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``My test`` () =
    Assert.True(true)

[<Fact>]
let ``Test Round``() =
    let r1 = Rational(11I, 3I)
    let r2 = Rational(11I, -3I)
    let r3 = Rational(10I, 3I)
    let r4 = Rational(10I, -3I)

    Assert.Equal(PNumber.Round(r1), Rational(4I, 1I))    
    Assert.Equal(PNumber.Round(r2), Rational(4I, -1I))
    Assert.Equal(PNumber.Round(r3), Rational(3I, 1I))
    Assert.Equal(PNumber.Round(r4), Rational(3I, -1I))