namespace dpenner1.PrimellF

type OperationModifier = 
  | Todo

module rec OperationLib =

    // for now these are immutable dicts, but they might be changed to mutable Dictionary on implementation of user-defined operators
    let public UnaryNumericOperators = 
      dict ["~", fun n -> PNumber.(~-) n |> Atom
            "++", fun n -> PrimeLib.NextPrime n |> Atom
            "--", fun n -> PrimeLib.PrevPrime n |> Atom
           ]

    let public UnaryListOperators = 
      dict ["_>", fun (l: PrimellList) -> l.Tail() |> PrimellList |> PList
            "_<", fun (l: PrimellList) -> l.Head()
            "_~", fun (l: PrimellList) -> l.Reverse() |> PrimellList |> PList
           ]

    let public BinaryNumericOperators = 
      dict ["+",  fun (left, right) -> PNumber.(+)(left, right) |> Atom
            "-",  fun (left, right) -> PNumber.(-)(left, right) |> Atom
            "..", fun (left, right) -> PNumber.Range left right |> Seq.map(fun n -> n |> Atom) |> PrimellList |> PList
           ]
    
    // TODO - I don't have any Binary List Operators implemented yet
    let public BinaryListOperators = 
      dict ["\\",  fun (left: PrimellList, right: PrimellList) -> PObject.Empty
           ]

    // opMods for consistency, but I don't think Primell will have any need for opMods on unary numeric operators
    let rec ApplyUnaryNumericOperation pobj operator opMods =
        match pobj with
        | Atom a -> operator a
        | PList l -> l |> Seq.map(fun x -> ApplyUnaryNumericOperation x operator opMods) |> PrimellList |> PList
        
    let rec ApplyUnaryListOperation pobj operator opMods : PrimellObject =
        match pobj with
        | PList l -> operator l
        | Atom a -> ApplyUnaryListOperation (Atom a |> Seq.singleton |> PrimellList |> PList) operator opMods

    let rec ApplyBinaryNumericOperation left right operator opMods : PrimellObject =
        match left, right with
        | Atom a1, Atom a2 -> 
            operator(a1, a2)
        | Atom a, PList l -> 
            l |> Seq.map(fun x -> ApplyBinaryNumericOperation (Atom a) x operator opMods) |> PrimellList |> PList
        | PList l, Atom a -> 
            l |> Seq.map(fun x -> ApplyBinaryNumericOperation x (Atom a) operator opMods) |> PrimellList |> PList
        | PList l1, PList l2 -> 
            (l1, l2) ||> Seq.map2 (fun x y -> ApplyBinaryNumericOperation x y operator opMods) |> PrimellList |> PList
            // TODO - F# truncates to the shortest list, but Primell's default is to virtually extend the shorter list with Emptys
            // Interesting solution provided here that could be adapted: https://stackoverflow.com/a/2840062/1607043

    let rec ApplyBinaryListOperation left right operator opMods : PrimellObject =
        match left, right with
        | PList l1, PList l2 -> 
            operator(l1, l2)
        | Atom a, PList l -> 
            ApplyBinaryListOperation (Atom a |> Seq.singleton |> PrimellList |> PList) right operator opMods
        | PList l, Atom a -> 
            ApplyBinaryListOperation left (Atom a |> Seq.singleton |> PrimellList |> PList) operator opMods
        | Atom a1, Atom a2 -> 
            ApplyBinaryListOperation (Atom a1 |> Seq.singleton |> PrimellList |> PList) (Atom a2 |> Seq.singleton |> PrimellList |> PList) operator opMods

    let rec ApplyListNumericOperation pList pNumber operator opMods : PrimellObject =
        match pList, pNumber with
        | PList l, Atom a -> 
            operator l a
        | PList l1, PList l2 -> 
            failwith "I need more coffee before figuring out this case"
        | Atom a1, Atom a2 -> 
            ApplyListNumericOperation (Atom a1 |> Seq.singleton |> PrimellList |> PList) pNumber operator opMods
        | Atom a, PList l -> 
            failwith "I need more coffee before figuring out this case"

    let rec IsTruth(pobj: PrimellObject, truthDef: TruthDefinition) =
      if pobj.IsEmpty then
        truthDef.EmptyIsTruth
      else 
        match pobj with
        | Atom a -> 
            if truthDef.PrimesAreTruth then
              PrimeLib.IsPrime a
            else
              match a with
              | NaN -> false
              | _ -> not a.IsZero

        | PList l ->  // infinite recursion is possible with infinite lists
            if truthDef.RequireAllTruth then
              l |> Seq.exists(fun x -> IsTruth(x, truthDef) |> not) |> not
              // TODO - since I'm piping a few times here, this probably isn't tail recursion
            else
              l |> Seq.exists(fun x -> IsTruth(x, truthDef))
                       

    let public Conditional (left: PObject) (right: PObject) truthDef =
      if IsTruth(left, truthDef) then right.Head() else right.Tail()

    let public NegativeConditional (left: PObject) (right: PObject) truthDef =
      if IsTruth(left, truthDef) then right.Tail() else right.Head()