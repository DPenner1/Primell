namespace dpenner1.Primell

open System.Collections.Generic
open dpenner1.Primell.Antlr4Generated
open dpenner1.Math
open Antlr4.Runtime

exception NonPrimeDectectionException of string

type private BinaryRHS =  // yes, this was a headache
  | NonConditional of PObject
  | EvaluatedConditional of PObject
  | PartialConditional of Evaluated: PObject * Deferred: PrimellParser.ConcatRtlTermContext seq  // for the append-in-place syntax
  | DeferredConditional of PrimellParser.ConcatRtlTermContext seq
// i think the NonCond and EvCond could be functionally merged, but it helps with understanding the code

type PrimellVisitor(control: PrimellProgramControl) as self = 
  inherit PrimellParserBaseVisitor<PObject>()
  let control = control
  let currentForEach = new Stack<PObject>()
  let operationLib = new OperationLib(control, self)

  // TODO - get rid of this
  let GetPositiveInt(n: ExtendedBigRational) = 
    match n with
    | Rational r when r >= BigRational.Zero -> (round r).Numerator |> int
    | _ -> PrimellProgrammerProblemException("Get Int should only be called with positive rational") |> raise

  interface IExternal with
    member this.Branch mode branchValue =
      match branchValue with
      | NaN | Infinity _ -> PObject.Empty
      | Rational r ->
          let intArg = BigRational.ToInt r
          let lineIndex = 
            match mode with
            | Forward -> control.CurrentLine + intArg
            | Backward -> control.CurrentLine - intArg
            | Absolute -> intArg

          if lineIndex >= control.LineResults.Length then 
            PObject.Empty
          else
            let effectiveLine = 
              if lineIndex >= 0 then lineIndex
              elif lineIndex % control.LineResults.Length = 0 then 0
              else lineIndex % control.LineResults.Length + control.LineResults.Length

            let lineRestore = control.CurrentLine
            control.CurrentLine <- effectiveLine
            let parser = PrimellVisitor.GetParser control.LineResults[effectiveLine].Text
            let result = PrimellVisitor(control).VisitLine(parser.line())
            // restore whatever the branch may have changed
            control.CurrentLine <- lineRestore
            control.LastOperationWasAssignment <- false
            control.LastOperationWasOutput <- false
            result

    member this.OutputToDefault (str: string): unit = 
        System.Console.Write str

    member this.FlushDefaultOutput (): unit = 
        System.Console.Out.Flush()

    member this.ReadLineFromDefault (): string = 
        System.Console.ReadLine()

  static member GetParser (line: string) =
    let stream = AntlrInputStream line
    let lexer = PrimellLexer stream
    let tokens = CommonTokenStream lexer
    let parser = PrimellParser tokens
    parser.BuildParseTree <- true
    parser.ErrorHandler <- BailErrorStrategy()
    parser
    
  

  override this.VisitLine context = 
    let result = this.Visit(context.termSeq())
    result

  override this.VisitParens context = this.Visit(context.termSeq())

  override this.VisitEmptyList context = PObject.Empty

  member private this.VisitConcatRtlTermSeq(rtlTermSeq: PrimellParser.ConcatRtlTermContext seq) =

    (Seq.empty, rtlTermSeq) ||> Seq.fold(fun retval concatRtlTerm ->
      control.LastOperationWasAssignment <- false   // putting this here is either genius or deranged
      control.LastOperationWasOutput <- false      // continued genius / derangement
      let left = concatRtlTerm.concat()
      let right = this.Visit(concatRtlTerm.rtlTerm())
      match left, right.Value with
      | null, _ ->
          Seq.append retval (Seq.singleton right)
      | _, Sequence l ->
          Seq.append retval l
      | _, Empty ->
          retval
      | _ -> 
          Seq.append retval (Seq.singleton right)
    ) 
    |> PObject.FromSeq

  member private this.VisitRtlTermSeqHead(rtlTermSeq: PrimellParser.ConcatRtlTermContext seq) =
    if Seq.isEmpty rtlTermSeq then
      PObject.Empty
    else 
      match (Seq.head rtlTermSeq).concat() with
      | null -> this.Visit(Seq.head rtlTermSeq)
      | _ -> PrimellProgrammerProblemException "You done wrong" |> raise

  member private this.VisitRtlTermSeqTail(rtlTermSeq: PrimellParser.ConcatRtlTermContext seq) =
    if Seq.isEmpty rtlTermSeq then
      PObject.Empty
    else 
      match (Seq.head rtlTermSeq).concat() with
      | null -> this.VisitConcatRtlTermSeq(Seq.tail rtlTermSeq)
      | _ -> PrimellProgrammerProblemException "Shouldn't have done that" |> raise
    

  override this.VisitTermSeq context = this.VisitConcatRtlTermSeq(context.concatRtlTerm())

  override this.VisitIntegerOrIdentifier context =
    let text = context.GetText() 

    let number =  // TODO - really hacky
      try ParseLib.ParseInteger control.Settings text |> Some
      with | PrimellInvalidSyntaxException _ -> None

    match number with
    | Some n ->
        if control.Settings.RestrictedSource && not(PPrimeLib.IsPrime n) then
          NonPrimeDectectionException (n.ToString()) |> raise
        n |> PNumber |> Atom |> PObject
    | None -> // implicitly an identifier
        if control.Settings.RestrictedSource then  // TODO - temporary, too euro-centric, need to block way more chars
          let regex = System.Text.RegularExpressions.Regex($"[0-9a-zA-Z{control.Settings.Character63}{control.Settings.Character64}]")
          if regex.IsMatch(text) then
            NonPrimeDectectionException text |> raise

        control.GetVariable(text)

  override this.VisitInfinity context = Infinity Positive |> PNumber |> Atom |> PObject

  override this.VisitString context = // TODO, just hardcoding UTF-32
    let runeEnumerable = seq { let mutable i = context.STRING().GetText().EnumerateRunes() in while i.MoveNext() do yield i.Current }
    runeEnumerable |> Seq.map(fun f -> 
      let value = f.Value |> BigRational |> Rational
      if control.Settings.RestrictedSource && not (PPrimeLib.IsPrime value) then
        NonPrimeDectectionException (f.ToString() + ": " + value.ToString()) |> raise
      value |> PNumber |> Atom |> PObject
    ) |> PObject.FromSeq
  
  override this.VisitNullaryOp context =
    operationLib.ApplyNullaryOperation (context.baseNullaryOp().GetText()) (ParseLib.ParseOperationModifiers (context.opMods().GetText()))
          
  member private this.ApplyUnaryOperation (pobj: PObject) (context: PrimellParser.UnaryOpContext) =
    match context.baseUnaryOp() with
    | null -> operationLib.ApplyFoldOperation pobj (context.baseBinaryOp().GetText()) [] 
    | _ as x -> operationLib.ApplyUnaryOperation pobj (x.GetText()) (ParseLib.ParseOperationModifiers (context.opMods().GetText()))
  
  override this.VisitUnaryOperation context =
    let originalValue = this.Visit(context.mulTerm())
    let result = this.ApplyUnaryOperation originalValue (context.unaryOp())
    
    match context.unaryOp().unaryAssign() with
    | null -> result
    | _ as uaCtxt -> 
        control.LastOperationWasAssignment <- true
        this.PerformAssign(originalValue, result, ParseLib.ParseOperationModifiers(uaCtxt.assignMods().GetText()))

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
    match cValue.Value with 
    | Atom _ -> 
        this.GetReplacementObjectWithListIndex (cValue |> PObject.Box) cListIndex newValue
    | Empty -> 
        this.GetReplacementObjectWithListIndex (Seq.empty |> PList |> Sequence |> PObject) cListIndex newValue
    | Sequence cList ->
        match newValue.Value with
        | Sequence newList ->
            let ivZip = Seq.zip cListIndex newList  // im 99% sure this is correct in base case
          
            (cList, ivZip) ||> Seq.fold(fun accList ivPair -> 
                match (fst ivPair).Value with
                | Atom a ->
                    match a with
                    | PNumber index ->
                        match round index with 
                        | NaN | Infinity Negative -> accList
                        | Infinity Positive -> PList(Seq.append accList (Seq.initInfinite(fun _ -> PObject.Empty)), Infinity Positive)
                        | Rational r when r < BigRational.Zero -> System.NotImplementedException "negative index" |> raise
                        | Rational r ->
                            let index = int r.Numerator
                            if index >= GetPositiveInt accList.Length then // extend list with empties
                                Seq.append accList (Seq.init (index - (GetPositiveInt accList.Length)) (fun _ -> PObject.Empty)) 
                                |> Seq.insertAt index (snd ivPair) 
                                |> PList
                            else 
                                accList |> Seq.updateAt index (snd ivPair) |> PList
                    | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise
                | _ -> System.NotImplementedException "Nested list index not yet supported" |> raise
            )
        | _ ->
            PrimellProgrammerProblemException "this isn't possible either right?" |> raise
    
  member private this.GetReplacementObjectWithNumericIndex(cValue: PObject)(cValueIndex: ExtendedBigRational)(newValue: PObject) =
    match cValue.Value with
    | Atom a ->
        match a with
        | PNumber _ ->
            match round cValueIndex with
            | NaN | Infinity Negative -> cValue
            | Infinity Positive -> (Seq.singleton cValue |> PList).AppendAll (PObject.Infinite PObject.Empty) |> PObject.FromSeq  //TODO TAG
            | _ as n when n <= ExtendedBigRational.Zero -> newValue
            | _ as n ->
                Seq.append (Seq.singleton cValue) (Seq.init((GetPositiveInt cValueIndex) - 1) (fun _ -> PObject.Empty)) 
                |> Seq.insertAt (GetPositiveInt cValueIndex) newValue
                |> PObject.FromSeq
        | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise
    | Empty ->
        match round cValueIndex with
        | NaN | Infinity Negative -> PObject.Empty
        | Infinity Positive -> PObject.Infinite PObject.Empty
        | _ as n when n < ExtendedBigRational.Zero -> PObject.Empty
        | _ as n when n = ExtendedBigRational.Zero -> newValue
        | _ as n ->
            Seq.init (GetPositiveInt cValueIndex) (fun _ -> PObject.Empty) 
            |> Seq.insertAt (GetPositiveInt cValueIndex) newValue
            |> PObject.FromSeq
    | Sequence l ->
        l |> Seq.mapi (fun i x -> 
            match cValueIndex with
            | Rational _ -> if i = l.GetEffectiveIndex cValueIndex then newValue else x
            | _ -> x
        )
          |> PObject.FromSeq

  member private this.GetReplacementObject(cValue: PObject)(cValueIndex: PObject)(newValue: PObject) =
    match cValueIndex.Value with
    | Atom a ->
        match a with
        | PNumber n -> this.GetReplacementObjectWithNumericIndex cValue n newValue
        | _ -> System.NotImplementedException "First-class operators not yet implemented" |> raise
    | Sequence l ->  this.GetReplacementObjectWithListIndex cValue l newValue |> PObject.FromSeq
    | Empty -> this.GetReplacementObjectWithListIndex cValue (Seq.empty |> PList) newValue |> PObject.FromSeq
  
  member private this.ReplaceReference (refObj: PObject) (refIndex: PObject) (newValue: PObject) =
    // the high level thing for base case is referencedObject@referenceIndex = newValue
    // at least in non-recursive case, assign mechanics is such that length of newValue does not exceed length of referenceIndex
    match refObj.Metadata.Reference with
    | Variable name ->
        let replacementValue = this.GetReplacementObject refObj refIndex newValue
        if control.TrySetVariable(name, refObj, replacementValue) then
          control.GetVariable name
        else 
          replacementValue
    | Reference (ro, ri) ->  // this recursion step feels too easy for this data structure, could be wrong
        this.ReplaceReference ro ri ((this.GetReplacementObject refObj refIndex newValue).WithReference(Reference(ro, ri)))  
    | Void -> PrimellProgrammerProblemException "This method shouldn't be called with Void reference" |> raise

  // called when left is either PAtom, empty list, or manually per operation modifier
  member private this.PerformAtomicAssign (left: PObject, right: PObject) =
    match left.Metadata.Reference with
    | Void -> right // no action needed
    | Variable name -> 
        if control.TrySetVariable(name, left, right) then
          control.GetVariable name
        else
          right
    | Reference (refObj, refIndex) -> this.ReplaceReference refObj refIndex right

  member private this.PerformListAssign (leftList: PObject, right: PObject, assignMods: OperationModifier list) =
    match leftList.Value with
    | Sequence left ->
        let newValue = 
          match right.Value with
          | Empty ->  
              let newLvalue = left |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods))
              newLvalue |> PObject.FromSeq
          | Sequence l ->
              let temp = (left, l) ||> Seq.zip |> Seq.map(fun x -> this.PerformAssign(fst x, snd x, assignMods))
              let real =  // TODO - more problems with infinite lists
                if left.Length > l.Length then 
                  Seq.append temp (left |> Seq.skip (Seq.length l))
                else temp
              real |> PObject.FromSeq
          | _ ->
                let newLvalue = left |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods))
                newLvalue |> PObject.FromSeq
        
        match leftList.Metadata.Reference with
        | Void -> newValue // no action needed
        | Variable name -> 
            if control.TrySetVariable(name, leftList, newValue) then
              control.GetVariable name
            else
              newValue
        | Reference (refObj, refIndex) -> this.ReplaceReference refObj refIndex newValue
    | _ -> PrimellProgrammerProblemException "Cannot call list assign with non-list" |> raise

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
    match left.Value, (assignMods |> List.contains Power) with
    | _, true ->
        this.PerformAtomicAssign(left, right)
    | Sequence _, _ ->
        this.PerformListAssign(left, right, assignMods)
    | _ -> 
        this.PerformAtomicAssign(left, right)
    // Note: in case you get the bright idea to stick the control.LastOperationWasAssignment here again, lazy eval
  
  override this.VisitStdAssign context =
    let assignMods = ParseLib.ParseOperationModifiers (context.binaryAssign().assignMods().GetText())

    let right = // must execute right side first
      match context.rtlTerm() with
      | null -> this.Visit(context.termSeq())
      | _ as rtlCtxt -> this.Visit(rtlCtxt)
    let left = this.Visit(context.mulTerm())

    let interimResult =
      match context.binaryAssign().binaryOp() with
      | null -> right
      | _ as opContext -> 
          if opContext.baseBinaryOp().conditionalOp() |> isNull then
            this.ApplyBinaryOperation left (right |> NonConditional) opContext           
          else
            System.NotImplementedException "Conditional + assign not implemented" |> raise

    control.LastOperationWasAssignment <- true
    this.PerformAssign(left, interimResult, assignMods)

  override this.VisitForEachLeftAssign context = 
    System.NotImplementedException "Assign foreach not implemented" |> raise
  
  override this.VisitForEachRightAssign context = 
    System.NotImplementedException "Assign foreach not implemented" |> raise

  
  member private this.VisitConditionalRight (right: BinaryRHS)(leftIsTruth: bool)(useHeadOnTruth: bool) =
    match right, leftIsTruth = useHeadOnTruth with
    | DeferredConditional ctxt, true -> this.VisitRtlTermSeqHead(ctxt)
    | DeferredConditional ctxt, false -> this.VisitRtlTermSeqTail(ctxt)
    | EvaluatedConditional pobj, true -> operationLib.Head pobj
    | EvaluatedConditional pobj, false -> operationLib.Tail pobj
    | PartialConditional (pobj, _), true -> operationLib.Head pobj
    | PartialConditional (pobj, rest), false -> 
        let start =
          match (operationLib.Tail pobj).Value with
          | Sequence l -> l
          | _ -> Seq.singleton pobj |> PList
        start.AppendAll (this.VisitConcatRtlTermSeq rest) |> PObject.FromSeq
    | _ -> PrimellProgrammerProblemException "non-conditional in conditional code" |> raise

  member private this.Conditional (left: PObject) (right: BinaryRHS) (condContext: PrimellParser.ConditionalOpContext) =
    let negate = condContext.cond_mod_neg() |> isNull |> not
    //let useHeadOnTruth = condContext.cond_mod_tail() |> isNull
    let effectiveBool = operationLib.IsTruth(left, control.Settings.PrimesAreTruth, control.Settings.RequireAllTruth) <> negate

    if effectiveBool then
      let loopIsDoWhile = 
        match condContext.condLoop() with
        | null -> None
        | _ as nestedCtxt -> Some (nestedCtxt.cond_loop_do_while() |> isNull |> not)
      
      let result = this.VisitConditionalRight right effectiveBool true
      match loopIsDoWhile with
      | None ->  // straight if-else expression
          result
      | Some _ ->
          System.NotImplementedException "conditional loops useless before first-class operators" |> raise

    else
      this.VisitConditionalRight right effectiveBool true

  member private this.ApplyBinaryOperation (left: PObject) (right: BinaryRHS) (context: PrimellParser.BinaryOpContext) =

    let opText = context.baseBinaryOp().GetText()
    match right with
    | NonConditional pobj -> 
        let retval = operationLib.ApplyBinaryOperation left pobj opText (ParseLib.ParseOperationModifiers (context.opMods().GetText()))
        if opText = "@" then  // index needs special handling for the whole reference-assign
          retval.WithReference(Reference(left, pobj))
        else 
          retval
    | _ ->   // TODO - ive just horridly realized that in theory you could put regular mods on conditional op... 
        this.Conditional left right (context.baseBinaryOp().conditionalOp())  

  member private this.EvaluateAtomForConditional(atomCtxt: PrimellParser.AtomTermContext) =
    match atomCtxt with
    | :? PrimellParser.ParensContext as parensCtxt ->
        this.DrillDownTermSeqForConditonal <| parensCtxt.termSeq()
    | _ ->  // we *want* to visit nullary/variables to get their real values!
            // their contents aren't evaluated yet due to laziness in Seq!
            // (and the other cases are constants where we might as well evaluate now)
        EvaluatedConditional <| this.Visit(atomCtxt)
    
  member private this.DrillDownTermSeqForConditonal(termSeqCtxt: PrimellParser.TermSeqContext) =
    let rtlTermSeq = termSeqCtxt.concatRtlTerm()
    match (Seq.head rtlTermSeq).concat() with  // per grammar always at least one concatRtlTerm
    | null -> 
        if termSeqCtxt.concatRtlTerm().Length = 1 then // it could be a "fake" atom (redundant nested parentheses)
          match (Seq.head rtlTermSeq).rtlTerm() with
          | :? PrimellParser.PassThroughRtlContext as ptRtlCtxt ->
            match ptRtlCtxt.mulTerm() with
            | :? PrimellParser.PassThroughMulTermContext as ptMulCtxt ->
                this.EvaluateAtomForConditional <| ptMulCtxt.atomTerm() 
            | _ -> DeferredConditional <| rtlTermSeq
          | _ -> DeferredConditional <| rtlTermSeq
        else DeferredConditional <| rtlTermSeq
    | _ -> 
        // note we're not actually evaluating the whole sequence, Seqs are lazy
        let indexedResults =  // basically evaluate ;concat terms until they stop being ;concat or stop being empty
          termSeqCtxt.concatRtlTerm() 
          |> Seq.takeWhile(fun x -> x.concat() |> isNull |> not) 
          |> Seq.map(fun x -> this.Visit(x)) 
          |> Seq.indexed  
        match indexedResults |> Seq.tryFind(fun x -> PObject.Empty.Equals x |> not) with
        | None -> DeferredConditional (rtlTermSeq |> Seq.skip (Seq.length indexedResults)) // all concat terms evaluated empty, skip them
        | Some result -> PartialConditional (snd result, rtlTermSeq |> Seq.skip((fst result) + 1))

  
  member private this.GetLeftRightBinaryOperands (leftContext: ParserRuleContext)(rightContext: PrimellParser.BinaryOpWithRSContext) =
  (* CAUTION: this method does incredibly unexpected stuff with conditionals
     This is because to get conditional to work the "intuitive" way, It actually *doesn't* defer evaluation of the RHS, 
     but defers evaluation of the *contents* of the RHS... this distinction is very rare, but consider:

     (some boolean stand-in) ?$ (nullary operation) 

     In order it does the following:
     1. Evaulate the nullary first due to $ RTL (possibly with side-effects),
     2. Evaluate the boolean condition (possibly with side-effects)
     3. Later (up the call chain) evaluate the head or tail based on the boolean

     Now, why did I decide to write this method *before* first-class operators, when deferred eval should be available?
     
     1. I want this to work independently of that in case the spec changes for one but not the other
     2. I anticipate the real difficulty in conditionals is more the selectively evaluating the head or tail part,
          and that would be the case regardless of a better deferred evaluation being available
     3. My first idea for generalized deferred eval is hacky, and I don't want the conditional to be hacky
     4. After i wrote the above, the ;append (concat) functionality may prove this approach definitely correct
  *)
    let isRtl = rightContext.RTL() |> isNull |> not

    let firstObj = 
      if isRtl then 
        this.VisitRhs rightContext
      else NonConditional <| this.Visit(leftContext)

    let secondObj =
      if isRtl then 
        NonConditional <| this.Visit(leftContext)
      else 
        this.VisitRhs rightContext

    if isRtl then 
      match secondObj with
      | NonConditional pobj -> pobj
      | _ -> PrimellProgrammerProblemException "not possible" |> raise
      , firstObj
    else 
      match firstObj with
      | NonConditional pobj -> pobj
      | _ -> PrimellProgrammerProblemException "not possible" |> raise
      , secondObj

  member private this.VisitRhs(rightContext: PrimellParser.BinaryOpWithRSContext) =
    let isConditional = rightContext.binaryOp().baseBinaryOp().conditionalOp() |> isNull |> not
    let ignoreConditional = 
      isConditional 
      && rightContext.L_BRACK() |> isNull |> not 
      &&  rightContext.termSeq().concatRtlTerm() |> Seq.exists(fun x -> x.concat() |> isNull |> not)

    let result = 
      match rightContext.atomTerm(), isConditional && not ignoreConditional with
      | null, true -> this.DrillDownTermSeqForConditonal <| rightContext.termSeq()
      | null, false -> NonConditional <| this.Visit(rightContext.termSeq())
      | _ as aCtxt, true -> this.EvaluateAtomForConditional <| aCtxt
      | _ as aCtxt, false -> NonConditional <| this.Visit(aCtxt)
    
    // a bit hacky to ignore the conditional then make it conditional... this is mostly so its cleanly tracked
    // what's really conditional and not, even though functionally, i don't think there's actually a difference
    // between an evaluated conditional and a non-conditional
    match result, ignoreConditional with   
    | NonConditional pobj, true -> EvaluatedConditional pobj
    | _ -> result

  member private this.ApplyForEachRight(left: PObject)(right: BinaryRHS)(boCtxt: PrimellParser.BinaryOpContext) =
    
    // awkwardly you have box the conditional right back up...
    // it sort of makes sense because with the for-each-right, theres two layers of deferred evals possible
    // but there are probably more efficient ways of doing it
    match right with
    | NonConditional pobj ->
        match pobj.Value with
        | Sequence l ->
            l |> Seq.map(fun x -> this.ApplyBinaryOperation left (NonConditional x) boCtxt) |> PObject.FromSeq
        | Empty -> System.NotImplementedException "Empty objects in conditionals not fully implemented" |> raise  // TODO
        | _ -> this.ApplyBinaryOperation left (NonConditional pobj) boCtxt
    | EvaluatedConditional pobj ->
        match pobj.Value with
        | Sequence l ->
            l |> Seq.map(fun x -> this.ApplyBinaryOperation left (EvaluatedConditional x) boCtxt) |> PObject.FromSeq
        | Empty -> System.NotImplementedException "Empty objects in conditionals not fully implemented" |> raise  // TODO
        | _ -> this.ApplyBinaryOperation left (EvaluatedConditional pobj) boCtxt
    | PartialConditional _ ->
        PrimellProgrammerProblemException "partial conditional not allowed in for-each right" |> raise
    | DeferredConditional tsCtxt ->
        System.NotImplementedException "Conditional in right-handed foreach planned for after first-class operators" |> raise
        // Actually here, deferred eval from first-class operators implementation should work fine, as we need to
        // defer the whole list without regards to head/tail (the individual sub items are the head/tail ones)

  override this.VisitBinaryOperation context =
    let rhsCtxt = context.binaryOpWithRS()
    match rhsCtxt.L_BRACK() with // presence of bracket indicates for-each right side
    | null -> 
        let left, right = this.GetLeftRightBinaryOperands (context.mulTerm()) rhsCtxt
        this.ApplyBinaryOperation left right (rhsCtxt.binaryOp())
    | _ ->
        let left = this.Visit(context.mulTerm())
        let right = this.VisitRhs rhsCtxt

        this.ApplyForEachRight left right (rhsCtxt.binaryOp())

  override this.VisitForEachUnary context =
    let pObj = this.Visit(context.termSeq())
    match pObj.Value with
    | Sequence l ->
        l |> Seq.map(fun pobj -> this.ApplyUnaryOperation pobj (context.unaryOp())) |> PObject.FromSeq
    | _ ->
        this.ApplyUnaryOperation pObj (context.unaryOp())

  override this.VisitForEachLeftBinary context =
    let rhsCtxt = context.binaryOpWithRS()
    let left, right = this.GetLeftRightBinaryOperands (context.termSeq()) rhsCtxt

    match left.Value with
    | Sequence l -> 
        l |> Seq.map(fun pobj -> this.ApplyBinaryOperation pobj right (rhsCtxt.binaryOp())) |> PObject.FromSeq 
    | _ ->
        this.ApplyBinaryOperation left right (rhsCtxt.binaryOp())

  member private this.OpChain (pobj: PObject) (context: PrimellParser.UnaryOrBinaryOpContext seq) =
    (pobj, context) ||> Seq.fold (fun p op ->
      match op.binaryOpWithRS() with
      | null -> 
          this.ApplyUnaryOperation p (op.unaryOp())
      | _ as rhsCtxt -> 
          let right = this.VisitRhs(rhsCtxt)
          match rhsCtxt.L_BRACK() with // presence of bracket indicates for-each right side
          | null ->
              this.ApplyBinaryOperation p right (rhsCtxt.binaryOp())
          | _ ->
              this.ApplyForEachRight p right (rhsCtxt.binaryOp())
    )

  override this.VisitForEachChain context =
    let left = this.Visit(context.termSeq())
    match left.Value with
    | Sequence l -> l |> Seq.map(fun x -> this.OpChain x (context.unaryOrBinaryOp())) |> PObject.FromSeq
    | _ -> this.OpChain left (context.unaryOrBinaryOp())
     

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