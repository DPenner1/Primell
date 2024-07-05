namespace dpenner1.PrimellF

// warning about overloading the comparison operators...
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

// I switched to F# for typing / functional-based approach better matching what I need for Primell
// but minor irritant here is that I can't override the default parameterless struct constructor here to prevent 0/0
// while C# added that ability in C# 10...
// TODO: generic math?
[<Struct; IsReadOnly>]
type R =

  val Numerator: bigint
  val Denominator: bigint
  
  // Couldn't figure out in F# how to avoid calculating gcd twice in constructor, so pass it in pre-calculated
  // (ideally would be reduce: bool) Some methods will pass in 1, knowing the fraction is already reduced
  private new(numerator: bigint, denominator: bigint, gcd: bigint) = 
    { 
      // sign always on denominator (for signed zero purposes)
      Numerator = if numerator.IsZero then 0I else bigint.Abs numerator / gcd
      Denominator = if denominator.IsZero then failwith "Divide by zero!" 
                    elif numerator.IsZero then bigint denominator.Sign  // signed zero
                    else denominator / gcd * bigint numerator.Sign
      // TODO use .NET DivideByZeroException
    }
  new(numerator: bigint, denominator: bigint) = R(numerator, denominator, bigint.GreatestCommonDivisor(numerator, denominator))

  member this.IsInteger = this.Denominator = 1I || this.Denominator = -1I

  member this.IsZero = this.Numerator.IsZero

  member this.IsOne = this.Numerator.IsOne && this.Denominator.IsOne

  member this.Sign = this.Denominator.Sign
  
  //if this.Numerator.IsZero then this else R(1I, bigint this.Denominator.Sign, 1)   // negative zero possible
  
  static member (~-) (r: R) = R(r.Numerator, -r.Denominator, 1)

  static member Reciprocal (r: R) = R(bigint.Abs r.Denominator, r.Numerator * bigint r.Denominator.Sign, 1)

  static member (+) (left: R, right: R) = // keeping sign on denominator and signed zero really hurts for this operation
    let signAdjust = bigint (left.Sign * right.Sign)
    let numeratorSum = left.Numerator*right.Denominator*signAdjust + right.Numerator*left.Denominator*signAdjust
    if numeratorSum.IsZero then 
      if left.Denominator.Sign = -1 && right.Denominator.Sign = -1 then R(0I, -1I, 1) else R(0I, 1I, 1) // signed zero
    else 
      R(numeratorSum, left.Denominator * bigint.Abs right.Denominator)

  static member (-) (left: R, right: R) = left + (-right)

  static member ( * ) (left: R, right: R) = R(left.Numerator*right.Numerator, left.Denominator*right.Denominator)

  static member ( / ) (left: R, right: R) = left * R.Reciprocal(right)

  static member Abs (r: R) = R(r.Numerator, bigint.Abs r.Denominator, 1)

  static member Ceiling (r: R) =
    if r.IsInteger then r 
    else
      let q, rem = bigint.DivRem(r.Numerator, r.Denominator)
      if r.Sign = 1 then 
        R(q + 1I, 1, 1)
      else
        R(-q, -1, 1)

  static member Floor (r: R) =
    if r.IsInteger then r 
    else
      let q, rem = bigint.DivRem(r.Numerator, r.Denominator)
      if r.Sign = 1 then 
        R(q, 1, 1)
      else
        R(-q + 1I, -1, 1)

  static member Round (r: R) = 
    let q, rem = bigint.DivRem(r.Numerator, r.Denominator) // In normalized form (sign on denom), rem will always be positive
              
    if rem * 2I > bigint.Abs r.Denominator then 
      R(q + bigint r.Denominator.Sign |> bigint.Abs, bigint r.Denominator.Sign, 1) 
    elif rem * 2I < bigint.Abs r.Denominator then
      R(bigint.Abs q, bigint r.Denominator.Sign, 1)
    else   // 0.5 remainder, round to even
      R(q + (if q.IsEven then 0I else bigint r.Denominator.Sign) |> bigint.Abs, bigint r.Denominator.Sign, 1)

  static member (<) (left: R, right: R) = 
    if left.Sign < right.Sign then true
    elif left.Sign > right.Sign then false
    elif left.Sign = 0 then false  // signed zero compares equal
    else left.Numerator * right.Denominator < right.Numerator * left.Denominator

  static member (>) (left: R, right: R) = 
    if left.Sign > right.Sign then true
    elif left.Sign < right.Sign then false
    elif left.Sign = 0 then false  // signed zero compares equal
    else left.Numerator * right.Denominator > right.Numerator * left.Denominator

  static member (=) (left: R, right: R) = 
    (left.Numerator = right.Numerator && left.Denominator = right.Denominator) || (left.IsZero && right.IsZero)

  static member (<=) (left: R, right: R) = left = right || left < right

  static member (>=) (left: R, right: R) = left = right || left > right

  static member (<>) (left: R, right: R) = left = right |> not

  override this.ToString() =
    let str1 = if this.Denominator.Sign = -1 then "-" else ""
    let str2 = this.Numerator.ToString()
    let str3 = if this.IsInteger then "" else "/" + bigint.Abs(this.Denominator).ToString()
    
    String.concat "" [str1; str2; str3] 
                      

  
