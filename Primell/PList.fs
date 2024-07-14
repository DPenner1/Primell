namespace dpenner1.PrimellF



// Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
type PrimellList(sequence: seq<PObject>, ?length: PNumber, ?parent: PObject, ?indexInParent: int) = 
  inherit PObject(?parent = parent, ?indexInParent = indexInParent)
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

  member this.Head() = Seq.head main
    
  member this.Tail() = Seq.tail main |> PrimellList   // TODO - may need to normalize

  // TODO - in future, could have a constructor which has the reversed list as an argument, for infinite sequences
  member this.Reverse() = Seq.rev main |> PrimellList

  // TODO - i know Lists are O(1) on prepend and not append, but how do Seqs behave on append vs prepend?
  member this.Append (pobj: PObject) = seq { this :> PObject; pobj } |> PrimellList

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

        
  override this.WithParent(parent: PObject, indexInParent: int) =
    PrimellList(main, this.Length, parent, indexInParent)  // TODO - length again
  
  override this.ToString() =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString())); ")"]

type PList = PrimellList  // abbreviation for sanity