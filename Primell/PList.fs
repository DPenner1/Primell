namespace dpenner1.PrimellF

[<Sealed>]
// Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
type PrimellList(sequence: seq<PObject>, ?length: PNumber) = 
  inherit PObject()
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

  member this.IsEmpty with get() = Seq.isEmpty main   // avoid calling this.Length due to potential long computation

  member this.Value with get() = main

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
    | NaN -> PrimellList.Empty :> PObject  // Makes sense, trying to index with NaN means you don't get anything
    | Infinity _ -> PrimellList.Empty :> PObject  // I don't like this one... For infinite lists, in theory there is something way out there, but it's undefined
    | Rational r ->
          if r.Sign = -1 then  // index from end
            let effectiveIndex =  r.Numerator - 1I |> BigRational |> Rational  // TODO is this logic correct?
            if effectiveIndex >= this.Length.Value then  // TODO - a lot of code copying here
              PrimellList.Empty :> PObject
            else this.Reverse() |> Seq.skip (int r.Numerator - 1) |> Seq.head
          else
            let effectiveIndex =  r.Numerator |> BigRational |> Rational
            if effectiveIndex >= this.Length.Value then 
              PrimellList.Empty :> PObject
            else
              main |> Seq.skip (int r.Numerator) |> Seq.head

  
  override this.ToString() =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString())); ")"]

  override this.ToString(variables) =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString(variables))); ")"]

type PList = PrimellList  // abbreviation for sanity



type PrimellReference(parent: PObject, indexInParent: PNumber) =
  inherit PObject()

  member this.Parent with get() = parent
  member this.IndexInParent with get() = indexInParent

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
    this.GetReferenceValue' this.Parent (List.singleton this.IndexInParent) variableValues

  override this.ToString() =
    String.concat "" [this.Parent.ToString(); "@"; this.IndexInParent.ToString()]

  override this.ToString(variables) =
    (this.GetReferenceValue' this.Parent (List.singleton this.IndexInParent) variables).ToString(variables)


type PReference = PrimellReference