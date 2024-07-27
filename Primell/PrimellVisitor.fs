namespace dpenner1.PrimellF

open Antlr4.Runtime.Misc
open System.Collections.Generic

exception NonPrimeDectectionException of string * ExtendedBigRational
// TODO - port from original C# code, very mutable stuff, see if you can get rid of non-functional stuff later

type PrimellVisitor(control: PrimellProgramControl) = 
  inherit PrimellBaseVisitor<PObject>()
  let control = control
  let placeHolders = new Stack<PObject>()
  let currentForEach = new Stack<PObject>()
  let incorporate = new Stack<bool>()
  let operationLib = new OperationLib(control)

  // TODO - can you get rid of this mutable?
  let mutable CurrentLine = 0
  
   // very temporary, clearly things are going poorly
  let GetInt(n: PNumber) = 
    match n.Value with
    | Rational r when r >= BigRational.Zero -> (round r).Numerator |> int
    | _ -> System.NotImplementedException("Index only with positive finite values for now") |> raise
    
  static member private Normalize (pobj: PObject) =
    match pobj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
    | :? PList as l when not(l.IsEmpty) && Seq.isEmpty(Seq.tail l) -> 
        Seq.head l |> PrimellVisitor.Normalize
    | _ -> pobj    

  override this.VisitLine context =
      CurrentLine <- control.Lines |> List.findIndex(fun l -> l = context)
      this.VisitChildren(context)

  override this.VisitParens context =
    //incorporate.Pop() |> ignore
    //incorporate.Push false

    if context.termSeq() |> isNull then PList.Empty else this.Visit(context.termSeq())

  override this.VisitTermSeq context =
    let mutable retval = PList.Empty
    for termContext in context.mulTerm() do
      //incorporate.Push false

      let pobj = this.Visit termContext
      //if incorporate.Pop() then 
        //retval <- retval.AppendAll pobj
      //else
      retval <- retval.Append pobj
        
    retval |> PrimellVisitor.Normalize

  override this.VisitInteger context =
    let text = context.GetText()
    let number = ParseLib.ParseInteger text control.Settings.SourceBase

    if control.Settings.RestrictedSource && not(PrimeLib.IsPrime number) then
      NonPrimeDectectionException("NON-PRIME DETECTED!", number) |> raise
    
    number |> PNumber :> PObject

  override this.VisitPositiveInfinity context = Infinity Positive |> PNumber :> PObject

  override this.VisitNegativeInfinity context = Infinity Negative |> PNumber :> PObject
  override this.VisitNullaryOp context : PObject =
    control.LastOperationWasAssignment <- false

    let opText = context.baseNullaryOp().GetText()
    let operator =
      if operationLib.NullaryOperators.ContainsKey(opText) then
        operationLib.NullaryOperators[opText]
      else  // implicitly assumed to be a variable
        fun () -> PVariable(opText, control.GetVariableValue(opText))

    operationLib.ApplyNullaryOperation operator []
          
  // TODO - switch this to UnaryNumeric in the grammar for consistency...
  override this.VisitNumericUnaryOperation context = 
    control.LastOperationWasAssignment <- false
    let operator = operationLib.UnaryNumericOperators[context.numUnaryOp().baseNumUnaryOp().GetText()]
    
    operationLib.ApplyUnaryNumericOperation (this.Visit(context.mulTerm())) operator []

  override this.VisitListUnaryOperation context =
    control.LastOperationWasAssignment <- false  
    let operator = operationLib.UnaryListOperators[context.listUnaryOp().baseListUnaryOp().GetText()]
    
    operationLib.ApplyUnaryListOperation (this.Visit(context.mulTerm())) operator []

  member private this.GetReplacedCapturedObjectListIndex(cValue: PObject)(cListIndex: PList)(newValue: PObject) =
    // at least in the base case, cListIndex.Length >= length of newValue (which itself should at least be 2 items)
    // but yes, the recursion step on this one scares me
    match cValue with 
    | (:? PAtom) -> 
        this.GetReplacedCapturedObjectListIndex (Seq.singleton cValue |> PList) cListIndex newValue
    | (:? PList as cList) ->
        match newValue with
        | :? PAtom ->
            PrimellProgrammerProblemException "surely not" |> raise
        | :? PList as newList ->
            let ivZip = Seq.zip cListIndex newList  // im 99% sure this is correct in base case
          
            (cList, ivZip) ||> Seq.fold(fun accList ivPair -> 
                match round (fst ivPair :?> PNumber).Value with // not dealing with nested list index yet
                | NaN | Infinity Negative -> accList
                | Infinity Positive -> PList(Seq.append accList (Seq.initInfinite(fun _ -> PList.Empty :> PObject)), Infinity Positive |> PNumber)
                | Rational r when r < BigRational.Zero -> System.NotImplementedException("negative index") |> raise
                | Rational r ->
                    let index = int r.Numerator
                    if index >= GetInt accList.Length then // extend list with empties
                        Seq.append accList (Seq.init (index - (GetInt accList.Length)) (fun _ -> PList.Empty :> PObject)) 
                        |> Seq.insertAt index (snd ivPair) 
                        |> PList
                    else 
                        accList |> Seq.removeAt index |> Seq.insertAt index (snd ivPair) |> PList
                )
        | _ ->
            PrimellProgrammerProblemException "this isn't possible either right?" |> raise
    | _ -> System.NotImplementedException "nested ref/var" |> raise
    

  member private this.GetReplacedCapturedObject(cValue: PObject)(cValueIndex: PNumber)(newValue: PObject) =
    match cValue with
    | :? PAtom ->
        match round cValueIndex.Value with
        | NaN | Infinity Negative -> cValue
        | Infinity Positive -> PList(Seq.append (Seq.singleton cValue) (Seq.initInfinite(fun _ -> PList.Empty :> PObject)), Infinity Positive |> PNumber)
        | _ as n when n < ExtendedBigRational.Zero -> System.NotImplementedException("negative index") |> raise
        | _ as n when n = ExtendedBigRational.Zero -> newValue
        | _ as n ->
            Seq.append (Seq.singleton cValue) (Seq.init((GetInt cValueIndex) - 1) (fun _ -> PList.Empty)) 
            |> Seq.insertAt (GetInt cValueIndex) newValue
            |> PList :> PObject
    | :? PList as l when l.IsEmpty ->
        match round cValueIndex.Value with
        | NaN | Infinity Negative -> l
        | Infinity Positive -> PList(Seq.initInfinite(fun _ -> PList.Empty :> PObject), Infinity Positive |> PNumber)
        | _ as n when n < ExtendedBigRational.Zero -> System.NotImplementedException("negative index") |> raise
        | _ as n when n = ExtendedBigRational.Zero -> newValue
        | _ as n ->
            Seq.init (GetInt cValueIndex) (fun _ -> PList.Empty :> PObject) 
            |> Seq.insertAt (GetInt cValueIndex) newValue
            |> PList :> PObject
    | :? PList as l ->
        l |> Seq.mapi (fun i x -> if i = GetInt cValueIndex then newValue else x) |> PList :> PObject
    | _ -> System.NotImplementedException "nested ref/var" |> raise
  
  member private this.ReplaceReference (pref: PReference) (newValue: PObject) =
    // the high level thing for base case is referencedObject@referenceIndex = newValue
    // at least in base case, assign mechanics is that length of newValue does not exceed length of referenceIndex
    match pref.ReferenceIndex with
    | (:? PNumber as nIndex) ->  // the "simple" case
        match pref.ReferencedObject with
        | :? PVariable as v ->
            control.SetVariable(v.Name, this.GetReplacedCapturedObject v.CapturedValue nIndex newValue)
        | :? PReference as r ->
            this.ReplaceReference r (this.GetReplacedCapturedObject r.CapturedValue nIndex newValue)  // this recursion feels too easy for this data structure, could be wrong
        | _ -> PrimellProgrammerProblemException "Non ref/var reference" |> raise
    | (:? PList as lIndex) -> // can't recursively one-by-one call, we run into that issue where we don't assign all at once
        match pref.ReferencedObject with
        | :? PVariable as v ->
            control.SetVariable(v.Name, this.GetReplacedCapturedObjectListIndex v.CapturedValue lIndex newValue)
        | :? PReference as r ->
            this.ReplaceReference r (this.GetReplacedCapturedObjectListIndex r.CapturedValue lIndex newValue)  // this recursion feels too easy for this data structure, could be wrong
        | _ -> PrimellProgrammerProblemException "Non ref/var reference" |> raise
    | _ -> System.NotImplementedException "I'm no miracle worker" |> raise
            
  member private this.PerformAssign  (left: PObject, right: PObject, assignMods: OperationModifier list): PObject =
    (* logic from original mutable C# version:
       if (left is empty) || (left is number) || (assignOptions has modifier for this)
         then replace left with right in-place
       else if (right is number)
            // replace all items in left list with right in-place
            foreach (i in left.size): left[i].Assign(right)
       else // list to list parallel assignment
            // if left is bigger than right, extra values left intact
            // if right is bigger than left, extra values are discarded

            foreach (i in min(left.size, right.size)): left[i].Assign(right[i])

       (* 
          note though that I'm shifting Primell to be a bit more functional/immutable than the original C#
          Assignments are still a thing, but imagine that each object can be tagged with a variable/reference
          So that assignments to an object can use the tag to update that variable / reference to variable

          Doing any operation generally creates a new object (not mutate in place) and that will destroy the tag
          It can also be imagined as a form of variable shadowing -> changing a variable means existing references are 
          to an old version of that variable, so assignment "fails" to stick in the same manner... i hope these are equivalent, 
          (well I don't think they are in general, but within Primell's context I can't come up with a counter-example)

          Simple assignments then work the same, but more complex nested/simultaneous assignments could have different results
       *)
    *)

    // now this is absolutely horrid recursion, because you need recurse down the data structure (normal for operators in Primell)
    // but once values are assigned, you need to make those assignments stick (in C# mutable, they would just be modified in place)
    // In F# immutable Primell, we need to recurse up the reference chain (which isn't the same as the nested list data structure!)

    match right with // unbox right side, its the left side that'll get sticky
    | :? PVariable as v -> this.PerformAssign(left, v.CapturedValue, assignMods)
    | :? PReference as r -> this.PerformAssign(left, r.CapturedValue, assignMods)
    | _ ->
      if assignMods |> List.contains Power then
        match left with 
        | :? PVariable as v -> control.SetVariable(v.Name, this.PerformAssign(v.CapturedValue, right, assignMods))
        | :? PReference as r -> this.ReplaceReference r (this.PerformAssign(r.CapturedValue, right, assignMods))
        | _ -> ()
        right
      else
        match left, right with
        | :? PVariable as pvar, _ ->
            let newValue = this.PerformAssign(pvar.CapturedValue, right, assignMods)
            control.SetVariable(pvar.Name, newValue)
            newValue
        | :? PReference as pref, _ -> 
            let newValue = this.PerformAssign(pref.CapturedValue, right, assignMods)
            this.ReplaceReference pref newValue
            newValue
        | :? PAtom, _ ->
            right
        | :? PList as l, _ when l.IsEmpty ->
            right
        | :? PList as l, :? PAtom ->
            let newLvalue = l |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods))
            newLvalue |> PList :> PObject   
        | (:? PList as l1), (:? PList as l2) when l2.IsEmpty ->  
            let newLvalue = l1 |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods))
            newLvalue |> PList :> PObject   
        | (:? PList as l1), (:? PList as l2) ->
            let temp = (l1, l2) ||> Seq.zip |> Seq.map(fun x -> this.PerformAssign(fst x, snd x, assignMods))
            let real =  // TODO - more problems with infinite lists
              if l1.Length.Value > l2.Length.Value then 
                Seq.append temp (l1 |> Seq.skip (Seq.length l2))
              else temp
            real |> PList :> PObject 
        | _ -> PrimellProgrammerProblemException("not possible") |> raise

    // todo - in theory the assign should not just return the bare new value, 
    //        but the PVariable or reference with the new capturedObject
    //        

  member this.ConditionalBranch (left: PObject) (right: PObject) (negate: bool) (isForward: bool) =
      if operationLib.IsTruth(left, control.Settings.TruthDefinition) <> negate then
        let head = operationLib.ApplyUnaryListOperation right operationLib.UnaryListOperators["_<"] []
        match head with
        | :? PNumber as n ->
            match n.Value with
            | NaN | Infinity _ -> PList.Empty :> PObject
            | Rational r ->
                let offset = ((round r).Numerator |> int) * (if isForward then 1 else -1)
                PrimellVisitor(control).VisitLine(control.Lines[CurrentLine + offset])
        | _ -> System.NotImplementedException("non-number in conditional head not yet implemented") |> raise
      else operationLib.ApplyUnaryListOperation right operationLib.UnaryListOperators["_>"] []

  member this.ApplyBinaryOperation left right (context: PrimellParser.BinaryOpContext) =
    let isAssign = context.ASSIGN() |> isNull |> not

    let interimResult =
      if context.baseNumBinaryOp() |> isNull |> not then
        let operator = operationLib.BinaryNumericOperators[context.baseNumBinaryOp().GetText()]
        operationLib.ApplyBinaryNumericOperation left right operator []
      elif context.baseListBinaryOp() |> isNull |> not then
        let opText = context.baseListBinaryOp().GetText()
        if opText = "@" then  // index needs special handling for the whole reference-assign
          operationLib.Index left right
        elif opText.StartsWith "?" then  // TODO - conditional stuff needs to not execute both branches
          if opText.Contains "/" || opText.Contains("\\") then
            this.ConditionalBranch left right (opText.Contains "~") (opText.Contains "/")
          else
            let operator = operationLib.Conditional left right (opText.Contains "~")
            operationLib.ApplyUnaryListOperation right operator []   //head or tail operator
        else
          let operator = operationLib.BinaryListOperators[context.baseListBinaryOp().GetText()]
          operationLib.ApplyBinaryListOperation left right operator []
      else 
        right

    control.LastOperationWasAssignment <- isAssign
      
    if isAssign then
      let mods = if context.assignMods() |> isNull then "" else context.assignMods().GetText()
      this.PerformAssign(left, interimResult, ParseLib.ParseOperationModifiers mods)
      // TODO - ideally we'd have it such that at this point interimResult is necessarily dereferenced from the binary op
    else 
      interimResult

  // TODO - did this one needed direct overriding? maybe could just override the deeper Numeric/List binary Operations
  //        also a bit ugly with the the ApplyBinaryOperation having Operation stuff and Parsing stuff
  override this.VisitBinaryOperation context =
    let left = this.Visit(context.mulTerm())
    
    // TODO - really need to adjust grammar to section off conditional, its a mess otherwise
    // TODO - here we need to handle conditional, and not visit head/tail of right, depending on the result
    // TODO - isn't this match just equivalent to VisitChildren()? 
    // TODO - basically all this sucks right now
    let right = match context.termSeq() with
                | null -> this.Visit(context.atomTerm())
                | _ as x -> this.Visit(x)

    this.ApplyBinaryOperation left right (context.binaryOp())

  override this.VisitForEachLeftTerm context =
    currentForEach.Push(this.Visit(context.mulTerm()))
    this.Visit(context.forEachBlock()) |> ignore
    currentForEach.Pop()

  override this.VisitForEachRightTerm context =
    let left = this.Visit(context.mulTerm())

    match this.Visit(context.termSeq()) with
    | :? PNumber as robj ->
        this.ApplyBinaryOperation left robj (context.binaryOp())
    | :? PList as l ->
        l |> Seq.map(fun robj -> this.ApplyBinaryOperation left robj (context.binaryOp())) |> PList :> PObject
    | _ -> PrimellProgrammerProblemException("not possible") |> raise


