using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    public partial class PathInfo
    {

        // Set https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/src/System/IO for real implementation.
        static public (string DirectoryName, string FileName, string Extension) SplitPath(string path)
        {
            return (
                System.IO.Path.GetDirectoryName(path),
                System.IO.Path.GetFileNameWithoutExtension(path),
                System.IO.Path.GetExtension(path)
                );
        }

        public PathInfo(string path)
        {
            // Assigning a tupe to individual variables (properties in this case).
            (DirectoryName, FileName, Extension) = SplitPath(path);
        }

        public PathInfo(
            string directoryName, string fileName, string extension)
        {
            (DirectoryName, FileName, Extension) =
                (directoryName, fileName, extension);
        }

        // ERROR: You can't override by return values - even on Tuples :)
        /*
        public (string PathRoot, string DirectoryName, string FileNameWithoutExtension, string Extension)
            SplitPath(string path)
        {
            return (
                Path.GetPathRoot(path),
                Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path),
                Path.GetExtension(path)
                );
        }
        */
    }

    [TestClass]
    public class ValueTupleTests
    {
        [TestMethod]
        public void ValueTuple_GivenTuple_VariableDeclarationWithVarAgainstTuple()
        {
            var (directoryName, fileName, extension) = 
                PathInfo.SplitPath(@"\\test\unc\path\to\something.ext");

            Assert.AreEqual<string>(
                @"\\test\unc\path\to", directoryName);
            Assert.AreEqual<string>(
                "something", fileName);
            Assert.AreEqual<string>(
                ".ext", extension);

            Assert.AreEqual<(string, string, string)>(
                (@"\\test\unc\path\to", "something", ".ext"),
                (directoryName, fileName, extension));
        }

        [TestMethod]
        public void ValueTuple_DiscardTupleItems()
        {
            var (_, fileName, extension) =
                PathInfo.SplitPath(@"\\test\unc\path\to\something.ext");

            Assert.AreEqual<string>(
                "something", fileName);
            Assert.AreEqual<string>(
                ".ext", extension);

            Assert.AreEqual<(string, string)>(
                ("something", ".ext"),
                (fileName, extension));
        }

        [TestMethod]
        public void SplitPath_GivenPath_SuccessfullySplitsIntoSingleTuple()
        {
            var normalizedPath =
                PathInfo.SplitPath(@"\\test\unc\path\to\something.ext");

            Assert.AreEqual<(string, string, string)>(
                (@"\\test\unc\path\to", "something", ".ext"),
                (normalizedPath.DirectoryName, normalizedPath.FileName, normalizedPath.Extension));
        }


        [TestMethod]
        public void Constructor_CreateTupleWithoutItemNames()
        {
            (string, string, string) pathData = 
                (@"\\test\unc\path\to", "something", ".ext");

            Assert.AreEqual<string>(
                @"\\test\unc\path\to", pathData.Item1);
            Assert.AreEqual<string>(
                "something", pathData.Item2);
            Assert.AreEqual<string>(
                ".ext", pathData.Item3);

            Assert.AreEqual<(string, string, string)>(
                (@"\\test\unc\path\to", "something", ".ext"),
                (pathData));

            Assert.AreEqual<Type>(
                typeof(ValueTuple<string, string, string>), 
                pathData.GetType());
        }

        [TestMethod]
        public void Constructor_CreateTupleWithItemNames()
        {
            (string DirectoryName, string FileName, string Extension) pathData1 =
                (@"\\test\unc\path\to", "something", ".ext");

            Assert.AreEqual<string>(
                @"\\test\unc\path\to", pathData1.DirectoryName);
            Assert.AreEqual<string>(
                "something", pathData1.FileName);
            Assert.AreEqual<string>(
                ".ext", pathData1.Extension);

            var pathData2 = (DirectoryName: @"\\test\unc\path\to",
                    FileName: "something", Extension: ".ext");
            Assert.AreEqual<string>(
                @"\\test\unc\path\to", pathData2.DirectoryName);
            Assert.AreEqual<string>(
                "something", pathData2.FileName);
            Assert.AreEqual<string>(
                ".ext", pathData2.Extension);

            Assert.AreEqual<Type>(
                typeof(ValueTuple<string, string, string>),
                pathData2.GetType());
        }

        [TestMethod]
        public void Constructor_ComparingTuplesWithMismathedItemNames()
        {
            (string DirectoryName, string FileName, string Extension) pathData1 =
                (@"\\test\unc\path\to", "something", ".ext");

            var pathData2 = (DirectoryName2: @"\\test\unc\path\to",
                    FileName2: "something", Extension2: ".ext");

            Assert.AreEqual<(string, string, string)>(pathData1, pathData2);

            Assert.AreEqual<(string DirectoryName, string FileName, string Extension)>(
                (DirectoryName: @"\\test\unc\path\to",
                    FileName: "something", Extension: ".ext"),
                (pathData2));
        }

        [TestMethod]
        public void Constructor_OmitItemName()
        {
            (string DirectoryName, string FileName, string Extension) pathData =
                (DirectoryName: @"\\test\unc\path\to",
                FileName: "something",
                Extension: ".ext");

            Assert.AreEqual<string>(
                @"\\test\unc\path\to", pathData.Item1);
            Assert.AreEqual<string>(
                "something", pathData.Item2);
            Assert.AreEqual<string>(
                ".ext", pathData.Item3);
        }

        [TestMethod]
        public void DeclaringTuples_WithMistmatchedTuplePropertyNames_IssuesWarning()
        {
            CompilerAssert.StatementsFailCompilation(
                @"
                (string DirectoryName, string FileName, string Extension) pathData =
                    (AltDirectoryName1: @""\\test\unc\path\to"", FileName: ""something"", Extension: "".ext"");
                pathData = (AltDirectoryName2: @""\\test\unc\path\to"", 
                    FileName: ""something"", Extension:"".ext"");
                pathData.GetType();
                ",
                @"Warning CS8123: The tuple element name 'AltDirectoryName1' is ignored because a different name is specified by the target type '(string DirectoryName, string FileName, string Extension)'.",
                "Warning CS8123: The tuple element name 'AltDirectoryName2' is ignored because a different name is specified by the target type '(string DirectoryName, string FileName, string Extension)'."
            );
            CompilerAssert.StatementsFailCompilation(
                @"
                (string, string, string) pathData =
                    (DirectoryName: @""\\test\unc\path\to"",
                    FileName: ""something"",
                    Extension: "".ext"");
                pathData.GetType();
                ",
            "Warning CS8123: The tuple element name 'DirectoryName' is ignored because a different name is specified by the target type '(string, string, string)'.",
            "Warning CS8123: The tuple element name 'Extension' is ignored because a different name is specified by the target type '(string, string, string)'.",
            "Warning CS8123: The tuple element name 'FileName' is ignored because a different name is specified by the target type '(string, string, string)'.");
        }

        [TestMethod]
        public void ValueTuple_GivenNamedTuple_ItemXHasSameValuesAsNames()
        {
            var normalizedPath = 
                (DirectoryName: @"\\test\unc\path\to", FileName: "something", 
                Extension: ".ext");

            Assert.AreEqual<string>(normalizedPath.Item1, normalizedPath.DirectoryName);
            Assert.AreEqual<string>(normalizedPath.Item2, normalizedPath.FileName);
            Assert.AreEqual<string>(normalizedPath.Item3, normalizedPath.Extension);
        }
    }
}
