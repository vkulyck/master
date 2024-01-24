using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.IO;

namespace GmWeb.Tests.Api.Data
{
    public class FileReader
    {
        public static string TestFileDirectory => Path.Combine(AssemblyDirectory, "Data", "Files");
        public static string AssemblyDirectory { get; private set; }
        static FileReader()
        {
            AssemblyDirectory = DeduceDataDirectory();
        }

        public static string DeduceDataDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static byte[] ReadCalendar(string filename)
            => ReadFile(filename, "source-calendar.ics");
        private static byte[] ReadFile(string filename, string defaultFilename)
        {
            var path = Path.Combine(TestFileDirectory, filename);
            if(!File.Exists(path))
            {
                path = Path.Combine(TestFileDirectory, defaultFilename);
            }
            return File.ReadAllBytes(path);
        }
    }
}
