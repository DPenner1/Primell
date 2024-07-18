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
  
  static member private Normalize (pobj: PObject) =
    match pobj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
    | :? PList as l when not(l.IsEmpty) && Seq.isEmpty(l.Tail()) -> 
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

  
  // don't call unless there actually is a parent!
  // parent object is stored in newChild
  member private this.UpdateParent(newChild: PObject, ?stopRecursingAt: PObject) =

    match newChild.Parent.Value with
    | :? PReference as ref ->
        control.SetVariable(ref.Name, newChild)
        //let oldValue = control.GetVariable(ref.Name)
        //match oldValue with
        //| :? PList as l when not l.IsEmpty ->
        //    let newValue = l |> Seq.removeAt newChild.IndexInParent.Value |> Seq.insertAt newChild.IndexInParent.Value newChild |> PList
        //    control.SetVariable(ref.Name, newValue)
        //| _ -> control.SetVariable(ref.Name, newChild)
        // we also stop recursing when we hit a reference type, as parents containing a reference will dynamically pull the new value
    | :? PList as l -> 
      match l.Parent with
      | None -> ()
      | Some grandParent -> 
          match stopRecursingAt with
          | Some pobj when obj.ReferenceEquals(stopRecursingAt, grandParent) -> ()
          | _ -> 
              let newParentValue = l |> Seq.removeAt newChild.IndexInParent.Value |> Seq.insertAt newChild.IndexInParent.Value newChild
              let newParent = PList(newParentValue, l.Length, ?parent = l.Parent, ?indexInParent = l.IndexInParent)
              this.UpdateParent (newParent, ?stopRecursingAt = stopRecursingAt) // need to recurse in case there is a higher variable to set
    
    | _ -> PrimellProgrammerProblemException("Should not have parent that isn't list or reference... actually with virtual list extension, maybe it is?") |> raise
  
  member private this.PerformAssign (left: PObject, right: PObject, assignMods: OperationModifier list, ?stopRecursingAt: PObject): PObject =
    
    (* logic from original mutable C# version:
       if (left is empty) || (left is number) || (assignOptions has power modifier)
         then replace left with right in-place
       else if (right is number)
            // replace all items in left list with right in-place
            foreach (i in left.size): left[i].Assign right
       else // list to list parallel assignment
            // if left is bigger than right, extra values left intact
            // if right is bigger than left, extra values are discarded  (though looking back, i find this inconsistent)

            foreach (i in left.size): left[i].assign(right[i])   
    *)

    // Note: there's both upwards and downwards recursion in this method which makes it confusing:
    //  The upwards recursion is to propagate any assignment updates to a containing reference type
    //  The downward recursion is similar to operators in general in Primell, operators just get applied recursively down the object tree a lot
    //  But with assignment there's a special stopRecursingAt parameter: this is to prevent recursion ping pong of those downstream objects recursing back up

    let newLeftValue = 
      if assignMods |> List.contains Power then
        right
      else
        match left, right with  // get references out of the way for now, i'm sure this could be done better
        | :? PAtom, _ ->
            right
        | :? PList as l, _ when l.IsEmpty ->
            right
        | :? PList as l, :? PAtom ->
            let newLvalue = l |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods, ?stopRecursingAt=Some left))
            newLvalue |> PList :> PObject     
        | (:? PList as l1), (:? PList as l2) ->
            let temp = (l1, l2) ||> Seq.zip |> Seq.map(fun x -> this.PerformAssign(fst x, snd x, assignMods, ?stopRecursingAt=Some left))
            let real =
              if l1.Length.Value > l2.Length.Value then seq { temp |> PList :> PObject; l1 |> Seq.skip(Seq.length l2.Value) |> PList :> PObject }
              else temp
            real |> PList :> PObject 
        | _ -> PrimellProgrammerProblemException("not possible") |> raise
    
    // currently unused
    //let newLeft = 
    //  match newLeftValue with
    //  | :? PNumber as n -> PNumber(n.Value, ?parent=left.Parent, ?indexInParent=left.IndexInParent) :> PObject
     // | :? PList as l -> PList(l.Value, l.Length, ?parent=left.Parent, ?indexInParent=left.IndexInParent) :> PObject
     // | _ -> PrimellProgrammerProblemException("not possible") |> raise

    match left with
    | :? PReference as ref -> control.SetVariable(ref.Name, newLeftValue)
    | _ ->
        match left.Parent with
        | None -> ()
        | Some p -> this.UpdateParent(newLeftValue.WithParent(p, left.IndexInParent.Value), ?stopRecursingAt = stopRecursingAt)

    newLeftValue

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
    else 
      interimResult

  // TODO - did this one needed direct overriding? maybe could just override the deeper Numeric/List binary Operations
  //        also a bit ugly with the the ApplyBinaryOperation having Operation stuff and Parsing stuff
  override this.VisitBinaryOperation context =
    let left = this.Visit(context.mulTerm())
    
    // TODO - really need to adjust grammar to section off conditional, its a mess otherwise
    // TODO - here we need to handle conditional, and not visit head/tail of right, depending on the result

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
