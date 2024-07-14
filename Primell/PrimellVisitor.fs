namespace dpenner1.PrimellF

open Antlr4.Runtime.Misc
open System.Collections.Generic

// TODO - port from original C# code, very mutable stuff, see if you can get rid of non-functional stuff later

type PrimellVisitor(control: PrimellProgramControl) = 
  inherit PrimellBaseVisitor<IPrimellObject>()
  let control = control
  let placeHolders = new Stack<IPrimellObject>()
  let currentForEach = new Stack<IPrimellObject>()
  let incorporate = new Stack<bool>()
  
  static member private Normalize (pobj: IPrimellObject) =
    match pobj with   // couldn't use when Seq.length = 1 as that potentially hangs on infinite sequence
    | :? PList as l when not(l.IsEmpty) && Seq.isEmpty(l.Tail()) -> 
        Seq.head l |> PrimellVisitor.Normalize
    | _ -> pobj    

  override this.VisitParens context =
    incorporate.Pop() |> ignore
    incorporate.Push true

    if context.termSeq() |> isNull then PList.Empty else this.Visit(context.termSeq())

  override this.VisitTermSeq context =
    let mutable retval = PList.Empty
    for termContext in context.mulTerm() do
      incorporate.Push true

      let pobj = this.Visit termContext
      if incorporate.Pop() then 
        retval <- retval.AppendAll pobj
      else
        retval <- retval.Append pobj
        
    retval |> PrimellVisitor.Normalize

  override this.VisitInteger context =
    let text = context.GetText()
    let number = ParseLib.ParseInteger text control.Settings.SourceBase

    if control.Settings.RestrictedSource && not(PrimeLib.IsPrime number) then
      failwith "NON-PRIME DETECTED!"
    
    number |> PNumber :> IPrimellObject

  override this.VisitPositiveInfinity context = Infinity Positive |> PNumber :> IPrimellObject

  override this.VisitNegativeInfinity context = Infinity Negative |> PNumber :> IPrimellObject
  override this.VisitNullaryOp context =
    match context.baseNullaryOp().GetText() with
    | ":_" -> control.GetCodeInput()
    | ":~" -> control.GetStringInput(); // TODO - i want to change the symbol to :"
    | ":," -> control.GetCsvInput();  // TODO - this is anticipatory
    | _ as varName ->  // not an input, implicitly assumed to be a variable - TODO make sure thats actually true, grammar-wise
        if control.Variables.ContainsKey(varName) |> not then
          control.Variables[varName] <- PrimellList.Empty
        
        control.Variables[varName]
          
  // TODO - switch this to UnaryNumeric in the grammar for consistency...
  override this.VisitNumericUnaryOperation context = 
    control.LastOperationWasAssignment <- false
    let operator = OperationLib.UnaryNumericOperators[context.numUnaryOp().baseNumUnaryOp().GetText()]
    
    OperationLib.ApplyUnaryNumericOperation (this.Visit(context.mulTerm())) operator []

  override this.VisitListUnaryOperation context =
    control.LastOperationWasAssignment <- false  
    let operator = OperationLib.UnaryListOperators[context.listUnaryOp().baseListUnaryOp().GetText()]
    
    OperationLib.ApplyUnaryListOperation (this.Visit(context.mulTerm())) operator []

  // don't call this unles newChild has parent set!
  

    //let x = parent |> Seq.removeAt newChild.IndexInParent.Value |> Seq.map(fun x -> x :> IPrimellObject) |> Seq.insertAt newChild.IndexInParent.Value newChild |> PList
    //parent

  member private this.PerformBasicAssign (left: IPrimellObject, right: IPrimellObject, ?stopRecursingAt: IPrimellObject) =
    match left.Name with
    | None -> 
        right.WithValueOnly()
    | Some name -> 
        control.SetVariable(name, right, ?stopRecursingAt = stopRecursingAt)
        control.Variables[name]


  member private this.PerformAssign (left: IPrimellObject, right: IPrimellObject, assignMods: OperationModifier list, ?stopRecursingAt: IPrimellObject): IPrimellObject =
    
    (* logic from original mutable C# version:
       if (left is empty) || (left is number) || (assignOptions has power modifier)
         then replace left with right in-place
       else if (right is number)
            // replace all items in left list with right in-place
            foreach (i in left.size): left[i].Assign right
       else // list to list parallel assignment
            // if left is bigger than right, extra values left intact
            // if right is bigger than left, extra values are discarded

            foreach (i in left.size): left[i].assign(right[i])   
    *)
    let newLeftValue = 
      if assignMods |> List.contains Power then
        this.PerformBasicAssign(left, right, ?stopRecursingAt=stopRecursingAt)
      else
        match left, right with
        | :? PNumber, _ ->
            this.PerformBasicAssign(left, right, ?stopRecursingAt=stopRecursingAt)
        | :? PList as l, _ when l.IsEmpty ->
            this.PerformBasicAssign(left, right, ?stopRecursingAt=stopRecursingAt)
        | :? PList as l, :? PNumber ->
            let newLvalue = l |> Seq.map(fun x -> this.PerformAssign(x, right, assignMods, ?stopRecursingAt=Some left))
            newLvalue |> PList :> IPrimellObject     
        | (:? PList as l1), (:? PList as l2) ->
            let temp = (l1, l2) ||> Seq.zip |> Seq.map(fun x -> this.PerformAssign(fst x, snd x, assignMods, ?stopRecursingAt=Some left))
            let real =
              if l1.Length.Value > l2.Length.Value then seq { temp |> PList :> IPrimellObject; l1 |> Seq.skip(Seq.length l2.Value) |> PList :> IPrimellObject }
              else temp
            real |> PList :> IPrimellObject 
        | _ -> failwith "not possible"

    let newLeft = 
      match newLeftValue with
      | :? PNumber as n -> PNumber(n.Value, ?name=left.Name, ?parent=left.Parent, ?indexInParent=left.IndexInParent) :> IPrimellObject
      | :? PList as l -> PList(l.Value, l.Length, ?name=left.Name, ?parent=left.Parent, ?indexInParent=left.IndexInParent) :> IPrimellObject
      | _ -> failwith "not possible"

    match left.Name with
    | Some n -> 
        control.SetVariable(n, newLeftValue)
    | None ->
        match left.Parent with
        | None -> ()
        | Some p -> control.UpdateParent(p :?> PList, newLeftValue)

    newLeftValue

  member this.ApplyBinaryOperation left right (context: PrimellParser.BinaryOpContext) =
    let isAssign = context.ASSIGN() |> isNull |> not

    let interimResult =
      if context.baseNumBinaryOp() |> isNull |> not then
        let operator = OperationLib.BinaryNumericOperators[context.baseNumBinaryOp().GetText()]
        OperationLib.ApplyBinaryNumericOperation left right operator []
      elif context.baseListBinaryOp() |> isNull |> not then
        let operator = OperationLib.BinaryListOperators[context.baseListBinaryOp().GetText()]
        OperationLib.ApplyBinaryListOperation left right operator []
      else 
        right

    control.LastOperationWasAssignment <- isAssign
      
    if isAssign then
      failwith "I'm dreading figuring this one out in F#"
    else 
      interimResult

  // TODO - did this one needed direct overriding? maybe could just override the deeper Numeric/List binary Operations
  //        also a bit ugly with the the ApplyBinaryOperation having Operation stuff and Parsing stuff
  override this.VisitBinaryOperation context =
    let left = this.Visit(context.mulTerm())

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
        l |> Seq.map(fun robj -> this.ApplyBinaryOperation left robj (context.binaryOp())) |> PList :> IPrimellObject
    | _ -> failwith "Not possible"
