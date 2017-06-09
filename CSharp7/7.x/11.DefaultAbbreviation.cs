using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp7
{
    public partial class CustomAsyncReturn
    {
        static public async ValueTask<long> GetDirectorySizeAsync(
            string path, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption).Any())
                return default;
            else
                return await Task.Run<long>(() => Directory.GetFiles(path, searchPattern,
                    SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)));
        }

    }
}