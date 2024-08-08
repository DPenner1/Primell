namespace dpenner1.Primell

open System.Collections.Generic
open System.Text

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
            "<,", fun () -> this.GetCsvInput()
           ]

    member this.UnaryNumericOperators: IDictionary<string, PNumber->PObject> = 
      dict ["~",   fun n -> ExtendedBigRational.(~-) n.Value |> PNumber :> PObject
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
            ">_", fun l -> this.OutputCsv(l)
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
            "..", fun (left, right) -> 
                    if control.Settings.UsePrimeOperators then 
                      PPrimeLib.PrimeRange left.Value right.Value :> PObject
                    else 
                      ExtendedBigRational.Range(left.Value, right.Value) |> Seq.map(fun x -> x |> PNumber :> PObject) |> PList :> PObject
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
            l |> Seq.map(fun x -> this.ApplyBinaryNumericOperation n x operator opMods) |> PList :> PObject
        | (:? PList as l), (:? PNumber as n) -> 
            l |> Seq.map(fun x -> this.ApplyBinaryNumericOperation x n operator opMods) |> PList :> PObject
        | (:? PList as l1), (:? PList as l2) -> 
            (l1, l2) ||> Seq.map2 (fun x y -> this.ApplyBinaryNumericOperation x y operator opMods) |> PList :> PObject
            // TODO - F# truncates to the shortest list, but Primell's default is to virtually extend the shorter list with Emptys
            // Interesting solution provided here that could be adapted: https://stackoverflow.com/a/2840062/1607043
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
      if this.NullaryOperators.ContainsKey opText then
        this.NullaryOperators[opText]()
      else
        PrimellProgrammerProblemException "Unrecognized operator" |> raise

    member this.ApplyUnaryOperation (pobj: PObject) (opText: string) opMods : PObject =
      control.LastOperationWasAssignment <- false
      control.LastOperationWasOutput <- false
      if this.UnaryNumericOperators.ContainsKey opText then
        this.ApplyUnaryNumericOperation pobj (this.UnaryNumericOperators[opText]) opMods
      elif this.UnaryListOperators.ContainsKey opText then
        this.ApplyUnaryListOperation pobj (this.UnaryListOperators[opText]) opMods
      else
        PrimellProgrammerProblemException "Unrecognized operator" |> raise

    member this.ApplyBinaryOperation (left: PObject) (right: PObject) (opText: string) opMods : PObject =
      control.LastOperationWasAssignment <- false
      control.LastOperationWasOutput <- false
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

    member this.GetCsvInput(): PObject =
      System.NotImplementedException() |> raise

    member this.OutputCsv(l: PList) =
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

    member this.Head(pobj: PObject) =
      match pobj with
      | :? PList as l -> l.Head()
      | _ -> pobj   // atom is its own head

    member this.Tail(pobj: PObject) =
      match pobj with
      | :? PList as l -> l.Tail()
      | _ -> PList.Empty  // atom has no tail