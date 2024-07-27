namespace dpenner1.Primell

[<Sealed>]
// Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
type PrimellList(sequence: seq<PObject>, ?length: PNumber, ?refersTo: Reference) = 
  inherit PObject(?refersTo = refersTo)
  let main = sequence  // star
  let mutable length = length
  // TODO - should guard against negative lengths

  interface seq<PObject> with
    member this.GetEnumerator() = main.GetEnumerator()
    member this.GetEnumerator() = main.GetEnumerator() :> System.Collections.IEnumerator   // why didn't they get rid of this with .NET core?
  
  
  static member Empty = Seq.empty |> PrimellList
  
  member this.Length with get() =
    match length with   // important to use match and not equality due to potential weirdness with NaN != NaN
    | None ->   
        length <- Seq.length main |> BigRational |> Rational |> PNumber |> Some
    | _ -> ()
    length.Value

  member this.IsGreaterOrEqualThanLength(value: int) =
    match main |> Seq.tryItem value with
    | None -> true
    | _ -> false
    

  member this.IsEmpty with get() = Seq.isEmpty main   // avoid calling this.Length due to potential long computation

  member this.Head() = 
    match Seq.tryHead main with
    | Some head -> head
    | None -> PrimellList.Empty
    
  // this is a bit tortured, but working around four things here
  //   - F# throws exceptions when Primell needs to return empty list
  //   - F# lib has a Seq.tryHead but not Seq.tryTail ??? 
  //   - Primell automatically unboxes one item lists
  //   - infinite sequences are possible so I can't call Seq.length
  member this.Tail() = 
    match this.IsEmpty with
    | true -> PrimellList.Empty :> PObject
    | false -> 
        let tail = Seq.tail main |> PrimellList
        match Seq.tryHead tail with
        | Some head when Seq.isEmpty (Seq.tail tail) -> // tail had one item in it, Primell unboxes it
            head
        | _ -> tail


  // TODO - in future, could have a constructor which has the reversed list as an argument, for infinite sequences
  member this.Reverse() = Seq.rev main |> PrimellList

  // TODO - i know Lists are O(1) on prepend and not append, but how do Seqs behave on append vs prepend?
  // TODO - the Seq.singleton causes some incorrect boxing thats masked by a later Normalize
  member this.Append (pobj: PObject) = Seq.append this (Seq.singleton pobj) |> PrimellList

  // TODO - is there a way to more cleanly do this without drilling into types?
  member this.AppendAll (pobj: PObject) =
    match pobj with
    | :? PrimellList as l -> Seq.append this l |> PrimellList
    | _ -> pobj |> Seq.singleton |> PrimellList |> this.AppendAll

  

  member this.Index(index: PNumber) =
    match ExtendedBigRational.Round index.Value with
    | NaN ->  // Makes sense, trying to index with NaN means you don't get anything
        PrimellList.Empty :> PObject  
    | Infinity _ -> 
        // I don't like this one... in theory there can be something way out there, but in general, it's undefined
        PrimellList.Empty :> PObject 
    | Rational r ->
          if r.Sign = -1 then  // index from end
            if this.IsGreaterOrEqualThanLength(int r.Numerator - 1) then
              PrimellList.Empty :> PObject
            else 
              this.Reverse() |> Seq.skip (int r.Numerator - 1) |> Seq.head
          else
            let effectiveIndex =  r.Numerator |> BigRational |> Rational
            if this.IsGreaterOrEqualThanLength(int r.Numerator) then 
              PrimellList.Empty :> PObject 
            else
              main |> Seq.skip (int r.Numerator) |> Seq.head

  
  override this.ToString() =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString())); ")"]

  override this.ToString(variables) =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString(variables))); ")"]

  override this.WithReference(ref) = PrimellList(main, ?length = length, ?refersTo = Some ref)

type PList = PrimellList  // abbreviation for sanity


(*
  interesting issue cam up with previous implementation of referencing simply being an object and an index *number* Consider:
  x = (2 3 5 7)
  x@(2 3) = (11 13)

  intuitively, you want this to result in x = (2 3 11 13). However with a singular index number and immutability, this was
  done in two steps, first producing (2 3 11 7), then the reference with index 3 necessarily has a stale value and overwrites
  to (2 3 5 13)... so we need to make assignments to the referenced object all at once... but with infinite lists how do we know
  that we have all the references (eg think x@(2..inf)), so we can't really do it one at a time that way
  so instead of a singular index number, we use the whole index object at once, eg. (2 3) or (2..inf) per the examples, which should
  allow us to defer evaluating an infinite list on assign... thats going to be tricky
*)
(*
type PrimellReference(referencedObject: PObject, referenceIndex: PObject, capturedValue: PObject) =
  inherit PObject()

  member val ReferencedObject = referencedObject with get
  member val ReferenceIndex = referenceIndex with get
  member val CapturedValue = capturedValue with get
  // technically could prob recurse up to get it, but there were issues beyond the recursion itself that made that tricky...

(*  Old stuff with for dynamic variables that may be useful later
  member private this.IndexDown(pobj: PObject)(indexes: list<PNumber>) =
    match List.tryHead indexes with
    | None -> pobj // end of recursion
    | Some head ->
        match pobj with
        | :? PList as l -> this.IndexDown(l.Index head)(indexes.Tail)
        | :? PAtom as a -> this.IndexDown(Seq.singleton(a :> PObject) |> PList) indexes
        | _ -> PrimellProgrammerProblemException "Not possible" |> raise

  member private this.GetReferenceValue' (pref: PObject)(indexes: list<PNumber>)(variables: System.Collections.Generic.IDictionary<string, PObject>) =
    match pref with
    | :? PVariable as v -> this.IndexDown(variables[v.Name]) indexes
    | :? PrimellReference as r -> this.GetReferenceValue' r.Parent (r.IndexInParent::indexes) variables
    | _ -> PrimellProgrammerProblemException "Not possible" |> raise

  member this.Dereference(variableValues: System.Collections.Generic.IDictionary<string, PObject>) =
    this.GetReferenceValue' this.Parent (List.singleton this.IndexInParent) variableValues *)

  override this.ToString() = this.CapturedValue.ToString()

  override this.ToString(variables) = this.CapturedValue.ToString()


type PReference = PrimellReference
*)