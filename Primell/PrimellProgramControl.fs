namespace dpenner1.Primell

open System.Collections.Generic

type LineRecord =
  {
    Text: string
    Result: PObject option
    Output: string  // tracking this has so far been purely for test verification, nothing internal to Primell
  }

type PrimellProgramControl(settings: PrimellConfiguration, lines: string seq, ?variables: Dictionary<string, PObject>) =

  member val Variables = defaultArg variables (Dictionary<string, PObject>()) with get  
  member val Settings = settings with get

  // TODO - can you get rid of this mutable?
  member val CurrentLine = 0 with get, set
  
  member val LineResults = 
    lines |> Array.ofSeq |> Array.map (fun l -> { Text = l; Result = None; Output = ""; }) with get

  // This is a port from original Primell, copying to have current examples execute as before,
  // Might consider changing the part of Primell requiring this
  member val LastOperationWasAssignment = false with get, set
  member val LastOperationWasOutput = false with get, set

  member this.GetVariable(name: string) = 
    if not <| this.Variables.ContainsKey(name) then
      this.Variables.Add(name, PList.Empty.WithReference(Variable name)) |> ignore
    this.Variables[name]

  // hopefully this works, but the idea is that with so much recursion potentially going on
  // (and cycles are probably possible, and definitely possible after first-class operators)
  // a variable we mean to set has already been set by a different recursive branch
  // so we first check that that hasn't happened with a Object.Reference equality
  member this.TrySetVariable(name: string, oldValue: PObject, newValue: PObject) = 

    if this.Variables.ContainsKey name then
      let valid = obj.ReferenceEquals(oldValue, this.Variables[name])
      if valid then this.Variables[name] <- newValue.WithReference(Variable name) 
      valid
    else 
      this.Variables[name] <- newValue.WithReference(Variable name)
      true
    
  override this.ToString() =
    let valueString = this.Variables |> Seq.map (fun kvp -> $"({kvp.Key}, {kvp.Value})") |> String.concat ""
    String.concat "" ["["; valueString; "]"]

