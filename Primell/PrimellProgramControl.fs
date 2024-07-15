namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl(settings: PrimellConfiguration) =

  let variables = new Dictionary<string, PObject>()
  member val Settings = settings with get
  

  // This is a port from original Primell, copying to have current examples execute as before,
  // Mingh consider changing the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set

  member this.GetVariable(name: string) = 
    if not <| variables.ContainsKey(name) then
      variables.Add(name, PList.Empty.WithParent(new PReference(name), 0)) |> ignore

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