// note: Tests not entirely comprehensive
module PListTests

open Xunit
open dpenner1.Primell
open dpenner1.Math

[<Fact>]
let ``Test Index Special``() =
    let testList = seq {ExtendedBigRational.Zero |> Number |> PObject; ExtendedBigRational.One |> Number |> PObject; ExtendedBigRational.Two |> Number |> PObject} |> PList
    
    Assert.Equal(PObject.Empty, testList.Index NaN)    
    Assert.Equal(PObject.Empty, testList.Index(Infinity Positive))
    Assert.Equal(PObject.Empty, testList.Index(Infinity Negative))

[<Fact>]
let ``Test Index``() =
    let testList = seq {ExtendedBigRational.Zero |> Number |> PObject; ExtendedBigRational.One |> Number |> PObject; ExtendedBigRational.Two |> Number |> PObject} |> PList
    
    Assert.Equal(ExtendedBigRational.Zero |> Number |> PObject, testList.Index(0 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.One |> Number |> PObject, testList.Index(1 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.Two |> Number |> PObject, testList.Index(2 |> BigRational |> Rational)) 
    Assert.Equal(PObject.Empty, testList.Index(3 |> BigRational |> Rational))

    Assert.Equal(ExtendedBigRational.Two |> Number |> PObject, testList.Index(-1 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.One |> Number |> PObject, testList.Index(-2 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.Zero |> Number |> PObject, testList.Index(-3 |> BigRational |> Rational)) 
    Assert.Equal(ExtendedBigRational.Two |> Number |> PObject, testList.Index(-4 |> BigRational |> Rational)) 


[<Fact>]
let ``Test AllIndexesOf``() =
  let testList = seq {ExtendedBigRational.Zero |> Number |> PObject
                      ExtendedBigRational.One |> Number |> PObject
                      ExtendedBigRational.Two |> Number |> PObject
                      NaN |> Number |> PObject
                      ExtendedBigRational.One |> Number |> PObject
                     } |> PList

  Assert.Equal(PObject.Empty, testList.AllIndexesOf(NaN |> Number |> PObject))
  Assert.Equal(seq { 1 |> BigRational |> Rational |> Number |> PObject; 4 |> BigRational |> Rational |> Number |> PObject } |> PObject.FromSeq, 
               testList.AllIndexesOf(ExtendedBigRational.One |> Number |> PObject))