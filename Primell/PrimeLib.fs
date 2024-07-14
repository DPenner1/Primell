namespace dpenner1.PrimellF

open System.Collections.Generic

// Really, an external library probably exists for this, but this is for fun and learning F#

// The public interface takes PNumber and mostly serves to filter to positive integers, 
// private members are bigint and are the real work (and can more easily be copied for more serious use...)
module PrimeLib =

  // memoization
  let private primes = new HashSet<bigint>()

  let private smallPrimes = [2I;3I;5I;7I;11I;13I;17I;19I;23I;29I;31I;37I;41I;43I;47I;53I;59I;61I;67I;71I;73I;79I;83I;89I;97I]
  smallPrimes |> List.iter (fun x -> primes.Add x |> ignore) // List instead of set, as order means efficiently checking mod 2 first
 
  // a little hacky, needed an integer coming out of sLoop function
  type private IntermedateResult = 
    | Undetermined of bigint
    | Composite
  
  
  let rec private FactorPowersOfTwo(n: bigint, power: bigint) = 
    if n.IsEven then FactorPowersOfTwo(n/2I, power+1I)
    else (n, power)

  // Variables match those from https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test
  let private MillerRabinRound(n: bigint, a: bigint, s: bigint, d: bigint) =

    let rec sLoop(s': bigint, x: bigint) =
      // would have used match if pattern match allowed bitint 0I literal; I think if-else slightly more readable in this case
      if s'.IsZero then  
        Undetermined x
      else
        let y = bigint.ModPow(x, 2, n)
        if y.IsOne && x <> 1I && x <> n - 1I then 
          Composite
        else 
          sLoop(s' - 1I, y)

    match sLoop(s, bigint.ModPow(a, d, n)) with
    | Undetermined y when y.IsOne -> Undetermined y // number doesn't matter anymore
    | _ -> Composite

  // Note: this is the deterministic (assuming ERH) Miller test, not the more common probabilitic Miller-Rabin
  // Because I plan on going to BPSW, I don't want to put in the effort to code the random trials required for Miller-Rabin
  // https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test#Miller_test
  let private Miller n =
    let limit = 2.0 * (bigint.Log n ** 2 |> ceil)
    (* note, this is a bit different than Wikipedia's limit of min(n - 2, floor( 2ln(n)^2 ))
       based on Wolfram Alpha, n-2 is greater when n >= 20, so I can ignore this as I'm not calling it with n < 20
       Then note the limit here is of type float, but it won't overflow until given a bigint around 10^10^153. 
       But because its float, I'm using ceil instead of floor, to be safe in case of rounding error in the float. 
       Miller is temporary until BPSW is implemented, so I'm OK with the slight rough edges here *)

    let d, s = FactorPowersOfTwo(n - 1I, 0I) // we aren't calling Miller with even numbers

    seq { 2I..(bigint limit) }
    |> Seq.exists (fun a -> MillerRabinRound(n, a, s, d) = Composite)
    |> not

  // Miller test for now to get things going, plan on going to BPSW
  let private IsPrime' n =
      if primes.Contains n   // we've already seen this prime
        then true
      elif List.exists (fun p -> (n % p).IsZero) smallPrimes  // checking small divisors is more efficient
        then false
      else Miller n

  let IsPrime x = 
    match x with
    | Rational r when r.IsInteger && r.Numerator > 1I -> 
        let isPrime = IsPrime' r.Numerator
        if isPrime then primes.Add r.Numerator |> ignore // memoize
        isPrime      
    | _ -> false

  let rec private NextPrime' n =
    let nextInt = n + 1I
    if IsPrime' nextInt then nextInt else NextPrime' nextInt

  let NextPrime x =
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> ExtendedBigRational.Two
    | Rational r when r.Sign < 1 -> ExtendedBigRational.Two
    | Rational r -> BigRational(NextPrime' ((floor r).Numerator), 1) |> Rational

  let rec private PrevPrime' n =
    let prevInt = n - 1I
    if prevInt < 2I then failwith "No primes less than 2, silly" 
    elif IsPrime' prevInt then prevInt 
    else PrevPrime' prevInt

  let PrevPrime x = 
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> NaN
    | Rational r when r <= BigRational(2, 1) -> NaN
    | Rational r -> BigRational(PrevPrime' ((ceil r).Numerator), 1) |> Rational

  let PrimeRange left right: seq<ExtendedBigRational> = 
    match left, right with
    | NaN, _ | _, NaN -> 
        Seq.empty
    | Infinity Positive, _ -> 
        seq { while true do yield Infinity Positive }
    | Infinity Negative, Infinity Positive ->
        ExtendedBigRational.Range ExtendedBigRational.Two (Infinity Positive) |> Seq.filter IsPrime
    | Rational _ as left', Infinity Positive -> 
        ExtendedBigRational.Range (max ExtendedBigRational.Two left') (Infinity Positive) |> Seq.filter IsPrime
    | Rational _ as left', Infinity Negative when left' < ExtendedBigRational.Two ->
        Seq.empty
    | Rational _ as left', Infinity Negative when left' = ExtendedBigRational.Two ->
        Seq.singleton ExtendedBigRational.Two  // TODO can get rid of this case when inclusive range is implemented
    | Rational _ as left', Infinity Negative when left' > ExtendedBigRational.Two ->
        ExtendedBigRational.Range left' ExtendedBigRational.Two |> Seq.filter IsPrime
    | Rational _ as left', (Rational _ as right') when left' < ExtendedBigRational.Two && right' <= ExtendedBigRational.Two ->
        Seq.empty
    | Rational _ as left', (Rational _ as right') when left' > right' ->
        ExtendedBigRational.Range left' (max right' ExtendedBigRational.Two) |> Seq.filter IsPrime
    | Rational _ as left', (Rational _ as right') when left' < right' ->
        ExtendedBigRational.Range (max left' ExtendedBigRational.Two) right' |> Seq.filter IsPrime
    | Rational _ as left', (Rational _ as right') when left' = right' ->
        Seq.empty
    | _ -> failwith "shouldn't be possible"