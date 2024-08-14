namespace dpenner1.Primell

open System.Collections.Generic
open System.Text
open dpenner1.Math

type BranchMode =
  | Forward
  | Backward
  | Absolute

type IExternal =  // Some operations need to interact with the outside world  TODO need better name
  abstract member Branch: BranchMode -> PNumber -> PObject

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

    member this.UnaryNumericOperators: IDictionary<string, PNumber->PObject> = 
      dict ["~",   fun n -> ExtendedBigRational.(~-) n.Value |> PNumber :> PObject
            "*~",  fun n -> ExtendedBigRational.Reciprocal n.Value |> PNumber :> PObject
            "/*",  fun n -> if control.Settings.UsePrimeOperators then PPrimeLib.PrimeFactorization n else System.NotImplementedException "factors not done" |> raise
            "/#",  fun n -> this.GetDigits(n, control.Settings.SourceBase)
            "++",  fun n -> if control.Settings.UsePrimeOperators then PPrimeLib.NextPrime n else n.Value + ExtendedBigRational.One |> PNumber :> PObject
            "--",  fun n -> if control.Settings.UsePrimeOperators then PPrimeLib.PrevPrime n else n.Value - ExtendedBigRational.One |> PNumber :> PObject
            "+-",  fun n -> if control.Settings.UsePrimeOperators then PPrimeLib.NearestPrime n else round n.Value |> PNumber :> PObject
            "!/",  fun n -> external.Branch Forward n
            "!\\", fun n -> external.Branch Backward n
            "!|",  fun n -> external.Branch Absolute n
            "#test", fun n -> ExtendedBigRational.(~-) n.Value |> PNumber :> PObject  // copied negate function
           ]

    member this.UnaryListOperators: IDictionary<string, PList->PObject> = 
      dict ["_<", fun l -> l.Head()
            "_>", fun l -> l.Tail()        
            "_~", fun l -> l.Reverse()
            "__", fun l -> l.Flatten()
            "_#", fun l -> l.Length
            ">_", fun l -> this.OutputList(l)
            ">\"", fun l -> this.OutputString(l)
            "_test", fun l -> l.Reverse()
           ]

    member this.BinaryNumericOperators: IDictionary<string, PNumber*PNumber->PObject> = 
      dict ["+",  fun (left, right) -> ExtendedBigRational.(+)(left.Value, right.Value) |> PNumber :> PObject
            "-",  fun (left, right) -> ExtendedBigRational.(-)(left.Value, right.Value) |> PNumber :> PObject
            "*",  fun (left, right) -> ExtendedBigRational.( * )(left.Value, right.Value) |> PNumber :> PObject
            "/",  fun (left, right) -> ExtendedBigRational.( / )(left.Value, right.Value) |> PNumber :> PObject
            ">",  fun (left, right) -> ExtendedBigRational.Max(left.Value, right.Value) |> PNumber :> PObject
            "<",  fun (left, right) -> ExtendedBigRational.Min(left.Value, right.Value) |> PNumber :> PObject
            "**", fun (left, right) -> ExtendedBigRational.Pow(left.Value, right.Value) |> PNumber :> PObject
            "..", fun (left, right) -> 
                    if control.Settings.UsePrimeOperators then 
                      PPrimeLib.PrimeRange left.Value right.Value :> PObject
                    else 
                      ExtendedBigRational.Range(left.Value, right.Value) |> Seq.map(fun x -> x |> PNumber :> PObject) |> PList :> PObject
            "...",fun (left, right) -> 
                    if control.Settings.UsePrimeOperators then 
                      System.NotImplementedException "inclusive prime range not implemented" |> raise
                    else 
                      ExtendedBigRational.Range(left.Value, right.Value, rightInclusive = true) |> Seq.map(fun x -> x |> PNumber :> PObject) |> PList :> PObject
            "#test#", fun (left, right) -> ExtendedBigRational.(-)(left.Value, right.Value) |> PNumber :> PObject // copied subtract 
           ]
    
    member this.BinaryListOperators: IDictionary<string, PList*PList->PObject> = 
      dict ["<::>",  fun (left: PList, right: PList) -> left.AppendAll right
            "@_",  fun (left: PList, right: PList) -> left.AllIndexesOf right
            "_test_", fun (left: PList, right: PList) -> left.AppendAll right
           ]
 
    member this.NumericListOperators: IDictionary<string, PNumber*PList->PObject> = 
      dict ["<::",  fun (left: PNumber, right: PList) -> right.Cons left
            "#test_",  fun (left: PNumber, right: PList) -> right.Cons left
           ]

    member this.ListNumericOperators: IDictionary<string, PList*PNumber->PObject> = 
      dict ["::>", fun (left: PList, right: PNumber) -> left.Append right
            "@",   fun (left: PList, right: PNumber) -> left.Index right
            "_test#", fun (left: PList, right: PNumber) -> left.Append right
           ]

    // opMods for consistency, but I don't think Primell will have any need for opMods on unary numeric operators
    member private this.ApplyUnaryNumericOperation (pobj: PObject) operator opMods =
        match pobj with
        | :? PNumber as n -> operator n
        | :? PList as l -> l |> Seq.map(fun x -> this.ApplyUnaryNumericOperation x operator opMods) |> PList :> PObject
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise
        
    // TODO - make this private when the references in Visitor are removed
    member this.ApplyUnaryListOperation (pobj: PObject) operator opMods : PObject =
        match pobj with
        | :? PList as l -> operator l
        | :? PNumber as n -> this.ApplyUnaryListOperation (n :> PObject |> Seq.singleton |> PList) operator opMods
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member private this.ApplyBinaryNumericOperation (left: PObject) (right: PObject) operator opMods : PObject =
        match left, right with
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            operator(n1, n2)
        | (:? PNumber as n), (:? PList as l) -> 
            if l.IsEmpty && opMods |> List.contains Truncate |> not then
              n
            else
              l |> Seq.map(fun x -> this.ApplyBinaryNumericOperation n x operator opMods) |> PList :> PObject
        | (:? PList as l), (:? PNumber as n) -> 
            if l.IsEmpty && opMods |> List.contains Truncate |> not then
              n
            else
              l |> Seq.map(fun x -> this.ApplyBinaryNumericOperation x n operator opMods) |> PList :> PObject
        | (:? PList as l1), (:? PList as l2) -> 
            if opMods |> List.contains Truncate then            
              (l1, l2) ||> Seq.map2 (fun x y -> this.ApplyBinaryNumericOperation x y operator opMods) |> PList :> PObject
            else   // Adapted from: https://stackoverflow.com/a/2840062/1607043
              let extL1 = Seq.append (l1 |> Seq.map (fun x -> Some x)) (Seq.initInfinite (fun _ -> None))
              let extL2 = Seq.append (l2 |> Seq.map (fun x -> Some x)) (Seq.initInfinite (fun _ -> None))
              Seq.zip extL1 extL2
              |> Seq.takeWhile (fun (x, y) -> Option.isSome x || Option.isSome y)
              |> Seq.map ( function
                  | (Some x, Some y) -> this.ApplyBinaryNumericOperation x y operator opMods
                  | (Some x, None  ) -> x
                  | (None  , Some y) -> y
                  | _ -> PrimellProgrammerProblemException("Not possible") |> raise)
              |> PList :> PObject
            
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member private this.ApplyBinaryListOperation (left: PObject) (right: PObject) operator opMods : PObject =
        match left, right with
        | (:? PList as l1), (:? PList as l2) -> 
            operator(l1, l2)
        | (:? PNumber as n), (:? PList as l) -> 
            this.ApplyBinaryListOperation (n :> PObject |> Seq.singleton |> PList) right operator opMods
        | (:? PList as l), (:? PNumber as n) -> 
            this.ApplyBinaryListOperation left (n :> PObject |> Seq.singleton |> PList) operator opMods
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            this.ApplyBinaryListOperation (n1 :> PObject |> Seq.singleton |> PList) (n2 :> PObject |> Seq.singleton |> PList) operator opMods
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member private this.ApplyNumericListOperation (pNumber: PObject) (pList: PObject) operator opMods : PObject =
        match pNumber, pList with
        | (:? PList as l), (:? PNumber as n) -> 
            this.ApplyNumericListOperation l (n :> PObject |> Seq.singleton |> PList) operator opMods
        | (:? PList as l1), (:? PList as l2) -> 
            l1 |> Seq.map(fun x -> this.ApplyNumericListOperation x l2 operator opMods) |> PList :> PObject
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            this.ApplyNumericListOperation n1 (n2 :> PObject |> Seq.singleton |> PList) operator opMods
        | (:? PNumber as n), (:? PList as l) -> 
            operator(n, l)
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member private this.ApplyListNumericOperation (pList: PObject) (pNumber: PObject) operator opMods : PObject =
        match pList, pNumber with
        | (:? PList as l), (:? PNumber as n) -> 
            operator(l, n)
        | (:? PList as l1), (:? PList as l2) -> 
            l2 |> Seq.map(fun x -> this.ApplyListNumericOperation l1 x operator opMods) |> PList :> PObject
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            this.ApplyListNumericOperation (n1 :> PObject |> Seq.singleton |> PList) pNumber operator opMods
        | (:? PNumber as n), (:? PList as l) -> 
            this.ApplyListNumericOperation (n :> PObject |> Seq.singleton |> PList) l operator opMods
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member this.ApplyNullaryOperation (opText: string) opMods =
      control.LastOperationWasAssignment <- false
      control.LastOperationWasOutput <- false

      if (opMods |> List.contains Unfold) then
        let opMods' = opMods |> List.where(fun x -> x <> Unfold)
        Seq.initInfinite(fun _ -> this.ApplyNullaryOperation opText opMods') |> PList :> PObject
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
        pobj |> Seq.unfold(fun x -> Some (x, this.ApplyUnaryOperation x opText opMods')) |> PList :> PObject
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
        left |> Seq.unfold(fun x -> Some (x, this.ApplyBinaryOperation x right opText opMods')) |> PList :> PObject
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
      match pobj with
      | :? PList as l ->
          l |> Seq.reduce(fun x y -> this.ApplyBinaryOperation x y opText opMods)
      | _ -> pobj


    member this.IsTruth(pobj: PObject, primesAreTruth: bool, requireAllTruth: bool) =
      match pobj with
      | :? PList as l when l.IsEmpty -> false
      | :? PList as l ->  // infinite recursion is possible with infinite lists
          if requireAllTruth then
            l |> Seq.exists(fun x -> this.IsTruth(x, primesAreTruth, requireAllTruth) |> not) |> not
              // TODO - since I'm piping a few times here, this probably isn't tail recursion
          else
            l |> Seq.exists(fun x -> this.IsTruth(x, primesAreTruth, requireAllTruth))
      | :? PNumber as n -> 
          if primesAreTruth then
            PPrimeLib.IsPrime n
          else
            match n.Value with
            | NaN -> false
            | _ as v -> not v.IsZero
      | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member this.GetStringInput(): PObject =
      System.NotImplementedException() |> raise

    member this.GetListInput(): PObject =
      System.NotImplementedException() |> raise

    member this.OutputList(l: PList) =
      System.NotImplementedException() |> raise

    member private this.OutputString'(l: PList) = 
      l |> Seq.iter(fun pobj -> 
        match pobj with
        | :? PList as subList -> this.OutputString' subList
        | :? PNumber as n -> 
            let charValue = 
              match n.Value with  // TODO - hardcoding UTF-32
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
                printf "%s" c
        | _ -> System.NotImplementedException() |> raise
      )

    member this.OutputString(l: PList) = 
      this.OutputString' l
      control.LastOperationWasOutput <- true
      l   // i guess?  probably makes sense to not modify the object "on the stack"

    // These are needed separately from regular Head/List due to language conditional feature
    member this.Head(pobj: PObject) =
      match pobj with
      | :? PList as l -> l.Head()
      | _ -> pobj   // atom is its own head

    member this.Tail(pobj: PObject) =
      match pobj with
      | :? PList as l -> l.Tail()
      | _ -> PList.Empty  // atom has no tail

    member this.GetDigits(n: PNumber, ``base``: bigint) =
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

      match n.Value with
      | NaN -> PList.Empty
      | Infinity _ -> seq { PList.Infinite(NaN |> PNumber) :> PObject; PList.Infinite(NaN |> PNumber) :> PObject} |> PList :> PObject
      | Rational r -> 
          let q, rem = bigint.DivRem(r.Numerator, bigint.Abs r.Denominator)
          let wholeDigits = 
            getBaseDigits(q, []) 
              |> Seq.ofList 
              |> Seq.map(fun x -> x |> BigRational |> Rational |> PNumber :> PObject)
              |> PList :> PObject
          let fractionalDigits = 
              getFractionalDigits(bigint.Abs r.Denominator, rem, Map.empty, Seq.empty)
              |> Seq.map(fun x -> x |> BigRational |> Rational |> PNumber :> PObject)
              |> PList :> PObject
              
          seq { wholeDigits; fractionalDigits } |> PList |> this.Normalize
    
    member this.Normalize (pobj: PObject) =
      match pobj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
      | :? PList as l when not(l.IsEmpty) && Seq.isEmpty(Seq.tail l) -> 
          Seq.head l |> this.Normalize
      | _ -> pobj    