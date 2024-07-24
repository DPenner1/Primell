namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl(settings: PrimellConfiguration, lines: list<PrimellParser.LineContext>) =

  let variables = new Dictionary<string, PObject>()
  member val Settings = settings with get
  
  member val Lines = lines with get

  // This is a port from original Primell, copying to have current examples execute as before,
  // Might consider changing the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set

  member this.GetVariableReference(name: string) = 
    if not <| variables.ContainsKey(name) then
      variables.Add(name, PList.Empty) |> ignore

    PrimellReference(PrimellVariable(name), ExtendedBigRational.Zero |> PNumber) // index doesn't matter refering to variable

  member this.GetVariableValue(name: string) = 
    if not <| variables.ContainsKey(name) then
      variables.Add(name, PList.Empty) |> ignore

    variables[name]
    
  // temp
  member this.GetVariableDictionary() = variables

  member this.SetVariable(name: string, newValue: PObject) = 
    if variables.ContainsKey name |> not then 
      variables.Add(name, PList.Empty)
    variables[name] <- newValue
  
  member this.GetCodeInput(): PObject =
    System.NotImplementedException() |> raise

  member this.GetStringInput(): PObject =
    System.NotImplementedException() |> raise

  member this.GetCsvInput(): PObject =
    System.NotImplementedException() |> raise

// TODO - i am really hacking stuff now trying to get it to work  

and PrimellReference(parent: PObject, indexInParent: PNumber) =
  inherit PObject()

  member this.Parent with get() = parent
  member this.IndexInParent with get() = indexInParent

  member private this.IndexDown(pobj: PObject)(indexes: list<PNumber>) =
    match List.tryHead indexes with
    | None -> pobj // end of recursion
    | Some head ->
        match pobj with
        | :? PList as l -> this.IndexDown(l.Index head)(indexes.Tail)
        | :? PAtom as a -> this.IndexDown(Seq.singleton(a :> PObject) |> PList) indexes
        | _ -> PrimellProgrammerProblemException "Not possible" |> raise

  member private this.GetReferenceValue' (pref: PObject)(indexes: list<PNumber>)(variables: IDictionary<string, PObject>) =
    match pref with  // should only ever be PVariable or PReference (may consider merging the two...)
    | :? PVariable as v -> variables[v.Name]
    | :? PrimellReference as r -> this.GetReferenceValue' r.Parent (r.IndexInParent::indexes) variables
    | _ -> PrimellProgrammerProblemException "Not possible" |> raise

  member this.GetReferenceValue (pref: PrimellReference)(control: PrimellProgramControl) =
    this.GetReferenceValue' pref.Parent (List.singleton pref.IndexInParent) (control.GetVariableDictionary())

  override this.ToString(variables) =
    (this.GetReferenceValue' this.Parent (List.singleton this.IndexInParent) variables).ToString(variables)
    
    


type PReference = PrimellReference