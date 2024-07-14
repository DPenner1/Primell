namespace dpenner1.PrimellF

// TODO - for some reason couldn't use the ?name syntax otherwise inheritance didn't work????
//type IPrimellObject(name: string, parent: Option<IPrimellObject>, indexInParent: Option<int>) =
[<AbstractClass>]
type IPrimellObject(?name: string, ?parent: IPrimellObject, ?indexInParent: int) =
  // these are (probably) necessary for keeping track of stuff for assignment & indexing 
  member this.Name with get() = name
  member this.Parent with get() = parent 
  // technically, parent is always a PList... but then we would have three mutually referencing types, so I'll downcast the rare time that's needed

  member this.IndexInParent with get() = indexInParent

  abstract member WithValueOnly: unit -> IPrimellObject

