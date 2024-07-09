namespace dpenner1.PrimellF

type OperationModifier = 
  | Todo

type PObject =
  | Atom of PNumber
  | PList of seq<PObject>  // Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
  // Empty was considered, but seems would cause unnecessary ambiguity with the possibility of PList being an empty sequence
  with override this.ToString() =
        match this with
        | Atom a -> a.ToString() // TODO - surely there's a cleaner way than the nested concat abomination I came up with
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
          | PList l -> BigRational(bigint(Seq.length l), 1) |> Number

        member this.Head() =
          match this with
          | Atom _ -> this
          | PList l when Seq.isEmpty l -> PObject.Empty
          | PList l -> Seq.head l

        member this.Tail() =
          match this with
          | Atom _ -> PObject.Empty
          | PList l when Seq.isEmpty l -> PObject.Empty
          | PList l -> Seq.tail l |> PList

        member this.Reverse() =
          match this with
          | Atom _ -> this
          | PList l -> Seq.rev l |> PList

        // opMods for consistency, but I don't think Primell will have any need for opMods on unary numeric operators
        static member UnaryNumericOperation pobj operator opMods : PObject =
          match pobj with
          | Atom a -> operator a
          | PList l -> l |> Seq.map(fun x -> PObject.UnaryNumericOperation x operator opMods) |> PList
          
        static member UnaryListOperation pobj operator opMods : PObject =
          match pobj with
          | PList l -> operator l
          | Atom a -> PObject.UnaryListOperation (Atom a |> Seq.singleton |> PList) operator opMods

        static member BinaryNumericOperation left right operator opMods : PObject =
          match left, right with
          | Atom a1, Atom a2 -> 
              operator a1 a2
          | Atom a, PList l -> 
              l |> Seq.map(fun x -> PObject.BinaryNumericOperation (Atom a) x operator opMods) |> PList
          | PList l, Atom a -> 
              l |> Seq.map(fun x -> PObject.BinaryNumericOperation x (Atom a) operator opMods) |> PList
          | PList l1, PList l2 -> 
              (l1, l2) ||> Seq.map2 (fun x y -> PObject.BinaryNumericOperation x y operator opMods) |> PList
              // TODO - F# truncates to the shortest list, but Primell's default is to virtually extend the shorter list with Emptys
              // Interesting solution provided here that could be adapted: https://stackoverflow.com/a/2840062/1607043

        static member BinaryListOperation left right operator opMods : PObject =
          match left, right with
          | PList l1, PList l2 -> 
              operator l1 l2
          | Atom a, PList l -> 
              PObject.BinaryListOperation (Atom a |> Seq.singleton |> PList) right operator opMods
          | PList l, Atom a -> 
              PObject.BinaryListOperation left (Atom a |> Seq.singleton |> PList) operator opMods
          | Atom a1, Atom a2 -> 
              PObject.BinaryListOperation (Atom a1 |> Seq.singleton |> PList) (Atom a2 |> Seq.singleton |> PList) operator opMods
 
        static member ListNumericOperation pList pNumber operator opMods : PObject =
          match pList, pNumber with
          | PList l, Atom a -> 
              operator l a
          | PList l1, PList l2 -> 
              failwith "I need more coffee before figuring out this case"
          | Atom a1, Atom a2 -> 
              PObject.ListNumericOperation (Atom a1 |> Seq.singleton |> PList) pNumber operator opMods
          | Atom a, PList l -> 
              failwith "I need more coffee before figuring out this case"

        // for convenience, so that I can use Seq.concat for Primell's flatten function
        //member private this.RaiseAtoms() =
          //match this with
          //| Atom _-> seq [a]
          //| 
          