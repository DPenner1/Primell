module PNumberTests

open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``Test Round Special``() =
    Assert.Equal(NaN, PNumber.Round(NaN))    
    Assert.Equal(Infinity Positive, PNumber.Round(Infinity Positive))
    Assert.Equal(Infinity Negative, PNumber.Round(Infinity Negative))

    let r1 = Rational(1I, 2I)
    let r2 = Rational(1I, -2I)

    //signed zero
    Assert.Equal(Rational(0I, 1I), PNumber.Round(r1))
    Assert.Equal(Rational(0I, -1I), PNumber.Round(r2))

[<Fact>]
let ``Test Round Thirds``() =
    let r1 = Rational(11I, 3I)
    let r2 = Rational(11I, -3I)
    let r3 = Rational(10I, 3I)
    let r4 = Rational(10I, -3I)

    Assert.Equal(Rational(4I, 1I), PNumber.Round(r1))    
    Assert.Equal(Rational(4I, -1I), PNumber.Round(r2))
    Assert.Equal(Rational(3I, 1I), PNumber.Round(r3))
    Assert.Equal(Rational(3I, -1I), PNumber.Round(r4))

[<Fact>]
let ``Test Round To Even``() =
    let r1 = Rational(9I, 2I)
    let r2 = Rational(9I, -2I)
    let r3 = Rational(7I, 2I)
    let r4 = Rational(7I, -2I)

    Assert.Equal(Rational(4I, 1I), PNumber.Round(r1))
    Assert.Equal(Rational(4I, -1I), PNumber.Round(r2))
    Assert.Equal(Rational(4I, 1I), PNumber.Round(r3))
    Assert.Equal(Rational(4I, -1I), PNumber.Round(r4))

[<Fact>]
let ``Test Ceil Special``() =
  Assert.Equal(NaN, PNumber.Ceiling NaN)
  Assert.Equal(Infinity Positive, PNumber.Ceiling(Infinity Positive))
  Assert.Equal(Infinity Negative, PNumber.Ceiling(Infinity Negative))


[<Fact>]
let ``Test Ceil``() =
  Assert.Equal(Rational(2I, 1I), PNumber.Ceiling(Rational(2I, 1I)))
  Assert.Equal(Rational(2I, -1I), PNumber.Ceiling(Rational(2I, -1I)))
  Assert.Equal(Rational(4I, 1I), PNumber.Ceiling(Rational(10I, 3I)))
  Assert.Equal(Rational(3I, -1I), PNumber.Ceiling(Rational(10I, -3I)))

[<Fact>]
let ``Test Add Special``() =
  Assert.Equal(NaN, NaN + NaN)
  Assert.Equal(NaN, NaN + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + NaN)
  Assert.Equal(NaN, NaN + Rational(1I, 1I))
  Assert.Equal(NaN, Rational(1I, 1I) + NaN)

  Assert.Equal(Infinity Positive, Infinity Positive + Rational(1I, 1I))
  Assert.Equal(Infinity Positive, Rational(1I, 1I) + Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative + Rational(1I, 1I))
  Assert.Equal(Infinity Negative, Rational(1I, 1I) + Infinity Negative)

  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, PNumber.NegativeZero + PNumber.Zero)
  Assert.Equal(PNumber.Zero, PNumber.Zero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, Rational(1I, -1I) + Rational(1I, 1I))


[<Fact>]
let ``Test Subtract Special``() =
  Assert.Equal(NaN, NaN - NaN)
  Assert.Equal(NaN, NaN - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, Infinity Positive - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - Infinity Negative)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, NaN - Infinity Negative)
  Assert.Equal(NaN, NaN - Rational(1I, 1I))
  Assert.Equal(NaN, Rational(1I, 1I) - NaN)

  Assert.Equal(Infinity Negative, Infinity Negative - Infinity Positive)
  Assert.Equal(Infinity Positive, Infinity Positive - Infinity Negative)
  Assert.Equal(Infinity Positive, Infinity Positive - Rational(1I, 1I))
  Assert.Equal(Infinity Negative, Rational(1I, 1I) - Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative - Rational(1I, 1I))
  Assert.Equal(Infinity Positive, Rational(1I, 1I) - Infinity Negative)

  // IEEE 754 standard has this gem
  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero - (-PNumber.NegativeZero))

  Assert.Equal(PNumber.Zero, Rational(1I, 1I) - Rational(1I, 1I))

[<Fact>]
let ``Test Negate Special``() =
  Assert.Equal(NaN, -NaN)
  Assert.Equal(Infinity Positive, -Infinity Negative)
  Assert.Equal(Infinity Negative, -Infinity Positive)
  Assert.Equal(Rational(1I, -1I), -Rational(1I, 1I))
  Assert.Equal(Rational(1I, 1I), -Rational(1I, -1I))

