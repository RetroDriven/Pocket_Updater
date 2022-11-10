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
    public class RD_Saves
    {
        class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
        {
            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                return (f1.Name == f2.Name);
            }
            public int GetHashCode(System.IO.FileInfo fi)
            {
                string s = fi.Name;
                return s.GetHashCode();
            }
        }
    public static void Sync(string sourcePath, string destinationPath)
        {
            // sourcePath = @"C:\Users\Administrator\Desktop\Source";
            //string destinationPath = @"C:\Users\Administrator\Desktop\Dest";

            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(sourcePath);
            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(destinationPath);

            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.sav",
            System.IO.SearchOption.AllDirectories);

            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.sav",
            System.IO.SearchOption.AllDirectories);

            bool IsInDestination = false;
            bool IsInSource = false;

            foreach (System.IO.FileInfo s in list1)
            {
                IsInDestination = true;

                foreach (System.IO.FileInfo s2 in list2)
                {
                    if (s.Name == s2.Name)
                    {
                        IsInDestination = true;
                        break;
                    }
                    else
                    {
                        IsInDestination = false;
                    }
                }

                if (!IsInDestination)
                {
                    System.IO.File.Copy(s.FullName, System.IO.Path.Combine(destinationPath, s.Name), true);
                }
            }

            list1 = dir1.GetFiles("*.sav", System.IO.SearchOption.AllDirectories);

            list2 = dir2.GetFiles("*.sav", System.IO.SearchOption.AllDirectories);

            bool areIdentical = list1.SequenceEqual(list2, new FileCompare());

            if (!areIdentical)
            {
                foreach (System.IO.FileInfo s in list2)
                {
                    IsInSource = true;

                    foreach (System.IO.FileInfo s2 in list1)
                    {
                        if (s.Name == s2.Name)
                        {
                            IsInSource = true;
                            break;
                        }
                        else
                        {
                            IsInSource = false;
                        }
                    }

                    if (!IsInSource)
                    {
                        System.IO.File.Copy(s.FullName, System.IO.Path.Combine(sourcePath, s.Name), true);
                        System.Diagnostics.Debug.WriteLine("Source Name:" + s.FullName);
                        //System.Diagnostics.Debug.WriteLine("Dest Name:" + System.IO.Path.Combine(sourcePath, s.Name));
                        string Test = s.FullName.Replace("common","");
                        System.Diagnostics.Debug.WriteLine(Test);
                    }
                }
            }
        }
    }
}
