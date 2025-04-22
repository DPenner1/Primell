module PNumberTests

open Xunit
open dpenner1.Primell
open dpenner1.Math

[<Fact>]
let ``Test NaN Equality``() =
  Assert.False((NaN |> Number |> PObject).NaNAwareEquals(NaN |> Number |> PObject))
  Assert.False((NaN |> Number |> PObject).NaNAwareEquals(Infinity Positive |> Number |> PObject))
  Assert.False((Infinity Negative |> Number |> PObject).NaNAwareEquals(NaN |> Number |> PObject))
  Assert.False((Infinity Negative |> Number |> PObject).NaNAwareEquals(Infinity Positive |> Number |> PObject))
  Assert.True((Infinity Positive |> Number |> PObject).NaNAwareEquals(Infinity Positive |> Number |> PObject))

[<Fact>]
let ``Test Regular Equality``() =  // make sure the framework Equals is respected (so NaN = NaN)
  Assert.True(NaN.Equals NaN)
  Assert.False(NaN.Equals(Infinity Positive))
  Assert.False((Infinity Negative).Equals NaN)
  Assert.False((Infinity Negative).Equals(Infinity Positive))
  Assert.True((Infinity Positive).Equals(Infinity Positive))