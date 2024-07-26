module PrimellProgramTests

open Xunit
open dpenner1.PrimellF

let TestProgram(program: string, settings: PrimellConfiguration, expectedResult: string) =
  let runner = PrimellRunner()
  let results, control = runner.Run program settings
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
  TestProgramFromFile("../../../../Examples/NumbersTo100.pll", PrimellConfiguration.PrimellDefault, "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100")

[<Fact>]
let ``Test Nullary``() =
  TestProgram(",", PrimellConfiguration.PrimellDefault, "()")
  // Note: this didn't work in the original C#
  TestProgram(", ()", PrimellConfiguration.PrimellDefault, "() ()")

[<Fact>]
let ``Test Index``() =
  TestProgram("(2 3 5)@2", PrimellConfiguration.PrimellDefault, "5")
  TestProgram("(2 3 5)@(2-2)", PrimellConfiguration.PrimellDefault, "2")
  TestProgram("(2 3 5 7)@$(2/2)~", PrimellConfiguration.PrimellDefault, "7")
  TestProgram("(2 3 5 7)@$(2+2)~", PrimellConfiguration.PrimellDefault, "2")

  TestProgram("(2 3 5 7)@(2 3)", PrimellConfiguration.PrimellDefault, "5 7")
  TestProgram("(2 3 5 7)@(3~ 2)", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram("(2 3 5 7)@(2 2~)", PrimellConfiguration.PrimellDefault, "5 5")
  
  TestProgram("(2 3 (5 7))@2", PrimellConfiguration.PrimellDefault, "5 7")

  TestProgram("(2 3 5 7)@(2 (3 2~))", PrimellConfiguration.PrimellDefault, "5 (7 5)")

  // implicit empties
  TestProgram("()@2", PrimellConfiguration.PrimellDefault, "()")
  TestProgram("()@$2~", PrimellConfiguration.PrimellDefault, "()")
  TestProgram("3@2", PrimellConfiguration.PrimellDefault, "()")
  TestProgram("3@$2~", PrimellConfiguration.PrimellDefault, "()")


[<Fact>]
let ``Test Assign``() =
  TestProgram(", = 3\n,", PrimellConfiguration.PrimellDefault, "3")
  TestProgram(", = 3\n, = 5\n,", PrimellConfiguration.PrimellDefault, "5")
  TestProgram(", = 3\n, = (3 5)\n,", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram(", = (2 3)\n,", PrimellConfiguration.PrimellDefault, "2 3")
  TestProgram(", = (2 3)\n, = 5\n,", PrimellConfiguration.PrimellDefault, "5 5")
   //  turns out variables side by side never even worked in the original C#
  TestProgram("(, ;) = (2 3)\n,\n;", PrimellConfiguration.PrimellDefault, "2\n3")

[<Fact>]
let ``Test Index + Assign``() =
  TestProgram(", = (2 3 5 7)\n,@2 = 11\n,", PrimellConfiguration.PrimellDefault, "2 3 11 7")
  TestProgram(", = (2 3 5 7)\n,@2 = (2 3)\n,", PrimellConfiguration.PrimellDefault, "2 3 (2 3) 7")
  TestProgram(", = (2 3 5 7)\n,@(2 3) = (11 13)\n,", PrimellConfiguration.PrimellDefault, "2 3 11 13")
  TestProgram(", = (2 3 5 7)\n,@(2 3) = 13\n,", PrimellConfiguration.PrimellDefault, "2 3 13 13")
  TestProgram(", = (2 3 (5 7))\n,@2 = 11\n,", PrimellConfiguration.PrimellDefault, "2 3 (11 11)")

  // implicit filling with empty
  TestProgram(",@2 = 5\n,", PrimellConfiguration.PrimellDefault, "() () 5")
  TestProgram(", = 2\n,@2 = 5\n,", PrimellConfiguration.PrimellDefault, "2 () 5")

  // test immediacy of evaluation
  TestProgram(", = (; 3 5)\n; = 2\n,\n;", PrimellConfiguration.PrimellDefault, "() 3 5\n2")

[<Fact>]
let ``Test Conditional``() =
  TestProgram("2?(2 3 5)", PrimellConfiguration.PrimellDefault, "2")
  TestProgram("2?~(2 3 5)", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram("(2+2)?(2 3 5)", PrimellConfiguration.PrimellDefault, "3 5")

  TestProgram("(2+2)?~(2 3 5)", PrimellConfiguration.PrimellDefault, "2")