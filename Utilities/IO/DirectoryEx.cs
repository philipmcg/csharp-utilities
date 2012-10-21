using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Utilities
{
    public static class DirectoryEx
    {
        /// <summary>
        /// Gets the name of the last directory in the path.
        /// GetInnerDirectoryName("C:\Windows\system32") would return system32.
        /// </summary>
        public static string GetInnerDirectoryName(string path)
        {
            return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }

        /// <summary>
        /// Recursively moves the contents of sourceDirectory to destinationDirectory, overwriting if files already exist.
        /// </summary>
        public static void MoveContents(string sourceDirectory, string destinationDirectory)
        {
            var files = Directory.GetFiles(sourceDirectory);
            foreach (var file in files)
            {
                string d = Path.Combine(destinationDirectory, Path.GetFileName(file));
                if (File.Exists(d))
                    File.Delete(d);
                File.Move(file, d);
            }

            var dirs = Directory.GetDirectories(sourceDirectory);
            foreach (var dir in dirs)
            {
                string d = Path.Combine(destinationDirectory, GetInnerDirectoryName(dir));
                MoveDirectory(dir, d);
            }
        }

        /// <summary>
        /// Recursively copies the contents of sourcePath to destinationPath, overwriting if files already exist.
        /// </summary>
        public static void CopyContents(string sourcePath, string destinationPath)
        {
            var source = new DirectoryInfo(sourcePath);
            var dest = new DirectoryInfo(destinationPath);
            CopyFilesRecursively(source, dest);
        }

        static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
        }

        /// <summary>
        /// Deletes the whole contents of the directory at sourcePath
        /// </summary>
        public static void DeleteContents(string sourcePath)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(sourcePath);

            foreach (System.IO.FileInfo file in directory.GetFiles()) 
                file.Delete();

            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories())
            {
                try
                {
                    subDirectory.Delete(true);
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(0);
                    subDirectory.Delete(true);
                }
            }

        }

        /// <summary>
        /// Recursively moves the directory at sourcePath to destinationPath.
        /// </summary>
        public static void MoveDirectory(string sourcePath, string destinationPath)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(sourcePath, destinationPath));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                }
            }
            Directory.Delete(sourcePath, true);
        }

        /// <summary>
        /// Helper class used by MoveDirectory
        /// </summary>
        class Folders
        {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target)
            {
                Source = source;
                Target = target;
            }
        }

        /// <summary>
        /// Returns the total size in bytes of the contents of the directory, recursively.
        /// </summary>
        public static long GetSizeOfDirectory(string directoryPath)
        {
            return GetSizeOfDirectory(new DirectoryInfo(directoryPath));
        }
        static long GetSizeOfDirectory(DirectoryInfo d)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += GetSizeOfDirectory(di);
            }
            return (Size);
        }

        /// <summary>
        /// Only works for file paths with extensions, or directory paths.  Will not work for a file path if the file has no extension.
        /// </summary>
        public static void EnsureDirectory(string path)
        {
            string directory = path; // If path is a directory, just use it
            if(!string.IsNullOrWhiteSpace(Path.GetExtension(path))) // If path has an extension, then we want the super directory
                directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static List<string> GetFilesInDirectoryRecursively(string path, string pattern = "*")
        {
            List<string> list = new List<string>();
            GetFilesRecursively(path, list, pattern);
            return list;
        }

        static void GetFilesRecursively(string path, List<string> list, string pattern = "*")
        {
            foreach (var file in Directory.GetFiles(path, pattern))
            {
                list.Add(file);
            }
            foreach (var subdir in Directory.GetDirectories(path))
            {
                GetFilesRecursively(subdir, list, pattern);
            }
        }

        static IEnumerable<FileInfo> GetFilesRecursively(DirectoryInfo dir, string pattern = "*")
        {
            foreach (var file in dir.GetFiles(pattern))
            {
                yield return file;
            }
            foreach (var subdir in dir.GetDirectories())
            {
                foreach (var file in GetFilesRecursively(subdir, pattern))
	            {
		             yield return file;
	            }
            }
        }

    }
}
