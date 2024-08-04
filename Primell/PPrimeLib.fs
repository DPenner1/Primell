namespace dpenner1.Primell

// adapt the more Primell-neutral PrimeLib to PNumber
module PPrimeLib =

  let IsPrime (x: PNumber) = 
    match x.Value with
    | Rational r when r.IsInteger && r.Sign > 0 -> 
        PrimeLib.IsPrime r.Numerator
    | _ -> false

  let NextPrime (x: PNumber) =
    match x.Value with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> ExtendedBigRational.Two
    | Rational r when r.Sign < 1 -> ExtendedBigRational.Two
    | Rational r -> BigRational(PrimeLib.NextPrime ((floor r).Numerator), 1) |> Rational
    |> PNumber

  let PrevPrime (x: PNumber) = 
    match x.Value with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> NaN
    | Rational r when r.Sign < 1 -> NaN
    | Rational r -> 
        let result = PrimeLib.PrevPrime ((ceil r).Numerator)
        match result with 
        | None -> NaN
        | Some p -> BigRational(p, 1) |> Rational
    |> PNumber

  let NearestPrime (x: PNumber) =
    match x.Value with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> ExtendedBigRational.Two
    | Rational _ as r -> 
        if IsPrime x then r
        else
          let prevPrime = PrevPrime x
          let nextPrime = NextPrime x
          // no real idea what to do in case of ties here
          if (r - prevPrime.Value) < (nextPrime.Value - r) 
            then prevPrime.Value 
          else nextPrime.Value
    |> PNumber


  let private PrimeConvert(s: ExtendedBigRational seq, length: PNumber option) =
    let result = 
      s |> Seq.filter(fun x -> IsPrime (x |> PNumber))
        |> Seq.map(fun x -> x |> PNumber :> PObject) 
    PList(result, ?length=length)
    
  let PrimeRange left right: PList = 
    match left, right with
    | NaN, _ | _, NaN -> 
        Seq.empty |> PList
    | Infinity Positive, _ -> 
        PList(seq { while true do yield Infinity Positive |> PNumber}, ?length = Some(Infinity Positive |> PNumber))
    | Infinity Negative, Infinity Positive ->
        PrimeConvert(ExtendedBigRational.Range(ExtendedBigRational.Two, Infinity Positive), Some(Infinity Positive |> PNumber))
    | Rational _ as left', Infinity Positive -> 
        PrimeConvert(ExtendedBigRational.Range(max ExtendedBigRational.Two left', Infinity Positive), Some(Infinity Positive |> PNumber))
    | Rational _ as left', Infinity Negative when left' < ExtendedBigRational.Two ->
        Seq.empty |> PList
    | Rational _ as left', Infinity Negative when left' = ExtendedBigRational.Two ->  // TODO can merge this case with below when inclusive range is implemented
        PList(Seq.singleton (ExtendedBigRational.Two |> PNumber :> PObject)) 
    | Rational _ as left', Infinity Negative when left' > ExtendedBigRational.Two ->
        PrimeConvert(ExtendedBigRational.Range(left', ExtendedBigRational.Two), Some(ExtendedBigRational.One |> PNumber))
    | Rational _ as left', (Rational _ as right') when left' < ExtendedBigRational.Two && right' <= ExtendedBigRational.Two ->
        Seq.empty |> PList
    | Rational _ as left', (Rational _ as right') when left' > right' ->
        PrimeConvert(ExtendedBigRational.Range(left', max right' ExtendedBigRational.Two), None)
    | Rational _ as left', (Rational _ as right') when left' < right' ->
        PrimeConvert(ExtendedBigRational.Range(max left' ExtendedBigRational.Two, right'), None)
    | Rational _ as left', (Rational _ as right') when left' = right' ->
        Seq.empty |> PList
    | _ -> PrimellProgrammerProblemException("shouldn't be possible") |> raise

  
  let PrimeFactorization (x: PNumber) =
    match x.Value with
    | NaN -> PList.Empty
    | Infinity Positive -> Seq.initInfinite(fun _ -> ExtendedBigRational.Two |> PNumber :> PObject) |> PList :> PObject
           // for infinity, there's an infinite number of factors, but you won't get past all those 2s!
    | Infinity Negative -> Seq.append [ExtendedBigRational.MinusOne |> PNumber :> PObject] 
                                      (Seq.initInfinite(fun _ -> ExtendedBigRational.Two |> PNumber :> PObject)) |> PList :> PObject
    | Rational r ->
        let denominatorPrimes = // since PrimeLib gives ordered primes, we need to reverse the denom ones (and deal with -1)
          PrimeLib.PrimeFactors(bigint.Abs r.Denominator) 
          |> Seq.rev
          |> Seq.map(fun p -> p |> BigRational |> BigRational.Reciprocal |> Rational |> PNumber :> PObject)
        let signSeq = if r.Sign = -1 then Seq.singleton (ExtendedBigRational.MinusOne |> PNumber :> PObject) else Seq.empty
        let numeratorPrimes = PrimeLib.PrimeFactors(r.Numerator) |> Seq.map (fun p -> p |> BigRational |> Rational |> PNumber :> PObject)
        
        numeratorPrimes |> Seq.append denominatorPrimes |> Seq.append signSeq |> PList :> PObject
    
    