namespace dpenner1.PrimellF

// warning about the comparison operators...
// I *think* this is due to the semantics around IEEE754 comparisons vs "normal" comparisons, 
// but I'm specifcally trying to mimic IEEE754 comparisons, so I'm ok with disabling this
#nowarn "86"

open System.Runtime.CompilerServices

[<Struct; IsReadOnly>]
type Infinity =
  | Positive
  | Negative
  with override this.ToString() = 
         match this with
         | Positive -> "∞"
         | Negative -> "-∞"

// In terms of Infinity, NaN, signed zero, this type follows IEEE-754 wherever possible
// If IEEE754 is silent, try to mimic BigInteger or Double APIs
[<Struct; IsReadOnly>]
type ExtendedBigRational = 
  | Rational of rational: BigRational 
  | Infinity of Infinity
  | NaN

   (* Notes on Rational:
        - Needed to give a unused name Number case to avoid 3204 compiler error: https://stackoverflow.com/q/59738472/1607043
        - Originally went for "of bigint * bigint", but couldn't reduce the fraction on construction,
          Benefit: Pattern matching was actually cumbersome here. Downside: construction is cumbersome, BigRational(n, d) |> Number
        - In theory, could later genericize the base type of Rational (eg. Real, though Complex would probably be two of these)
   *)


  with override this.ToString() =
         match this with 
         | NaN -> "NaN"
         | Infinity x -> x.ToString()
         | Rational r -> r.ToString()
                                                                
        static member One = BigRational(1I, 1I) |> Rational
        static member MinusOne = BigRational(1I, -1I) |> Rational
        static member Zero = BigRational(0I, 1I) |> Rational
        static member NegativeZero = BigRational(0I, -1I) |> Rational
        static member Two = BigRational(2I, 1I) |> Rational  // lowest Prime
        
        
        member this.IsZero =
          match this with
          | Rational _ -> this = ExtendedBigRational.Zero
          | _ -> false

        member this.IsOne =
          match this with
          | Rational _ -> this = ExtendedBigRational.One
          | _ -> false

        member this.IsMinusOne =
          match this with
          | Rational _ -> this = ExtendedBigRational.MinusOne
          | _ -> false

        member this.IsInteger =
          match this with
          | Rational r -> r.IsInteger
          | _ -> false

        member this.Sign =
          match this with
          | NaN -> NaN  // .Net double.Sign crashes, I would like not to crash
          | Infinity Negative -> ExtendedBigRational.MinusOne
          | Infinity Positive -> ExtendedBigRational.One
          | Rational r -> BigRational(1, bigint r.Sign) |> Rational

        static member (~-) x =
          match x with
          | NaN -> NaN
          | Infinity Positive -> Infinity Negative
          | Infinity Negative -> Infinity Positive
          | Rational r -> Rational -r

        static member (+) (left, right) =
          match left, right with
          | NaN, _ | _, NaN -> NaN
          | Infinity Positive, Infinity Negative | Infinity Negative, Infinity Positive -> NaN
          | Infinity _ as left', _ -> left'
          | _, (Infinity _ as right') -> right'
          | Rational left', Rational right' -> left' + right' |> Rational

        static member (-) (left: ExtendedBigRational, right: ExtendedBigRational): ExtendedBigRational = left + (-right)

        // TODO
        //static member (~++)
        static member ( * ) (left, right) =
          match left, right with
          | NaN, _ | _, NaN -> NaN
          | Infinity _, Rational r | Rational r, Infinity _ when r.IsZero -> NaN
          | Infinity Positive, _ when right.Sign.IsMinusOne -> Infinity Negative
          | Infinity Positive, _ when right.Sign.IsOne-> Infinity Positive
          | Infinity Negative, _ when right.Sign.IsMinusOne -> Infinity Positive
          | Infinity Negative, _ when right.Sign.IsOne -> Infinity Negative
          | Rational left', Rational right' -> left' * right' |> Rational
          | _ -> failwith "Shouldn't be possible"

        static member Reciprocal x =
          match x with
          | NaN -> NaN
          | Infinity Positive -> ExtendedBigRational.Zero
          | Infinity Negative -> ExtendedBigRational.NegativeZero
          | Rational r when r.IsZero && r.Denominator.Sign = -1 -> Infinity Negative 
          | Rational r when r.IsZero && r.Denominator.Sign = 1 -> Infinity Positive
          | Rational r -> BigRational.Reciprocal r |> Rational

        static member (/) (left, right) = left * ExtendedBigRational.Reciprocal right

        static member Abs x = 
          match x with
          | NaN -> NaN
          | Infinity _ -> Infinity Positive
          | Rational r -> BigRational.Abs r |> Rational
   
        static member (<) (left, right) =
          match left, right with
          | NaN, _ | _, NaN -> false
          | Infinity Negative, Infinity Negative -> false
          | Infinity Negative, _ -> true
          | Infinity Positive, _ -> false
          | _, Infinity Positive -> true
          | _, Infinity Negative -> false
          | Rational left', Rational right' -> left' < right'
          
        static member (>) (left, right) =
          match left, right with
          | NaN, _ | _, NaN -> false
          | Infinity Positive, Infinity Positive -> false
          | Infinity Negative, _ -> false
          | Infinity Positive, _ -> true
          | _, Infinity Positive -> false
          | _, Infinity Negative -> true
          | Rational left', Rational right' -> left' > right'

        static member (=) (left, right) =
          match left, right with
          | NaN, NaN -> false   // the weird case!
          | Infinity Positive, Infinity Positive -> true
          | Infinity Negative, Infinity Negative -> true
          | Rational left', Rational right' -> left' = right'
          | _ -> false

        static member (<=) (left: ExtendedBigRational, right: ExtendedBigRational) = left = right || left < right

        static member (>=) (left: ExtendedBigRational, right: ExtendedBigRational) = left = right || left > right

        static member (<>) (left: ExtendedBigRational, right: ExtendedBigRational) = left = right |> not  // NaN <> NaN is true

        static member Ceiling x =
          match x with
          | Rational r -> ceil r |> Rational
          | _ -> x

        static member Floor x = 
          match x with
          | Rational r -> floor r |> Rational
          | _ -> x

        static member Round x =
          match x with
          | Rational r -> round r |> Rational
          | _ -> x

        // behaviour of NaN is weird with min/max. I think its correct to return the number.
        // with Max/Min you're asking which between a number and NaN which one is the max/min number, which can't be NaN by definition
        static member Max left right =
          if left > right then left else right

        static member Min left right =
          if left < right then left else right
        
        static member Range left right: seq<ExtendedBigRational> =
          match left, right with
          | NaN, _ | _, NaN -> 
              Seq.empty
          | Infinity Positive, _ -> 
              seq { while true do yield Infinity Positive }
          | Infinity Negative, _ -> 
              seq { while true do yield Infinity Negative }
          | Rational left', Infinity Positive -> 
              Seq.initInfinite(fun i -> ceil left' + BigRational(i,1) |> Rational)
          | Rational left', Infinity Negative -> 
              Seq.initInfinite(fun i -> ceil left' - BigRational(i,1) |> Rational)
          | Rational left', Rational right' ->  // Is the cast more functional? In C# i'd go with a covariant (contra?) generic
              BigRational.Range(left', right', BigRational(1, 1), true, false) |> Seq.map(fun r -> r |> Rational)

// TODO - if we generate a range from +inf..1 it infinitely returns +inf...
        //        but if we reverse that range, then we would actually want it to start generating from 1
        //        so, you'll effectively need to return a "ReversibleRange" object with two possible ranges
        //        or we leave it to the caller to call both Range left right and Range right left
        //        also a Length property that doesn't hang on infinite sequences would be good...
//and ReversibleInfiniteSeq<'T> =
//  inherit seq<'T>