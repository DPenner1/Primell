// note: Tests not entirely comprehensive
module PListTests

open Xunit
open dpenner1.Primell

[<Fact>]
let ``Test Index Special``() =
    let testList = seq {ExtendedBigRational.Zero |> PNumber :> PObject; ExtendedBigRational.One |> PNumber :> PObject; ExtendedBigRational.Two |> PNumber :> PObject} |> PList
    
    Assert.Equal(PList.Empty :> PObject, testList.Index(NaN |> PNumber))    
    Assert.Equal(PList.Empty :> PObject, testList.Index(Infinity Positive |> PNumber))
    Assert.Equal(PList.Empty :> PObject, testList.Index(Infinity Negative |> PNumber))

(* Issues with equality comparison
[<Fact>]
let ``Test Index``() =
    let testList = seq {ExtendedBigRational.Zero |> PNumber :> PObject; ExtendedBigRational.One |> PNumber :> PObject; ExtendedBigRational.Two |> PNumber :> PObject} |> PList
    
    Assert.Equal(ExtendedBigRational.Zero |> PNumber :> PObject, testList.Index(0 |> BigRational |> Rational |> PNumber))    
    Assert.Equal(ExtendedBigRational.One |> PNumber :> PObject, testList.Index(1 |> BigRational |> Rational |> PNumber))    
    Assert.Equal(ExtendedBigRational.Two |> PNumber :> PObject, testList.Index(2 |> BigRational |> Rational |> PNumber)) 
    Assert.Equal(PList.Empty :> PObject, testList.Index(3 |> BigRational |> Rational |> PNumber))

    Assert.Equal(ExtendedBigRational.Two |> PNumber :> PObject, testList.Index(-1 |> BigRational |> Rational |> PNumber))    
    Assert.Equal(ExtendedBigRational.One |> PNumber :> PObject, testList.Index(-2 |> BigRational |> Rational |> PNumber))    
    Assert.Equal(ExtendedBigRational.Zero |> PNumber :> PObject, testList.Index(-3 |> BigRational |> Rational |> PNumber)) 
    Assert.Equal(PList.Empty :> PObject, testList.Index(-4 |> BigRational |> Rational |> PNumber)) 

*)