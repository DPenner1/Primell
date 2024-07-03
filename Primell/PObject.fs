namespace dpenner1.PrimellF

type PObject =
  | Atom of PNumber
  | List of seq<PObject>  // Ok a bit weird to call the seq a List, but conceptually, that's how I view the Primell object, as lists
  // Empty was considered, but seems would cause unnecessary ambiguity with the possibility of List being an empty sequence

  static member Empty =
    List Seq.empty

  member this.IsEmpty =
    match this with
    | Atom _ -> false
    | List l -> Seq.isEmpty l

  member this.Length =
    match this with
    | Atom _ -> PNumber.One
    | List l -> Rational(bigint(Seq.length l), 1I)

  static member Normalize(p: PObject) =
    match p with
    | List l when Seq.length l = 1 -> Seq.head l |> PObject.Normalize
    | _ -> p