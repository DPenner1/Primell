namespace dpenner1.Primell

open dpenner1.Math

exception PrimellProgrammerProblemException of string

type private ListLength =  // it ended up being a bit too annoying interally to co-opt ExtenedBigRational for this
  | Unknown
  | Infinite
  | Finite of bigint

[<Sealed>]
// Ok a bit weird to call the seq a list, but conceptually, that's how I view the Primell object, as lists
type PrimellList(sequence: PObject seq, ?length: ExtendedBigRational, ?refersTo: Reference) =
  let main = sequence |> Seq.cache  // side effects, some expensive computations (primes)
  // TODO - eventually you need to figure out how to dispose nested sequences

  // mutable for memoization, to move from Unknown to either Infinite/Finite
  let mutable length = 
    match length with
    | None -> Unknown
    | Some x ->
        match x with
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
                 | Infinite -> Some (Infinity Positive) 
                 | Finite x -> Some (BigRational x |> Rational)),
      ?refersTo = refersTo)

  interface seq<PObject> with
    member this.GetEnumerator() = main.GetEnumerator()
    member this.GetEnumerator() = main.GetEnumerator() :> System.Collections.IEnumerator   // why didn't they get rid of this with .NET core?


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

  member val private listLength = length with get

  //member this.IsGreaterOrEqualThanLength(value: int) =
  //  match main |> Seq.tryItem value with
  //  | None -> true
  //  | _ -> false

  member val IsEmpty = Seq.isEmpty main with get

  member this.Head() = 
    match Seq.tryHead main with
    | Some head -> head
    | None -> PObject.Empty
    
  // this is a bit tortured, but working around four things here
  //   - F# throws exceptions when Primell needs to return empty list
  //   - F# lib has a Seq.tryHead but not Seq.tryTail ??? 
  //   - Primell automatically unboxes one item lists
  //   - infinite sequences are possible so I can't call Seq.length
  member this.Tail() = 
    match this.IsEmpty with
    | true -> PObject.Empty
    | false -> 
        let newLength = 
          match length with
          | Finite l -> Finite (l - 1I)
          | _ -> length
        let tail = PrimellList(Seq.tail main, newLength)

        match Seq.tryHead tail with
        | Some head when Seq.isEmpty (Seq.tail tail) -> // tail had one item in it, Primell unboxes it
            head
        | _ -> tail |> Sequence |> PObject

  member this.Reverse() = 
    match length with
    | Infinite ->
      PrimellList(Seq.initInfinite(fun _ -> PObject.Empty), length)
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
    match pobj.Value with
    | Sequence l -> 
        let newLength = 
          match length, l.listLength with
          | Finite x, Finite y -> Finite(x + y)  // known lengths, just add them
          // if one is known to be infinite length, that overrides
          | Infinite, _ | _, Infinite -> Infinite
          | _ -> Unknown

        PrimellList(Seq.append this l, newLength)  
    | _ -> 
        this.Append pobj

  // only call with rationals, don't index from end of infinite list
  // TODO - consider not raising exception and returning -1 (raising is my default, but it seems to be a nice use in this case) 
  member internal this.GetEffectiveIndex(n: ExtendedBigRational) = 
    match n with
    | Rational r ->  
      let index = BigRational.ToBigInt r

      if index.Sign = -1 then  // index from end     
          match length with
          | Infinite -> 
              PrimellProgrammerProblemException "Dont call this with negative index and infinite list" |> raise
          | _ ->               
              if this.IsEmpty then 0
              else 
                // at this point, because we're returning a specific object in a sequence,
                // i believe it's impossible to avoid evaluating a possibly infinite length
                match this.EvaluatedLength() with
                | Finite x -> 
                    let modulo = (index % x) 
                    (if modulo.IsZero then modulo else modulo + x) |> int  // i really hate negative modulo, why is this a thing
                | _ -> PrimellProgrammerProblemException "impossible, unless you actually reached the end of an infinite list" |> raise
      else
        int index
    | _ -> PrimellProgrammerProblemException "Dont call this with non-rational" |> raise


  member this.Index(n: ExtendedBigRational) =
    match n with
    | NaN ->
        PObject.Empty
    | Infinity _ -> 
        PObject.Empty
    | Rational r ->
        let index = BigRational.ToBigInt r

        match length, index.Sign with
        | Infinite, -1 -> PObject.Empty
        | _ -> 
          match Seq.tryItem (this.GetEffectiveIndex n) main with
          | None -> PObject.Empty
          | Some x -> x


  static member private AllIndexesOf' (pObj: PObject)(searchSeq: PObject seq)(retval: PrimellList)(indexAdjust: int) = 
    match searchSeq |> Seq.tryFindIndex(fun x -> x.NaNAwareEquals pObj) with
    | None -> retval |> PObject.FromSeq
    | Some n -> PrimellList.AllIndexesOf' pObj (searchSeq |> Seq.skip (n + 1)) (retval.Append (n + indexAdjust |> BigRational |> Rational |> Number |> PObject)) (n + 1)

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
      match x.Value with 
      | Sequence l -> l :> seq<PObject>
      | Empty -> Seq.empty
      | _ -> Seq.singleton x)

  member this.Flatten() =
    this.RaiseAtoms() |> Seq.concat |> PrimellList
  

  // code copying with NaNAwareEquals and Equals, but my brain keeps breaking trying to merge them
  member this.NaNAwareEquals (pList: PList) =
        match length, pList.listLength with
        | Finite _, Infinite | Infinite, Finite _ -> false
        | Finite x, Finite y when x <> y -> false
        | _ ->
          if pList.Length <> this.Length then false  // TODO - just to get it going, but with infinite possible, you can do better
          else
            (this, pList) ||> Seq.exists2 (fun x y -> x.NaNAwareEquals y |> not) |> not
    

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

