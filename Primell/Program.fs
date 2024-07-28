open dpenner1.Primell

let runner = PrimellRunner()
runner.InteractiveMode()

//let control = runner.Run "(2 (3 5) 7) = 11" PrimellConfiguration.PrimellDefault
//let control = runner.Run "5/0" PrimellConfiguration.Listell

//printfn "%s" <| runner.GetResultString (List.last results |> fst) control


(* Potentially useful (but out of date) simplification of the objects
open System.Collections.Generic
// setting up types for the example, see below line for main code with issue
[<AbstractClass>]
type TestObject() =
  abstract member ToString: Dictionary<string, TestObject> -> string
  default this.ToString(variables: Dictionary<string, TestObject>) =
    this.ToString()

type TestSeq(values: seq<TestObject>) =
  inherit TestObject()
  let values = values
  interface seq<TestObject> with
    member this.GetEnumerator() = values.GetEnumerator()
    member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

  override this.ToString(variables) =
    String.concat " " (values |> Seq.map (fun x -> x.ToString(variables)))

type TestStateVariable(name: string, value: TestObject) =
  inherit TestObject()
  member val Name = name with get
  member val CapturedValue = value with get
  override this.ToString(variables) = this.CapturedValue.ToString()
  
type TestNumber(value: int) =
  inherit TestObject()
  member val Value = value with get
  override this.ToString() = this.Value.ToString()

//-----------------------------Main Code----------------------------------//
let programVariables = Dictionary<string, TestObject>()
programVariables.Add("x", TestNumber 0)
programVariables.Add("y", TestNumber 0)

let capturedY = TestStateVariable("y", programVariables["y"])
let newSeq = seq { capturedY  :> TestObject; TestNumber 1; TestNumber 2 } |> TestSeq

let newX = newSeq |> Seq.map(fun x ->
                                match x with
                                | :? TestStateVariable as sv -> sv.CapturedValue
                                | _ -> x) // would also have to recurse a nested TestSeq
                  |> TestSeq

programVariables["x"] <- newX
printfn "%s" <| programVariables["x"].ToString(programVariables)  // 0 1 2

programVariables["y"] <- TestNumber 33  // I don't want to see this change reflected in x
printfn "%s" <| programVariables["x"].ToString(programVariables) // 33 1 2 (not desired)
*)