namespace dpenner1.Primell

open System.Collections.Generic

// Really, an external library probably exists for this, but this is for fun and learning F#

// The public interface takes PNumber and mostly serves to filter to positive integers, 
// private members are bigint and are the real work (and can more easily be copied for more serious use...)
module PPrimeLib =

  let IsPrime x = 
    match x with
    | Rational r when r.IsInteger && r.Sign > 0 -> 
        PrimeLib.IsPrime r.Numerator
    | _ -> false

  let NextPrime x =
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> ExtendedBigRational.Two
    | Rational r when r.Sign < 1 -> ExtendedBigRational.Two
    | Rational r -> BigRational(PrimeLib.NextPrime ((floor r).Numerator), 1) |> Rational

  let PrevPrime x = 
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> NaN
    | Rational r when r.Sign < 1 -> NaN
    | Rational r -> 
        let result = PrimeLib.PrevPrime ((ceil r).Numerator)
        match result with 
        | None -> NaN
        | Some p -> BigRational(p, 1) |> Rational

  let private PrimeConvert(s: ExtendedBigRational seq, length: PNumber option) =
    let result = 
      s |> Seq.filter IsPrime
        |> Seq.map(fun x -> x |> PNumber :> PObject) 
    PList(result, ?length=length)
    
  let PrimeRange left right: PList = 
    match left, right with
    | NaN, _ | _, NaN -> 
        Seq.empty |> PList
    | Infinity Positive, _ -> 
        PList(seq { while true do yield Infinity Positive |> PNumber}, ?length = Some(Infinity Positive |> PNumber))
    | Infinity Negative, Infinity Positive ->
        PrimeConvert(ExtendedBigRational.Range ExtendedBigRational.Two (Infinity Positive), Some(Infinity Positive |> PNumber))
    | Rational _ as left', Infinity Positive -> 
        PrimeConvert(ExtendedBigRational.Range (max ExtendedBigRational.Two left') (Infinity Positive), Some(Infinity Positive |> PNumber))
    | Rational _ as left', Infinity Negative when left' < ExtendedBigRational.Two ->
        Seq.empty |> PList
    | Rational _ as left', Infinity Negative when left' = ExtendedBigRational.Two ->  // TODO can merge this case with below when inclusive range is implemented
        PList(Seq.singleton (ExtendedBigRational.Two |> PNumber :> PObject)) 
    | Rational _ as left', Infinity Negative when left' > ExtendedBigRational.Two ->
        PrimeConvert(ExtendedBigRational.Range left' ExtendedBigRational.Two, Some(ExtendedBigRational.One |> PNumber))
    | Rational _ as left', (Rational _ as right') when left' < ExtendedBigRational.Two && right' <= ExtendedBigRational.Two ->
        Seq.empty |> PList
    | Rational _ as left', (Rational _ as right') when left' > right' ->
        PrimeConvert(ExtendedBigRational.Range left' (max right' ExtendedBigRational.Two), None)
    | Rational _ as left', (Rational _ as right') when left' < right' ->
        PrimeConvert(ExtendedBigRational.Range (max left' ExtendedBigRational.Two) right', None)
    | Rational _ as left', (Rational _ as right') when left' = right' ->
        Seq.empty |> PList
    | _ -> PrimellProgrammerProblemException("shouldn't be possible") |> raise