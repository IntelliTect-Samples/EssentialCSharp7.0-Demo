using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    public partial class PathInfo
    {
        // Set https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/src/System/IO for real implementation.
        static public void SplitPath(string path,
            out string DirectoryName, out string FileName, out string Extension)
        {
            DirectoryName = System.IO.Path.GetDirectoryName(path);
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path);
        }
    }
        [TestClass]
    public class InlineOutVariableDeclaration
    {
        [TestMethod]
        public void DeclareOutParameters_InvokeMethod()
        {
            string directoryName, fileName, extension;

            // E.g. 1: Out inline declaration and assignment
            PathInfo.SplitPath(@"\\test\unc\path\to\something.ext",
                out directoryName,
                out fileName,
                out extension);

            Assert.AreEqual<string>(@"\\test\unc\path\to", directoryName);
            Assert.AreEqual<string>("something", fileName);
            Assert.AreEqual<string>(".ext", extension);
        }

        [TestMethod]
        public void DeclareOutParametersInlineWithInvocation()
        {
            // E.g. 1: Out inline declaration and assignment
            PathInfo.SplitPath(@"\\test\unc\path\to\something.ext",
                out string directoryName,
                out string fileName,
                out string extension);

            Assert.AreEqual<string>(@"\\test\unc\path\to", directoryName);
            Assert.AreEqual<string>("something", fileName);
            Assert.AreEqual<string>(".ext", extension);
        }

        [TestMethod]
        public void DiscardOutParametersWithInvocation()
        {
            // E.g. 1: Out inline declaration and assignment
            PathInfo.SplitPath(@"\\test\unc\path\to\something.ext",
                out string _,
                out string fileName,
                out string extension);

            Assert.AreEqual<string>("something", fileName);
            Assert.AreEqual<string>(".ext", extension);
        }
    }
}
