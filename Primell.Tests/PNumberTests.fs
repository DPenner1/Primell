module PNumberTests

open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``Test Round``() =
    let r1 = Rational(11I, 3I)
    let r2 = Rational(11I, -3I)
    let r3 = Rational(10I, 3I)
    let r4 = Rational(10I, -3I)

    // Special cases
    Assert.Equal(PNumber.Round(NaN), NaN)    
    Assert.Equal(PNumber.Round(Infinity Positive), Infinity Positive)
    Assert.Equal(PNumber.Round(Infinity Negative), Infinity Negative)

    // rounding numbers up and down, positive and negative
    Assert.Equal(PNumber.Round(r1), Rational(4I, 1I))    
    Assert.Equal(PNumber.Round(r2), Rational(4I, -1I))
    Assert.Equal(PNumber.Round(r3), Rational(3I, 1I))
    Assert.Equal(PNumber.Round(r4), Rational(3I, -1I))