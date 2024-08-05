namespace dpenner1.Primell

exception PrimellProgrammerProblemException of string

[<AbstractClass>]
type PrimellObject internal (?refersTo: Reference) =

  // I really don't like this one: In mutable paradigm, this would just be implemented right here.
  // But in immutable, it effectively requires a copy constructor to get the object with a reference
  // and in principle, without having reference to downstream types, this forces them
  // to implement what is effectively a mechanical parsing detail
  abstract member WithReference: Reference -> PrimellObject

  member val Reference = defaultArg refersTo Void with get

  abstract member NaNAwareEquals: PrimellObject -> bool
  default this.NaNAwareEquals pobj = this.Equals(pobj)

// references are a way to deal with indexing mutability (and is more of an implementation detail instead of a object within Primell)
// so if you have a variable x, and index it at index i (x@i), the parser simply generates a reference with values x and i, 
// figuring assignments out based on that later. References can also reference other references [citation needed]

and Reference =  // TODO - naming, not sure i like type and one of the options being named the same
  | Variable of string
  | Reference of ReferencedObject: PrimellObject * ReferenceIndex: PrimellObject
  | Void   // I could have gone without this and then done option<Reference>, but this feels like it better reflects the intent

type PObject = PrimellObject

[<AbstractClass>]
type PrimellAtom(?refersTo: Reference) =  // in future there might be more atomic items, in particular first-class operators
  inherit PObject(?refersTo = refersTo)

type PAtom = PrimellAtom


