module PrimellProgramTests

open Xunit
open dpenner1.Primell

let GetOutput(control: PrimellProgramControl) = 
  control.LineResults 
    |> Array.filter(fun lineRec -> System.String.IsNullOrEmpty lineRec.Output |> not)
    |> Array.map(fun lineRec -> lineRec.Output)
    |> String.concat "\n"
  

let TestProgram(program: string, settings: PrimellConfiguration, expectedResult: string) =
  let runner = PrimellRunner()
  let control = runner.Run program settings
  Assert.Equal(expectedResult, GetOutput control)

let TestEquivalentProgram(program1: string, settings1: PrimellConfiguration, program2: string, settings2: PrimellConfiguration) =
  let runner1 = PrimellRunner()
  let control1 = runner1.Run program1 settings1
  let runner2 = PrimellRunner()
  let control2 = runner2.Run program2 settings2

  Assert.Equal(GetOutput control1, GetOutput control2)
  

let TestProgramFromFile(filePath: string, settings: PrimellConfiguration, expectedResult: string) = 
  let lines = System.IO.File.ReadAllLines(filePath)
  TestProgram(lines |> String.concat "\n", settings, expectedResult)

[<Fact>]
let ``Test Included Examples``() =
  TestProgramFromFile("../../../../Examples/PrimesTo100.pll", PrimellConfiguration.PrimellDefault, "2 3 5 7 11 13 17 19 23 29 31 37 41 43 47 53 59 61 67 71 73 79 83 89 97")
  TestProgramFromFile("../../../../Examples/NumbersTo100.pll", PrimellConfiguration.PrimellDefault, "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100")
  TestProgramFromFile("../../../../Examples/HelloWorld.pll", PrimellConfiguration.PrimellDefault, "Hello, World!")

  TestProgramFromFile("../../../../Examples/NumbersTo100.lll", PrimellConfiguration.Listell, "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100")
  TestProgramFromFile("../../../../Examples/HelloWorld.lll", PrimellConfiguration.Listell, "Hello, World!")

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
  TestProgram("3@2", PrimellConfiguration.PrimellDefault, "()")

  // negative index wrap-around
  TestProgram("()@$2~", PrimellConfiguration.PrimellDefault, "()")
  TestProgram("3@$2~", PrimellConfiguration.PrimellDefault, "3")
  TestProgram("(2 3 5 7)@$7~", PrimellConfiguration.PrimellDefault, "3")
  TestProgram("(2 3 5 7 11)@$7~", PrimellConfiguration.PrimellDefault, "7")
  TestProgram("(2 3 5 7)@$11~", PrimellConfiguration.PrimellDefault, "3")
  TestProgram("(2 3 5 7 11)@$11~", PrimellConfiguration.PrimellDefault, "11")


[<Fact>]
let ``Test OpMod Truncate``() =
  TestProgram("(2 3 5)-`(2 2 2 2)", PrimellConfiguration.PrimellDefault, "0 1 3")
  TestProgram("(2 3 5 7)-`(2 2 2)", PrimellConfiguration.PrimellDefault, "0 1 3")
  
  // non truncated versions
  TestProgram("(2 3 5)-(2 2 2 2)", PrimellConfiguration.PrimellDefault, "0 1 3 2")
  TestProgram("(2 3 5 7)-(2 2 2)", PrimellConfiguration.PrimellDefault, "0 1 3 7")


[<Fact>]
let ``Test OpMod Unold``() =
// these will generate infinite sequences, so we'll need to combine with index to select a particular value
  TestProgram("3 +: 5 @ 3", PrimellConfiguration.PrimellDefault, "18")
  TestProgram("(2 3 5 7 11 13 17)_>: @ 3", PrimellConfiguration.PrimellDefault, "7 11 13 17")


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
let ``Test Assign + Op``()  =
  TestProgram(", =+ 3\n,", PrimellConfiguration.PrimellDefault, "3")  // this one currently fails due to list extension not being implemented
  TestProgram(", = 2\n, =+ 3\n,", PrimellConfiguration.PrimellDefault, "5")

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
let ``Test Branch``() =
  TestProgram(", + 5\n, = 2\n2!\\", PrimellConfiguration.PrimellDefault, "()\n7")
  TestProgram(", + 5\n(, =$ 2/2)!\\", PrimellConfiguration.PrimellDefault, "()\n6")
  TestProgram(", + 5\n(, =$ 2-2)!|", PrimellConfiguration.PrimellDefault, "()\n5")
  TestProgram(", = 2\n, =+ 5\n2/2 !\\", PrimellConfiguration.PrimellDefault, "12")
  TestProgram(",=2\n, =+ 5\n, =+ 7\n(2/2 2)!\\", PrimellConfiguration.PrimellDefault, "21 26")


