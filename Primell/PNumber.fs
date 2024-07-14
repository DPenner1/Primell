namespace dpenner1.PrimellF

// really, a Record type would work great here, but due to all the nested typing, 
// I just want the ability for a single-value constructor for easy piping |>
type PrimellNumber(value: ExtendedBigRational, ?name: string, ?parent: IPrimellObject, ?indexInParent: int) =
  inherit IPrimellObject(?name = name, ?parent = parent, ?indexInParent = indexInParent)
  member this.Value with get() = value

  override this.ToString() = this.Value.ToString()

  override this.WithValueOnly() = PrimellNumber value

type PNumber = PrimellNumber  // abbreviation for sanity