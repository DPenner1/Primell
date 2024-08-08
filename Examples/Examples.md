# Hello World

Get out your ASCII Table for this.

"Hello, World!" would be:

    (72 101 108 108 111 44 32 87 111 114 108 100 33) >"

However, Primell will complain about this! This is because non-prime numbers were used in source code. This is not allowed (note that the `>"` at the end simply tells Primell to output a string instead of a list). We simply need to write each number as a prime number. This is always possible. Here is an example expressing each number as a product of primes:

    (2*2*2*3*3 101 2*2*3*3*3 2*2*3*3*3 3*37 2*2*11 2*2*2*2*2 3*29 3*37 2*3*19 2*2*3*3*3 2*2*5*5 3*11) >"

That wasn't so bad was it?

# Numbers 1 to 100

A common exercise for beginner programmers is to find all prime numbers from 1 to 100. This is very easy in Primell:

     2..101

Note that the range is inclusive on the lower bound, exclusive on the upper bound.

More difficult in Primell is to find all numbers from 1 to 100. To do so we need some clever tricks.

	, = 2*2*5*5
	, =- 2
	,, = 2/2

	(,, =^ (,,_< + 3 - 2 ;,,))_< - , ?~ (2-2!/ ,,_~)

Let's go step by step:

 1. `, = 2*2*5*5`: This is simply initializing the `,` variable to 100. Any *n* can be put here. Note that the $ sign means evaluate everything to the right of it first.
 2. `, =- 2`: Decrement the value of `,` by 2. We do this to avoid an off-by-2 error in step 4, which is common to make in Primell.
 3. `,, = 2/2`: Assign the `,,` variable the value 1. The `^` modifier prevents the entire list from being assigned the value 1. Note that `2/2` is the standard way of writing the number 1 in Primell. 
 4. `(,, =^ (,,_< + 3 - 2 ;,,))_< - , ?~ (2-2!/ ,,_~)`: Generate all numbers 1 to 100.

While not as easy generating the primes, it is still quite doable.

Wait, you want more explanation for step 4? Here's a more detailed breakdown then:

 1. `(,, =^ (,,_< + 3 - 2 ;,,))_<`: Create a list with members `,,_< + 3-2` and `,,`, assigning it back to `,,`. The `^` assignment modifier performs a replacement assignment rather than a parallel list assignment. Let's assume that `,,` has the value `(2 1)`. This will then create the list `(3 (2 1))`.
 2. `_<`: Return the head of the list.
 3. `- ,`: Subtract `,` from the value obtained in step 2. In our example, this is 3 - 98 = -95. Note that once we reach our desired value of 100, we will have 100 - 98 = 2.
 4. `?~ (2-2!/ ,,_~)`: The `?~` is the "if false evaluate head" operator. In this case the head is relative branch, executing the current line again (Note that `2-2` is the standard way of writing 0 in Primell). See the "Branches of Truth" article for more details. Given the info in step 3, we know that a prime number will not occur until we hit 100 (since negative numbers, 0, and 1, are not considered prime). At this point, `,,_~` is returned. 
 5. `_~`: Reverse the list, as from left to right, we've been generating these values highest to lowest.


# Turing Machine simulation

_It probably works._

This is really only here as proof of Turing-completeness. It is left here as a challenge to the reader: Once understood, the reader is considered an advanced beginner at Primell. To help here is a description of the first five lines, which are simply initializations:
 
 - The tape is represented by the `,,,` variable and is unbounded to the right. Any value can go on the tape. It is initialized with all 0s except the zeroth index which is -2 (to more easily mark left-end of tape).
 - The current state is stored in `,@2`. States should be positive numbers, except for -2 representing the halting state
 - The tape reader/head's current position is stored in `,@3`
 - The table of instructions for the Turing machine is stored at `,,@(3..∞)`. The 4th line of code is an example instruction table that can be edited for different instructions.
 - Each instruction is a 5-tuple of the following format: (currentState, currentValueUnderHead, nextState, nextValue, headMoveOffset)
 - The instruction matching the currentState and currentValueUnderHead is stored at `,,@2` and is initialized as junk to begin with.

Also, the code arbitrarily outputs the first 4 values of the tape.

	,,, =$ 2-2
	,,,@(2-2) =$ 2~
	,@(2 3) =$ 2-2 2-2
	,, =$ (2-2 2~ 3 2~ 2/2) (3 2-2 2~ 7 2/2)
	,, =$ (()()(3 3 3 3 3) ;,,)

	,@2~ ?~$ (2/2!/ 2 3 2-2) ,,,@(2-2 2/2 2 3)

	,,@2 =$ ,,@$ ,,[@(2-2 2/2)] @_ (,@2 ;,,,@$,@3) _<
	,,,@(,@3) =$ ,,@2@3
	,@(2 3) =$ ,,@2@2 ,@3 + ,,@2@(2*2)
