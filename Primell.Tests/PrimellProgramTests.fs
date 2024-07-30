module PrimellProgramTests

open Xunit
open dpenner1.Primell

let TestProgram(program: string, settings: PrimellConfiguration, expectedResult: string) =
  let runner = PrimellRunner()
  let control = runner.Run program settings
  let outputCheck = control.LineResults 
                    |> Array.filter(fun lineRec -> match lineRec.Output with | Some _ -> true | _ -> false)
                    |> Array.map(fun lineRec -> lineRec.Output.Value)
                    |> String.concat "\n"
  Assert.Equal(expectedResult, outputCheck)

let TestProgramFromFile(filePath: string, settings: PrimellConfiguration, expectedResult: string) = 
  let lines = System.IO.File.ReadAllLines(filePath)
  TestProgram(lines |> String.concat "\n", settings, expectedResult)

[<Fact>]
let ``Test Included Examples``() =
  TestProgramFromFile("../../../../Examples/PrimesTo100.pll", PrimellConfiguration.PrimellDefault, "2 3 5 7 11 13 17 19 23 29 31 37 41 43 47 53 59 61 67 71 73 79 83 89 97")
  TestProgramFromFile("../../../../Examples/NumbersTo100.pll", PrimellConfiguration.PrimellDefault, "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100")

[<Fact>]
let ``Test Concat``() =
  TestProgram(",", PrimellConfiguration.PrimellDefault, "()")
  TestProgram(", ()", PrimellConfiguration.PrimellDefault, "() ()")
  TestProgram(",;()", PrimellConfiguration.PrimellDefault, "()")
  TestProgram(";,()", PrimellConfiguration.PrimellDefault, "()")
  TestProgram("2 3;(5 7)", PrimellConfiguration.PrimellDefault, "2 3 5 7")
  TestProgram(";(2 3) 5 7", PrimellConfiguration.PrimellDefault, "2 3 5 7")


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
  TestProgram("(11 13 (17 19) 23)@((3 2)(2 3))", PrimellConfiguration.PrimellDefault, "(23 (17 19)) ((17 19) 23)")

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
  TestProgram(", = (2 3)\n, = ()\n,", PrimellConfiguration.PrimellDefault, "() ()")

  TestProgram("(, ,,) = (2 3)\n, ,,", PrimellConfiguration.PrimellDefault, "2 3")
  TestProgram(", = ,, = 3\n, ,,", PrimellConfiguration.PrimellDefault, "3 3")
  TestProgram(", = 2 3\n,", PrimellConfiguration.PrimellDefault, "2 3\n2") // assignment isnt last op, concat is
  TestProgram(", = ,, = 3\n,=5\n, ,,", PrimellConfiguration.PrimellDefault, "5 3")
  TestProgram(", = ,, = 3\n,,=5\n, ,,", PrimellConfiguration.PrimellDefault, "3 5")
  
  TestProgram(", = (2 3 5)\n, = (7 11 13)\n,", PrimellConfiguration.PrimellDefault, "7 11 13")
  TestProgram(", = (2 3 5)\n, = (7 11)\n,", PrimellConfiguration.PrimellDefault, "7 11 5")
  TestProgram(", = (2 3)\n, = (5 7 11)\n,", PrimellConfiguration.PrimellDefault, "5 7")  // spec might change in future

[<Fact>]
let ``Test Index + Assign``() =
  TestProgram(", = (2 3 5 7)\n,@2 = 11\n,", PrimellConfiguration.PrimellDefault, "2 3 11 7")
  TestProgram(", = (2 3 5 7)\n,@2 = (2 3)\n,", PrimellConfiguration.PrimellDefault, "2 3 (2 3) 7")
  TestProgram(", = (2 3 5 7)\n,@(2 3) = (11 13)\n,", PrimellConfiguration.PrimellDefault, "2 3 11 13")
  TestProgram(", = (2 3 5 7)\n,@(2 3) = 13\n,", PrimellConfiguration.PrimellDefault, "2 3 13 13")
  TestProgram(", = (2 3 (5 7))\n,@2 = 11\n,", PrimellConfiguration.PrimellDefault, "2 3 (11 11)")

  // test immediacy of evaluation
  TestProgram(", = (,, 3 5)\n,, = 2\n,\n,,", PrimellConfiguration.PrimellDefault, "() 3 5\n2")

  // implicit filling with empty
  TestProgram(",@2 = 5\n,", PrimellConfiguration.PrimellDefault, "() () 5")
  TestProgram(", = 2\n,@2 = 5\n,", PrimellConfiguration.PrimellDefault, "2 () 5")
  TestProgram(",@(2 3) = (5 7)\n,", PrimellConfiguration.PrimellDefault, "() () 5 7")
  

[<Fact>]
let ``Test Conditional``() =
  TestProgram("2?(2 3 5)", PrimellConfiguration.PrimellDefault, "2")
  TestProgram("2?~(2 3 5)", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram("2+2 ?(2 3 5)", PrimellConfiguration.PrimellDefault, "3 5")

  TestProgram("2+2 ?~(2 3 5)", PrimellConfiguration.PrimellDefault, "2")

[<Fact>]
let ``Test Foreach Binary``() =
  TestProgram("2::{(3 5)(7 11)}", PrimellConfiguration.PrimellDefault, "(2 3 5) (2 7 11)")
  TestProgram("3 2::{(3 5)(7 11)}", PrimellConfiguration.PrimellDefault, "3 ((2 3 5) (2 7 11))")
  TestProgram("3 ; 2::{(3 5)(7 11)}", PrimellConfiguration.PrimellDefault, "3 (2 3 5) (2 7 11)")

  TestProgram("[(2 3 5 7)(11 13 17) 19]@2", PrimellConfiguration.PrimellDefault, "5 17 ()")
  TestProgram("[(2 3 5 7)(11 13 17) 19]@2 3", PrimellConfiguration.PrimellDefault, "(5 17 ()) 3")
  TestProgram(";[(2 3 5 7)(11 13 17) 19]@2 3", PrimellConfiguration.PrimellDefault, "5 17 () 3")

[<Fact>]
let ``Test Foreach Unary``() =
  TestProgram("[(3 5)(7 11)]_~", PrimellConfiguration.PrimellDefault, "(5 3) (11 7)")

[<Fact>]
let ``Test Foreach Chain``() =
  TestProgram("((2 3 5)(7 11 13))[_~_<]", PrimellConfiguration.PrimellDefault, "5 13")

  