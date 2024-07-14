namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl(settings: PrimellConfiguration) =

  let variables = new Dictionary<string, PObject>()
  member val Settings = settings with get
  

  // This is a port from original Primell, copying to have current examples execute as before,
  // Mingh consider changing the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set

  // don't call unless there actually is a parent!
  member this.UpdateParent(parent: PObject, newChild: PObject, ?stopRecursingAt: PObject) =

    // highly suspect stuff
    match parent with
    | :? PReference as ref ->
        let currentValue = variables[ref.Name]
        let newValue = 
          match currentValue with
          | :? PList as l ->
              l |> Seq.removeAt newChild.IndexInParent.Value |> Seq.insertAt newChild.IndexInParent.Value newChild |> PList :> PObject
          | _ -> newChild
        variables[ref.Name] = newValue |> ignore
    | :? PList as l -> 
      match parent.Parent with
      | None -> ()
      | Some grandParent -> 
          match stopRecursingAt with
          | Some pobj when obj.ReferenceEquals(stopRecursingAt, grandParent) -> ()
          | _ -> 
              let newParentValue = l |> Seq.removeAt newChild.IndexInParent.Value |> Seq.insertAt newChild.IndexInParent.Value newChild
              let newParent = PList(newParentValue, l.Length, ?parent = l.Parent, ?indexInParent = l.IndexInParent)
              this.UpdateParent (grandParent, newParent, ?stopRecursingAt = stopRecursingAt) // need to recurse in case there is a higher variable to set
    
    | _ -> failwith "Should not have parent that isn't list or reference"

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