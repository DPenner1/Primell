namespace dpenner1.PrimellF

// warning about the comparison operators...
// I *think* this is due to the semantics around IEEE754 comparisons vs "normal" IComparable comparisons, 
// but I'm specifcally trying to mimic IEEE754 comparisons, so I'm ok with disabling this
#nowarn "86"

// I switched to F# for typing / functional-based approach better matching what I need for Primell
// but minor irritant here is that I can't override the default parameterless struct constructor here to prevent 0/0
// while C# added that ability in C# 10...
// TODO: generic math?
[<Struct; System.Runtime.CompilerServices.IsReadOnly>]
type BigRational =

  val Numerator: bigint
  val Denominator: bigint
  
  // Couldn't figure out in F# how to avoid calculating gcd twice in constructor, so pass it in pre-calculated
  // (ideally would be reduce: bool) Some methods will pass in 1, knowing the fraction is already reduced
  private new(numerator: bigint, denominator: bigint, gcd: bigint) = 
    { 
      // sign always on denominator (for signed zero purposes)
      Numerator = if numerator.IsZero then 0I else bigint.Abs numerator / gcd
      Denominator = if denominator.IsZero then System.DivideByZeroException() |> raise
                    elif numerator.IsZero then bigint denominator.Sign  // signed zero
                    else denominator / gcd * bigint numerator.Sign
    }
  new(numerator: bigint, denominator: bigint) = BigRational(numerator, denominator, bigint.GreatestCommonDivisor(numerator, denominator))
  new(integer: bigint) = BigRational(integer, 1, 1)

  member this.IsInteger = this.Denominator = 1I || this.Denominator = -1I

  member this.IsZero = this.Numerator.IsZero

  member this.IsOne = this.Numerator.IsOne && this.Denominator.IsOne

  member this.Sign = if this.Numerator.IsZero then 0 else this.Denominator.Sign
  
  static member (~-) (r: BigRational) = BigRational(r.Numerator, -r.Denominator, 1)

  static member Reciprocal (r: BigRational) = BigRational(bigint.Abs r.Denominator, r.Numerator * bigint r.Denominator.Sign, 1)

  static member (+) (left: BigRational, right: BigRational) = // keeping sign on denominator and signed zero really hurts for this operation
    let signAdjust = left.Denominator.Sign * right.Denominator.Sign |> bigint
    let numeratorSum = left.Numerator*right.Denominator*signAdjust + right.Numerator*left.Denominator*signAdjust
    if numeratorSum.IsZero then 
      if left.Denominator.Sign = -1 && right.Denominator.Sign = -1 then BigRational(0I, -1I, 1) else BigRational(0I, 1I, 1) // signed zero
    else 
      BigRational(numeratorSum, left.Denominator * right.Denominator |> bigint.Abs)

  static member (-) (left: BigRational, right: BigRational) = left + (-right)

  static member ( * ) (left: BigRational, right: BigRational) = BigRational(left.Numerator*right.Numerator, left.Denominator*right.Denominator)

  static member ( / ) (left: BigRational, right: BigRational) = left * BigRational.Reciprocal right

  static member Abs (r: BigRational) = BigRational(r.Numerator, bigint.Abs r.Denominator, 1)

  static member Ceiling (r: BigRational) =
    if r.IsInteger then r 
    else
      let q, rem = bigint.DivRem(r.Numerator, r.Denominator)
      if r.Sign = 1 then 
        BigRational(q + 1I, 1, 1)
      else
        BigRational(-q, -1, 1)

  static member Floor (r: BigRational) =
    if r.IsInteger then r 
    else
      let q, rem = bigint.DivRem(r.Numerator, r.Denominator)
      if r.Sign = 1 then 
        BigRational(q, 1, 1)
      else
        BigRational(-q + 1I, -1, 1)

  static member Round (r: BigRational) = 
    let q, rem = bigint.DivRem(r.Numerator, r.Denominator) // In normalized form (sign on denom), rem will always be positive
              
    if rem * 2I > bigint.Abs r.Denominator then 
      BigRational(q + bigint r.Denominator.Sign |> bigint.Abs, bigint r.Denominator.Sign, 1) 
    elif rem * 2I < bigint.Abs r.Denominator then
      BigRational(bigint.Abs q, bigint r.Denominator.Sign, 1)
    else   // 0.5 remainder, round to even
      BigRational(q + (if q.IsEven then 0I else bigint r.Denominator.Sign) |> bigint.Abs, bigint r.Denominator.Sign, 1)

  static member (<) (left: BigRational, right: BigRational) = 
    if left.Sign < right.Sign then true
    elif left.Sign > right.Sign then false
    elif left.Sign = 0 then false  // signed zero compares equal
    else left.Numerator * right.Denominator < right.Numerator * left.Denominator

  static member (>) (left: BigRational, right: BigRational) = 
    if left.Sign > right.Sign then true
    elif left.Sign < right.Sign then false
    elif left.Sign = 0 then false  // signed zero compares equal
    else left.Numerator * right.Denominator > right.Numerator * left.Denominator

  static member (=) (left: BigRational, right: BigRational) = 
    (left.Numerator = right.Numerator && left.Denominator = right.Denominator) || (left.IsZero && right.IsZero)

  static member (<=) (left: BigRational, right: BigRational) = left = right || left < right

  static member (>=) (left: BigRational, right: BigRational) = left = right || left > right

  static member (<>) (left: BigRational, right: BigRational) = left = right |> not

  // way more general than I currently need for Primell, but fun to figure out
  static member Range (left: BigRational, right: BigRational, ?step: BigRational, ?leftInclusive: bool, ?rightInclusive: bool) =
    let stepValue = defaultArg step <| BigRational(1, 1)
    let includeLeft = defaultArg leftInclusive true
    let includeRight = defaultArg rightInclusive false
    let leftOrEmpty condition = 
      if condition then Seq.singleton left else Seq.empty
      
    // some of the edge cases here are judgment calls
    if left = right then  // based on Python [x..x] returning empty, return empty unless both are inclusive
      leftOrEmpty(includeLeft && includeRight)
    elif stepValue.IsZero then
      seq { while true do yield left } // right-of-way
    else 
      let diff = right - left
      if diff.Sign <> stepValue.Sign then
        leftOrEmpty includeLeft
      else
        let numSteps = diff / stepValue
        let wholeSteps, fractionalSteps = bigint.DivRem(numSteps.Numerator, numSteps.Denominator)
        
        if wholeSteps.IsZero then 
          leftOrEmpty includeLeft
        else
          let startIndex = if includeLeft then 0I else 1I
          let endIndex = wholeSteps - startIndex - (if includeRight && fractionalSteps = 0I then 0I else 1I) 
          
          seq { startIndex..endIndex } |> Seq.map(fun i -> left + stepValue * BigRational(i, 1))

  override this.ToString() =
    let str1 = if this.Denominator.Sign = -1 then "-" else ""
    let str2 = this.Numerator.ToString()
    let str3 = if this.IsInteger then "" else "/" + bigint.Abs(this.Denominator).ToString()
    
    String.concat "" [str1; str2; str3] 