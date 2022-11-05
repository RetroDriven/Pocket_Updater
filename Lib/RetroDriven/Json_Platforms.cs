using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroDriven
{
    public class CoreInfo
    {
        public Platform platform { get; set; }

        public static IEnumerable<string> GetJsonFiles(string directoryPath, string extension, SearchOption searchOption)
        {
            //System.Diagnostics.Debug.WriteLine(directoryPath);
            return Directory.GetFiles(directoryPath, extension, SearchOption.TopDirectoryOnly);
        }
    }

    public class Platform
    {
        public string category { get; set; }
        public string name { get; set; }
        public string manufacturer { get; set; }
        public int year { get; set; }
    }

}
