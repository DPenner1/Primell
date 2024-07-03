namespace dpenner1.PrimellF

[<Struct>]
type Infinity =
  | Positive
  | Negative
  with override this.ToString() = 
         match this with
         | Positive -> "∞"
         | Negative -> "-∞"

// In terms of Infinity, NaN, signed zero, this type follows IEEE-754 wherever possible
// If IEEE754 is silent, try to mimic BigInteger API
[<Struct>]
type PNumber = 
  | Rational of bigint * bigint  // Sign on denominator so that positive/negative zero can work
  | Infinity of Infinity         // thought of having Sign as a discriminated union, but I think that would be annoying
  | NaN
  with override this.ToString() =
         match this with 
         | NaN -> "NaN"
         | Infinity x -> x.ToString()
         | Rational(n, d) -> String.concat "" [if d.Sign = -1 then "-" else ""; 
                                               n.ToString(); 
                                               if d = 1I || d = -1I then "" else "/" + d.ToString()]
                                                                
        static member One = Rational(1I, 1I)
        static member MinusOne = Rational(1I, -1I)
        static member Zero = Rational(0I, 1I)
        static member NegativeZero = Rational(0I, -1I)
        static member Two = Rational(2I, 1I)  // lowest Prime
        
        
        member this.IsZero =
          match this with
          | Rational(n, d) -> n.IsZero
          | _ -> false

        member this.IsOne =
          match this with
          | Rational(n, d) -> n.IsOne && d.IsOne
          | _ -> false

        member this.IsMinusOne =
          match this with
          | Rational(n, d) ->  n.IsOne && (-d).IsOne
          | _ -> false

        member this.IsInteger =
          match this with
          | Rational(_, d) -> d = 1I || d = -1I
          | _ -> false

        // ideally, would call this on construction, though it appears thats not possible in F# discriminated unions
        static member private Normalize x =
          match x with
          | Rational(n, d) when n.IsZero && d.IsZero -> NaN
          | Rational(n, d) when n.IsZero && d.Sign = 1 -> Rational(bigint.Zero, bigint.One)
          | Rational(n, d) when n.IsZero && d.Sign = -1 -> Rational(bigint.Zero, bigint.MinusOne)
          | Rational(n, d) when d.IsZero && n.Sign = 1 -> Infinity Positive
          | Rational(n, d) when d.IsZero && n.Sign = -1 -> Infinity Negative
          | Rational(n, d) -> let gcd = bigint.GreatestCommonDivisor(n, d)
                              Rational(bigint.Abs n/gcd, d / gcd * bigint n.Sign)
          | _ -> x        

        member this.Sign =
          match this with
          | NaN -> NaN  // .Net double.Sign crashes, I would like not to crash
          | Infinity Negative -> PNumber.MinusOne
          | Infinity Positive -> PNumber.One
          | Rational(n, d) when n.IsZero && d.Sign = 1  -> PNumber.Zero
          | Rational(n, d) when n.IsZero && d.Sign = -1 -> PNumber.NegativeZero  // this may trip people up and that's amazing (until I get hit with it)
          | Rational(n, d) -> Rational(bigint.One, bigint d.Sign)

        static member (~-) x =
          match x with
          | NaN -> NaN
          | Infinity Positive -> Infinity Negative
          | Infinity Negative -> Infinity Positive
          | Rational(n, d) -> Rational(n, -d)

        // TODO - see if you can figure out an F# way to not care about matching order
        static member (+) (x, y) =
          match x, y with
          | NaN, _ | _, NaN -> NaN
          | Infinity Positive, Infinity Negative | Infinity Negative, Infinity Positive -> NaN
          | Infinity _ as x', _ -> x'
          | _, (Infinity _ as y') -> y'
          | Rational(n1, d1), Rational(n2, d2) -> 
              let r = PNumber.Normalize <| Rational(n1*d2 + n2*d1, d1*d2) 
              if r.IsZero then // fun IEEE-754 compliance
                if d1.Sign = -1 && d2.Sign = -1 then PNumber.NegativeZero else PNumber.Zero
              else 
                r  

        static member (-) (x, y) = x + (-y)

        // TODO
        //static member (~++)

        // TODO - see if you can figure out an F# way to not care about matching order
        static member ( * ) (x, y) =
          match x, y with
          | NaN, _ | _, NaN -> NaN
          | Infinity _, Rational(n, d) when n.IsZero -> NaN
          | Rational(n, d), Infinity _ when n.IsZero -> NaN
          | Infinity Positive, _ when y.Sign.IsMinusOne -> Infinity Negative
          | Infinity Positive, _ when y.Sign.IsOne-> Infinity Positive
          | Infinity Negative, _ when y.Sign.IsMinusOne -> Infinity Positive
          | Infinity Negative, _ when y.Sign.IsOne -> Infinity Negative
          | Rational(n1, d1), Rational(n2, d2) -> Rational(n1*n2, d1*d2) |> PNumber.Normalize
          | _ -> failwith "Shouldn't be possible"

        static member Reciprocal x =
          match x with
          | NaN -> NaN
          | Infinity Positive -> PNumber.Zero
          | Infinity Negative -> PNumber.NegativeZero
          | Rational(n, d) when n.IsZero && d.Sign = 1 -> Infinity Positive
          | Rational(n, d) when n.IsZero && d.Sign = -1 -> Infinity Negative
          | Rational(n, d) when d.Sign = 1 -> Rational(d, n)
          | Rational(n, d) when d.Sign = -1 -> Rational(-d, -n)
          | _ -> failwith "Shouldn't be possible"

        static member (/) (x, y) = x * PNumber.Reciprocal y

        static member Abs x = 
          match x with
          | NaN -> NaN
          | Infinity _ -> Infinity Positive
          | Rational(n, d) -> Rational(n, bigint.Abs d)

        

         // TODO: compiler warnings on < > operators 

        static member (<) (x, y) =
          match x, y with
          | NaN, _ | _, NaN -> false
          | Infinity Negative, Infinity Negative -> false
          | Infinity Negative, _ -> true
          | Infinity Positive, _ -> false
          | _, Infinity Positive -> true
          | _, Infinity Negative -> false
          | Rational(n1, d1), Rational(n2, d2) -> (y - x).Sign.IsOne

        static member (<=) (x, y) =
          match x, y with
          | NaN, _ | _, NaN -> false
          | _, Infinity Positive -> true
          | Infinity Negative, _ -> true
          | Infinity Positive, _ -> false          
          | _, Infinity Negative -> false
          | Rational(n1, d1), Rational(n2, d2) -> (y - x).Sign.IsOne || (y - x).Sign.IsZero
          
        static member (>) (x, y) =
          match x, y with
          | NaN, _ | _, NaN -> false
          | Infinity Positive, Infinity Positive -> false
          | Infinity Negative, _ -> false
          | Infinity Positive, _ -> true
          | _, Infinity Positive -> false
          | _, Infinity Negative -> true
          | Rational(n1, d1), Rational(n2, d2) -> (x - y).Sign.IsOne

        // TODO verify this for negative numbers
        static member Ceiling x =
          match x with
          | Rational(_,_) as r when r.IsInteger -> r
          | Rational(n, d) as r -> 
              let q, rem = bigint.DivRem(n, d)
              if r.Sign.IsMinusOne then Rational(bigint.Abs(q), bigint q.Sign)
              else Rational(q + bigint.One, bigint.One)
          | _ -> x

        static member Floor x = PNumber.Ceiling x - PNumber.One

        static member Round x =
          match x with
          | NaN -> NaN
          | Infinity s -> Infinity s
          | Rational(_,_) as r when r.IsInteger -> r 
          | Rational(n, d) -> 
              let q, rem = bigint.DivRem(n, d) // In normalized form (sign on d), rem will always be positive
              
              if rem * 2I > bigint.Abs d then 
                Rational(q + bigint d.Sign |> bigint.Abs, bigint d.Sign) 
              elif rem * 2I < bigint.Abs d then
                Rational(bigint.Abs q, bigint d.Sign)
              else   // 0.5 remainder, round to even
                Rational(q + (if q.IsEven then 0I else bigint d.Sign) |> bigint.Abs, bigint d.Sign)
              
        
        static member Range left right =
          failwith "not implemented"
          