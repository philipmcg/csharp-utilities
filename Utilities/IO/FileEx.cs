using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;

namespace Utilities
{
    public class FileEx
    {
        /// <summary>
        /// If the directory path does not exist, it will be created
        /// </summary>
        public static void WriteAllLines(string filename, IEnumerable<string> lines)
        {
            string path = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (lines is string[])
            {
                File.WriteAllLines(filename, (string[])lines);
            }
            else
            {
                StreamWriter writer = new StreamWriter(filename);
                foreach (var line in lines)
                    writer.WriteLine(line);
                writer.Close();
            }
        }

        public static string FindLastModifiedFile(string directory, string pattern)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            var files = dir.GetFiles(pattern);
            var file = files.OrderByDescending(f => f.LastWriteTime.ToSeconds()).FirstOrDefault();
            if (file != null)
                return file.FullName;
            else
                return null;
        }
        public static void MoveSafely(string start, string dest)
        {
            if (File.Exists(start))
            {
                try
                {
                    if (File.Exists(dest))
                        File.Delete(dest);
                }
                catch
                {
                    return;
                }


                try
                {
                    if (!File.Exists(dest))
                    {
                        File.Copy(start, dest);
                        File.Delete(start);
                    }
                }
                catch
                {
                }
            }
        }

        public static string ReadFirstStringInFile(string path)
        {
            return ReadFirstStringInFile(path, f => MessageBox.Show("File not found: " + f), null);
        }
        public static string ReadFirstStringInFile(string path, Action<string> act, string def)
        {
            var data = (new DelimReader() { NoFileExists = act }).ReadToString(path);
            if (data != null)
                if (data.Count > 0)
                    return data[0];
            return def;
        }

    }
}
