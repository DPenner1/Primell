namespace dpenner1.PrimellF

open Antlr4.Runtime.Misc
open System.Collections.Generic

// TODO - port from original C# code, very mutable stuff, see if you can get rid of non-functional stuff later

type PrimellVisitor(control: PrimellProgramControl) = 
  inherit PrimellBaseVisitor<PObject>()
  let control = control
  let placeHolders = new Stack<PObject>()
  let currentForEach = new Stack<PObject>()
  let incorporate = new Stack<bool>()
  
  override this.VisitParens context =
    incorporate.Pop() |> ignore
    incorporate.Push true

    if context.termSeq() |> isNull then PObject.Empty else this.Visit(context.termSeq())

  override this.VisitTermSeq context =
    let mutable retval = PObject.Empty
    for termContext in context.mulTerm() do
      incorporate.Push true

      let pobj = this.Visit termContext
      if incorporate.Pop() then 
        retval <- retval.AppendAll pobj
      else
        retval <- retval.Append pobj
        
    retval |> PObject.Normalize

  override this.VisitInteger context =
    let text = context.GetText()
    let number = ParseLib.ParseInteger text control.Settings.SourceBase

    if control.Settings.RestrictedSource && not(PrimeLib.IsPrime number) then
      failwith "NON-PRIME DETECTED!"
    
    number |> Atom

  override this.VisitPositiveInfinity context = Infinity Positive |> Atom

  override this.VisitNegativeInfinity context = Infinity Negative |> Atom
  override this.VisitNullaryOp context =
    match context.baseNullaryOp().GetText() with
    | ":_" -> control.GetCodeInput()
    | ":~" -> control.GetStringInput(); // TODO - i want to change the symbol to :"
    | ":," -> control.GetCsvInput();  // TODO - this is anticipatory
    | _ as varName ->  // not an input, implicitly assumed to be a variable - TODO make sure thats actually true, grammar-wise
        if not <| control.Variables.ContainsKey(varName) then
          control.Variables[varName] <- PObject.Empty
        
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
    | Atom _ as robj ->
        this.ApplyBinaryOperation left robj (context.binaryOp())
    | PList l ->
        l |> Seq.map(fun robj -> this.ApplyBinaryOperation left robj (context.binaryOp())) |> PrimellList |> PList