namespace dpenner1.Primell

open System.Collections.Generic
open System.Text
open dpenner1.Math

type BranchMode =
  | Forward
  | Backward
  | Absolute

type IExternal =  // Some operations need to interact with the outside world  TODO need better name
  abstract member Branch: BranchMode -> ExtendedBigRational -> PObject
  abstract member OutputToDefault: string -> unit
  abstract member FlushDefaultOutput: unit -> unit
  abstract member ReadLineFromDefault: unit -> string

// note: as part of trying to get references to work, OperationLib was converted from module to class type
//       This may have the later benefit of being able to swap out operations dependent on control.UsePrimeOperators
type OperationLib(control: PrimellProgramControl, external: IExternal) =

    let control = control
    let external = external

    // all dictionaries have test item for now, this is planning for user-defined operators,
    // the test entries are for test cases so I don't inadvertently break these plans while working on other things
    
    member this.NullaryOperators: IDictionary<string, unit->PObject> =
      dict ["<\"", fun () -> this.GetStringInput()
            "<_", fun () -> this.GetListInput()
           ]

    member this.UnaryNumericOperators: IDictionary<string, ExtendedBigRational->PObject> = 
      dict ["~",   fun n -> ExtendedBigRational.(~-) n |> Number |> PObject
            "*~",  fun n -> ExtendedBigRational.Reciprocal n |> Number |> PObject
            "/*",  fun n -> if control.Settings.UsePrimeOperators then PPrimeLib.PrimeFactorization n else System.NotImplementedException "factors not done" |> raise
            "/#",  fun n -> this.GetDigits(n, control.Settings.SourceBase)
            "++",  fun n -> (if control.Settings.UsePrimeOperators then PPrimeLib.NextPrime n else n + ExtendedBigRational.One) |> Number |> PObject
            "--",  fun n -> (if control.Settings.UsePrimeOperators then PPrimeLib.PrevPrime n else n - ExtendedBigRational.One) |> Number |> PObject
            "+-",  fun n -> (if control.Settings.UsePrimeOperators then PPrimeLib.NearestPrime n else round n) |> Number |> PObject
            "!/",  fun n -> external.Branch Forward n
            "!\\", fun n -> external.Branch Backward n
            "!|",  fun n -> external.Branch Absolute n
            "#test", fun n -> ExtendedBigRational.(~-) n |> Number |> PObject  // copied negate function
           ]

    member this.UnaryListOperators: IDictionary<string, PList->PObject> = 
      dict ["_<", fun l -> l.Head()
            "_>", fun l -> l.Tail()        
            "_~", fun l -> l.Reverse() |> Sequence |> PObject
            "__", fun l -> l.Flatten() |> Sequence |> PObject
            "_#", fun l -> l.Length |> Number |> PObject
            ">_", fun l -> this.OutputList(l)
            ">\"", fun l -> this.OutputString(l)
            "_test", fun l -> l.Reverse() |> Sequence |> PObject
           ]

    member this.BinaryNumericOperators: IDictionary<string, ExtendedBigRational*ExtendedBigRational->PObject> = 
      dict ["+",  fun (left, right) -> ExtendedBigRational.(+)(left, right) |> Number |> PObject
            "-",  fun (left, right) -> ExtendedBigRational.(-)(left, right) |> Number |> PObject
            "*",  fun (left, right) -> ExtendedBigRational.( * )(left, right) |> Number |> PObject
            "/",  fun (left, right) -> ExtendedBigRational.( / )(left, right) |> Number |> PObject
            ">",  fun (left, right) -> ExtendedBigRational.Max(left, right) |> Number |> PObject
            "<",  fun (left, right) -> ExtendedBigRational.Min(left, right) |> Number |> PObject
            "**", fun (left, right) -> ExtendedBigRational.Pow(left, right) |> Number |> PObject
            "..", fun (left, right) -> 
                    if control.Settings.UsePrimeOperators then 
                      PPrimeLib.PrimeRange left right
                    else 
                      ExtendedBigRational.Range(left, right) |> Seq.map(fun x -> x |> Number |> PObject) |> PObject.FromSeq
            "...",fun (left, right) -> 
                    if control.Settings.UsePrimeOperators then 
                      System.NotImplementedException "inclusive prime range not implemented" |> raise
                    else 
                      ExtendedBigRational.Range(left, right, rightInclusive = true) |> Seq.map(fun x -> x |> Number |> PObject) |> PObject.FromSeq
            "#test#", fun (left, right) -> ExtendedBigRational.(-)(left, right) |> Number |> PObject // copied subtract 
           ]
    
    member this.BinaryListOperators: IDictionary<string, PList*PList->PObject> = 
      dict ["<::>",  fun (left: PList, right: PList) -> left.AppendAll (right |> Sequence |> PObject) |> PObject.FromSeq
            "@_",  fun (left: PList, right: PList) -> left.AllIndexesOf (right |> Sequence |> PObject)
            "_test_", fun (left: PList, right: PList) -> left.AppendAll (right |> Sequence |> PObject) |> PObject.FromSeq
           ]
 
    member this.NumericListOperators: IDictionary<string, ExtendedBigRational*PList->PObject> = 
      dict ["<::",  fun (left: ExtendedBigRational, right: PList) -> right.Cons (left |> Number |> PObject) |> PObject.FromSeq
            "#test_",  fun (left: ExtendedBigRational, right: PList) -> right.Cons (left |> Number |> PObject) |> PObject.FromSeq
           ]

    member this.ListNumericOperators: IDictionary<string, PList*ExtendedBigRational->PObject> = 
      dict ["::>", fun (left: PList, right: ExtendedBigRational) -> left.Append (right |> Number |> PObject) |> PObject.FromSeq
            "@",   fun (left: PList, right: ExtendedBigRational) -> left.Index right
            "_test#", fun (left: PList, right: ExtendedBigRational) -> left.Append (right |> Number |> PObject) |> PObject.FromSeq
           ]

    // opMods for consistency, but I don't think Primell will have any need for opMods on unary numeric operators
    member private this.ApplyUnaryNumericOperation (pobj: PObject) operator opMods =
        match pobj.Value with
        | Number n -> operator n
        | Empty -> PObject.Empty
        | Sequence l -> l |> Seq.map(fun x -> this.ApplyUnaryNumericOperation x operator opMods) |> PObject.FromSeq
        | _ -> PrimellProgrammerProblemException "Not possible"  |> raise
        
    // TODO - make this private when the references in Visitor are removed
    member this.ApplyUnaryListOperation (pobj: PObject) operator opMods : PObject =
        match pobj.Value with
        | Sequence l -> operator l
        | Empty -> this.ApplyUnaryListOperation (Seq.empty |> PList |> Sequence |> PObject) operator opMods
        | Number n -> this.ApplyUnaryListOperation (n |> Number |> PObject |> Seq.singleton |> PList |> Sequence |> PObject) operator opMods
        | Operator _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise

    member private this.ApplyBinaryNumericOperation (left: PObject) (right: PObject) operator opMods : PObject =
        match left.Value, right.Value with
        | Number n1, Number n2 -> 
            operator(n1, n2)
        | Number _, Empty ->
            left
        | Empty, Number _ ->
            right
        | Number _, Sequence l -> 
            if opMods |> List.contains Truncate |> not then
              left
            else
              l |> Seq.map(fun x -> this.ApplyBinaryNumericOperation left x operator opMods) |> PObject.FromSeq
        | Sequence l, Number _ -> 
            if opMods |> List.contains Truncate |> not then
              right
            else
              l |> Seq.map(fun x -> this.ApplyBinaryNumericOperation x right operator opMods) |> PObject.FromSeq
        | Sequence l1, Sequence l2 -> 
            if opMods |> List.contains Truncate then            
              (l1, l2) ||> Seq.map2 (fun x y -> this.ApplyBinaryNumericOperation x y operator opMods) |> PObject.FromSeq
            else   // Adapted from: https://stackoverflow.com/a/2840062/1607043
              let extL1 = Seq.append (l1 |> Seq.map (fun x -> Some x)) (Seq.initInfinite (fun _ -> None))
              let extL2 = Seq.append (l2 |> Seq.map (fun x -> Some x)) (Seq.initInfinite (fun _ -> None))
              Seq.zip extL1 extL2
              |> Seq.takeWhile (fun (x, y) -> Option.isSome x || Option.isSome y)
              |> Seq.map ( function
                  | (Some x, Some y) -> this.ApplyBinaryNumericOperation x y operator opMods
                  | (Some x, None  ) -> x
                  | (None  , Some y) -> y
                  | _ -> PrimellProgrammerProblemException "Not possible" |> raise)
              |> PObject.FromSeq
            
        | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise

    member private this.ApplyBinaryListOperation (left: PObject) (right: PObject) operator opMods : PObject =
        match left.Value, right.Value with
        | Sequence l1, Sequence l2 -> 
            operator(l1, l2)
        | Number _, _ -> 
            this.ApplyBinaryListOperation (left |> Seq.singleton |> PList |> Sequence |> PObject) right operator opMods
        | _, Number _ -> 
            this.ApplyBinaryListOperation left (right |> Seq.singleton |> PList |> Sequence |> PObject) operator opMods
        | Empty, _ -> 
            this.ApplyBinaryListOperation (Seq.empty |> PList |> Sequence |> PObject) right operator opMods
        | _, Empty -> 
            this.ApplyBinaryListOperation left (Seq.empty |> PList |> Sequence |> PObject) operator opMods
        | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise

    member private this.ApplyNumericListOperation (pNumber: PObject) (pList: PObject) operator opMods : PObject =
        match pNumber.Value, pList.Value with
        | Number n, Sequence l -> 
            operator(n, l)
        | Empty, _ ->
            PObject.Empty
        | Sequence l, __ -> 
            l |> Seq.map(fun x -> this.ApplyNumericListOperation x pList operator opMods) |> PObject.FromSeq
        | _, Number _ ->
            this.ApplyNumericListOperation pNumber (pList |> Seq.singleton |> PList |> Sequence |> PObject) operator opMods
        | _, Empty ->
            this.ApplyNumericListOperation pNumber (Seq.empty |> PList |> Sequence |> PObject) operator opMods
        | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise

    member private this.ApplyListNumericOperation (pList: PObject) (pNumber: PObject) operator opMods : PObject =
        match pList.Value, pNumber.Value with
        | Sequence l, Number n -> 
            operator(l, n)
        | Number _, _ ->
            this.ApplyListNumericOperation (pList |> Seq.singleton |> PList |> Sequence |> PObject) pNumber operator opMods
        | Empty, _ ->
            this.ApplyListNumericOperation (Seq.empty |> PList |> Sequence |> PObject) pNumber operator opMods
        | _, Empty ->
            PObject.Empty
        | _, Sequence l -> 
            l |> Seq.map(fun x -> this.ApplyListNumericOperation pList x operator opMods) |> PObject.FromSeq
        | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise

    member this.ApplyNullaryOperation (opText: string) opMods =
      control.LastOperationWasAssignment <- false
      control.LastOperationWasOutput <- false

      if opMods |> List.contains Unfold then
        let opMods' = opMods |> List.where(fun x -> x <> Unfold)
        Seq.initInfinite(fun _ -> this.ApplyNullaryOperation opText opMods') |> PList |> Sequence |> PObject
      else
        if this.NullaryOperators.ContainsKey opText then
          this.NullaryOperators[opText]()
        else
          PrimellProgrammerProblemException "Unrecognized operator" |> raise

    member this.ApplyUnaryOperation (pobj: PObject) (opText: string) opMods : PObject =
      control.LastOperationWasAssignment <- false
      control.LastOperationWasOutput <- false

      if (opMods |> List.contains Unfold) then
        let opMods' = opMods |> List.where(fun x -> x <> Unfold)
        pobj |> Seq.unfold(fun x -> Some (x, this.ApplyUnaryOperation x opText opMods')) |> PObject.FromSeq
      else
        if this.UnaryNumericOperators.ContainsKey opText then
          this.ApplyUnaryNumericOperation pobj (this.UnaryNumericOperators[opText]) opMods
        elif this.UnaryListOperators.ContainsKey opText then
          this.ApplyUnaryListOperation pobj (this.UnaryListOperators[opText]) opMods
        else
          PrimellProgrammerProblemException "Unrecognized operator" |> raise

    member this.ApplyBinaryOperation (left: PObject) (right: PObject) (opText: string) opMods : PObject =
      control.LastOperationWasAssignment <- false
      control.LastOperationWasOutput <- false

      if (opMods |> List.contains Unfold) then
        let opMods' = opMods |> List.where(fun x -> x <> Unfold)
        left |> Seq.unfold(fun x -> Some (x, this.ApplyBinaryOperation x right opText opMods')) |> PObject.FromSeq
      else
        if this.BinaryListOperators.ContainsKey opText then
          this.ApplyBinaryListOperation left right (this.BinaryListOperators[opText]) opMods
        elif this.BinaryNumericOperators.ContainsKey opText then
          this.ApplyBinaryNumericOperation left right (this.BinaryNumericOperators[opText]) opMods
        elif this.ListNumericOperators.ContainsKey opText then
          this.ApplyListNumericOperation left right (this.ListNumericOperators[opText]) opMods
        elif this.NumericListOperators.ContainsKey opText then
          this.ApplyNumericListOperation left right (this.NumericListOperators[opText]) opMods
        else
          PrimellProgrammerProblemException "Unrecognized operator" |> raise

    member this.ApplyFoldOperation (pobj: PObject) (opText: string) opMods : PObject =
      match pobj.Value with
      | Sequence l ->
          l |> Seq.reduce(fun x y -> this.ApplyBinaryOperation x y opText opMods)
      | _ -> pobj


    member this.IsTruth(pobj: PObject, primesAreTruth: bool, requireAllTruth: bool) =
      match pobj.Value with
      | Empty -> false
      | Sequence l ->  // infinite recursion is possible with infinite lists
          if requireAllTruth then
            l |> Seq.exists(fun x -> this.IsTruth(x, primesAreTruth, requireAllTruth) |> not) |> not
              // TODO - since I'm piping a few times here, this probably isn't tail recursion
          else
            l |> Seq.exists(fun x -> this.IsTruth(x, primesAreTruth, requireAllTruth))
      | Number n -> 
          if primesAreTruth then
            PPrimeLib.IsPrime n
          else
            match n with
            | NaN -> false
            | _ as v -> not v.IsZero
      | _ -> PrimellProgrammerProblemException "Not possible" |> raise

    member this.GetStringInput(): PObject =
      external.ReadLineFromDefault().ToCharArray() 
      |> Array.toList 
      |> List.map(fun c -> System.Convert.ToInt32 c |> BigRational |> Rational |> Number |> PObject) 
      |> PObject.FromSeq
      // TODO - this is just UTF-16 for simplicity right now
      

    member this.GetListInput(): PObject =
      System.NotImplementedException() |> raise

    member this.OutputList(l: PList) =
      System.NotImplementedException() |> raise

    member private this.OutputString'(l: PList) = 
      l |> Seq.iter(fun pobj -> 
        match pobj.Value with
        | Sequence subList -> this.OutputString' subList
        | Number n -> 
            let charValue = 
              match n with  // TODO - hardcoding UTF-32
              | NaN -> None
              | Infinity Positive -> Some 0x10FFFF
              | Infinity Negative -> Some 0
              | Rational r -> 
                  let intValue = (round r).Numerator
                  if intValue <= 0I then Some 0
                  elif intValue > bigint 0x10FFFF then Some 0x10FFFF
                  else Some (int intValue)
            
            match charValue with
            | None -> ()
            | Some v ->
                // I learned about Rune after i did UTF-32 manually here... well it works, so im leaving it for now...
                let bytes = [|byte v; byte (v >>> 8); byte (v >>> 16); byte (v >>> 24)|]
                let c = Encoding.UTF32.GetString(bytes)
                control.LineResults[control.CurrentLine] <- 
                  { control.LineResults[control.CurrentLine] with Output = control.LineResults[control.CurrentLine].Output + c}
                external.OutputToDefault c
        | Empty -> external.OutputToDefault "()"
        | _ -> System.NotImplementedException() |> raise
      )

    member this.OutputString(l: PList) = 
      this.OutputString' l
      external.FlushDefaultOutput()
      control.LastOperationWasOutput <- true
      l |> Sequence |> PObject  // i guess?  probably makes sense to not modify the object "on the stack"

    // These are needed separately from regular Head/List due to language conditional feature
    member this.Head(pobj: PObject) =
      match pobj.Value with
      | Sequence l -> l.Head()
      | _ -> pobj   // atom/empty is its own head

    member this.Tail(pobj: PObject) =
      match pobj.Value with
      | Sequence l -> l.Tail()
      | _ -> PObject.Empty  // atom/empty has no tail

    member this.GetDigits(n: ExtendedBigRational, ``base``: bigint) =
      let rec getBaseDigits(n': bigint, digitsSoFar: bigint list) =
        let q, rem = bigint.DivRem(n', ``base``)
        let newList = rem::digitsSoFar
        if q.IsZero then newList
        else getBaseDigits(q, newList)
      // remainders seen: map remainder to position in digit sequence, for possible repeated decimal
      let rec getFractionalDigits(divisor: bigint, rem: bigint, remaindersSeen: Map<bigint, int>, digits: bigint seq) =
        if rem.IsZero then digits   // terminating decimal expansion
        elif remaindersSeen |> Map.containsKey rem then   // repeated decimal
          let repeatStartIndex = remaindersSeen[rem]
          Seq.append (digits |> Seq.take repeatStartIndex) (seq {while true do yield! (Seq.skip repeatStartIndex digits)})
        else
          let q, rem' = bigint.DivRem(rem * ``base``, divisor)
          let newDigits = getBaseDigits(q, [])
          getFractionalDigits(divisor, rem', remaindersSeen |> Map.add rem (Seq.length digits), Seq.append digits newDigits)

      match n with
      | NaN -> PObject.Empty
      | Infinity _ -> seq { PObject.Infinite(NaN |> Number |> PObject); PObject.Infinite(NaN |> Number |> PObject) } |> PList |> Sequence |> PObject
      | Rational r -> 
          let q, rem = bigint.DivRem(r.Numerator, bigint.Abs r.Denominator)
          let wholeDigits = 
            getBaseDigits(q, []) 
              |> Seq.ofList 
              |> Seq.map(fun x -> x |> BigRational |> Rational |> Number |> PObject)
              |> PObject.FromSeq
          let fractionalDigits = 
              getFractionalDigits(bigint.Abs r.Denominator, rem, Map.empty, Seq.empty)
              |> Seq.map(fun x -> x |> BigRational |> Rational |> Number |> PObject)
              |> PObject.FromSeq

          seq { wholeDigits; fractionalDigits } |> PList |> Sequence |> PObject
    