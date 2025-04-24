// note: Tests not entirely comprehensive
module PListTests

open Xunit
open dpenner1.Primell
open dpenner1.Math

[<Fact>]
let ``Test Index Special``() =
    let testList = seq {ExtendedBigRational.Zero |> PNumber |> Atom |> PObject; ExtendedBigRational.One |> PNumber |> Atom |> PObject; ExtendedBigRational.Two |> PNumber |> Atom |> PObject} |> PList
    
    Assert.Equal(PObject.Empty, testList.Index NaN)    
    Assert.Equal(PObject.Empty, testList.Index(Infinity Positive))
    Assert.Equal(PObject.Empty, testList.Index(Infinity Negative))

[<Fact>]
let ``Test Index``() =
    let testList = seq {ExtendedBigRational.Zero |> PNumber |> Atom |> PObject; ExtendedBigRational.One |> PNumber |> Atom |> PObject; ExtendedBigRational.Two |> PNumber |> Atom |> PObject} |> PList
    
    Assert.Equal(ExtendedBigRational.Zero |> PNumber |> Atom |> PObject, testList.Index(0 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.One |> PNumber |> Atom |> PObject, testList.Index(1 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.Two |> PNumber |> Atom |> PObject, testList.Index(2 |> BigRational |> Rational)) 
    Assert.Equal(PObject.Empty, testList.Index(3 |> BigRational |> Rational))

    Assert.Equal(ExtendedBigRational.Two |> PNumber |> Atom |> PObject, testList.Index(-1 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.One |> PNumber |> Atom |> PObject, testList.Index(-2 |> BigRational |> Rational))    
    Assert.Equal(ExtendedBigRational.Zero |> PNumber |> Atom |> PObject, testList.Index(-3 |> BigRational |> Rational)) 
    Assert.Equal(ExtendedBigRational.Two |> PNumber |> Atom |> PObject, testList.Index(-4 |> BigRational |> Rational)) 


[<Fact>]
let ``Test AllIndexesOf``() =
  let testList = seq {ExtendedBigRational.Zero |> PNumber |> Atom |> PObject
                      ExtendedBigRational.One |> PNumber |> Atom |> PObject
                      ExtendedBigRational.Two |> PNumber |> Atom |> PObject
                      NaN |> PNumber |> Atom |> PObject
                      ExtendedBigRational.One |> PNumber |> Atom |> PObject
                     } |> PList

  Assert.Equal(PObject.Empty, testList.AllIndexesOf(NaN |> PNumber |> Atom |> PObject))
  Assert.Equal(seq { 1 |> BigRational |> Rational |> PNumber |> Atom |> PObject; 4 |> BigRational |> Rational |> PNumber |> Atom |> PObject } |> PObject.FromSeq, 
               testList.AllIndexesOf(ExtendedBigRational.One |> PNumber |> Atom |> PObject))