# Premise

Primell is a language that likes prime numbers. So much in fact, that it is the only thing Primell would like to allow. Unfortunately, this turned out to be too inconvenient, so Primell has relented in the following ways:

 - Symbols are allowed in source code.
 - All numbers are permitted to be computed, though only primes may exist in source code.
 - Because humans almost universally use base 10, that is the default though all bases up to 64 are allowed.
 - Lists are allowed. Primell in fact quite likes lists because it allows more storage of prime numbers
 - Primell doesn't make mistakes: All valid programs either execute in a fully defined manner or execute infinitely, there are no runtime errors

*A "serious" version is being planned for which doesn't have source restriction.*

# Why?

Primell was created as an experiment. I had intended on creating a more serious language down the road and Primell was a learning step. Also, it was amusing to create a programming language that doesn't allow 0 or 1 in its code.

# Overview

_This overview presents high-level highlights of Primell, see the [wiki](https://github.com/DPenner1/Primell/wiki) for details._
 
## Data types

Primell has but two data types:

- Numbers (arbitrary precision rational, IEEE 754-like support of infinity, signed-zero and NaN)
- "Lists" of these two datatypes (including infinite length lists)
	
Other familiar data types are only simulated with these. For example:
- Boolean: A non-empty list consisting of only prime numbers is considered true. Anything else is false. (This is the truest truth, but alternative truths are configurable.)
- There aren't really others yet.

## Operators

With the Latin alphabet being reserved for use as base-64 numbers, operators are completely made of symbols. Most of them are intuitive though. Really. Especially with stretches of imagination.

Since Primell is not very familiar with how operators work, Primell for the most part executes them in strict left-to-right order. There are a few exceptions: 

- Parentheses may be used naturally like in math.
- Square brackets also group similar to parentheses, but have the semantic meaning of specifying a for-each loop.
- The `$` symbol acts like in Haskell, that is, executes everything to the right of it first. This is mainly syntactic sugar to avoid too many nested parentheses.
- The "default" operator is concatenation. Placing multiple lists/numbers next to each other creates a single list containing those elements. This is the lowest priority operation.

Operators may only take up to two arguments. Though all operators are able to work on both numbers and lists, it is convenient to define them in terms of one or the other. See wiki for a list of the operators.

## Program Flow 

Every program must start with a number, list, or implicitly declared variable so that Primell has something to work with. Then, a sequence of operations are applied to get new values. Here is an single-line example:

    (2 3 5) + 2 _~ _> * 3 

Since Primell usually executes in strict left-to-right order, it is quite easy to figure out what happens. Let's go step by step.

 1. `(2 3 5)`: We start with the list (2 3 5). In this case the parentheses are necessary to stop the 5 + 2 from executing first.
 2. `+ 2`: This creates the list `(4 5 7)`.
 3. `_~`: Reverses the list giving `(7 5 4)`.
 4. `_>`: Gets the tail of the list (all elements after 0th). We now have `(5 4)`.
 5. `* 3`: Produces our final value `(15 12)`.

You'll note that the parentheses were not printed to the screen. The outermost parentheses are never printed because otherwise it would be possible to distinguish between a 1-element list and its contents (which Primell forbids). Consider the case of `(3)`.

Note that spaces aren't usually necessary, `(2 3 5)+2_~_>*3` would have been an identical program.

Programs may have multiple lines and the lines are executed from top to bottom in the same manner. Primell doesn't quite have traditional control-flow like other programming languages, but instead does have the capability to incorporate lines of code within another line. Check out the conditional branch operator `?/`. This is probably sufficient for simulating a Turing machine (see the [Examples.md](https://github.com/DPenner1/Primell/blob/main/Examples/Examples.md) file for details).

# Running Primell #

Console commands are prefixed with `?`, which on its own pulls up the help menu. To run a file, use:

    ?run <file path>

For example, included example programs can be run with the following (Linux file path):

    ?run ../Examples/HelloWorld.prime

Input not starting with `?` are taken to be code and executed immediately on the console.

# Notes on the source code #

Don't look at it. It's painful. If you choose not to heed that warning, read on. Though I guess that was implicit.

- This was originally coded around 2017 in a C# solution. Seven years later, I came back to it and ported it to F#, mainly for better infinite sequence support and to bring Primell in a more functional direction (though the mutable assignment function in Primell proved far more challenging than anticipated). F# was new to me, I selected it over other functional languages so I still had familiar territory with .NET and ANTLR's C# target.
- I was speed learning F#, I opted to glaze over access modifiers for the most part, I couldn't usually remember what the default access modifier was for a given context. Partially for that reason, I stuck to C# naming and casing conventions, but also I wasn't initially sure if the entire implementation would be F# or just the data structures. 
- On naming, I had originally simply called the language Prime, but changed to avoid potential trademark issue with HP Prime. Primell was derived as Prime List Language.
- I opted not to add the Antlr4 generated source files to the `.gitignore` to allow for users to run the project without self-generating the files.
