module PNumberTests

open Xunit
open dpenner1.Primell

[<Fact>]
let ``Test NaN Equality``() =
  Assert.False((NaN |> PNumber).NaNAwareEquals(NaN |> PNumber))
  Assert.False((NaN |> PNumber).NaNAwareEquals(Infinity Positive |> PNumber))
  Assert.False((Infinity Negative |> PNumber).NaNAwareEquals(NaN |> PNumber))
  Assert.False((Infinity Negative |> PNumber).NaNAwareEquals(Infinity Positive |> PNumber))
  Assert.True((Infinity Positive |> PNumber).NaNAwareEquals(Infinity Positive |> PNumber))

[<Fact>]
let ``Test Regular Equality``() =  // make sure the framework Equals is respected (so NaN = NaN)
  Assert.True((NaN |> PNumber).Equals(NaN |> PNumber))
  Assert.False((NaN |> PNumber).Equals(Infinity Positive |> PNumber))
  Assert.False((Infinity Negative |> PNumber).Equals(NaN |> PNumber))
  Assert.False((Infinity Negative |> PNumber).Equals(Infinity Positive |> PNumber))
  Assert.True((Infinity Positive |> PNumber).Equals(Infinity Positive |> PNumber))