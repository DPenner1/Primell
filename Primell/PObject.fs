namespace dpenner1.Primell

exception PrimellProgrammerProblemException of string

[<AbstractClass>]
type PrimellObject() =
  // these are (probably) necessary for keeping track of stuff for assignment & indexing 
  abstract member ToString: System.Collections.Generic.IDictionary<string, PrimellObject> -> string
  default this.ToString(variables) =
    this.ToString()

type PObject = PrimellObject

[<AbstractClass>]
type PrimellAtom() =  // in future there might be more atomic items, in particular first-class operators
  inherit PObject()

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
type PrimellVariable(name: string, capturedValue: PObject) =
  inherit PObject()

  member val Name = name with get
  member val CapturedValue = capturedValue with get

  override this.ToString() = this.CapturedValue.ToString()
    
  override this.ToString(variables) =
    if variables.ContainsKey(name) then
      variables[name].ToString()
    else "()"  // just hack upon hack now (because we've boxed we've not attempted to retrieve the thing...)

type PVariable = PrimellVariable

// references are a way to deal with indexing mutability (and is more of an implementation detail instead of a object within Primell)
// so if you have a variable x, and index it at index i (x@i), the parser simply generates a reference with values x and i, 
// figuring assignments out based on that later. References can also reference other references [citation needed]
