namespace dpenner1.Primell

open System.Collections.Generic

type PrimellProgramControl(settings: PrimellConfiguration, lines: list<PrimellParser.LineContext>) =

  member val Variables = new Dictionary<string, PObject>() with get
  member val Settings = settings with get
  
  member val Lines = lines with get

  // This is a port from original Primell, copying to have current examples execute as before,
  // Might consider changing the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set

  member this.GetVariable(name: string) = 
    if not <| this.Variables.ContainsKey(name) then
      this.Variables.Add(name, PrimellList(Seq.empty, ExtendedBigRational.Zero |> PNumber, Variable name)) |> ignore
    this.Variables[name]

  member this.SetVariable(name: string, newValue: PObject) = 
    if this.Variables.ContainsKey name |> not then 
      this.Variables.Add(name, PList.Empty)
    this.Variables[name] <- newValue
  
  member this.GetCodeInput(): PObject =
    System.NotImplementedException() |> raise

  member this.GetStringInput(): PObject =
    System.NotImplementedException() |> raise

  member this.GetCsvInput(): PObject =
    System.NotImplementedException() |> raise
    
  override this.ToString() =
    let valueString = this.Variables |> Seq.map (fun kvp -> $"({kvp.Key}, {kvp.Value})") |> String.concat ""
    String.concat "" ["["; valueString; "]"]

