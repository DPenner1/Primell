module PNumberTests

open Xunit
open dpenner1.Primell
open dpenner1.Math

[<Fact>]
let ``Test NaN Equality``() =
  Assert.False((NaN |> PNumber |> Atom |> PObject).NaNAwareEquals(NaN |> PNumber |> Atom |> PObject))
  Assert.False((NaN |> PNumber |> Atom |> PObject).NaNAwareEquals(Infinity Positive |> PNumber |> Atom |> PObject))
  Assert.False((Infinity Negative |> PNumber |> Atom |> PObject).NaNAwareEquals(NaN |> PNumber |> Atom |> PObject))
  Assert.False((Infinity Negative |> PNumber |> Atom |> PObject).NaNAwareEquals(Infinity Positive |> PNumber |> Atom |> PObject))
  Assert.True((Infinity Positive |> PNumber |> Atom |> PObject).NaNAwareEquals(Infinity Positive |> PNumber |> Atom |> PObject))

[<Fact>]
let ``Test Regular Equality``() =  // make sure the framework Equals is respected (so NaN = NaN)
  Assert.True(NaN.Equals NaN)
  Assert.False(NaN.Equals(Infinity Positive))
  Assert.False((Infinity Negative).Equals NaN)
  Assert.False((Infinity Negative).Equals(Infinity Positive))
  Assert.True((Infinity Positive).Equals(Infinity Positive))