(* Old referential assignment which might be useful later on with lazier eval

  member private this.GetReplacementObject (pObj: PObject) (indexes: list<PNumber>) =
    match indexes.Tail.IsEmpty with
    | true -> pObj
    | false -> this.GetReplacementObject (operationLib.Index pObj indexes.Head) indexes.Tail
    
  // TODO - the recursion here was written before the change to PVar/PRef mechanics, but somehow still passes most of the current test cases?
  member private this.ReplaceVariableValue (newValue: PObject) (pvar: PVariable) (indexes: list<PNumber>) =
    if indexes.IsEmpty then
      control.SetVariable(pvar.Name, newValue)
    elif indexes.Tail.IsEmpty then
      let oldValue = pvar.CapturedValue
      match oldValue, indexes.Head.Value with
      | (:? PList as l), (Rational _ as r) -> 
          let index = GetInt(r |> PNumber)
          if r < ExtendedBigRational.Zero then System.NotImplementedException "Index/assign wonky" |> raise      
          if index >= GetInt(l.Length) then System.NotImplementedException "Index/assign wonky" |> raise
          control.SetVariable(pvar.Name, l |> Seq.mapi(fun i x -> if i = index then newValue else x) |> PList)
      | (:? PAtom as a), (Rational _ as r) -> 
          let index = GetInt(r |> PNumber)
          if index <> 0 then System.NotImplementedException "Index/assign wonky" |> raise
          control.SetVariable(pvar.Name, newValue)
      | _ -> System.NotImplementedException "Index/assign wonky" |> raise
    else // still keep indexing
      this.ReplaceVariableValue (this.GetReplacementObject pvar.CapturedValue indexes) pvar (indexes |> List.rev |> List.tail |> List.rev)


  member private this.ReplaceReferenceValue (newValue: PObject) (pref: PReference) (indexes: list<PNumber>) =
    match pref.ReferencedObject with
    | :? PVariable as v ->  // TODO - temp downcasting to avoid compile errors
        this.ReplaceVariableValue newValue v ((pref.ReferenceIndex :?> PNumber)::indexes)
    | :? PReference as r->
        this.ReplaceReferenceValue newValue r ((pref.ReferenceIndex :?> PNumber)::indexes)
    | _ -> PrimellProgrammerProblemException("not possible") |> raise

*)