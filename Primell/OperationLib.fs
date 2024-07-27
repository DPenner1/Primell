namespace dpenner1.Primell

open System.Collections.Generic


// note: as part of trying to get PReference to work, OperationLib was converted from module to class type
//       This may have the later benefit of being able to swap out operations dependent on control.UsePrimeOperators
type OperationLib(control: PrimellProgramControl) =

    let control = control
    
    // very temporary, clearly things are going poorly
    let GetInt(n: PNumber) = 
      match n.Value with
      | Rational r -> (round r).Numerator |> int
      | _ -> System.NotImplementedException("Indexing is a little wonky right now") |> raise

    member private this.RaiseAtoms(plist: PrimellList) =
        plist |> Seq.map(fun x -> match x with 
                                  | :? PAtom as a -> Seq.singleton (a :> PObject)
                                  | :? PrimellList as l -> l
                                  | :? PReference as ref ->
                                      match ref.CapturedValue with
                                      | :? PAtom as a -> Seq.singleton (a :> PObject)
                                      | :? PrimellList as l -> l
                                      | _ -> PrimellProgrammerProblemException("not possible") |> raise
                                  | :? PVariable as v ->
                                        match v.CapturedValue with
                                        | :? PAtom as a -> Seq.singleton (a :> PObject)
                                        | :? PrimellList as l -> l
                                        | _ -> PrimellProgrammerProblemException("not possible") |> raise
                                  | _ -> PrimellProgrammerProblemException("not possible") |> raise)

    member this.Flatten(plist: PrimellList) =
      this.RaiseAtoms plist |> Seq.concat |> PrimellList

    member this.Index(left: PObject) (right: PObject): PObject =
        match left, right with
        | _, (:? PVariable as v) -> this.Index left v.CapturedValue
        | _, (:? PReference as r) -> this.Index left r.CapturedValue
        | (:? PVariable as v), _ -> PReference(v, right, this.Index v.CapturedValue right)    
        | (:? PReference as r), _ -> PReference(r, right, this.Index r.CapturedValue right)
        | (:? PList as l), (:? PNumber as n) -> l.Index n
        | :? PNumber as n, _ -> this.Index(n :> PObject |> Seq.singleton |> PList) right
        | _, (:? PList as l) -> l |> Seq.map(fun x -> this.Index left x) |> PList :> PObject
        | _ -> PrimellProgrammerProblemException "Not possible" |> raise
        
    member this.NullaryOperators: IDictionary<string, unit->PObject> =
      dict [":_", fun () -> control.GetCodeInput()
            ":~", fun () -> control.GetStringInput()  // symbol to change
            ":,", fun () -> control.GetCsvInput()
           ]

    // for now these are immutable dicts, but they might be changed to mutable Dictionary on implementation of user-defined operators
    member this.UnaryNumericOperators: IDictionary<string, PNumber->PObject> = 
      dict ["~", fun n -> ExtendedBigRational.(~-) n.Value |> PNumber :> PObject
            "++", fun n -> PrimeLib.NextPrime n.Value |> PNumber :> PObject
            "--", fun n -> PrimeLib.PrevPrime n.Value |> PNumber :> PObject
           ]

    member this.UnaryListOperators: IDictionary<string, PList->PObject> = 
      dict ["_<", fun (l: PrimellList) -> l.Head()
            "_>", fun (l: PrimellList) -> l.Tail()        
            "_~", fun (l: PrimellList) -> l.Reverse()
            "__", fun (l: PrimellList) -> this.Flatten(l)
           ]

    member this.BinaryNumericOperators: IDictionary<string, PNumber*PNumber->PObject> = 
      dict ["..", fun (left, right) -> PrimeLib.PrimeRange left.Value right.Value |> Seq.map(fun n -> n |> PNumber :> PObject) |> PList :> PObject
            "+",  fun (left, right) -> ExtendedBigRational.(+)(left.Value, right.Value) |> PNumber :> PObject
            "-",  fun (left, right) -> ExtendedBigRational.(-)(left.Value, right.Value) |> PNumber :> PObject
            "*",  fun (left, right) -> ExtendedBigRational.( * )(left.Value, right.Value) |> PNumber :> PObject
            "/",  fun (left, right) -> ExtendedBigRational.( / )(left.Value, right.Value) |> PNumber :> PObject
            ">",  fun (left, right) -> ExtendedBigRational.Max(left.Value, right.Value) |> PNumber :> PObject
            "<",  fun (left, right) -> ExtendedBigRational.Min(left.Value, right.Value) |> PNumber :> PObject
           ]
    
    // TODO - I don't have any Binary List Operators implemented yet
    member this.BinaryListOperators: IDictionary<string, PList*PList->PObject> = 
      dict ["\\",  fun (left: PrimellList, right: PrimellList) -> PrimellList.Empty
           ]
 

    // opMods for consistency, but I don't think Primell will have any need for opMods on nullary operators
    member this.ApplyNullaryOperation operator opMods : PObject =
      operator()

    // opMods for consistency, but I don't think Primell will have any need for opMods on unary numeric operators
    member this.ApplyUnaryNumericOperation (pobj: PObject) operator opMods =
        match pobj with
        | :? PVariable as v -> this.ApplyUnaryNumericOperation v.CapturedValue operator opMods
        | :? PReference as r -> this.ApplyUnaryNumericOperation r.CapturedValue operator opMods
        | :? PNumber as n -> operator n
        | :? PList as l -> l |> Seq.map(fun x -> this.ApplyUnaryNumericOperation x operator opMods) |> PList :> PObject
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise
        
    member this.ApplyUnaryListOperation (pobj: PObject) operator opMods : PObject =
        match pobj with
        | :? PVariable as v -> this.ApplyUnaryListOperation v.CapturedValue operator opMods
        | :? PReference as r -> this.ApplyUnaryListOperation r.CapturedValue operator opMods
        | :? PList as l -> operator l
        | :? PNumber as n -> this.ApplyUnaryListOperation (n :> PObject |> Seq.singleton |> PList) operator opMods
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member this.ApplyBinaryNumericOperation (left: PObject) (right: PObject) operator opMods : PObject =
        match left, right with
        | (:? PVariable as v), _ -> this.ApplyBinaryNumericOperation v.CapturedValue right operator opMods
        | _, (:? PVariable as v) -> this.ApplyBinaryNumericOperation left v.CapturedValue operator opMods
        | (:? PReference as r), _ -> this.ApplyBinaryNumericOperation r.CapturedValue right operator opMods
        | _, (:? PReference as r) -> this.ApplyBinaryNumericOperation left r.CapturedValue operator opMods
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

    member this.ApplyBinaryListOperation (left: PObject) (right: PObject) operator opMods : PObject =
        match left, right with
        | (:? PVariable as v), _ -> this.ApplyBinaryListOperation v.CapturedValue right operator opMods
        | _, (:? PVariable as v) -> this.ApplyBinaryListOperation left v.CapturedValue operator opMods
        | (:? PReference as r), _ -> this.ApplyBinaryListOperation r.CapturedValue right operator opMods
        | _, (:? PReference as r) -> this.ApplyBinaryListOperation left r.CapturedValue operator opMods
        | (:? PList as l1), (:? PList as l2) -> 
            operator(l1, l2)
        | (:? PNumber as n), (:? PList as l) -> 
            this.ApplyBinaryListOperation (n :> PObject |> Seq.singleton |> PList) right operator opMods
        | (:? PList as l), (:? PNumber as n) -> 
            this.ApplyBinaryListOperation left (n :> PObject |> Seq.singleton |> PList) operator opMods
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            this.ApplyBinaryListOperation (n1 :> PObject |> Seq.singleton |> PList) (n2 :> PObject |> Seq.singleton |> PList) operator opMods
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member this.ApplyListNumericOperation (pList: PObject) (pNumber: PObject) operator opMods : PObject =
        match pList, pNumber with
        | (:? PVariable as v), _ -> this.ApplyListNumericOperation v.CapturedValue pNumber operator opMods
        | _, (:? PVariable as v) -> this.ApplyListNumericOperation pList v.CapturedValue operator opMods
        | (:? PReference as r), _ -> this.ApplyListNumericOperation r.CapturedValue pNumber operator opMods
        | _, (:? PReference as r) -> this.ApplyListNumericOperation pList r.CapturedValue operator opMods
        | (:? PList as l), (:? PNumber as n) -> 
            operator(l, n)
        | (:? PList as l1), (:? PList as l2) -> 
            l2 |> Seq.map(fun x -> this.ApplyListNumericOperation l1 x operator opMods) |> PList :> PObject
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            this.ApplyListNumericOperation (n1 :> PObject |> Seq.singleton |> PList) pNumber operator opMods
        | (:? PNumber as n), (:? PList as l) -> 
            this.ApplyListNumericOperation (n :> PObject |> Seq.singleton |> PList) l operator opMods
        | _ -> PrimellProgrammerProblemException("Not possible") |> raise

    member this.IsTruth(pobj: PObject, truthDef: TruthDefinition) =
      match pobj with
      | :? PReference as r -> this.IsTruth(r.CapturedValue, truthDef)
      | :? PList as l when l.IsEmpty -> truthDef.EmptyIsTruth
      | :? PList as l ->  // infinite recursion is possible with infinite lists
          if truthDef.RequireAllTruth then
            l |> Seq.exists(fun x -> this.IsTruth(x, truthDef) |> not) |> not
              // TODO - since I'm piping a few times here, this probably isn't tail recursion
          else
            l |> Seq.exists(fun x -> this.IsTruth(x, truthDef))
      | :? PNumber as n -> 
          if truthDef.PrimesAreTruth then
            PrimeLib.IsPrime n.Value
          else
            match n.Value with
            | NaN -> false
            | _ as v -> not v.IsZero
      | _ -> PrimellProgrammerProblemException("Not possible") |> raise


    member this.Conditional (left: PObject) (right: PObject) (negate: bool) =
      if this.IsTruth(left, control.Settings.TruthDefinition) <> negate then this.UnaryListOperators["_<"] else this.UnaryListOperators["_>"]

    

    