namespace dpenner1.PrimellF

// really, a Record type would work great here, but due to all the nested typing, 
// I just want the ability for a single-value constructor for easy piping |>
type PrimellNumber(value: ExtendedBigRational, ?parent: PObject, ?indexInParent: int) =
  inherit PObject(?parent = parent, ?indexInParent = indexInParent)
  member this.Value with get() = value

  override this.WithParent(parent: PObject, indexInParent: int) =
    PrimellNumber(this.Value, parent, indexInParent)

  override this.ToString() = this.Value.ToString()

type PNumber = PrimellNumber  // abbreviation for sanity