namespace dpenner1.Primell

// really, a Record type would work great here, but due to all the nested typing, 
// I just want the ability for a single-value constructor for easy piping |>
type PrimellNumber(value: ExtendedBigRational, ?refersTo: Reference) =
  inherit PAtom(?refersTo = refersTo)
  member this.Value with get() = value
  override this.ToString() = this.Value.ToString()

  override this.WithReference(ref) = PrimellNumber(value, ref)

type PNumber = PrimellNumber  // abbreviation for sanity

