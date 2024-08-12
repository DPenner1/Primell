namespace dpenner1.Primell

open dpenner1.Math

// really, a Record type would work great here, but due to all the nested typing, 
// I just want the ability for a single-value constructor for easy piping |>
type PrimellNumber(value: ExtendedBigRational, ?refersTo: Reference) =
  inherit PAtom(?refersTo = refersTo)
  member val Value = value with get
  override this.ToString() = this.Value.ToString()

  override this.WithReference(ref) = PrimellNumber(value, ref)

  member this.NaNAwareCompare(other: PrimellNumber) = 
    match this.Value, other.Value with
    | NaN, NaN -> -1   // as long as its not zero
    | _, NaN -> 1
    | NaN, _ -> -1
    | _ ->
        if this.Equals(other.Value) then 0
        elif this.Value < other.Value then -1
        else 1

  override this.NaNAwareEquals pobj =
    match pobj with
    | :? PrimellNumber as n ->
        match this.Value, n.Value with
        | NaN, _ | _, NaN -> false // always false
        | _ -> this.Value.Equals n.Value
    | _ -> false

  override this.Equals(other) =
    match other with
    | :? PrimellNumber as n -> this.Value.Equals n.Value
    | _ -> false

  override this.GetHashCode() =
    this.Value.GetHashCode()

type PNumber = PrimellNumber  // abbreviation for sanity

