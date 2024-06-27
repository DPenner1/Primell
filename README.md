# Premise

Primell is a language that likes prime numbers. So much in fact, that it is the only thing Primell would like to allow. Unfortunately, this turned out to be too inconvenient, so Primell has relented in the following ways:

 - Symbols are allowed in source code.
 - All numbers are permitted to be computed, though only primes may exist in source code.
 - Because humans almost universally use base 10, that is the default though but all bases up to 64 are allowed.
 - Lists are allowed. Primell in fact quite likes lists because it allows more storage of prime numbers

# Why? 

Primell was created as an experiment. I had intended on creating a more serious language down the road and Primell was a learning step. Also, it was amusing to create a programming language that doesn't allow 0 or 1 in its code.

# Data types

Primell has but two data types:

	- Numbers (arbitrary precision rational, IEEE 754-like support of infinity and NaN)
	- "Lists" of these two datatypes
	
Other familiar data types are only simulated with these. For example:

	- Boolean: A non-empty list consisting of only prime numbers is considered true. Anything else is false.
	- There aren't really others yet.

Primell's lists are not really lists but they are more like dynamic arrays. Here are its properties:

	- 0-indexed
	- Can be infinite
	- Attempting to access an index not yet set implicitly fills the list up to that point with empty lists
	- A 1-element list is not distinguishable from its containing element

A corollary of that last point is that a number can be treated as a one-element list in every context.

# Operators

With most allowed characters being used for numbers, operators are completely made of symbols. Most of them are intuitive though. Really. Especially with stretches of imagination.

Since Primell is not very familiar with how operators work, Primell for the most part executes them in strict left-to-right order. There are 4 exceptions: 

    - Parentheses may be used naturally like in math.
	- Square brackets also group similar to parentheses, but have the semantic meaning of specifying a for-each loop.
    - The $ symbol acts like in Haskell, that is, executes everything to the right of it first. This is mainly syntactic sugar to avoid too many nested parentheses.
    - The "default" operator is concatenation. Placing multiple lists/numbers next to each other creates a single list containing those elements. This is the lowest priority operation.

Operators may only take up to two arguments. Though all operators are able to work on both numbers and lists, it is convenient to define them in terms of one or the other. See wiki for a list of the operators.

# Program Flow #

Every program must start with a number or a list so that Primell has something to work with. Then, a sequence of operations are applied to get new values. Here is an single-line example:

    (2 3 5) + 2 _~ _> * 3 

Since Primell usually executes in strict left-to-right order, it is quite easy to figure out what happens. Let's go step by step.

 1. `(2 3 5)`: We start with the list (2 3 5). In this case the parentheses are necessary to stop the 5 + 2 from executing first.
 2. `+ 2`: This creates the list `(4 5 7)`.
 3. `_~`: Reverses the list giving `(7 5 4)`.
 4. `_>`: Gets the tail of the list (all elements after 0th). We now have `(5 4)`.
 5. `* 3`: Produces our final value `(15 12)`.

You'll note that the parentheses were not printed to the screen. The outermost parentheses are never printed because otherwise it would be possible to distinguish between a 1-element list and its contents (which Primell forbids). Consider the case of `(3)`.

Note that spaces aren't usually necessary, `(2 3 5)+2_~_>*3` would have been an identical program.

Programs may have multiple lines and the lines are executed from top to bottom in the same manner. Primell doesn't quite have traditional control-flow like other programming languages, but instead does have the capability to incorporate lines of code within another line. Check out the conditional branch operator `?/`. This is actually sufficient for simulating a Turing machine (see the Examples.md file for details).

# Running Primell #

At the moment, the interpreter engine only supports running code from a file and does not yet support a REPL mode (an unforgivable sin for an interpreted language).

Interpreter commands are prefixed with `?`, which on its own pulls up the help menu. To run a file, use:

    ?run <file path>

For example, included example programs can be run with the following (Linux file path):

    ?run ../Examples/HelloWorld.prime

# Future #

 - Strings (though only syntactic sugar for it, they will strictly be interpreted as a list of numbers)
 - User-defined operators?
 - Operators as a first class data type? (I like this feature in programming languages - might be useful to test it out here before I try on a possible future "serious" programming language)
 - If/When the language is stabilized, re-implement in Haskell? (infinite lists are so much easier there)
 - Allowing up to base-64 brings up an interesting programming challenge. The first 62 characters can be standard latin alphanumeric, but then we're out of characters (standard versions of base64 tend to use symbols which I needed to reserve for various operations). I hard-coded uppercase/lowercase thorn for characters 63 & 64, but could these be made configurable, thus having a variable lexer?

# Notes on the source code #

Don't look at it. It's painful. If you choose not to heed that warning, read on. Though I guess that was implicit.

Based on having originally used Antlr 4.7 and .NET Framework 4.x, I think I originally coded this project in 2017. In order to work on my current Linux .NET Core set-up, I had to upgrade both Antlr and .NET. But irritatingly, I lost the grammar file and had to reconstruct it by looking at the generated code. There might be some rough edges, but at least that meant I refamiliarized myself with the solution and am actually motivated to clean up the rough edges.

The "PL" prefix used everywhere stands for "Primell Language". The PLNumber class was adapted from a BigRational class I had previously developed. However, it did not support infinity, NaN or negative zero like PLNumber does, so there may be bugs in there.

On naming, I had originally simply called the language Prime, and that is evident in source code. Last minute change to avoid potential rademark issue with HP Prime. Primell was derived as Prime List Language.

The infinite lists were coded in a hurry. There was probably a framework class that could have helped.
