namespace dpenner1.PrimellF

type PObject =
  | Atom of PNumber
  | List of seq<PObject>  // Ok a bit weird to call the seq a List, but conceptually, that's how I view the Primell object, as lists
  // Empty was considered, but seems would cause unnecessary ambiguity with the possibility of List being an empty sequence

  member this.IsEmpty =
    match this with
    | Atom _ -> false
    | List x -> Seq.isEmpty x

  static member Normalize(p: PObject) =
    match p with
    | List x when Seq.length x = 1 -> Seq.head x
    | _ -> p