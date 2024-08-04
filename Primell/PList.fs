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

  member val private OptLength = length with get

  member this.IsGreaterOrEqualThanLength(value: int) =
    match main |> Seq.tryItem value with
    | None -> true
    | _ -> false

  member val IsEmpty = Seq.isEmpty main with get

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
        let newLength = 
          match length with 
          | None -> length 
          | Some l -> Some(l.Value - ExtendedBigRational.One |> PNumber)
        let tail = PrimellList(Seq.tail main, ?length = newLength)

        match Seq.tryHead tail with
        | Some head when Seq.isEmpty (Seq.tail tail) -> // tail had one item in it, Primell unboxes it
            head
        | _ -> tail


  member this.Reverse() = PrimellList(Seq.rev main, ?length = length)

  // TODO - i know Lists are O(1) on prepend and not append, but how do Seqs behave on append vs prepend?
  // TODO - the Seq.singleton causes some incorrect boxing thats masked by a later Normalize
  member this.Append (pobj: PObject) = 
    let newLength = 
      match length with 
      | None -> length 
      | Some l -> Some(l.Value + ExtendedBigRational.One |> PNumber)
    
    PrimellList(Seq.append this (Seq.singleton pobj), ?length = newLength)


  member this.AppendAll (pobj: PObject) =
    match pobj with
    | :? PrimellList as l -> 
        let newLength = 
          match length, l.OptLength with
          | Some x, Some y -> Some(x.Value + y.Value |> PNumber)  // known lengths, just add them
          // if one is known to be infinite length, that overrides
          | Some x, _ -> 
              match x.Value with 
              | Infinity Positive -> Some(Infinity Positive |> PNumber) 
              | _ -> None
          | _, Some y ->
              match y.Value with
              | Infinity Positive -> Some(Infinity Positive |> PNumber) 
              | _ -> None
          | _ -> None
        // reference 
        PrimellList(Seq.append this l, ?length = newLength)  
    | _ -> 
        let newLength = 
          match length with 
          | None -> length 
          | Some l -> Some(l.Value + ExtendedBigRational.One |> PNumber)

        PrimellList(Seq.append main (Seq.singleton pobj), ?length = newLength)


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
            if this.IsGreaterOrEqualThanLength(int r.Numerator) then 
              PrimellList.Empty :> PObject 
            else
              main |> Seq.skip (int r.Numerator) |> Seq.head

  member this.Cons(pObj: PObject) =
    main |> Seq.insertAt 0 pObj |> PrimellList

  member private this.RaiseAtoms() =
    main |> Seq.map(fun x -> 
      match x with 
      | :? PrimellList as l -> l :> seq<PObject>
      | _ as x -> Seq.singleton x)

  member this.Flatten() =
    this.RaiseAtoms() |> Seq.concat |> PrimellList
  
  override this.WithReference(ref) = PrimellList(main, ?length = length, ?refersTo = Some ref)

  override this.ToString() =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString())); ")"]

  override this.Equals(other) =
    match other with
    | :? PrimellList as l ->
        if l.Length <> this.Length then false  // TODO - just to get it going, but with infinite possible, you can do better
        else
          (this, l) ||> Seq.exists2 (fun x y -> x.Equals y |> not) |> not
    | _ -> false

  override this.GetHashCode() =
    let truncInfTo = 16  // arbitrary, but we definitely don't want to evaluate infinitely here
    
    match length with
    | None ->
        main |> Seq.truncate truncInfTo  
    | Some n -> 
        match n.Value with
        | Infinity _ -> main |> Seq.truncate truncInfTo  
        | _ -> main
    |> Seq.fold (fun hashcode x -> hashcode ^^^ x.GetHashCode()) 0

type PList = PrimellList  // abbreviation for sanity


