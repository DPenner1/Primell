namespace dpenner1.Math
// Note: this was designed to work independently of Primell, so different namespace

open System.Collections.Generic

type private LucasParameters = { U: bigint; V: bigint; Q: bigint; k: bigint }  // k not explicitly needed, but helps with debugging

// Really, an external library probably exists for this, but this was for fun and learning F#
module PrimeLib =

  // memoization
  let private primes = HashSet<bigint>()
  // a bit hacky, but we're simultaneously memoizing whether a number is composite, and when computed their prime factors here
  let private compositeFactors = Dictionary<bigint, bigint seq option>()

  // List instead of set, as order means efficiently checking mod 2 first
  let private smallPrimes = [2I;3I;5I;7I;11I;13I;17I;19I;23I;29I;31I;37I;41I;43I;47I;53I;59I;61I;67I;71I;73I;79I;83I;89I;97I]
  smallPrimes |> List.iter (fun x -> primes.Add x |> ignore) 
 
  let private quadraticResiduesMod256 = set [0uy;1uy;4uy;9uy;16uy;17uy;25uy;33uy;36uy;41uy;49uy;57uy;64uy;
                                             65uy;68uy;73uy;81uy;89uy;97uy;100uy;105uy;113uy;121uy;
                                             129uy;132uy;137uy;144uy;145uy;153uy;161uy;164uy;169uy;177uy;185uy;
                                             193uy;196uy;201uy;209uy;217uy;225uy;228uy;233uy;241uy;249uy]
                                             
  // a little hacky, but the algos often need an integer result coming out of possible prime state
  type private IntermedateResult = 
    | Undetermined of bigint
    | Composite
  
  let private FactorPowersOfTwo(n: bigint) = 
    let rec factor(n: bigint, power:bigint) = 
      if n.IsEven then factor(n/2I, power+1I)
      else (n, power)   // returns (odd d, power), where n = d * 2^power
    
    factor(n, 0I)

  // Variables match those from https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test
  let private MillerRabinRound(n: bigint, a: bigint, s: bigint, d: bigint) =

    let rec sLoop(s': bigint, x: bigint) =
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
  // I used this before I coded BPSW and didn't want to put in the effort to code the random trials required for Miller-Rabin
  // https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test#Miller_test
  let private Miller n =
    let limit = 2.0 * (bigint.Log n ** 2 |> ceil)
    (* note, this is a bit different than Wikipedia's limit of min(n - 2, floor( 2ln(n)^2 ))
       based on Wolfram Alpha, n-2 is greater when n >= 20, so I can ignore this as I'm not calling it with n < 20
       Then note the limit here is of type float, but it won't overflow until given a bigint around 10^10^153. 
       But because its float, I'm using ceil instead of floor, to be safe in case of rounding error in the float. 
       Miller was temporary until BPSW is implemented, so I'm OK with the slight rough edges here *)

    let d, s = FactorPowersOfTwo(n - 1I) // we aren't calling Miller with even numbers

    seq { 2I..(bigint limit) }
    |> Seq.exists (fun a -> MillerRabinRound(n, a, s, d) = Composite)
    |> not
  
  // just for cleaner usage (we can't do an IsComposite and call IsPrime, that's method ping pong)
  let private IsKnownComposite n =
    compositeFactors.ContainsKey n

  // basic algorithm from https://en.wikipedia.org/wiki/Jacobi_symbol but rearranged to fit functional immutable style
  // in particular, noticed that t was basically just a boolean toggle until the output, so I implemented it as such
  // Note: since Jacobi is logarithmic time, not going to memoize (as memory would be quadratic, a*n)
  let JacobiSymbol(a: bigint, n:bigint) =
    let rec reduceA(a': bigint, n': bigint, t:bool) =
      if a'.IsZero then (a', n', t)
      else
        let d, s = FactorPowersOfTwo a'
        let newT =  // looks like n' % 8 is computed twice, but compiler will probably optimize since n' is immutable
          t <> ((not s.IsEven) && (n' % 8I = 3I || n' % 8I = 5I)) <> (d % 4I = 3I && n' % 4I = 3I)

        reduceA(n' % d, d, newT)

    if n.IsEven || n.Sign = -1 then
      System.ArgumentException "invalid Jacobi args" |> raise

    let modulo = a % n 
    let a' = (if modulo.Sign = -1 then modulo + n else modulo) // dealing with negative modulo...
    let (_, n', t) = reduceA(a', n, true)
    
    if n'.IsOne then 
      if t then 1 else -1 
    else 0

  // https://math.stackexchange.com/a/41355/60690
  let private IsSquareWithSeed(n: bigint, initialGuess: bigint) =
    // lastLastGuess is my silly way of detecting 2-period oscillation...based on my reading of the MSE Q&A, 
    // it shouldn't be possible to have more than 2-period oscillation... i hope
    let rec newton(n: bigint, guess: bigint, lastGuess: bigint, lastLastGuess: bigint) =
      if guess = lastLastGuess then false  // oscillation (which per MSE comments means n+1 is square)
      else 
        let square = bigint.Pow(guess, 2)
        if square = n then true
        elif guess = lastGuess then false  // steady state
        else newton(n, (square + n)/(2I * guess), guess, lastGuess)

    newton(n, initialGuess, -1I, -2I)

  // technically not really a PrimeLib function, but it's either that or BigRational in this project, neither is a perfect fit
  let IsSquare(n: bigint) =
    if quadraticResiduesMod256 |> Set.contains (byte(n % 256I)) |> not then false  // screen cases of definitely not squares before newton's method
    else
      // https://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Binary_estimates
      // basically just computing the 2^n portion (maybe with off-by-one error), without fussing with the a value (because that's annoying)
      let seedGuess = 1I <<< (int (n.GetBitLength() >>> 1)) 
      IsSquareWithSeed(n, seedGuess)

  // https://en.wikipedia.org/wiki/Lucas_pseudoprime
  // also  https://arxiv.org/pdf/2006.14425  section 2.4
  let StrongLucasTest(n: bigint, p:bigint, q: bigint, jacobiD: bigint) =
    // helper functions for computing Lucas sequences
    let getUVQkDouble(n: bigint, lucasParams: LucasParameters) =
      { U = lucasParams.U * lucasParams.V % n
        V = (bigint.Pow(lucasParams.V, 2) - 2I * lucasParams.Q) % n
        Q = bigint.ModPow(lucasParams.Q, 2, n)
        k = lucasParams.k * 2I
      }
    let getUVQkPlus1(n:bigint, lucasParams: LucasParameters, jacobiD: bigint, lucasP: bigint, lucasQ: bigint) =
      { U = let temp = lucasP * lucasParams.U + lucasParams.V
            (if temp.IsEven then temp else temp + n) / 2I % n
        V = let temp = jacobiD * lucasParams.U + lucasP * lucasParams.V
            let temp2 = (if temp.IsEven then temp else temp + n) / 2I % n  
            if temp2.Sign = -1 then temp2 + n else temp2  //negative modulo again...
        Q = lucasParams.Q * lucasQ % n
        k = lucasParams.k + 1I
      }
    let rec checkVZeroCongruence(n: bigint, lucasParams: LucasParameters, sCounter: bigint) =
      if lucasParams.V.IsZero then true
      elif sCounter.IsZero then false // (can't be true as we just checked for V = 0)
      else checkVZeroCongruence(n, getUVQkDouble(n, lucasParams), sCounter - 1I)

    // main Lucas test code
    let d, s = FactorPowersOfTwo(n + 1I)  // s is at least one, as n is odd (and if not, you really messed up the calling code)
    let bitSeq = System.Collections.BitArray(d.ToByteArray(true, false)) 
                   |> Seq.cast 
                   |> Seq.rev 
                   |> Seq.skipWhile (fun x -> not x)  // remove leading zeroes
                   |> Seq.skip 1  // skip the initial leading bit which is handled specially through UVQk initialization
    
    let lucasParams = ({ U = 1I; V = p; Q = q; k = 1I }, bitSeq) ||> Seq.fold(fun lparams bit ->
      let lucasDouble = getUVQkDouble(n, lparams) 
      if bit then getUVQkPlus1(n, lucasDouble, jacobiD, p, q) 
      else lucasDouble)

    // we're now done the d bits (s bits are all 0)
    if lucasParams.U.IsZero then true
    else checkVZeroCongruence(n, lucasParams, s - 1I) // one less than s because congruence is up to V_(d * 2^(s - 1))

    // the paper has 2 new follow-up tests added to the original BPSW to further strengthen the primality test
    // I'm not implementing that because (a) that's work (b) it would actually be cool to stumble upon a BPSW counter-example

  // basically following allong this paper: https://arxiv.org/pdf/2006.14425
  let BpswTest(n: bigint) = 
    let rec getJacobiD(n: bigint, d: bigint) =
      match JacobiSymbol(d, n) with
      | -1 -> 
          Undetermined d
      | 0 when (bigint.Abs d < n) || (d % n <> 0I) ->  // 0 allows for possible early abort 
          Composite
      | _ -> 
          getJacobiD(n, -(d + bigint (d.Sign * 2)))

    let d, s = FactorPowersOfTwo(n - 1I)
    match MillerRabinRound(n, 2I, s, d) with
    | Composite -> false
    | _ -> // carry on my wayward son
        // paper actually suggests trying Jacobi step a few times first, but my test cases have checking for squares upfront executing faster 
        // (yes, the paper is by smarter ppl who would know whats asymptotically faster, but I'm interested in what's practically faster for my use case)
        // I can't figure out why it's faster in my case, but im guessing it's branch prediction gone wrong with pausing Jacobi search after some iterations
        if IsSquare n then false  
        else
          match getJacobiD(n, 5I) with
          | Undetermined d when d = 5I -> StrongLucasTest(n, d, d, d)
          | Undetermined d -> StrongLucasTest(n, 1I, (1I - d)/4I, d)
          | Composite -> false
    
  let IsPrime n =
      if n < 2I
        then false
      elif primes.Contains n   // we've already seen this prime
        then true
      elif IsKnownComposite n
        then false
      elif List.exists (fun p -> (n % p).IsZero) smallPrimes  // checking small divisors is more efficient
        then compositeFactors.Add(n, None) |> ignore; false
      else 
        let result = BpswTest n
        if result then primes.Add n |> ignore
        else compositeFactors.Add(n, None) |> ignore
        result

  let rec NextPrime n =
    let nextInt = n + 1I
    if IsPrime nextInt then nextInt else NextPrime nextInt

  let rec PrevPrime n =
    let prevInt = n - 1I
    if prevInt < 2I 
      then None
    elif IsPrime prevInt 
      then Some prevInt 
    else 
      PrevPrime prevInt

  let rec private PrimeFactors'' n primeDivisor factors =
    let q, rem = bigint.DivRem(n, primeDivisor)
    if rem.IsZero then
      PrimeFactors'' q primeDivisor (Seq.append factors [primeDivisor])
    elif n.IsOne then
      factors
    else
      PrimeFactors'' n (NextPrime primeDivisor) factors

  let rec private PrimeFactors' (n: bigint) =
    if IsKnownComposite n then  // check if known to be composite, before potentially expensive primality test
      match compositeFactors[n] with
      | Some x -> x // memoized already
      | None ->
          compositeFactors[n] <- Some (PrimeFactors'' n 2I [])
          compositeFactors[n].Value
    elif IsPrime n then  // primes and composites are separately memoized
      Seq.singleton n
    else // try again (IsPrime will now have identified whether its composite or not)
      PrimeFactors' n
  
  let rec PrimeFactors (n: bigint) = 
    if n.Sign = -1 then  // i guess since bigint allows negative we'll handle it here
      Seq.append (Seq.singleton -1I) (PrimeFactors -n)
    elif n.IsZero then
      2I |> Seq.unfold(fun x -> Some (x, NextPrime x))  // all primes divide zero
    elif n.IsOne then  // i think this trashes the main algo if not handled separately
      Seq.empty
    else
      PrimeFactors' n

