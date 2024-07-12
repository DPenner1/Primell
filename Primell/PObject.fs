namespace dpenner1.PrimellF



// Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
// TODO - this is mainly just a wrapper for now, but in some cases this allows for smoother handling of infinite seqs than native F# seq,
//        eg length is passed in so that known infinite sequences can be tagged as such and not result in Seq.length computing infinitely
type PrimellList(sequence: seq<PrimellObject>, ?length: PNumber) = 
  let main = sequence   // star
  let mutable length = defaultArg length NaN
  // TODO - should also guard against negative lengths

  interface seq<PrimellObject> with
    member this.GetEnumerator() = main.GetEnumerator()
    member this.GetEnumerator() = main.GetEnumerator() :> System.Collections.IEnumerator   // why didn't they get rid of this with .NET core?
  
  // a little hacky, didn't want to bother with Option type when we have NaN which won't ever be a valid result for length
  member this.Length with get() =
    match length with   // important to use match and not equality due to potential weirdness with NaN != NaN
    | NaN ->   
        length <- Seq.length main |> BigRational |> Number   // memoize
    | _ -> ()
    length

  member this.IsEmpty with get() = Seq.isEmpty main   // avoid calling this.Length due to potential long computation

  member this.Head() = Seq.head main
    
  member this.Tail() = Seq.tail main

  // TODO - in future, could have a constructor which has the reversed list as an argument, for infinite sequences
  member this.Reverse() = Seq.rev main

  override this.ToString() =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString())); ")"]


and PrimellObject =
  | Atom of PNumber
  | PList of PrimellList  
  // Empty was considered, but seems would cause unnecessary ambiguity with the possibility of PList being an empty sequence
  with override this.ToString() =
        match this with
        | Atom a -> a.ToString() 
        | PList l -> l.ToString()

        static member Empty = PrimellList(Seq.empty, PNumber.Zero) |> PList

        static member Normalize obj =
          match obj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
          | PList l when not(l.IsEmpty) && Seq.isEmpty(l.Tail()) -> 
              Seq.head l
          | _ -> obj    

        member this.IsEmpty =
          match this with
          | Atom _ -> false
          | PList l -> l.IsEmpty

        member this.Length =
          match this with
          | Atom _ -> PNumber.One
          | PList l -> l.Length

        member this.Head() =
          match this with
          | Atom _ -> this
          | PList l when l.IsEmpty -> PrimellObject.Empty
          | PList l -> l.Head()

        member this.Tail() =
          match this with
          | Atom _ -> PrimellObject.Empty
          | PList l when l.IsEmpty -> PrimellObject.Empty
          | PList l -> l.Tail() |> PrimellList |> PList

        member this.Reverse() =
          match this with
          | Atom _ -> this
          | PList l -> l.Reverse() |> PrimellList |> PList

        // TODO - i know Lists are O(1) on prepend and not append, but how do Seqs behave on append vs prepend?
        member this.Append (pobj: PrimellObject) = seq { this; pobj } |> PrimellList |> PList

        member this.AppendAll pobj =
          match this, pobj with
          | PList l1, PList l2 -> Seq.append l1 l2 |> PrimellList |> PList
          | PList l, Atom _ -> Seq.singleton pobj |> PrimellList |> PList |> this.AppendAll
          | Atom _, PList l -> (Seq.singleton this |> PrimellList |> PList).AppendAll pobj
          | Atom _, Atom _ -> seq {this; pobj} |> PrimellList |> PList

        
        // for convenience, so that I can use Seq.concat for Primell's flatten function
        //member private this.RaiseAtoms() =
          //match this with
          //| Atom _-> seq [a]
          //| 

// abbreviations for sanity
type PObject = PrimellObject