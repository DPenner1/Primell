module PNumberTests

open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``Test Round Special``() =
    Assert.Equal(NaN, PNumber.Round(NaN))    
    Assert.Equal(Infinity Positive, PNumber.Round(Infinity Positive))
    Assert.Equal(Infinity Negative, PNumber.Round(Infinity Negative))

    let r1 = Rational <| R(1I, 2I)
    let r2 = Rational <| R(1I, -2I)

    //signed zero
    Assert.Equal(Rational <| R(0I, 1I), PNumber.Round(r1))
    Assert.Equal(Rational <| R(0I, -1I), PNumber.Round(r2))

[<Fact>]
let ``Test Round Thirds``() =
    let r1 = Rational <| R(11I, 3I)
    let r2 = Rational <| R(11I, -3I)
    let r3 = Rational <| R(10I, 3I)
    let r4 = Rational <| R(10I, -3I)

    Assert.Equal(Rational <| R(4I, 1I), PNumber.Round(r1))    
    Assert.Equal(Rational <| R(4I, -1I), PNumber.Round(r2))
    Assert.Equal(Rational <| R(3I, 1I), PNumber.Round(r3))
    Assert.Equal(Rational <| R(3I, -1I), PNumber.Round(r4))

[<Fact>]
let ``Test Round To Even``() =
    let r1 = Rational <| R(9I, 2I)
    let r2 = Rational <| R(9I, -2I)
    let r3 = Rational <| R(7I, 2I)
    let r4 = Rational <| R(7I, -2I)

    Assert.Equal(Rational <| R(4I, 1I), PNumber.Round(r1))
    Assert.Equal(Rational <| R(4I, -1I), PNumber.Round(r2))
    Assert.Equal(Rational <| R(4I, 1I), PNumber.Round(r3))
    Assert.Equal(Rational <| R(4I, -1I), PNumber.Round(r4))

[<Fact>]
let ``Test Ceil Special``() =
  Assert.Equal(NaN, PNumber.Ceiling NaN)
  Assert.Equal(Infinity Positive, PNumber.Ceiling(Infinity Positive))
  Assert.Equal(Infinity Negative, PNumber.Ceiling(Infinity Negative))


[<Fact>]
let ``Test Ceil``() =
  Assert.Equal(Rational <| R(2I, 1I), PNumber.Ceiling(Rational <| R(2I, 1I)))
  Assert.Equal(Rational <| R(2I, -1I), PNumber.Ceiling(Rational <| R(2I, -1I)))
  Assert.Equal(Rational <| R(4I, 1I), PNumber.Ceiling(Rational <| R(10I, 3I)))
  Assert.Equal(Rational <| R(3I, -1I), PNumber.Ceiling(Rational <| R(10I, -3I)))

[<Fact>]
let ``Test Floor Special``() =
  Assert.Equal(NaN, floor NaN)
  Assert.Equal(Infinity Positive, PNumber.Floor(Infinity Positive))
  Assert.Equal(Infinity Negative, PNumber.Floor(Infinity Negative))


[<Fact>]
let ``Test Floor``() =
  Assert.Equal(Rational <| R(2I, 1I), PNumber.Floor(Rational <| R(2I, 1I)))
  Assert.Equal(Rational <| R(2I, -1I), PNumber.Floor(Rational <| R(2I, -1I)))
  Assert.Equal(Rational <| R(4I, 1I), PNumber.Floor(Rational <| R(10I, 3I)))
  Assert.Equal(Rational <| R(3I, -1I), PNumber.Floor(Rational <| R(10I, -3I)))

[<Fact>]
let ``Test Add Special``() =
  Assert.Equal(NaN, NaN + NaN)
  Assert.Equal(NaN, NaN + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + NaN)
  Assert.Equal(NaN, NaN + (Rational <| R(1I, 1I)))
  Assert.Equal(NaN, (Rational <| R(1I, 1I)) + NaN)

  Assert.Equal(Infinity Positive, Infinity Positive + (Rational <| R(1I, 1I)))
  Assert.Equal(Infinity Positive, (Rational <| R(1I, 1I)) + Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative + (Rational <| R(1I, 1I)))
  Assert.Equal(Infinity Negative, (Rational <| R(1I, 1I)) + Infinity Negative)

  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, PNumber.NegativeZero + PNumber.Zero)
  Assert.Equal(PNumber.Zero, PNumber.Zero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, (Rational <| R(1I, -1I)) + (Rational <| R(1I, 1I)))


[<Fact>]
let ``Test Subtract Special``() =
  Assert.Equal(NaN, NaN - NaN)
  Assert.Equal(NaN, NaN - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, Infinity Positive - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - Infinity Negative)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, NaN - Infinity Negative)
  Assert.Equal(NaN, NaN - (Rational <| R(1I, 1I)))
  Assert.Equal(NaN, (Rational <| R(1I, 1I)) - NaN)

  Assert.Equal(Infinity Negative, Infinity Negative - Infinity Positive)
  Assert.Equal(Infinity Positive, Infinity Positive - Infinity Negative)
  Assert.Equal(Infinity Positive, Infinity Positive - (Rational <| R(1I, 1I)))
  Assert.Equal(Infinity Negative, (Rational <| R(1I, 1I)) - Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative - (Rational <| R(1I, 1I)))
  Assert.Equal(Infinity Positive, (Rational <| R(1I, 1I)) - Infinity Negative)

  // IEEE 754 standard has this gem
  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero - (-PNumber.NegativeZero))

  Assert.Equal(PNumber.Zero, (Rational <| R(1I, 1I)) - (Rational <| R(1I, 1I)))

[<Fact>]
let ``Test Negate Special``() =
  Assert.Equal(NaN, -NaN)
  Assert.Equal(Infinity Positive, -Infinity Negative)
  Assert.Equal(Infinity Negative, -Infinity Positive)
  Assert.Equal(Rational <| R(1I, -1I), -(Rational <| R(1I, 1I)))
  Assert.Equal(Rational <| R(1I, 1I), -(Rational <| R(1I, -1I)))

