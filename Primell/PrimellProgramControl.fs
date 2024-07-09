namespace dpenner1.PrimellF

open System.Collections.Generic

type PrimellProgramControl =
  val Settings: PrimellConfiguration
  val Variables: Dictionary<string, PObject>