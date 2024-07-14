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
    

  member this.SetVariable(name: string, value: PObject) = 
    if variables.ContainsKey name then 
      variables[name] <- value.WithParent(new PReference(name), 0) // index doesn't matter
    else failwith "I don't think this is ever called without an existing variable (but not sure, so explicit exception)"
  
  member this.GetCodeInput() =
    failwith "Not yet implemented"

  member this.GetStringInput() =
    failwith "Not yet implemented"

  member this.GetCsvInput() =
    failwith "Not yet implemented"