namespace dpenner1.Primell

open System.Collections.Generic
open Antlr4.Runtime


exception NonPrimeDectectionException of string * ExtendedBigRational
// TODO - port from original C# code, very mutable stuff, see if you can get rid of non-functional stuff later

type PrimellVisitor(control: PrimellProgramControl) = 
  inherit PrimellParserBaseVisitor<PObject>()
  let control = control
  let currentForEach = new Stack<PObject>()
  let operationLib = new OperationLib(control)

  let GetInt(n: PNumber) = 
    match n.Value with
    | Rational r when r >= BigRational.Zero -> (round r).Numerator |> int
    | _ -> System.NotImplementedException("Index only with positive finite values for now") |> raise
  
   // very temporary, clearly things are going poorly
  
    
  static member private Normalize (pobj: PObject) =
    match pobj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
    | :? PList as l when not(l.IsEmpty) && Seq.isEmpty(Seq.tail l) -> 
        Seq.head l |> PrimellVisitor.Normalize
    | _ -> pobj    

  override this.VisitLine context = this.Visit(context.termSeq())

  override this.VisitParens context = this.Visit(context.termSeq()) // not entirely sure why i need to explicitly visit this

  override this.VisitEmptyList context = PList.Empty

  override this.VisitTermSeq context =
    let mutable retval = PList.Empty
    for termContext in context.mulTerm() do
      let pobj = this.Visit termContext
      retval <- retval.Append pobj
        
    retval |> PrimellVisitor.Normalize

  override this.VisitInteger context =
    let text = context.GetText()
    let number = ParseLib.ParseInteger text control.Settings.SourceBase

    if control.Settings.RestrictedSource && not(PrimeLib.IsPrime number) then
      NonPrimeDectectionException("NON-PRIME DETECTED!", number) |> raise
    
    number |> PNumber :> PObject

  override this.VisitInfinity context = Infinity Positive |> PNumber :> PObject
  override this.VisitNullaryOp context : PObject =
    control.LastOperationWasAssignment <- false

    let opText = context.baseNullaryOp().GetText()
    let operator =
      if operationLib.NullaryOperators.ContainsKey(opText) then
        operationLib.NullaryOperators[opText]
      else  // implicitly assumed to be a variable
        fun () -> control.GetVariable(opText)

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

  (*
    interesting issue came up with previous implementation of referencing simply being an object and an index *number* Consider:
    x = (2 3 5 7)
    x@(2 3) = (11 13)

    intuitively, you want this to result in x = (2 3 11 13). However with a singular index number and immutability, this was
    done in two steps, first producing (2 3 11 7), then the reference with index 3 necessarily has a stale value and overwrites
    to (2 3 5 13)... so we need to make assignments to the referenced object all at once... but with infinite lists how do we know
    that we have all the references (eg think x@(2..inf)), so we can't really do it one at a time that way
    so instead of a singular index number, we use the whole index object at once, eg. (2 3) or (2..inf) per the examples, which should
    allow us to defer evaluating an infinite list on assign... thats going to be tricky
  *)

  member private this.GetReplacementObjectWithListIndex(cValue: PObject)(cListIndex: PList)(newValue: PObject) =
    // at least in the base case, cListIndex.Length >= length of newValue (which itself should at least be 2 items)
    // but yes, the recursion step on this one scares me
    match cValue with 
    | (:? PAtom) -> 
        this.GetReplacementObjectWithListIndex (Seq.singleton cValue |> PList) cListIndex newValue
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
    

  member private this.GetReplacementObjectWithNumericIndex(cValue: PObject)(cValueIndex: PNumber)(newValue: PObject) =
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

  member private this.GetReplacementObject(cValue: PObject)(cValueIndex: PObject)(newValue: PObject) =
    match cValueIndex with
    | :? PNumber as n -> this.GetReplacementObjectWithNumericIndex cValue n newValue
    | :? PList as l ->  this.GetReplacementObjectWithListIndex cValue l newValue
    | _ -> PrimellProgrammerProblemException "not yet possible" |> raise
  
  member private this.ReplaceReference (refObj: PObject) (refIndex: PObject) (newValue: PObject) =
    // the high level thing for base case is referencedObject@referenceIndex = newValue
    // at least in non-recursive case, assign mechanics is such that length of newValue does not exceed length of referenceIndex
    match refObj.Reference with
    | Variable name ->
        control.SetVariable(name, (this.GetReplacementObject refObj refIndex newValue).WithReference(Variable name))
        control.GetVariable name
    | Reference (ro, ri) ->  // this recursion step feels too easy for this data structure, could be wrong
        this.ReplaceReference ro ri ((this.GetReplacementObject refObj refIndex newValue).WithReference(Reference(ro, ri)))  
    | Void -> PrimellProgrammerProblemException "This method shouldn't be called with Void reference" |> raise

  // called when left is either PAtom, empty list, or manually per operation modifier
  member private this.PerformAtomicAssign (left: PObject, right: PObject) =
    match left.Reference with
    | Void -> right // no action needed
    | Variable name -> 
        control.SetVariable(name, right.WithReference(Variable name))
        control.GetVariable name
    | Reference (refObj, refIndex) -> this.ReplaceReference refObj refIndex right

  member private this.PerformListAssign (left: PList, right: PObject, assignMods: OperationModifier list) =
    let newValue = 
      match right with
      | :? PAtom ->
            let newLvalue = left |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods))
            newLvalue |> PList :> PObject   
      | :? PList as l when l.IsEmpty ->  
          let newLvalue = left |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods))
          newLvalue |> PList :> PObject   
      | :? PList as l ->
          let temp = (left, l) ||> Seq.zip |> Seq.map(fun x -> this.PerformAssign(fst x, snd x, assignMods))
          let real =  // TODO - more problems with infinite lists
            if left.Length.Value > l.Length.Value then 
              Seq.append temp (left |> Seq.skip (Seq.length l))
            else temp
          real |> PList :> PObject 
      | _ -> PrimellProgrammerProblemException "not possible" |> raise
    
    match left.Reference with
    | Void -> newValue // no action needed
    | Variable name -> 
        control.SetVariable(name, newValue.WithReference(Variable name))
        control.GetVariable name
    | Reference (refObj, refIndex) -> this.ReplaceReference refObj refIndex newValue

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

    match left, (assignMods |> List.contains Power) with
    | _, true | :? PAtom, _ ->
        this.PerformAtomicAssign(left, right)
    | :? PList as l, _ when l.IsEmpty ->
        this.PerformAtomicAssign(left, right)
    | :? PList as l, _ ->
        this.PerformListAssign(l, right, assignMods)
    | _ -> PrimellProgrammerProblemException("not possible") |> raise


  member this.ConditionalBranch (left: PObject) (right: PObject) (negate: bool) (isForward: bool) =
      if operationLib.IsTruth(left, control.Settings.TruthDefinition) <> negate then
        let head = operationLib.ApplyUnaryListOperation right operationLib.UnaryListOperators["_<"] []
        match head with
        | :? PNumber as n ->
            match n.Value with
            | NaN | Infinity _ -> PList.Empty :> PObject
            | Rational r ->
                let offset = ((round r).Numerator |> int) * (if isForward then 1 else -1)

                // TODO - i didn't want this here
                let visitor = PrimellVisitor control
                let stream = AntlrInputStream control.LineResults[control.CurrentLine + offset].Text
                let lexer = PrimellLexer stream
                let tokens = CommonTokenStream lexer
                let parser = PrimellParser tokens
                parser.BuildParseTree <- true
                PrimellVisitor(control).VisitLine(parser.line())
        | :? PList as l ->
            // Original C# didn't have nested list implemented, since I'm recursing it's easiest to do so here.
            // While executing all lines it just returned last value, i'm mimicking that for now
            l |> Seq.map (fun x -> this.ConditionalBranch left x negate isForward) |> Seq.last
        | _ -> System.NotImplementedException "You're doing crazy stuff" |> raise
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
          (operationLib.Index left right).WithReference(Reference(left, right))
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