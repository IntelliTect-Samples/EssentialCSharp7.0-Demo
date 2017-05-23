using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    [TestClass]
    public class LocalFunctionTests
    {
        bool IsPalindrome(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            bool InternalIsPalindrome(string target)
            {
                target = target.Trim();  // Start by removing any surrounding whitespace.
                if (target.Length <= 1) return true;
                else
                {
                    return char.ToLower(target[0]) == char.ToLower(target[target.Length - 1]) &&
                        InternalIsPalindrome(
                            target.Substring(1, target.Length - 2));
                }
            }
            return InternalIsPalindrome(text);
        }
        [TestMethod]
        public void IsPalindrome_GivenNonPalindrome_ReturnsFalse()
        {
            Assert.IsFalse(IsPalindrome(""));
            Assert.IsFalse(IsPalindrome(null));
            Assert.IsFalse(IsPalindrome("   "));
            Assert.IsFalse(IsPalindrome("   not"));
            Assert.IsFalse(IsPalindrome("not"));
            Assert.IsFalse(IsPalindrome("Even letter length"));
            Assert.IsFalse(IsPalindrome("Odd letter length"));
        }

        [TestMethod]
        public void IsPalindrome_GivenPalindrome_ReturnsTrue()
        {
            void AssertIsPalindrome(string text)
            {
                Assert.IsTrue(IsPalindrome(text),
                    $"'{text}' was not a Palindrome.");
            }
            AssertIsPalindrome("7");
            AssertIsPalindrome("7X7");
            AssertIsPalindrome("   tnt");
            AssertIsPalindrome("Was it a car or a cat I saw");
            AssertIsPalindrome("Never odd or even");
        }
    }
}
