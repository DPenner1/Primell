namespace dpenner1.PrimellF

type PObject =
  | Atom of PNumber
  | PList of seq<PObject>  // Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
  // Empty was considered, but seems would cause unnecessary ambiguity with the possibility of PList being an empty sequence
  with override this.ToString() =
        match this with
        | Atom a -> a.ToString() // TODO - surely there's a cleaner way than I came up with for this nested concat abomination
        | PList l -> String.concat "" ["("; String.concat " " (l |> Seq.map(fun obj -> obj.ToString())); ")"]

        static member Empty = PList Seq.empty

        static member Normalize obj =
          match obj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
          | PList l when not(Seq.isEmpty l) && Seq.isEmpty(Seq.tail l) -> Seq.head l |> PObject.Normalize  
          | _ -> obj    

        member this.IsEmpty =
          match this with
          | Atom _ -> false
          | PList l -> Seq.isEmpty l

        member this.Length =
          match this with
          | Atom _ -> PNumber.One
          | PList l -> Rational <| R(bigint(Seq.length l), 1)

        member this.Head() =
          match this with
          | Atom _ -> this
          | PList l when Seq.isEmpty l -> PObject.Empty
          | PList l -> Seq.head l

        member this.Tail() =
          match this with
          | Atom _ -> PObject.Empty
          | PList l when Seq.isEmpty l -> PObject.Empty
          | PList l -> PList (Seq.tail l)

        // for convenience, so that I can use Seq.concat for Primell's flatten function
        //member private this.RaiseAtoms() =
          //match this with
          //| Atom _-> seq [a]
          //| 
          