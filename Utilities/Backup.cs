

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilities
{
    /// <summary>
    /// Provides a function to backup a file.
    /// </summary>
    public static class Backup
    {
        /// <summary>
        /// Filename extension to append to backup files.
        /// </summary>
        private static string BackupExtension { get { return "bak"; } }

        /// <summary>
        /// Internal function used to get a string of zeroes with
        /// the same length as the string representation of max.
        /// </summary>
        private static string GetNumberFormatString(int max)
        {
            int n = max.ToString().Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append("0");
            }
            return sb.ToString();
        }
        /// <summary>
        /// Moves the specified file to a backup location.  Does NOT copy the file.  If a backup already exists, it renames it incrementally, leaving a maximum of 'numBackups' files.
        /// </summary>
        /// <param name="file">File to backup.</param>
        /// <param name="numBackups">Number of backup files to keep.</param>
        public static void BackupFile(string file, int numBackups)
        {
            BackupFile(file, numBackups, false);
        }

        /// <summary>
        /// Makes a backup of the specified file.  If a backup already exists, it renames it incrementally, leaving a maximum of 'numBackups' files.
        /// </summary>
        /// <param name="file">File to backup.</param>
        /// <param name="numBackups">Number of backup files to keep.</param>
        /// <param name="copy">If false, the original file will be deleted after the backup is complete.</param>
        public static void BackupFile(string file, int numBackups, bool copy)
        {
            int max = numBackups - 1;

            string format = GetNumberFormatString(Math.Max(10, max));

            if (!File.Exists(file))
                return;

            string ext = Path.GetExtension(file) + "." + BackupExtension;

            int num = 0;
            while (num < max)
            {
                if (File.Exists(Path.ChangeExtension(file, ext + num.ToString(format))))
                    num++;
                else
                    break;
            }

            if (num == max)
                File.Delete(Path.ChangeExtension(file, ext + num.ToString(format)));

            while (num > 0)
            {
                num--;
                string current = Path.ChangeExtension(file, ext + (num).ToString(format));
                string push = Path.ChangeExtension(file, ext + (num + 1).ToString(format));

                File.Move(current, push);
            }

            string backup = Path.ChangeExtension(file, ext + (0).ToString(format));

            if (copy)
                File.Copy(file, backup);
            else
                File.Move(file, backup);
        }

        /// <summary>
        /// Gets an unused backup file path in the target directory.
        /// </summary>
        /// <param name="path">The directory where the unused path will be.</param>
        /// <param name="max">The maxmimum number of backup files to keep.</param>
        public static string GetSafePath(string path, int max, Action<string> last)
        {
            string format = GetNumberFormatString(Math.Max(10, max));

            string dir = Path.GetDirectoryName(path);
            string file = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            int count = 0;
            string newPath = path;
            do
            {
                if (count == max)
                {
                    if (last != null)
                        last(newPath);

                    return newPath;
                }
                newPath = Path.Combine(dir, file + count.ToString(format) + ext);
                count++;
            }
            while (File.Exists(newPath));

            return newPath;
        }
    }

}


