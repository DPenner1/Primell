namespace dpenner1.PrimellF

exception PrimellProgrammerProblemException of string

[<AbstractClass>]
type PrimellObject(?parent: PrimellObject, ?indexInParent: int) =
  // these are (probably) necessary for keeping track of stuff for assignment & indexing 
  member this.Parent with get() = parent 
  member this.IndexInParent with get() = indexInParent
  abstract member WithParent: PrimellObject * int -> PrimellObject
  
  abstract member ToString: System.Collections.Generic.IDictionary<string, PrimellObject> -> string
  default this.ToString(variables) =
    this.ToString()

type PObject = PrimellObject

[<AbstractClass>]
type PrimellAtom(?parent: PrimellObject, ?indexInParent: int) =
  inherit PObject(?parent = parent, ?indexInParent = indexInParent)

type PAtom = PrimellAtom

(* A functional approach to reference maybe? It's hard to avoid, because Primell does have mutable variables. Consider the program:

x = (y 3 5)
y = 2
x

  that last line could print (2 3 5) and its hard to do that without some indirection
  I can't make list mutable: I've committed to seqs as a design choice knowing the variable mutability would be tough,
  but I could make the contents of y be a reference to something mutable

  But what if we wanted to evaluate? if immediately evaluated, that would () = (() 3 5), 
  Re-reading old comments, Primell was immediately evaluated, but the initial x = () evaluation didn't prevent () 
  from being assigned and referenced -> so that's the re-implementation for now
*)

// note: its actually been a bit cumbersome having indirection for mutability,
//       might try to directly store the object boxed here mutably
type PrimellReference(name: string, ?parent: PrimellObject, ?indexInParent: int) =
  inherit PAtom(?parent = parent, ?indexInParent = indexInParent)

  member this.Name with get() = name

  // i don't think this ones really ever important
  override this.WithParent(parent: PrimellObject, indexInParent: int) =
    PrimellReference(this.Name, parent, indexInParent)

  override this.ToString(variables) =
    if variables.ContainsKey(name) then
      variables[name].ToString()
    else "()"  // just hack upon hack now (because we've boxed we've not attempted to retrieve the thing...)

type PReference = PrimellReference
