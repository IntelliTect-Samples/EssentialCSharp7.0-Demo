
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    public partial class PathInfo
    {

        public string Path
        {
            get { return System.IO.Path.Combine(DirectoryName, FileName, Extension); }
        }

        public void Deconstruct(
            out string directoryName, 
            out string fileName, 
            out string extension)
        {
            directoryName = DirectoryName;
            fileName = FileName;
            extension = Extension;
        }
        #region Single parameter constructors are not supported as deconstructors
        public void Deconstruct(out string path)
        {
            path = Path;
        }
        public void Deconstruct(out FileInfo file)
        {
            file = new FileInfo(Path);
        }
        public void Deconstruct(out DirectoryInfo directory)
        {
            directory = new DirectoryInfo(Path);
        }
        #endregion Single parameter constructors are not supported as deconstructors
    }

    public static class PathInfoEx
    {
        static public void Deconstruct(this PathInfo pathInfo,
            out string directoryName,
            out string fileName)
        {
            directoryName = pathInfo.DirectoryName;
            fileName = pathInfo.FileName;
        }
    }

    [TestClass]
    public partial class PathInfoTests
    {
        void VerifyExpectedValue(string directoryName, string fileName, string extension)
        {
            Assert.AreEqual<(string DirectoryName, string FileName, string Extension)>(
                (@"\\test\unc\path\to", "something", ".ext"),
                (directoryName, fileName, extension));
        }

        void VerifyExpectedValue(string directoryName, string fileName)
        {
            Assert.AreEqual<(string DirectoryName, string FileName)>(
                (@"\\test\unc\path\to", "something"),
                (directoryName, fileName));
        }

        [TestMethod]
        public void Deconstruct_CalledExplicitly_Success()
        {
            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

                // E.g. 1: Deconstructing declaration and assignment
                pathInfo.Deconstruct(
                    out string directoryName,
                    out string fileName,
                    out string extension);

                VerifyExpectedValue(directoryName, fileName, extension);
        }

        [TestMethod]
        public void Deconstruct_GivenFileNamePathInfo_SeparateDirectoryNameFileNameExtension()
        {
            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

            string directoryName, fileName, extension = null;

            // E.g. 2: Deconstructing assignment
            (directoryName, fileName, extension) = pathInfo;

            VerifyExpectedValue(directoryName, fileName, extension);
        }

        [TestMethod]
        public void Deconstruct_GivenFileNamePathInfo_DeconstructImplicitlyWithVar()
        {
            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

            // E.g. 3: Deconstructing declaration and assignment with var
            var (directoryName, fileName, extension) = pathInfo;
            VerifyExpectedValue(directoryName, fileName, extension);
        }

        [TestMethod]
        public void ChangeThing()
        {
            (string name, int number) thing = ("Inigo Montoya", 42);
            thing.number++;
            Assert.AreEqual<int>(43, thing.number);
            Method(thing);
            Assert.AreEqual<int>(43, thing.number);
            Assert.AreEqual<string>("Inigo Montoya", thing.name);
        }

        void Method((string name, int number) thing)
        {
            thing.number++;
            thing.name = "Inigo Montoya the 3rd";
        }


        [TestMethod]
        public void ChangeThingWithRef()
        {
            (string name, int number) thing = ("Inigo Montoya", 42);
            thing.number++;
            Assert.AreEqual<int>(43, thing.number);
            Method(ref thing);
            Assert.AreEqual<int>(44, thing.number);
            Assert.AreEqual<string>("Inigo Montoya the 3rd", thing.name);
        }

        void Method(ref (string name, int number) thing)
        {
            thing.number++;
            thing.name = "Inigo Montoya the 3rd";
        }




        [TestMethod]
        public void DeconstructExtensionMethod_CalledImplicitly_Success()
        {
            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

            (string directoryName, string fileName) = pathInfo;
            VerifyExpectedValue(directoryName, fileName);
        }

        [TestMethod]
        public void Deconstruct_AssignmentToValueTypeIsNotSupported()
        {
            string[] expectedErrors = {
                        "Error CS0029: Cannot implicitly convert type 'CSharp7.PathInfo' to '(string DirectoryName, string FileName, string Extension)'"
                    };
            CompilerAssert.StatementsFailCompilation(
                @"
                    PathInfo pathInfo = new PathInfo(
                            @""\\test\unc\path\to\something.ext"");
                    // E.g. 4: Deconstructing declaration and assignment to a ValueTuple
                    (string DirectoryName, string FileName, string Extension) path = pathInfo;
                ",
                expectedErrors);

        }

        [TestMethod]
        public void Deconstruct_GivenPathInfo_DeconstructingAndAssignment()
        {
            string directoryName, fileName, extension = null;

            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

            // deconstructing assignment
            (directoryName, fileName, extension) = pathInfo;

            Assert.AreEqual<(string DirectoryName, string FileName, string Extension)>(
                (@"\\test\unc\path\to", "something", ".ext"),
                (directoryName, fileName, extension));
        }

        [TestMethod]
        public void Deconstruct_GivenPathInfo_DeconstructingAndVarAssignment()
        {
            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

            // deconstructing declaration and assignment with var
            var (directoryName, fileName, extension) = pathInfo;

            Assert.AreEqual<(string DirectoryName, string FileName, string Extension)>(
                (@"\\test\unc\path\to", "something", ".ext"),
                (directoryName, fileName, extension));
        }


        [TestMethod]
        public void Deconstruct_GivenSingleOutOParameter_FailsCompilation()
        {
            string[] expectedErrors = {
                        "Error CS1002: ; expected",
                        "Error CS1002: ; expected",
                        "Error CS1513: } expected",
                        "Error CS1525: Invalid expression term '='",
                        "Error CS0119: 'FileInfo' is a type, which is not valid in the given context",
                        "Error CS0119: 'FileInfo' is a type, which is not valid in the given context",
                        "Error CS0103: The name 'path' does not exist in the current context",
                        "Error CS1026: ) expected"
                    };


            CompilerAssert.StatementsFailCompilation(
                @"
                        PathInfo pathInfo = new PathInfo(
                            @""\\test\unc\path\to\something.ext"");
                        (FileInfo path) = pathInfo; 
                ",
                expectedErrors);
        }

        [TestMethod]
        public void SplitPath_GivenPath_SuccessfullySplitsIntoConsituentNamedParts()
        {

            PathInfo pathInfo = new PathInfo(@"\\test\unc\path\to\something.ext");

            (string directoryName, string fileName, string extension) = pathInfo;

            Assert.AreEqual<(string DirectoryName, string FileName, string Extension)>(
                (@"\\test\unc\path\to", "something", ".ext"),  // Expected
                (directoryName, fileName, extension)          // Actual
                );
        }

    }
}
