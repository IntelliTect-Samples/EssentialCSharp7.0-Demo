using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp7
{

    partial class TemporaryFile
    {
        ~TemporaryFile() => Dispose();
        FileInfo _File;
        public FileInfo File
        {
            get => _File;
            set => _File = value;
        }
        void Dispose() => File?.Delete();
    }

}
