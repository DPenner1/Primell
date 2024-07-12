// note: Tests not entirely comprehensive
module PNumberTests

open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``Test Round Special``() =
    Assert.Equal(NaN, round NaN)    
    Assert.Equal(Infinity Positive, round <| Infinity Positive)
    Assert.Equal(Infinity Negative, round <| Infinity Negative)

    let r1 = BigRational(1, 2) |> Number
    let r2 = BigRational(1, -2) |> Number

    //signed zero
    Assert.Equal(BigRational(0, 1) |> Number, round r1)
    Assert.Equal(BigRational(0, -1) |> Number, round r2)

[<Fact>]
let ``Test Round Thirds``() =
    let r1 = BigRational(11, 3) |> Number
    let r2 = BigRational(11, -3) |> Number
    let r3 = BigRational(10, 3) |> Number
    let r4 = BigRational(10, -3) |> Number

    Assert.Equal(BigRational(4, 1) |> Number, round r1)    
    Assert.Equal(BigRational(4, -1) |> Number, round r2)
    Assert.Equal(BigRational(3, 1) |> Number, round r3)
    Assert.Equal(BigRational(3, -1) |> Number, round r4)

[<Fact>]
let ``Test Round To Even``() =
    let r1 = BigRational(9, 2) |> Number
    let r2 = BigRational(9, -2) |> Number
    let r3 = BigRational(7, 2) |> Number
    let r4 = BigRational(7, -2) |> Number

    Assert.Equal(BigRational(4, 1) |> Number, round r1)
    Assert.Equal(BigRational(4, -1) |> Number, round r2)
    Assert.Equal(BigRational(4, 1) |> Number, round r3)
    Assert.Equal(BigRational(4, -1) |> Number, round r4)

[<Fact>]
let ``Test Round Near Zero``() =
    let r1 = BigRational(2, 3) |> Number
    let r2 = BigRational(2, -3) |> Number
    let r3 = BigRational(1, 3) |> Number
    let r4 = BigRational(1, -3) |> Number

    Assert.Equal(BigRational(1, 1) |> Number, round r1)
    Assert.Equal(BigRational(1, -1) |> Number, round r2)
    Assert.Equal(BigRational(0, 1) |> Number, round r3)
    Assert.Equal(BigRational(0, -1) |> Number, round r4)

[<Fact>]
let ``Test Ceil Special``() =
  Assert.Equal(NaN, ceil NaN)
  Assert.Equal(Infinity Positive, ceil <| Infinity Positive)
  Assert.Equal(Infinity Negative, ceil <| Infinity Negative)


[<Fact>]
let ``Test Ceil``() =
  Assert.Equal(BigRational(2, 1) |> Number, ceil(BigRational(2, 1) |> Number))
  Assert.Equal(BigRational(2, -1) |> Number, ceil(BigRational(2, -1) |> Number))
  Assert.Equal(BigRational(4, 1) |> Number, ceil(BigRational(10, 3) |> Number))
  Assert.Equal(BigRational(3, -1) |> Number, ceil(BigRational(10, -3) |> Number))

[<Fact>]
let ``Test Floor Special``() =
  Assert.Equal(NaN, floor NaN)
  Assert.Equal(Infinity Positive, floor <| Infinity Positive)
  Assert.Equal(Infinity Negative, floor <| Infinity Negative)


[<Fact>]
let ``Test Floor``() =
  Assert.Equal(BigRational(2, 1) |> Number, floor(BigRational(2, 1) |> Number))
  Assert.Equal(BigRational(2, -1) |> Number, floor(BigRational(2, -1) |> Number))
  Assert.Equal(BigRational(3, 1) |> Number, floor(BigRational(10, 3) |> Number))
  Assert.Equal(BigRational(4, -1) |> Number, floor(BigRational(10, -3) |> Number))

[<Fact>]
let ``Test Add Special``() =
  Assert.Equal(NaN, NaN + NaN)
  Assert.Equal(NaN, NaN + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + NaN)
  Assert.Equal(NaN, NaN + (BigRational(1, 1) |> Number))
  Assert.Equal(NaN, (BigRational(1, 1) |> Number) + NaN)

  Assert.Equal(Infinity Positive, Infinity Positive + (BigRational(1, 1) |> Number))
  Assert.Equal(Infinity Positive, (BigRational(1, 1) |> Number) + Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative + (BigRational(1, 1) |> Number))
  Assert.Equal(Infinity Negative, (BigRational(1, 1) |> Number) + Infinity Negative)

  

[<Fact>]
let ``Test Add Zero``() =
//signed zero
  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, PNumber.NegativeZero + PNumber.Zero)
  Assert.Equal(PNumber.Zero, PNumber.Zero + PNumber.NegativeZero)
  Assert.Equal(PNumber.Zero, (BigRational(1, -1) |> Number) + (BigRational(1, 1) |> Number))


  Assert.Equal(2 |> BigRational |> Number, PNumber.Two + PNumber.Zero)
  Assert.Equal(2 |> BigRational |> Number, PNumber.Two + PNumber.NegativeZero)
  Assert.Equal(-2 |> BigRational |> Number, -PNumber.Two + PNumber.Zero)
  Assert.Equal(-2 |> BigRational |> Number, -PNumber.Two + PNumber.NegativeZero)

[<Fact>]
let ``Test Subtract Special``() =
  Assert.Equal(NaN, NaN - NaN)
  Assert.Equal(NaN, NaN - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, Infinity Positive - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - Infinity Negative)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, NaN - Infinity Negative)
  Assert.Equal(NaN, NaN - (BigRational(1, 1) |> Number))
  Assert.Equal(NaN, (BigRational(1, 1) |> Number) - NaN)

  Assert.Equal(Infinity Negative, Infinity Negative - Infinity Positive)
  Assert.Equal(Infinity Positive, Infinity Positive - Infinity Negative)
  Assert.Equal(Infinity Positive, Infinity Positive - (BigRational(1, 1) |> Number))
  Assert.Equal(Infinity Negative, (BigRational(1, 1) |> Number) - Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative - (BigRational(1, 1) |> Number))
  Assert.Equal(Infinity Positive, (BigRational(1, 1) |> Number) - Infinity Negative)

  // IEEE 754 standard has this gem
  Assert.Equal(PNumber.NegativeZero, PNumber.NegativeZero - (-PNumber.NegativeZero))

  Assert.Equal(PNumber.Zero, (BigRational(1, 1) |> Number) - (BigRational(1, 1) |> Number))

[<Fact>]
let ``Test Negate Special``() =
  Assert.Equal(NaN, -NaN)
  Assert.Equal(Infinity Positive, -Infinity Negative)
  Assert.Equal(Infinity Negative, -Infinity Positive)
  Assert.Equal(BigRational(1, -1) |> Number, -(BigRational(1, 1) |> Number))
  Assert.Equal(BigRational(1, 1) |> Number, -(BigRational(1, -1) |> Number))

[<Fact>]
let ``Test Range``() = 
  let expected = seq { 2..100 } |> Seq.map(fun x -> x |> BigRational |> Number)
  let actual = PNumber.Range (2 |> BigRational |> Number) (101 |> BigRational |> Number)

  Assert.Equal(Seq.length expected, Seq.length actual)
  
  Seq.zip expected actual |> Seq.iter(fun pair -> Assert.Equal(fst pair, snd pair))
  



