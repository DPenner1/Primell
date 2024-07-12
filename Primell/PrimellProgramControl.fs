namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl =
  val Settings: PrimellConfiguration
  val Variables: Dictionary<string, PObject>

  // This is a port from original Primell, copying to have current examples execute as before,
  // But I think I want to change the part of Primell requiring this
  val mutable LastOperationWasAssignment: bool

  member this.GetCodeInput() =
    failwith "Not yet implemented"

  member this.GetStringInput() =
    failwith "Not yet implemented"

  member this.GetCsvInput() =
    failwith "Not yet implemented"