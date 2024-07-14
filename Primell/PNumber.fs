namespace dpenner1.PrimellF

// really, a Record type would work great here, but due to all the nested typing, 
// I just want the ability for a single-value constructor for easy piping |>
type PrimellNumber(value: ExtendedBigRational, ?name: string, ?parent: IPrimellObject, ?indexInParent: int) =
  inherit IPrimellObject(name, parent, indexInParent)
  member this.Value with get() = value

type PNumber = PrimellNumber  // abbreviation for sanity