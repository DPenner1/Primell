// note: Tests not entirely comprehensive
module ExtendedBigRationalTests

open Xunit
open dpenner1.PrimellF

[<Fact>]
let ``Test Round Special``() =
    Assert.Equal(NaN, round NaN)    
    Assert.Equal(Infinity Positive, round <| Infinity Positive)
    Assert.Equal(Infinity Negative, round <| Infinity Negative)

    let r1 = BigRational(1, 2) |> Rational
    let r2 = BigRational(1, -2) |> Rational

    //signed zero
    Assert.Equal(BigRational(0, 1) |> Rational, round r1)
    Assert.Equal(BigRational(0, -1) |> Rational, round r2)

[<Fact>]
let ``Test Round Thirds``() =
    let r1 = BigRational(11, 3) |> Rational
    let r2 = BigRational(11, -3) |> Rational
    let r3 = BigRational(10, 3) |> Rational
    let r4 = BigRational(10, -3) |> Rational

    Assert.Equal(BigRational(4, 1) |> Rational, round r1)    
    Assert.Equal(BigRational(4, -1) |> Rational, round r2)
    Assert.Equal(BigRational(3, 1) |> Rational, round r3)
    Assert.Equal(BigRational(3, -1) |> Rational, round r4)

[<Fact>]
let ``Test Round To Even``() =
    let r1 = BigRational(9, 2) |> Rational
    let r2 = BigRational(9, -2) |> Rational
    let r3 = BigRational(7, 2) |> Rational
    let r4 = BigRational(7, -2) |> Rational

    Assert.Equal(BigRational(4, 1) |> Rational, round r1)
    Assert.Equal(BigRational(4, -1) |> Rational, round r2)
    Assert.Equal(BigRational(4, 1) |> Rational, round r3)
    Assert.Equal(BigRational(4, -1) |> Rational, round r4)

[<Fact>]
let ``Test Round Near Zero``() =
    let r1 = BigRational(2, 3) |> Rational
    let r2 = BigRational(2, -3) |> Rational
    let r3 = BigRational(1, 3) |> Rational
    let r4 = BigRational(1, -3) |> Rational

    Assert.Equal(BigRational(1, 1) |> Rational, round r1)
    Assert.Equal(BigRational(1, -1) |> Rational, round r2)
    Assert.Equal(BigRational(0, 1) |> Rational, round r3)
    Assert.Equal(BigRational(0, -1) |> Rational, round r4)

[<Fact>]
let ``Test Ceil Special``() =
  Assert.Equal(NaN, ceil NaN)
  Assert.Equal(Infinity Positive, ceil <| Infinity Positive)
  Assert.Equal(Infinity Negative, ceil <| Infinity Negative)


[<Fact>]
let ``Test Ceil``() =
  Assert.Equal(BigRational(2, 1) |> Rational, ceil(BigRational(2, 1) |> Rational))
  Assert.Equal(BigRational(2, -1) |> Rational, ceil(BigRational(2, -1) |> Rational))
  Assert.Equal(BigRational(4, 1) |> Rational, ceil(BigRational(10, 3) |> Rational))
  Assert.Equal(BigRational(3, -1) |> Rational, ceil(BigRational(10, -3) |> Rational))

[<Fact>]
let ``Test Floor Special``() =
  Assert.Equal(NaN, floor NaN)
  Assert.Equal(Infinity Positive, floor <| Infinity Positive)
  Assert.Equal(Infinity Negative, floor <| Infinity Negative)


[<Fact>]
let ``Test Floor``() =
  Assert.Equal(BigRational(2, 1) |> Rational, floor(BigRational(2, 1) |> Rational))
  Assert.Equal(BigRational(2, -1) |> Rational, floor(BigRational(2, -1) |> Rational))
  Assert.Equal(BigRational(3, 1) |> Rational, floor(BigRational(10, 3) |> Rational))
  Assert.Equal(BigRational(4, -1) |> Rational, floor(BigRational(10, -3) |> Rational))

