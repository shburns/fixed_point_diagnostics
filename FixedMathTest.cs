using System;
using System.Diagnostics;

namespace Arithmetic_Diagnostic {
	class FixedMathTest {

		//Total number of iterations to run through when looping the arithmetic to get a useable diagnostic time.
		private const int TOTAL_TESTS = 1000000;

		//M_Fixed is the fixed-point arithmetic as implemented in the original 1995-released DOOM source code.
		//Used here for purely instructive purposes.
		private class M_Fixed {
			private const int MININT = ((int)-0x80000000);
			private const int MAXINT = ((int)0x7fffffff);
			private const int FRACBITS = 16;
			private const int FRACUNIT = (1 << FRACBITS);

			//*******************************************************************************************
			// fixed_t
			//		The 16.16 fixed point data structure used for fixed point arithmetic.
			//******************************************************************************************
			public struct fixed_t { public int val; };

			//********************************************************************************************
			// FixedMul
			//		Multiplies the values of two fixed point 16.16 precision decimal values.
			// @param a : fixed_t
			//		The first value to be multiplied.
			// @param b : fixed_t
			//		The second value to be multiplied.
			//@return fixed_t
			//		The fixed point data structure which contains the product of the two values.
			//*******************************************************************************************
			public static fixed_t FixedMul (fixed_t a, fixed_t b) {
				return new fixed_t {
					val = (int)(((long)(a.val) * (long)(b.val)) >> FRACBITS)
				};
			}

			//********************************************************************************************
			// FixedDiv
			//		Performs the division of two fixed_t-type 16.16 precision fixed point numbers
			// @param a: fixed_t
			//		The dividend of the equation
			// @param b: fixed_t
			//		The divisor of the equation
			//	@return fixed_t
			//		The result of the operation
			public static fixed_t FixedDiv (fixed_t a, fixed_t b) {
				//First, test to be sure that the division result can be represented by the precision of the data-type.
				if ((System.Math.Abs(a.val) >> 14) >= System.Math.Abs(b.val)) {
					//If a XOR'd with b is less than zero, the result value is too low.
					//Otherwise, the result value is too high. Simply return max value.
					return ((a.val ^ b.val) < 0) ? new fixed_t { val = MININT } : new fixed_t { val = MAXINT };
				}
				//If the precision is within the bounds of the data-type, perform the math.
				return FixedDiv2(a, b);
			}

			//********************************************************************************************
			// FixedDiv2
			//		Private inner function to determine the result of the division of two 16.16 numbers.
			// @param a: fixed_t
			//		The dividend of the equation
			// @param b: fixed_t
			//		The divisor of the equation
			// @return fixed_t
			//		The result of the operation
			//********************************************************************************************
			private static fixed_t FixedDiv2 (fixed_t a, fixed_t b) {
				long c;
				c = (((long)a.val) << FRACBITS) / ((long)b.val);

				if (c >= 2147483648.0 || c < -2147483648.0) {
					throw new Exception("FixedDiv: divide by zero");
				}
				return new fixed_t { val = (int)c };
			}
		}

		//********************************************************************************************
		// Main
		//		Performs the basic test. Creates six variables (two each of floating, fixed_t, and
		//		native fixed), then runs each set of two through a loop of arbitrary length and prints
		//		the total time it take for the loop to complete, thus giving a consistent guage of
		//		the performance of each type.
		//********************************************************************************************
		public static void Main (String[] args) {
			//Initialize Stopwatch and the four variables used in the calculations.
			Stopwatch sw = new Stopwatch();

			//Native float.
			float x = 75.75F;
			float y = 100.1F;

			//Class based fixed point type.
			M_Fixed.fixed_t fixed_x = new M_Fixed.fixed_t {
				val = (int)(75.75F * 65536)
			};
			M_Fixed.fixed_t fixed_y = new M_Fixed.fixed_t {
				val = (int)(100.1F * 65536)
			};

			//Native fixed point arithmetic.
			int fix_x = (int)(75.75F * 65536);
			int fix_y = (int)(100.1F * 65536);

			//Floating point tests.
			sw.Reset();
			sw.Start();
			for (var i = 0; i < TOTAL_TESTS; i++) {
				float a = x * y;
				float b = x / y;
				if (i == 1) {
					Console.WriteLine("Floating Point multiplication: " + a);
					Console.WriteLine("Floating Point division: " + b);
				}
			}

			//Stop and print diagnostics. Reset stopwatch and confirm with console statement.
			sw.Stop();
			Console.WriteLine("Total time for " + TOTAL_TESTS + " floating-point operations: " + sw.Elapsed);
			sw.Reset();
			Console.WriteLine("Stopwatch reading in between executions: " + sw.Elapsed);

			//Fixed point tests.
			sw.Start();
			for (var i = 0; i < TOTAL_TESTS; i++) {
				int a = (int)(((long)(fix_x) * (long)(fix_y)) >> 16);
				int b = (int)((((long)fix_x) << 16) / ((long)fix_y));
				if (i == 1) {
					Console.WriteLine("Fixed Point multiplication: " + (float)(a / 65536.0f));
					Console.WriteLine("Fixed Point division: " + (float)(b / 65536.0f));
				}
			}

			//Stop and print diagnostic. Wait for user input before terminating.
			sw.Stop();
			Console.WriteLine("Total Time for " + TOTAL_TESTS + " fixed-point (native) operations: " + sw.Elapsed);
			
			//Reset stopwatch and confirm with console statement.
			sw.Reset();
			Console.WriteLine("Stopwatch reading in between executions: " + sw.Elapsed);
			sw.Start();

			//Fixed point tests.
			for (var i = 0; i < TOTAL_TESTS; i++) {
				M_Fixed.fixed_t a = M_Fixed.FixedMul(fixed_x, fixed_y);
				M_Fixed.fixed_t b = M_Fixed.FixedDiv(fixed_x, fixed_y);
				if (i == 1) {
					Console.WriteLine("Fixed Point multiplication: " + (float)(a.val / 65536.0f));
					Console.WriteLine("Fixed Point division: " + (float)(b.val / 65536.0f));
				}
			}

			//Stop and print diagnostic.
			sw.Stop();
			Console.WriteLine("Total time for " + TOTAL_TESTS + " floating-point (class-based) operations: " + sw.Elapsed);
			Console.ReadLine();
		}
	}
}
