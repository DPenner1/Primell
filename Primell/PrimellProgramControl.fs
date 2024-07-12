namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl(settings: PrimellConfiguration) =
  member val Settings = settings with get
  member val Variables = new Dictionary<string, PObject>()

  // This is a port from original Primell, copying to have current examples execute as before,
  // But I think I want to change the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set

  
  
  member this.GetCodeInput() =
    failwith "Not yet implemented"

  member this.GetStringInput() =
    failwith "Not yet implemented"

  member this.GetCsvInput() =
    failwith "Not yet implemented"