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
      // would have used match if pattern match allowed 0I, I think if-else slightly more readable in this case
      if s'.IsZero then  
        Undetermined(x)
      else
        let y = bigint.ModPow(x, 2, n)
        if y.IsOne && x <> 1I && x <> n - 1I then 
          Composite
        else 
          sLoop(s' - 1I, y)

    match sLoop(s, bigint.ModPow(a, d, n)) with
    | Composite -> Composite
    | Undetermined y when y.IsOne -> Undetermined y // number doesn't matter anymore
    | _ -> Composite

  // Note: this is the deterministic (assuming ERH) Miller test, not the more common probabilitic Miller-Rabin
  // Because I plan on going to BPSW, I don't want to put in the effort to code the random trials required for Miller-Rabin
  // https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test#Miller_test
  let private Miller n =
    let limit = 2.0 * bigint.Log n ** 2 |> ceil
    (* note, this is a bit different than Wikipedia's limit of min(n - 2, floor( 2ln(n)^2 ))
       based on Wolfram Alpha, n-2 is greater when n >= 20, so I can ignore this as I'm not calling it with n < 20
       Then note the limit here is of type Double, but it won't overflow until given a bigint around 10^10^153. 
       But because its Double, I'm using ceil instead of floor, to be safe in case of rounding error in the Double. 
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
    | Rational(n, d) as r when n > 1I && d.IsOne -> 
        let isPrime = IsPrime' n
        if isPrime then primes.Add n |> ignore // memoize
        isPrime      
    | _ -> false

  let rec private NextPrime' x =
    let nextInt = x + 1I
    if IsPrime' nextInt then nextInt else NextPrime' nextInt

  let NextPrime x =
    match x with
    | NaN -> NaN
    | Infinity Positive -> Infinity Positive
    | Infinity Negative -> PNumber.Two
    | Rational(_,_) as r when r.Sign.IsMinusOne -> PNumber.Two
    | Rational(n,d) as r ->   // the PNumber type is really awkward here 
                              // (I don't want to expose a Numerator:bigint as that has no meaning for NaN/Infinity)
        match PNumber.Floor(r) with
        | Rational(n', d') -> Rational(NextPrime' n', d')
        | _ -> failwith "Shouldn't be possible"
