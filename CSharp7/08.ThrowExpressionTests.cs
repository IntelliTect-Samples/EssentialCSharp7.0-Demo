using System;
using System.IO;
namespace CSharp7
{
    public partial class TemporaryFile
    {
        public TemporaryFile(string fileName) =>
            File = new FileInfo(
                fileName ?? throw new ArgumentNullException());
    }
}
