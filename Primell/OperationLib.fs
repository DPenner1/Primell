namespace dpenner1.PrimellF

open System.Collections.Generic

// TODO - this is probably better as an enum (if that exists in F#)
type OperationModifier = 
  | Power

module rec OperationLib =

    // for now these are immutable dicts, but they might be changed to mutable Dictionary on implementation of user-defined operators
    let public UnaryNumericOperators: IDictionary<string, PNumber->IPrimellObject> = 
      dict ["~", fun n -> ExtendedBigRational.(~-) n.Value |> PNumber :> IPrimellObject
            "++", fun n -> PrimeLib.NextPrime n.Value |> PNumber :> IPrimellObject
            "--", fun n -> PrimeLib.PrevPrime n.Value |> PNumber :> IPrimellObject
           ]

    let public UnaryListOperators: IDictionary<string, PList->IPrimellObject> = 
      dict ["_<", fun (l: PrimellList) -> l.Head()
            "_>", fun (l: PrimellList) -> l.Tail()        
            "_~", fun (l: PrimellList) -> l.Reverse()
           ]

    let public BinaryNumericOperators: IDictionary<string, PNumber*PNumber->IPrimellObject> = 
      dict ["..", fun (left, right) -> PrimeLib.PrimeRange left.Value right.Value |> Seq.map(fun n -> n |> PNumber :> IPrimellObject) |> PList :> IPrimellObject
            "+",  fun (left, right) -> ExtendedBigRational.(+)(left.Value, right.Value) |> PNumber :> IPrimellObject
            "-",  fun (left, right) -> ExtendedBigRational.(-)(left.Value, right.Value) |> PNumber :> IPrimellObject
            
           ]
    
    // TODO - I don't have any Binary List Operators implemented yet
    let public BinaryListOperators: IDictionary<string, PList*PList->IPrimellObject> = 
      dict ["\\",  fun (left: PrimellList, right: PrimellList) -> PrimellList.Empty
           ]

    // opMods for consistency, but I don't think Primell will have any need for opMods on unary numeric operators
    let rec ApplyUnaryNumericOperation (pobj: IPrimellObject) operator opMods =
        match pobj with
        | :? PNumber as n -> operator n
        | :? PList as l -> l |> Seq.map(fun x -> ApplyUnaryNumericOperation x operator opMods) |> PList :> IPrimellObject
        | _ -> failwith "Not possible"
        
    let rec ApplyUnaryListOperation (pobj: IPrimellObject) operator opMods : IPrimellObject =
        match pobj with
        | :? PList as l -> operator l
        | :? PNumber as n -> ApplyUnaryListOperation (n :> IPrimellObject |> Seq.singleton |> PList) operator opMods
        | _ -> failwith "Not possible"

    let rec ApplyBinaryNumericOperation (left: IPrimellObject) (right: IPrimellObject) operator opMods : IPrimellObject =
        match left, right with
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            operator(n1, n2)
        | (:? PNumber as n), (:? PList as l) -> 
            l |> Seq.map(fun x -> ApplyBinaryNumericOperation n x operator opMods) |> PList :> IPrimellObject
        | (:? PList as l), (:? PNumber as n) -> 
            l |> Seq.map(fun x -> ApplyBinaryNumericOperation x n operator opMods) |> PList :> IPrimellObject
        | (:? PList as l1), (:? PList as l2) -> 
            (l1, l2) ||> Seq.map2 (fun x y -> ApplyBinaryNumericOperation x y operator opMods) |> PList :> IPrimellObject
            // TODO - F# truncates to the shortest list, but Primell's default is to virtually extend the shorter list with Emptys
            // Interesting solution provided here that could be adapted: https://stackoverflow.com/a/2840062/1607043
        | _ -> failwith "Not possible"

    let rec ApplyBinaryListOperation (left: IPrimellObject) (right: IPrimellObject) operator opMods : IPrimellObject =
        match left, right with
        | (:? PList as l1), (:? PList as l2) -> 
            operator(l1, l2)
        | (:? PNumber as n), (:? PList as l) -> 
            ApplyBinaryListOperation (n :> IPrimellObject |> Seq.singleton |> PList) right operator opMods
        | (:? PList as l), (:? PNumber as n) -> 
            ApplyBinaryListOperation left (n :> IPrimellObject |> Seq.singleton |> PList) operator opMods
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            ApplyBinaryListOperation (n1 :> IPrimellObject |> Seq.singleton |> PList) (n2 :> IPrimellObject |> Seq.singleton |> PList) operator opMods
        | _ -> failwith "Not possible"

    let rec ApplyListNumericOperation (pList: IPrimellObject) (pNumber: IPrimellObject) operator opMods : IPrimellObject =
        match pList, pNumber with
        | (:? PList as l), (:? PNumber as n) -> 
            operator l n
        | (:? PList as l1), (:? PList as l2) -> 
            failwith "I need more coffee before figuring out this case"
        | (:? PNumber as n1), (:? PNumber as n2) -> 
            ApplyListNumericOperation (n1 :> IPrimellObject |> Seq.singleton |> PList) pNumber operator opMods
        | (:? PNumber as n), (:? PList as l) -> 
            failwith "I need more coffee before figuring out this case"
        | _ -> failwith "Not Possible"

    let rec IsTruth(pobj: IPrimellObject, truthDef: TruthDefinition) =
      match pobj with
      | :? PList as l when l.IsEmpty -> truthDef.EmptyIsTruth
      | :? PList as l ->  // infinite recursion is possible with infinite lists
          if truthDef.RequireAllTruth then
            l |> Seq.exists(fun x -> IsTruth(x, truthDef) |> not) |> not
              // TODO - since I'm piping a few times here, this probably isn't tail recursion
          else
            l |> Seq.exists(fun x -> IsTruth(x, truthDef))
      | :? PNumber as n -> 
          if truthDef.PrimesAreTruth then
            PrimeLib.IsPrime n.Value
          else
            match n.Value with
            | NaN -> false
            | _ as v -> not v.IsZero
      | _ -> failwith "Not possible"

    let public Conditional (left: IPrimellObject) (right: IPrimellObject) truthDef =
      if IsTruth(left, truthDef) then UnaryListOperators["_<"] else UnaryListOperators["_>"]

    let public NegativeConditional (left: IPrimellObject) (right: IPrimellObject) truthDef =
      if IsTruth(left, truthDef) then UnaryListOperators["_>"] else UnaryListOperators["_<"]
