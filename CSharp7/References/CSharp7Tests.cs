using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    [TestClass]
    public class CSharp7Tests
    {
        // Confirm C# 7.0 is active using a binary literal
        public long LargestSquareNumberUsingAllDigits = 
            0b0010_0100_1000_1111_0110_1101_1100_0010_0100;  // 9,814,072,356
        // Confirm C# 7.0 is active digit separator
        long MaxInt64 { get; } = 9_223_372_036_854_775_807;  // Equivalent to long.MaxValue

        public (int Height, int Width, int _) DefaultCubeSize = (1, 2, 3);  // Tuples

        #region Out Parameter Declaration
        public long DivideWithRemainder(
            long numerator, long denominator, out long remainder)
        {
            remainder = numerator % denominator;
            return (numerator / denominator);
        }

        [TestMethod]
        public void DivideTest()
        {
            Assert.AreEqual<long>(21, 
                DivideWithRemainder(42, 2, out long remainder));
            Assert.AreEqual<long>(0, remainder);

             // This is another great place for tuples but 
            // I didn't use them to avoid overlapping C# 7.0 features.
            void assertDivide(
                long expected, long expectedRemainder,
                long numerator, long denomenator)
            {
                Assert.AreEqual<long>(
                    expected, DivideWithRemainder(numerator, denomenator, out long remaining));
                Assert.AreEqual<decimal>(expectedRemainder, remaining);
            }
            assertDivide(21, 0, 42, 2);
            assertDivide(21, 1, 43, 2);


        }

        [TestMethod]
        public void OutScopePreventsReusingLocalVariableName()
        {
            // ERROR: A local variable named 'remainder' is already defined in the scope.
            CompilerAssert.StatementsFailCompilation(
            @"int.TryParse(""42"", out int result);
            int.TryParse(""42"", out int result);",
            "Error CS0128: A local variable or function named 'result' is already defined in this scope");

        }
        #endregion // Out Parameter Declaration




        [TestMethod]
        public void CSharpCompileEnabled()
        {
            bool LocalFunction()
            {
                return true;
            }

            Assert.IsTrue(LocalFunction());
        }
    }
}
