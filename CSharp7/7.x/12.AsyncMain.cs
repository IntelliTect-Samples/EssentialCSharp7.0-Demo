using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp7
{

    public class Program
    {
        public static async Task<long> Main()
        {

            IAsyncEnumerable<FileInfo> files = Directory.EnumerateFilesAsync(
                @"c:\", "*.cs", SearchOption.AllDirectories);

            foreach await(FileStream file in files.Select(
                item=>item.OpenReadAsync()) { /*...*/ }
            
            using await (IAsyncEnumerable<FileStream> files = await files.Select(
                item => item.OpenReadAsync())
        }
    }
    
    interface IAsyncEnumerable<T> :IEnumerable<T> { }
}