[<Fact>]
let ``Test Add Special``() =
  Assert.Equal(NaN, NaN + NaN)
  Assert.Equal(NaN, NaN + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + Infinity Positive)
  Assert.Equal(NaN, Infinity Negative + NaN)
  Assert.Equal(NaN, NaN + (BigRational(1, 1) |> Rational))
  Assert.Equal(NaN, (BigRational(1, 1) |> Rational) + NaN)

  Assert.Equal(Infinity Positive, Infinity Positive + (BigRational(1, 1) |> Rational))
  Assert.Equal(Infinity Positive, (BigRational(1, 1) |> Rational) + Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative + (BigRational(1, 1) |> Rational))
  Assert.Equal(Infinity Negative, (BigRational(1, 1) |> Rational) + Infinity Negative)

  

[<Fact>]
let ``Test Add Zero``() =
//signed zero
  Assert.Equal(ExtendedBigRational.NegativeZero, ExtendedBigRational.NegativeZero + ExtendedBigRational.NegativeZero)
  Assert.Equal(ExtendedBigRational.Zero, ExtendedBigRational.NegativeZero + ExtendedBigRational.Zero)
  Assert.Equal(ExtendedBigRational.Zero, ExtendedBigRational.Zero + ExtendedBigRational.NegativeZero)
  Assert.Equal(ExtendedBigRational.Zero, (BigRational(1, -1) |> Rational) + (BigRational(1, 1) |> Rational))


  Assert.Equal(2 |> BigRational |> Rational, ExtendedBigRational.Two + ExtendedBigRational.Zero)
  Assert.Equal(2 |> BigRational |> Rational, ExtendedBigRational.Two + ExtendedBigRational.NegativeZero)
  Assert.Equal(-2 |> BigRational |> Rational, -ExtendedBigRational.Two + ExtendedBigRational.Zero)
  Assert.Equal(-2 |> BigRational |> Rational, -ExtendedBigRational.Two + ExtendedBigRational.NegativeZero)

[<Fact>]
let ``Test Subtract Special``() =
  Assert.Equal(NaN, NaN - NaN)
  Assert.Equal(NaN, NaN - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, Infinity Positive - Infinity Positive)
  Assert.Equal(NaN, Infinity Negative - Infinity Negative)
  Assert.Equal(NaN, Infinity Negative - NaN)
  Assert.Equal(NaN, NaN - Infinity Negative)
  Assert.Equal(NaN, NaN - (BigRational(1, 1) |> Rational))
  Assert.Equal(NaN, (BigRational(1, 1) |> Rational) - NaN)

  Assert.Equal(Infinity Negative, Infinity Negative - Infinity Positive)
  Assert.Equal(Infinity Positive, Infinity Positive - Infinity Negative)
  Assert.Equal(Infinity Positive, Infinity Positive - (BigRational(1, 1) |> Rational))
  Assert.Equal(Infinity Negative, (BigRational(1, 1) |> Rational) - Infinity Positive)
  Assert.Equal(Infinity Negative, Infinity Negative - (BigRational(1, 1) |> Rational))
  Assert.Equal(Infinity Positive, (BigRational(1, 1) |> Rational) - Infinity Negative)

  // IEEE 754 standard has this gem
  Assert.Equal(ExtendedBigRational.NegativeZero, ExtendedBigRational.NegativeZero - (-ExtendedBigRational.NegativeZero))

  Assert.Equal(ExtendedBigRational.Zero, (BigRational(1, 1) |> Rational) - (BigRational(1, 1) |> Rational))

[<Fact>]
let ``Test Negate Special``() =
  Assert.Equal(NaN, -NaN)
  Assert.Equal(Infinity Positive, -Infinity Negative)
  Assert.Equal(Infinity Negative, -Infinity Positive)
  Assert.Equal(BigRational(1, -1) |> Rational, -(BigRational(1, 1) |> Rational))
  Assert.Equal(BigRational(1, 1) |> Rational, -(BigRational(1, -1) |> Rational))

[<Fact>]
let ``Test Range``() = 
  let expected = seq { 2..100 } |> Seq.map(fun x -> x |> BigRational |> Rational)
  let actual = ExtendedBigRational.Range (2 |> BigRational |> Rational) (101 |> BigRational |> Rational)

  Assert.Equal(Seq.length expected, Seq.length actual)
  
  Seq.zip expected actual |> Seq.iter(fun pair -> Assert.Equal(fst pair, snd pair))
  



