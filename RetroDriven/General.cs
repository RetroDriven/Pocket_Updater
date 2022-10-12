using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO.Compression;

namespace RetroDriven
{
    public class General
    {

        public static IEnumerable<string> GetFilesByExtension(string directoryPath, string extension, SearchOption searchOption)
        {
            return Directory.GetFiles(directoryPath, extension, SearchOption.AllDirectories);
        }

        public static void CreateZipFile(string fileName, IEnumerable<string> files)
        {
            /// Create a ZIP file of the files provided.
            /// fileName = The full path and name to store the ZIP file at
            /// files = The list of files to be added

            // Create and open a new ZIP file
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }
        public static void CreateDir(string Dir)
        {
            try
            {
                if (!Directory.Exists(Dir))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Dir);
                }
            }
            catch (IOException ioex)
            {
                MessageBox.Show(ioex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}