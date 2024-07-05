// note: Tests not entirely comprehensive
module PNumberTests

open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``Test Round Special``() =
    Assert.Equal(NaN, round NaN)    
    Assert.Equal(Infinity Positive, round <| Infinity Positive)
    Assert.Equal(Infinity Negative, round <| Infinity Negative)

    let r1 = Rational <| R(1, 2)
    let r2 = Rational <| R(1, -2)

    //signed zero
    Assert.Equal(Rational <| R(0, 1), round r1)
    Assert.Equal(Rational <| R(0, -1), round r2)

[<Fact>]
let ``Test Round Thirds``() =
    let r1 = Rational <| R(11, 3)
    let r2 = Rational <| R(11, -3)
    let r3 = Rational <| R(10, 3)
    let r4 = Rational <| R(10, -3)

    Assert.Equal(Rational <| R(4, 1), round r1)    
    Assert.Equal(Rational <| R(4, -1), round r2)
    Assert.Equal(Rational <| R(3, 1), round r3)
    Assert.Equal(Rational <| R(3, -1), round r4)

[<Fact>]
let ``Test Round To Even``() =
    let r1 = Rational <| R(9, 2)
    let r2 = Rational <| R(9, -2)
    let r3 = Rational <| R(7, 2)
    let r4 = Rational <| R(7, -2)

    Assert.Equal(Rational <| R(4, 1), round r1)
    Assert.Equal(Rational <| R(4, -1), round r2)
    Assert.Equal(Rational <| R(4, 1), round r3)
    Assert.Equal(Rational <| R(4, -1), round r4)

[<Fact>]
let ``Test Round Near Zero``() =
    let r1 = Rational <| R(2, 3)
    let r2 = Rational <| R(2, -3)
    let r3 = Rational <| R(1, 3)
    let r4 = Rational <| R(1, -3)

    Assert.Equal(Rational <| R(1, 1), round r1)
    Assert.Equal(Rational <| R(1, -1), round r2)
    Assert.Equal(Rational <| R(0, 1), round r3)
    Assert.Equal(Rational <| R(0, -1), round r4)

[<Fact>]
let ``Test Ceil Special``() =
  Assert.Equal(NaN, ceil NaN)
  Assert.Equal(Infinity Positive, ceil <| Infinity Positive)
  Assert.Equal(Infinity Negative, ceil <| Infinity Negative)


[<Fact>]
let ``Test Ceil``() =
  Assert.Equal(Rational <| R(2, 1), ceil(Rational <| R(2, 1)))
  Assert.Equal(Rational <| R(2, -1), ceil(Rational <| R(2, -1)))
  Assert.Equal(Rational <| R(4, 1), ceil(Rational <| R(10, 3)))
  Assert.Equal(Rational <| R(3, -1), ceil(Rational <| R(10, -3)))

[<Fact>]
let ``Test Floor Special``() =
  Assert.Equal(NaN, floor NaN)
  Assert.Equal(Infinity Positive, floor <| Infinity Positive)
  Assert.Equal(Infinity Negative, floor <| Infinity Negative)


[<Fact>]
let ``Test Floor``() =
  Assert.Equal(Rational <| R(2, 1), floor(Rational <| R(2, 1)))
  Assert.Equal(Rational <| R(2, -1), floor(Rational <| R(2, -1)))
  Assert.Equal(Rational <| R(3, 1), floor(Rational <| R(10, 3)))
  Assert.Equal(Rational <| R(4, -1), floor(Rational <| R(10, -3)))

[<Fact>]
let ``Test Add Special``() =
  Assert.Equal(NaN, NaN + NaN)
  Assert.Equal(NaN, NaN + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + NaN)
  Assert.Equal(NaN, NaN + (Rational <| R(1, 1)))
  Assert.Equal(NaN, (Rational <| R(1, 1)) + NaN)

  Assert.Equal(Infinity Positive, Infinity Positive + (Rational <| R(1, 1)))
  Assert.Equal(Infinity Positive, (Rational <| R(1, 1)) + Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative + (Rational <| R(1, 1)))
  Assert.Equal(Infinity Negative, (Rational <| R(1, 1)) + Infinity Negative)

  //signed zero
  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, PNumber.NegativeZero + PNumber.Zero)
  Assert.Equal(PNumber.Zero, PNumber.Zero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, (Rational <| R(1, -1)) + (Rational <| R(1, 1)))


[<Fact>]
let ``Test Subtract Special``() =
  Assert.Equal(NaN, NaN - NaN)
  Assert.Equal(NaN, NaN - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, Infinity Positive - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - Infinity Negative)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, NaN - Infinity Negative)
  Assert.Equal(NaN, NaN - (Rational <| R(1, 1)))
  Assert.Equal(NaN, (Rational <| R(1, 1)) - NaN)

  Assert.Equal(Infinity Negative, Infinity Negative - Infinity Positive)
  Assert.Equal(Infinity Positive, Infinity Positive - Infinity Negative)
  Assert.Equal(Infinity Positive, Infinity Positive - (Rational <| R(1, 1)))
  Assert.Equal(Infinity Negative, (Rational <| R(1, 1)) - Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative - (Rational <| R(1, 1)))
  Assert.Equal(Infinity Positive, (Rational <| R(1, 1)) - Infinity Negative)

  // EEE 754 standard has this gem
  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero - (-PNumber.NegativeZero))

  Assert.Equal(PNumber.Zero, (Rational <| R(1, 1)) - (Rational <| R(1, 1)))

[<Fact>]
let ``Test Negate Special``() =
  Assert.Equal(NaN, -NaN)
  Assert.Equal(Infinity Positive, -Infinity Negative)
  Assert.Equal(Infinity Negative, -Infinity Positive)
  Assert.Equal(Rational <| R(1, -1), -(Rational <| R(1, 1)))
  Assert.Equal(Rational <| R(1, 1), -(Rational <| R(1, -1)))

