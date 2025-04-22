namespace dpenner1.Primell

open dpenner1.Math
// adapt the more Primell-neutral PrimeLib to PNumber
module PPrimeLib =

  let IsPrime (x: ExtendedBigRational) = 
    match x with
    | Rational r when r.IsInteger && r.Sign > 0 -> 
        PrimeLib.IsPrime r.Numerator
    | _ -> false

  let NextPrime (x: ExtendedBigRational) =
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> ExtendedBigRational.Two
    | Rational r when r.Sign < 1 -> ExtendedBigRational.Two
    | Rational r -> BigRational(PrimeLib.NextPrime ((floor r).Numerator), 1) |> Rational

  let PrevPrime (x: ExtendedBigRational) = 
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

  let NearestPrime (x: ExtendedBigRational) =
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> ExtendedBigRational.Two
    | Rational _ as r -> 
        if IsPrime x then r
        else
          let prevPrime = PrevPrime x
          let nextPrime = NextPrime x
          // no real idea what to do in case of ties here
          if (r - prevPrime) < (nextPrime - r) 
            then prevPrime 
          else nextPrime

  let private PrimeConvert(s: ExtendedBigRational seq, length: ExtendedBigRational option) =
    let result = 
      s |> Seq.filter(fun x -> IsPrime x)
        |> Seq.map(fun x -> x |> Number |> PObject) 
    PList(result, ?length=length) |> PObject.FromSeq
    
  let PrimeRange left right = 
    match left, right with
    | NaN, _ | _, NaN -> 
        PObject.Empty
    | Infinity Positive, _ -> 
        PObject.Infinite(Infinity Positive |> Number |> PObject)
    | Infinity Negative, Infinity Positive ->
        PrimeConvert(ExtendedBigRational.Range(ExtendedBigRational.Two, Infinity Positive), Some(Infinity Positive))
    | Rational _ as left', Infinity Positive -> 
        PrimeConvert(ExtendedBigRational.Range(max ExtendedBigRational.Two left', Infinity Positive), Some(Infinity Positive))
    | Rational _ as left', Infinity Negative when left' < ExtendedBigRational.Two ->
        PObject.Empty
    | Rational _ as left', Infinity Negative when left' = ExtendedBigRational.Two ->  // TODO can merge this case with below when inclusive range is implemented
        ExtendedBigRational.Two |> Number |> PObject 
    | Rational _ as left', Infinity Negative when left' > ExtendedBigRational.Two ->
        PrimeConvert(ExtendedBigRational.Range(left', ExtendedBigRational.Two), Some ExtendedBigRational.One)
    | Rational _ as left', (Rational _ as right') when left' < ExtendedBigRational.Two && right' <= ExtendedBigRational.Two ->
        PObject.Empty
    | Rational _ as left', (Rational _ as right') when left' > right' ->
        PrimeConvert(ExtendedBigRational.Range(left', max right' ExtendedBigRational.Two), None)
    | Rational _ as left', (Rational _ as right') when left' < right' ->
        PrimeConvert(ExtendedBigRational.Range(max left' ExtendedBigRational.Two, right'), None)
    | Rational _ as left', (Rational _ as right') when left' = right' ->
        PObject.Empty
    | _ -> PrimellProgrammerProblemException("shouldn't be possible") |> raise

  
  let PrimeFactorization (x: ExtendedBigRational) =
    match x with
    | NaN -> PObject.Empty
    | Infinity Positive -> PObject.Infinite(NaN |> Number |> PObject)
           // for infinity, there's an infinite number of factors, but you won't get past all those 2s!
    | Infinity Negative -> (Seq.singleton (ExtendedBigRational.MinusOne |> Number |> PObject) |> PList).AppendAll (PObject.Infinite(NaN |> Number |> PObject)) 
                           |> PList |> Sequence |> PObject
    | Rational r ->
        let denominatorPrimes = // since PrimeLib gives ordered primes, we need to reverse the denom ones (and deal with -1)
          PrimeLib.PrimeFactors(bigint.Abs r.Denominator) 
          |> Seq.rev
          |> Seq.map(fun p -> p |> BigRational |> BigRational.Reciprocal |> Rational |> Number |> PObject)
        let signSeq = if r.Sign = -1 then Seq.singleton (ExtendedBigRational.MinusOne |> Number |> PObject) else Seq.empty
        let numeratorPrimes = PrimeLib.PrimeFactors(r.Numerator) |> Seq.map (fun p -> p |> BigRational |> Rational |> Number |> PObject)
        
        numeratorPrimes |> Seq.append denominatorPrimes |> Seq.append signSeq |> PList |> Sequence |> PObject
    
    