namespace dpenner1.PrimellF


[<AbstractClass>]
type PrimellObject(?parent: PrimellObject, ?indexInParent: int) =
  // these are (probably) necessary for keeping track of stuff for assignment & indexing 
  member this.Parent with get() = parent 
  member this.IndexInParent with get() = indexInParent
  abstract member WithParent: PrimellObject * int -> PrimellObject
type PObject = PrimellObject



(* A functional approach to reference maybe? It's hard to avoid, because Primell does have mutable variables. Consider the program:

x = (y 3 5)
y = 2
x
  that last line should print (2 3 5) and its hard to do that without some indirection
  I can't make list mutable: I've committed to seqs as a design choice knowing the variable mutability would be tough,
  but I could make the contents of y be a reference to something mutable
*)

type PrimellReference(name: string, ?parent: PrimellObject, ?indexInParent: int) =
  inherit PObject(?parent = parent, ?indexInParent = indexInParent)

  member this.Name with get() = name

  // i don't think this ones really ever important
  override this.WithParent(parent: PrimellObject, indexInParent: int) =
    PrimellReference(this.Name, parent, indexInParent)

type PReference = PrimellReference