and PList = PrimellList  // abbreviation for sanity

and PrimellValue =
  | Sequence of PList
  | Number of ExtendedBigRational
  | Operator of string  // temp, I'm thinking the operator will reference the operator library
  | Empty

and Reference =  // TODO - naming, not sure i like type and one of the options being named the same
  | Variable of string
  | Reference of ReferencedObject: PrimellObject * ReferenceIndex: PrimellObject
  | Void   // I could have gone without this and then done option<Reference>, but this feels like it better reflects the intent

and PrimellMetaData =
  { Reference : Reference } 

  static member Default = { Reference = Void }

and PrimellObject (value: PrimellValue, ?metadata: PrimellMetaData) =

  member val Value = value with get
  member val Metadata = defaultArg metadata PrimellMetaData.Default with get
  static member Empty with get() = Empty |> PrimellObject

  static member FromSeq(pSeq: PrimellObject seq) =
    match Seq.isEmpty pSeq with
    | true -> PObject.Empty
    | _ ->
        let tail = Seq.tail pSeq
        match Seq.tryHead tail with
        | None -> Seq.head pSeq
        | Some _ -> pSeq |> PList |> Sequence |> PObject

  // Boxes up a pObject into a singleton pObject. This is not a valid object in Primell, but is used as a convenient intermediate value in operations
  static member internal Box (pObj: PrimellObject) =
    pObj |> Seq.singleton |> PList |> Sequence |> PObject

  static member Infinite(pobj: PObject) = PrimellList(Seq.initInfinite(fun _ -> pobj), Infinity Positive) |> Sequence |> PObject


  override this.ToString (): string = 
      match this.Value with
      | Number n -> n.ToString()
      | Sequence l -> l.ToString()
      | Empty -> "()"
      | Operator _ -> System.NotImplementedException "first class operators not yet implemented" |> raise

  override this.GetHashCode (): int = 
      match this.Value with
      | Number n -> n.GetHashCode()
      | Sequence l -> l.GetHashCode()
      | Empty -> 123456789
      | Operator _ -> System.NotImplementedException "first class operators not yet implemented" |> raise

  override this.Equals (obj: obj): bool = 
      match obj with
      | :? PObject as pObj ->
          match this.Value, pObj.Value with
          | Number n1, Number n2  -> n1.Equals n2
          | Sequence l1, Sequence l2 -> l1.Equals l2
          | Empty, Empty -> true
          | Operator o1, Operator o2 -> o1.Equals o2
          | _ -> false
      | _ -> false
        
      

  member this.NaNAwareEquals (pobj: PrimellObject) =
    match this.Value, pobj.Value with
    | Empty, Empty -> true
    | Operator o1, Operator o2 -> o1.Equals o2
    | Number n1, Number n2 ->
        match n1, n2 with
        | NaN, _ | _, NaN -> false // always false
        | _ -> n1.Equals n2
    | Sequence s1, Sequence s2 -> s1.NaNAwareEquals s2
    | _ -> false

  member this.WithReference (ref: Reference) =
    PrimellObject(this.Value, {this.Metadata with Reference = ref})
    
  // I really don't like this one: In mutable paradigm, this would just be implemented right here.
  // But in immutable, it effectively requires a copy constructor to get the object with a reference
  // and in principle, without having reference to downstream types, this forces them
  // to implement what is effectively a mechanical parsing detail
  (* abstract member WithReference: Reference -> PrimellObject

  member val Reference = defaultArg refersTo Void with get

  abstract member NaNAwareEquals: PrimellObject -> bool
  default this.NaNAwareEquals pobj = this.Equals(pobj)
  *)

// references are a way to deal with indexing mutability (and is more of an implementation detail instead of a object within Primell)
// so if you have a variable x, and index it at index i (x@i), the parser simply generates a reference with values x and i, 
// figuring assignments out based on that later. References can also reference other references [citation needed]

and PObject = PrimellObject

