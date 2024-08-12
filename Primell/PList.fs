namespace dpenner1.Primell

open dpenner1.Math

type private ListLength =  // it ended up being a bit too annoying interally to co-opt ExtenedBigRational for this
  | Unknown
  | Infinite
  | Finite of bigint

[<Sealed>]
// Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
type PrimellList(sequence: PObject seq, ?length: PNumber, ?refersTo: Reference) =
  inherit PObject(?refersTo = refersTo)
  let main = sequence |> Seq.cache  // side effects, some expensive computations (primes)
  // TODO - eventually you need to figure out how to dispose nested sequences

  // mutable for memoization, to move from Unknown to either Infinite/Finite
  let mutable length = 
    match length with
    | None -> Unknown
    | Some x ->
        match x.Value with
        | Infinity Positive -> Infinite
        | Rational r when r.IsInteger && r.Sign >= 0 -> Finite r.Numerator 
        | _ -> PrimellProgrammerProblemException "Invalid list length" |> raise

  // it is a bit awkward this constructor takes ListLength, passes to public constructor as PNumber which unboxes back to ListLength
  // but the external constructor still has to be PNumber for now due to existing code, might change that later if possible
  // Note you don't want to expose ListLength! Outside classes should not need to know that a length is currently unknown
  private new(sequence: PObject seq, length: ListLength, ?refersTo) = 
    PrimellList(
      sequence, 
      ?length = (match length with 
                 | Unknown -> None 
                 | Infinite -> Some (Infinity Positive |> PNumber) 
                 | Finite x -> Some (BigRational x |> Rational |> PNumber)),
      ?refersTo = refersTo)

  interface seq<PObject> with
    member this.GetEnumerator() = main.GetEnumerator()
    member this.GetEnumerator() = main.GetEnumerator() :> System.Collections.IEnumerator   // why didn't they get rid of this with .NET core?
  
  static member Empty = PrimellList(Seq.empty, Finite 0I) :> PObject  
  // upcast for convenience, as most code needs an "empty object", the detail of its implementation as an empty list is usually annoying 
  
  member private this.EvaluatedLength() =
    match length with 
    | Unknown ->   // calculate the finite length when requested (infinite will just hang)
        length <- Seq.length main |> bigint |> Finite
    | _ -> ()
    length

  member this.Length with get() =
    
    match this.EvaluatedLength() with
    | Infinite -> Infinity Positive
    | Finite x -> x |> BigRational |> Rational
    | _ -> PrimellProgrammerProblemException "not possible" |> raise
    |> PNumber

  member val private listLength = length with get

  //member this.IsGreaterOrEqualThanLength(value: int) =
  //  match main |> Seq.tryItem value with
  //  | None -> true
  //  | _ -> false

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
    | true -> PrimellList.Empty
    | false -> 
        let newLength = 
          match length with
          | Finite l -> Finite (l - 1I)
          | _ -> length
        let tail = PrimellList(Seq.tail main, newLength)

        match Seq.tryHead tail with
        | Some head when Seq.isEmpty (Seq.tail tail) -> // tail had one item in it, Primell unboxes it
            head
        | _ -> tail

  member this.Reverse() = 
    match length with
    | Infinite ->
      PrimellList(Seq.initInfinite(fun _ -> PrimellList.Empty), length)
    | _ ->
      PrimellList(Seq.rev main, length)

  // TODO - i think the Seq.singleton causes some incorrect boxing thats masked by a later Normalize
  member this.Append (pobj: PObject) = 
    let newLength = 
      match length with 
      | Finite l -> Finite (l + 1I)
      | _ -> length
    
    PrimellList(Seq.append this (Seq.singleton pobj), newLength)

  member this.AppendAll (pobj: PObject) =
    match pobj with
    | :? PrimellList as l -> 
        let newLength = 
          match length, l.listLength with
          | Finite x, Finite y -> Finite(x + y)  // known lengths, just add them
          // if one is known to be infinite length, that overrides
          | Infinite, _ | _, Infinite -> Infinite
          | _ -> Unknown

        PrimellList(Seq.append this l, newLength)  
    | _ -> 
        this.Append pobj

  member this.Index(n: PNumber) =
    match n.Value with
    | NaN ->
        PrimellList.Empty
    | Infinity _ -> 
        PrimellList.Empty
    | Rational r ->
        let index = BigRational.ToBigInt r

        if index.Sign = -1 then  // index from end     
            match length with
            | Infinite -> 
                PrimellList.Empty  // index from end of list
            | _ ->               
                if this.IsEmpty then PrimellList.Empty
                else 
                  // at this point, because we're returning a specific object in a sequence,
                  // i believe it's impossible to avoid evaluating a possibly infinite length
                  match this.EvaluatedLength() with
                  | Finite x -> 
                      let modulo = (index % x) // i really hate negative modulo, why is this a thing
                      main |> Seq.item ((if modulo.IsZero then modulo else modulo + x) |> int)
                  | _ -> PrimellProgrammerProblemException "impossible, unless you actually reached the end of an infinite list" |> raise
        else
          match Seq.tryItem (int index) main with
          | None -> PrimellList.Empty 
          | Some x -> x

  static member private AllIndexesOf' (pObj: PObject)(searchSeq: PObject seq)(retval: PrimellList)(indexAdjust: int) = 
    match searchSeq |> Seq.tryFindIndex(fun x -> x.NaNAwareEquals pObj) with
    | None -> retval
    | Some n -> PrimellList.AllIndexesOf' pObj (searchSeq |> Seq.skip (n + 1)) (retval.Append (n + indexAdjust |> BigRational |> Rational |> PNumber)) (n + 1)

  member this.AllIndexesOf(pObj: PObject) =
    PrimellList.AllIndexesOf' pObj main (Seq.empty |> PrimellList) 0

  member this.Cons(pObj: PObject) =
    let newLength = 
      match length with 
      | Finite l -> Finite (l + 1I)
      | _ -> length
    PrimellList(main |> Seq.insertAt 0 pObj, newLength)

  member private this.RaiseAtoms() =
    main |> Seq.map(fun x -> 
      match x with 
      | :? PrimellList as l -> l :> seq<PObject>
      | _ as x -> Seq.singleton x)

  member this.Flatten() =
    this.RaiseAtoms() |> Seq.concat |> PrimellList
  
  override this.WithReference(ref) = PrimellList(main, length, ?refersTo = Some ref)

  // code copying with NaNAwareEquals and Equals, but my brain keeps breaking trying to merge them
  override this.NaNAwareEquals pobj =
    match pobj with
    | :? PrimellList as l ->
        match length, l.listLength with
        | Finite _, Infinite | Infinite, Finite _ -> false
        | Finite x, Finite y when x <> y -> false
        | _ ->
          if l.Length <> this.Length then false  // TODO - just to get it going, but with infinite possible, you can do better
          else
            (this, l) ||> Seq.exists2 (fun x y -> x.NaNAwareEquals y |> not) |> not
    | _ -> false

  override this.ToString() =  // TODO - surely there's a cleaner way than the nested concat abomination I came up with
    String.concat "" ["("; String.concat " " (main |> Seq.map(fun obj -> obj.ToString())); ")"]

  override this.Equals(other) =
    match other with
    | :? PrimellList as l ->
        match length, l.listLength with
        | Finite _, Infinite | Infinite, Finite _ -> false
        | Finite x, Finite y when x <> y -> false
        | _ ->
          if l.Length <> this.Length then false  // TODO - just to get it going, but with infinite possible, you can do better
          else
            (this, l) ||> Seq.exists2 (fun x y -> x.Equals y |> not) |> not
    | _ -> false

  override this.GetHashCode() = 
    let arbitrary = 0x456789AB
    match length with
    | Unknown | Infinite ->
        main |> Seq.truncate 42    // arbitrary, but we definitely don't want to evaluate infinitely here
    | Finite n -> 
        main
    |> Seq.fold (fun hashcode x -> // TODO this could recurse infinitely down
        hashcode ^^^ x.GetHashCode()) 0

type PList = PrimellList  // abbreviation for sanity