[<Fact>]
let ``Test Conditional``() =
  TestProgram("2?(2 3 5)", PrimellConfiguration.PrimellDefault, "2")
  TestProgram("2?~(2 3 5)", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram("2+2 ?(2 3 5)", PrimellConfiguration.PrimellDefault, "3 5")
  TestProgram("2+2 ?~(2 3 5)", PrimellConfiguration.PrimellDefault, "2")

  //TODO: really need a test case for conditional + ;append-all, but as I write this, I've decided that's future me's problem

[<Fact>]
let ``Test Conditional Assign``() =
  TestProgram("2?((, = 3)(, = 5))\n,", PrimellConfiguration.PrimellDefault, "3")
  TestProgram("2?~((, = 3)(, = 5))\n,", PrimellConfiguration.PrimellDefault, "5")
  TestProgram("2+2 ?((, = 3)(, = 5))\n,", PrimellConfiguration.PrimellDefault, "5")
  TestProgram("2+2 ?~((, = 3)(, = 5))\n,", PrimellConfiguration.PrimellDefault, "3")

  // quick screen this still works with $
  TestProgram("2?$(, = 3)(, = 5)\n,", PrimellConfiguration.PrimellDefault, "3")
  TestProgram("2?~$(, = 3)(, = 5)\n,", PrimellConfiguration.PrimellDefault, "5")

[<Fact>]
let ``Test Conditional Branch``() =
  TestProgram("7\n11\n2?(2!\\ 2/2)", PrimellConfiguration.PrimellDefault, "7\n11\n7")
  TestProgram("7\n11\n(2-2)?(2!\\ 2/2)", PrimellConfiguration.PrimellDefault, "7\n11\n1")

[<Fact>]
let ``Test Foreach Binary``() =
  TestProgram("2[<::(3 5)(7 11)]", PrimellConfiguration.PrimellDefault, "(2 3 5) (2 7 11)")
  TestProgram("3 2[<::(3 5)(7 11)]", PrimellConfiguration.PrimellDefault, "3 ((2 3 5) (2 7 11))")
  TestProgram("3 ; 2[<::(3 5)(7 11)]", PrimellConfiguration.PrimellDefault, "3 (2 3 5) (2 7 11)")

  TestProgram("[(2 3 5 7)(11 13 17) 19]@2", PrimellConfiguration.PrimellDefault, "5 17 ()")
  TestProgram("[(2 3 5 7)(11 13 17) 19]@2 3", PrimellConfiguration.PrimellDefault, "(5 17 ()) 3")
  TestProgram(";[(2 3 5 7)(11 13 17) 19]@2 3", PrimellConfiguration.PrimellDefault, "5 17 () 3")

[<Fact>]
let ``Test Foreach Unary``() =
  TestProgram("[(3 5)(7 11)]_~", PrimellConfiguration.PrimellDefault, "(5 3) (11 7)")

let ``Test Foreach Double``() =
  TestProgram("[2 3 5][<:: 11 13 17]", PrimellConfiguration.PrimellDefault, "((2 11) (2 13) (2 17)) ((3 11) (3 13) (3 17)) ((5 11) (5 13) (5 17))")

[<Fact>]
let ``Test Foreach Chain``() =
  TestProgram("[(2 3 5)(7 11 13) | _~_<]", PrimellConfiguration.PrimellDefault, "5 13")
  TestProgram("[(2 3 5)(7 11 13) | _~ _> +(3 5)]", PrimellConfiguration.PrimellDefault, "(6 7) (14 12)")
  TestProgram("[(2 3 5)(7 11 13) | _<[<::(23 29)(31 37)]]", PrimellConfiguration.PrimellDefault, "((2 23 29) (2 31 37)) ((7 23 29) (7 31 37))")

[<Fact>]
let ``Test Side Effects``()  =
  TestProgram(",=2\n(2 , 2 2 , 2 2 , 2)+(, , (, = 5) , , (, = 7) , , ,)", PrimellConfiguration.PrimellDefault, "4 4 7 7 7 9 9 9 9")
  TestProgram(",=2\n(2 , 2 2 (, = 3) 2 2 , 2)+(, , (, = 5) , , (, = 7) , , ,)", PrimellConfiguration.PrimellDefault, "5 5 7 7 8 9 9 10 9")

[<Fact>]
let ``Test Side Effects Foreach``()  =
  TestProgram(",=2\n[(2 3)(3 5)(5 7)]::>(, =+ 3)", PrimellConfiguration.PrimellDefault, "(2 3 5) (3 5 5) (5 7 5)")
  TestProgram(",=2\n[(2 ,)(3 (, = 3))(5 ,)]::>5", PrimellConfiguration.PrimellDefault, "(2 2 5) (3 3 5) (5 3 5)")

  TestProgram(",=2\n(, =+ 3)[<::(2 3)(3 5)(5 7)]", PrimellConfiguration.PrimellDefault, "(5 2 3) (5 3 5) (5 5 7)")
  TestProgram(",=2\n5[<::(2 ,)(3 (, = 3))(5 ,)]", PrimellConfiguration.PrimellDefault, "(5 2 2) (5 3 3) (5 5 3)")
 
[<Fact>]
let ``Test User Operations``() =  // User-defined ops not yet done, but a few hard-coded syntactical tests so I don't actually break stuff
  TestEquivalentProgram("3 2[<::(3 5)(7 11)]", PrimellConfiguration.Listell, "3 2[#test_(3 5)(7 11)]", PrimellConfiguration.Listell)
  TestEquivalentProgram("(3 5 7)::>5", PrimellConfiguration.Listell, "(3 5 7)_test#5", PrimellConfiguration.Listell)
  TestEquivalentProgram("[3 5 7]<::>(3 5 7)", PrimellConfiguration.Listell, "[3 5 7]_test_(3 5 7)", PrimellConfiguration.Listell)
  TestEquivalentProgram("[3 5 7]-(3 5 7)", PrimellConfiguration.Listell, "[3 5 7]#test#(3 5 7)", PrimellConfiguration.Listell)
  TestEquivalentProgram("[(3 5)(7 11)]_~", PrimellConfiguration.Listell, "[(3 5)(7 11)]_test", PrimellConfiguration.Listell)
  TestEquivalentProgram("(3 5)(7 11)~", PrimellConfiguration.Listell, "(3 5)(7 11)#test", PrimellConfiguration.Listell)

  