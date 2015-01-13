# fixed_point_diagnostics
Diagnostic code testing performance of fixed-point arithmetic vs. native floating point arithmetic as
processed by the .NET CLR.

This code was written as a test for determining speed and efficiency of the fixed_t 16.16 fixed point
data type used in the original DOOM source code vs. native fixed and floating point operations as
performed by the .NET CLR.

The private inner class is simply a classed based implementation of the m_fixed.c file from the original
DOOM source release. It's purpose was simply to determine the performance loss of implementing a "class-based"
approach to the custom type and, by extension, the individual source files of DOOM.  It implements the functions
exactly as they were written in the original code.

The test driver is written directly into the Main function. It simply creates six variables --
Two of floating type, two of fixed_t type, and two integers whose bits correlate to the
16.16-precision fixed point number type, all with equal values. To run the actual tests and get
useful diagnostics it uses System.Diagnostics.Stopwatch to time the total amount of time necessary to
complete an arbitrarily large loop (sufficiently large enough to illustrate the
difference in performance between the two implementations) within which the numbers of
corresponding type are multiplied and divided with both results stored to a loop-scoped variable.
The result is printed to screen once and, on exiting the loop, the stop watch is stopped
and the time printed to screen.

Using this test, it is easy to determine that native floating point operations are worlds
faster than implementing the fixed_t data-type using a class-based design modeled after the
original engine code.  This is pretty clearly a result of the relatively high overhead of
branching to the class functions for arithmetic operations vs. the native inline operators.
However, it wasn't until TOTAL_TESTS was set to about 1M that it became clear that,
over-time, the floating point operations were faster even than the native
16.16 fixed point operations.

It will be obvious to anyone familiar with the lower-level implementation of the
arithmetic, but it also bears noting that the native floating point arithmetic was more
precise than both fixed point implementations, as well as being faster.