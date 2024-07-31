namespace dpenner1.Primell

open System.Collections.Generic

// Really, an external library probably exists for this, but this was for fun and learning F#

module PrimeLib =

  // memoization
  let private primes = HashSet<bigint>()
  let private nonPrimes = HashSet<bigint>()

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
  let IsPrime n =
      if n < 2I
        then false
      elif primes.Contains n   // we've already seen this prime
        then true
      elif nonPrimes.Contains n
        then false
      elif List.exists (fun p -> (n % p).IsZero) smallPrimes  // checking small divisors is more efficient
        then nonPrimes.Add n |> ignore; false
      else 
        let result = Miller n
        if result then primes.Add n |> ignore
        else nonPrimes.Add n |> ignore
        result

  let rec NextPrime n =
    let nextInt = n + 1I
    if IsPrime nextInt then nextInt else NextPrime nextInt

  let rec PrevPrime n =
    let prevInt = n - 1I
    if prevInt < 2I 
      then Some n
    elif IsPrime prevInt 
      then Some prevInt 
    else 
      PrevPrime prevInt