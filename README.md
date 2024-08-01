# Premise

Primell is a language that likes prime numbers. So much in fact, that it is the only thing Primell would like to allow. Unfortunately, this turned out to be too inconvenient, so Primell has relented in the following ways:

 - Symbols are allowed in source code.
 - All numbers are permitted to be computed, though only primes may exist in source code.
 - Because humans almost universally use base 10, that is the unfortunately non-prime default, though all bases up to 64 are allowed.
 - Lists are allowed. Primell in fact quite likes lists because it allows more storage of prime numbers and so allows for infinite lists.
 - Primell doesn't make mistakes: All valid programs either execute in a fully defined manner or execute infinitely, there are no runtime errors.

A "serious" version called Listell without the source restriction is configurable.

# Why?

Primell was created as an experiment. I had intended on creating a more serious language down the road and Primell was a learning step. Also, it was amusing to create a programming language that doesn't allow 0 or 1 in its code.

# Overview

_This overview presents high-level highlights of Primell, see the [wiki](https://github.com/DPenner1/Primell/wiki) for details._

## Cool features

While never intended to be serious, there are a few things in Primell/Listell that I think are legitimately useful.

### 1. List multi-indexing

No, not the Pandas kind, but that did make it hard to search for other examples. Listell's index operator `@` generalizes list indexing concisely; it is effectively a way to select any items from a list and in any order:

    (5 10 11 14 17 19 20)@(5 2)
    = 19 11

This was not explicitly designed for, but is just a result of how operators in Primell automatically apply a for-each approach when given a list argument instead of an expected numeric argument (in this case, the index). This generality meant it could then be combined elegantly with the range operator `..` to achieve Python-like list slicing _without_ any dedicated list slicing implementation code (inclusive lower, exclusive upper bounds like Python):

    (5 10 11 14 17 19 20)@(2..6)
    = 11 14 17 19

Listell is limited to binary operations, so you can't directly specify start, end _and_ step in a range, but a crude approximation of range-step is possible by simply multiplying the range (the bounds also get multiplied):

    (0 1 2 3 4 5 6 7 8 9 10)@(2..5 * 2)
    = 4 6 8

And while the implementation for this is currently broken, negative numbers will allow the ability to wrap around a list in either direction (due to syntax quirk, -1 is written as `1~`):

    (5 10 11 14 17 19 20)@(1~..2)
    = 20 5 10 11
    (5 10 11 14 17 19 20)@(2..(1~))
    = 11 10 5 20

As far as I'm aware, this kind of multi-indexing is unique to Primell/Listell, though would be happy to learn from other examples (note: this section specifically used Listell, because in Primell the "range" operator skips non-primes!).

### 2. Concise functional mapping sequence syntax

Or in more procedural language, apply a sequence of functions for each element in a list. In general, the syntax is `[ list to be mapped | mapping function sequence ]`. The following example takes a 2-item list, each itself nested with 3-item lists and applies a sequence of functions to it (including a binary operator).

    [(2 3 5)(7 11 13) | reverse tail +(3 5)]
    = (6 7) (14 12)

Step by step, it reverses each list `(5 3 2)(13 11 7)`, takes the tail of each list `(3 2)(11 7)` then adds `(3 5)` to each list, which itself adds across like-indexes to produce the final result of `(6 7) (14 12)`. Of course I wanted to show how the syntax might look in a "serious" language, but Primell/Listell makes heavy use of symbols in its operators so it actually looks like this:

    [(2 3 5)(7 11 13) | _~ _> +(3 5)]

This is of course not unique to Primell, but I was particularly happy with how concise yet readable (in my opinion) the syntax turned out.

## Data types

Primell has but two data types:

- Numbers (arbitrary precision rational, IEEE 754-like support of infinity, signed-zero and NaN)
- "Lists" of these two datatypes (including infinite length lists)
	
Other familiar data types are only simulated with these. For example:
- Boolean: A non-empty list consisting of only prime numbers is considered true. Anything else is false. (This is the truest truth, but alternative truths are configurable.)
- There aren't really others yet.



 1. List multi-indexing (no, not the Pandas kind, but that did make it hard to search for examples):

## Operators

With the Latin alphabet being reserved for use as base-64 numbers, operators are completely made of symbols. Most of them are intuitive though. Really. Especially with stretches of imagination.

Since Primell is not very familiar with how operators work, Primell for the most part executes them in strict left-to-right order. There are a few exceptions: 

- Parentheses may be used naturally like in math.
- Square brackets also group similar to parentheses, but have the semantic meaning of specifying a for-each loop.
- The `$` symbol acts like in Haskell, that is, executes everything to the right of it first. This is mainly syntactic sugar to avoid too many nested parentheses.
- The "default" operator is concatenation. Placing multiple lists/numbers next to each other creates a single list containing those elements. This is the lowest priority operation.

Operators may only take up to two arguments. Though all operators are able to work on both numbers and lists, it is convenient to define them in terms of one or the other. See the [List of Operators](https://github.com/DPenner1/Primell/wiki/List-of-Operators) on the wiki for the full list.

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