// In terms of Infinity, NaN, signed zero, this type follows IEEE-754 wherever possible
// If IEEE754 is silent, try to mimic BigInteger API
[<Struct; IsReadOnly>]
type PNumber = 
  | Rational of R: R          // not naming R type gave compiler error 3204. 
                              // Originally went for "of bigint * bigint", but couldn't reduce the fraction on construction,
  | Infinity of Infinity      // Benefit: Pattern matching was actually cumbersome here. Downside: construction is now Rational <| R(n, d)
  | NaN
  with override this.ToString() =
         match this with 
         | NaN -> "NaN"
         | Infinity x -> x.ToString()
         | Rational r -> r.ToString()
                                                                
        static member One = Rational <| R(1I, 1I)
        static member MinusOne = Rational <| R(1I, -1I)
        static member Zero = Rational <| R(0I, 1I)
        static member NegativeZero = Rational <| R(0I, -1I)
        static member Two = Rational <| R(2I, 1I)  // lowest Prime
        
        
        member this.IsZero =
          match this with
          | Rational _ -> this = PNumber.Zero
          | _ -> false

        member this.IsOne =
          match this with
          | Rational _ -> this = PNumber.One
          | _ -> false

        member this.IsMinusOne =
          match this with
          | Rational _ -> this = PNumber.MinusOne
          | _ -> false

        member this.IsInteger =
          match this with
          | Rational r -> r.IsInteger
          | _ -> false

        member this.Sign =
          match this with
          | NaN -> NaN  // .Net double.Sign crashes, I would like not to crash
          | Infinity Negative -> PNumber.MinusOne
          | Infinity Positive -> PNumber.One
          | Rational r -> Rational <| R(1, bigint r.Sign)

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
          | Rational left', Rational right' -> Rational (left' + right')

        static member (-) (left: PNumber, right: PNumber): PNumber = left + (-right)

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
          | Rational left', Rational right' -> Rational <| left' * right'
          | _ -> failwith "Shouldn't be possible"

        static member Reciprocal x =
          match x with
          | NaN -> NaN
          | Infinity Positive -> PNumber.Zero
          | Infinity Negative -> PNumber.NegativeZero
          | Rational r when r.IsZero && r.Denominator.Sign = -1 -> Infinity Negative 
          | Rational r when r.IsZero && r.Denominator.Sign = 1 -> Infinity Positive
          | Rational r -> Rational <| R.Reciprocal r

        static member (/) (left, right) = left * PNumber.Reciprocal right

        static member Abs x = 
          match x with
          | NaN -> NaN
          | Infinity _ -> Infinity Positive
          | Rational r -> Rational <| R.Abs r

        

         // TODO: compiler warnings on < > operators 
   
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

        static member (<=) (left: PNumber, right: PNumber) = left = right || left < right

        static member (>=) (left: PNumber, right: PNumber) = left = right || left > right

        static member (<>) (left: PNumber, right: PNumber) = left = right |> not  // NaN <> NaN is true

        static member Ceiling x =
          match x with
          | Rational r -> Rational <| ceil r
          | _ -> x

        static member Floor x = 
          match x with
          | Rational r -> Rational <| floor r
          | _ -> x

        static member Round x =
          match x with
          | Rational r -> Rational <| round r
          | _ -> x
              
        
        static member Range left right =
          failwith "not implemented"

