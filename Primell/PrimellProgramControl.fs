namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl(settings: PrimellConfiguration) =
  member val Settings = settings with get
  member val Variables = new Dictionary<string, IPrimellObject>()

  // This is a port from original Primell, copying to have current examples execute as before,
  // But I think I want to change the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set

  member this.UpdateParent(parent: PList, newChild: IPrimellObject, ?stopRecursingAt: IPrimellObject) =
    let newParentValue = parent |> Seq.removeAt newChild.IndexInParent.Value |> Seq.insertAt newChild.IndexInParent.Value newChild
    let newParent = PList(newParentValue, parent.Length, ?name = parent.Name, ?parent = parent.Parent, ?indexInParent = parent.IndexInParent)
    
    match parent.Name with
    | None -> 
      match parent.Parent with
      | None -> ()
      | Some grandParent -> 
          match stopRecursingAt with
          | Some pobj when obj.ReferenceEquals(stopRecursingAt, grandParent) -> ()
          | _ -> this.UpdateParent (grandParent :?> PList, newParent, ?stopRecursingAt = stopRecursingAt) // need to recurse in case there is a higher variable to set
    
    | Some name -> this.SetVariable(name, newParent, ?stopRecursingAt = stopRecursingAt)  // don't need to recurse here, because SetVariable will call UpdateParent

  member this.SetVariable(variable: string, value: IPrimellObject, ?stopRecursingAt: IPrimellObject) = 
    if this.Variables.ContainsKey variable then 
      this.Variables[variable] <- value
      match this.Variables[variable].Parent with
      | None -> ()
      | Some p -> 
        match stopRecursingAt with
        | Some pobj when obj.ReferenceEquals(stopRecursingAt, p) -> ()
        | _ -> this.UpdateParent (p :?> PList, this.Variables[variable], ?stopRecursingAt = stopRecursingAt)
    else failwith "I don't think this is ever called without an existing variable (but not sure, so explicit exception)"
  
  member this.GetCodeInput() =
    failwith "Not yet implemented"

  member this.GetStringInput() =
    failwith "Not yet implemented"

  member this.GetCsvInput() =
    failwith "Not yet implemented"