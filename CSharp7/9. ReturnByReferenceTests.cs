using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    [TestClass]
    public class ReturnByReferenceTests
    {
        static public ref byte FindFirstRedEyePixel(byte[] image)
        {
            //// Do fancy image detection perhaps with machine learning
            for (int counter = 0; counter < image.Length; counter++)
            {
                if (image[counter] == (byte)ConsoleColor.Red)
                {
                    // Identify the pixel that is Red 
                    // and return a reference to it.
                    return ref image[counter];
                }
            }
            throw new InvalidOperationException("No pixels are red.");
        }

        [TestMethod]
        public void FindFirstRedEyePixel_GivenRedPixels_ReturnFirst()
        {
            byte[] image;
            #region Load Image...
            image = new byte[42];  /// Really small image. :)
            List<long> redItems = new List<long>();
            Random random = new Random();
            for (int counter = 0; counter < image.Length; counter++)
            {
                image[counter] = (byte)random.Next(0, 15);
                if(image[counter] == (byte)ConsoleColor.Red) { redItems.Add(counter); }
            }
            image[image.Length - 1] = (byte)ConsoleColor.Red;  // Mock in a Red pixel at the end.
            redItems.Add(image.Length - 1);
            #endregion // Load Image

            // Obtain a reference to the first red pixel
            ref byte redPixel = ref FindFirstRedEyePixel(image);
            // Update it to be Black.
            redPixel = (byte)ConsoleColor.Black;
            Assert.AreEqual<byte>((byte)ConsoleColor.Black, image[redItems[0]]);
        }


        public char[] Name = "fred".ToCharArray();
        ref char GetName(char[] name) { return ref Name[2]; }
        public class Thing
        {
            public string A;
            public int B;
            public int C;
        }

        public ref string ReturnUninitializedProperty()
        {
            Thing[] Things = new Thing[42];
            foreach (Thing item in Things)
            {
                if (string.IsNullOrEmpty(item.A))
                {
                    return ref item.A;
                }
            }
            throw new InvalidOperationException($"{nameof(Thing.A)} is already initialized.");
        }

        [TestMethod]
        public void ByRefCompileErrors()
        {
            void AssertStatementsFailCompilation(string body, params string[] expectedText)
            {
                string MethodScaffolding = $@"#pragma warning disable CS0168 // Disable variable declared but never used.
                {body}
                #pragma warning restore CS0168";
                CompilerAssert.StatementsFailCompilation(MethodScaffolding, expectedText);

            }
            AssertStatementsFailCompilation("ref int number;",
                "Error CS8174: A declaration of a by-reference variable must have an initializer");
            AssertStatementsFailCompilation("ref int number = ref 42;",
                "Error CS1510: A ref or out value must be an assignable variable");
            AssertStatementsFailCompilation("ref int number = null;",
                "Error CS1510: A ref or out value must be an assignable variable",
                "Error CS8172: Cannot initialize a by-reference variable with a value");
            AssertStatementsFailCompilation("ref string number = \"Inigo Montoya\";",
                "Error CS1510: A ref or out value must be an assignable variable",
                "Error CS8172: Cannot initialize a by-reference variable with a value");
            AssertStatementsFailCompilation(
                @"Guid guid = Guid.NewGuid();
                 ref object obj = ref guid;",
                "Error CS8173: The expression must be of type 'object' because it is being assigned by reference");
            AssertStatementsFailCompilation(
                @"Guid guid = Guid.NewGuid();
                 ref object obj = ref guid;",
                "Error CS8173: The expression must be of type 'object' because it is being assigned by reference");
            AssertStatementsFailCompilation(
                @"IEnumerable<ref int> numbers;",
                "Error CS1073: Unexpected token 'ref'");
            //AssertStatementsFailCompilation(
            //    @"ref int[] numbers; ",
            //    "Error CS1073: Unexpected token 'ref'");
        }
        class SampleObject1
        {
            public static string _Text = "Inigo Montoya";
            public ref string Text { get { return ref _Text; } }
            public ref string Method1() => ref Text;
        }
        class SampleObject2
        {
            ref string Method1()
            {
                SampleObject1 sampleObject = new SampleObject1();
                return ref sampleObject.Text;
            }
        }

        //byte[] _Image;
        //public ref byte[] Image { get {  return ref _Image; } }

    }
}
