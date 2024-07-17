module PrimellProgramTests

open Xunit
open dpenner1.PrimellF

let TestProgram(program: string, settings: PrimellConfiguration, expectedResult: string) =
  let runner = PrimellRunner()
  let control = PrimellProgramControl settings
  let results = runner.Run program control
  let actualResult = results 
                     |> List.where(fun r -> snd r) 
                     |> List.map(fun r -> runner.GetResultString (fst r) control)
                     |> String.concat "\n"
  Assert.Equal(expectedResult, actualResult)

let TestProgramFromFile(filePath: string, settings: PrimellConfiguration, expectedResult: string) = 
  let lines = System.IO.File.ReadAllLines(filePath)
  TestProgram(lines |> String.concat "\n", settings, expectedResult)

[<Fact>]
let ``Test Included Examples``() =
  TestProgramFromFile("../../../../Examples/PrimesTo100.pll", PrimellConfiguration.PrimellDefault, "2 3 5 7 11 13 17 19 23 29 31 37 41 43 47 53 59 61 67 71 73 79 83 89 97")

[<Fact>]
let ``Test Index``() =
  TestProgram("(2 3 5)@2", PrimellConfiguration.PrimellDefault, "5")
  TestProgram("(2 3 5)@(2-2)", PrimellConfiguration.PrimellDefault, "2")
  TestProgram("(2 3 5 7)@$(2/2)~", PrimellConfiguration.PrimellDefault, "7")
  TestProgram("(2 3 5 7)@$(2+2)~", PrimellConfiguration.PrimellDefault, "2")

  TestProgram("(2 3 5 7)@(2 3)", PrimellConfiguration.PrimellDefault, "5 7")
  TestProgram("(2 3 5 7)@(3~ 2)", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram("(2 3 5 7)@(2 2~)", PrimellConfiguration.PrimellDefault, "5 5")

  TestProgram("(2 3 5 7)@(2 (3 2~))", PrimellConfiguration.PrimellDefault, "5 (7 5)")

  // implicit empties
  TestProgram("()@2", PrimellConfiguration.PrimellDefault, "()")
  TestProgram("()@$2~", PrimellConfiguration.PrimellDefault, "()")


[<Fact>]
let ``Test Assign``() =
  TestProgram(", = 3\n,", PrimellConfiguration.PrimellDefault, "3")
  TestProgram(", = 3\n, = (3 5)\n,", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram(", = (2 3)\n,", PrimellConfiguration.PrimellDefault, "2 3")
  TestProgram(", = (2 3)\n, = 5\n,", PrimellConfiguration.PrimellDefault, "5 5")