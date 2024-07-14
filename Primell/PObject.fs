namespace dpenner1.PrimellF

// TODO - for some reason couldn't use the ?name syntax otherwise inheritance didn't work????
type IPrimellObject(name: Option<string>, parent: Option<IPrimellObject>, indexInParent: Option<int>) =

  // these are (probably) necessary for keeping track of stuff for assignment & indexing 
  member this.Name with get() = name
  member this.Parent with get() = parent
  member this.IndexInParent with get() = indexInParent
