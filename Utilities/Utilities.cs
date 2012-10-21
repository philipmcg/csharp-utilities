using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.IO;
using System.Windows.Forms;

using System.Security.Cryptography;
 using Microsoft.Win32;

namespace Utilities
{

    public static class Util
    {

        public static string TruncateWithEllipsis(string str, int length)
        {
            if (str.Length > length + 3)
                str = str.Substring(0, length) + "...";
            return str;
        }

        /// <summary>
        /// Returns the directory that the current process's executable file is in.
        /// </summary>
        public static string ApplicationDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        public static string ReadRegistryKey (string path,string key)
        {
            RegistryKey masterKey = Registry.LocalMachine.CreateSubKey(path);
            string val = "";

            if (masterKey != null)
            {
                val = masterKey.GetValue(key).ToString();
                masterKey.Close();
            }

            return val;
		}

        public static void RunBgSilent(Action complete, Action work)
        {
            bool busy = true;

            System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();

                bg.DoWork += (s, e) =>
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    work();
                };
            

            if (complete != null)
                bg.RunWorkerCompleted += (s, e) => complete();

            bg.RunWorkerAsync();

            while (busy)
                Application.DoEvents();

            return;
        }

        public static void RunBgThread(Action work)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() => work());
            thread.Start();
        }

        public static void StartAsyncSilent(Action complete, Action work)
        {
            System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();

            bg.DoWork += (s, e) =>
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                work();
            };


            if (complete != null)
                bg.RunWorkerCompleted += (s, e) => complete();

            bg.RunWorkerAsync();
        }


        public static string Ordinal(int number)
        {
            var work = number.ToString();
            int n = number % 100;
            if (n == 11 || n == 12 || n == 13)
                return work + "th";
            switch (number % 10)
            {
                case 1: work += "st"; break;
                case 2: work += "nd"; break;
                case 3: work += "rd"; break;
                default: work += "th"; break;
            }
            return work;
        }

        public static string HexStr(byte[] p)
        {

            char[] c = new char[p.Length * 2];

            byte b;

            for (int y = 0, x = 0; y < p.Length; ++y, ++x)
            {

                b = ((byte)(p[y] >> 4));

                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

                b = ((byte)(p[y] & 0xF));

                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

            }

            return new string(c);
        }
        public static string GetMD5HashFromString(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var bytes= md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static int SmallestPowerOfTwo(int biggerThan)
        {

            int[] pow2 = new int[16];
            for (int i = 1; i < pow2.Length; i++)
            {
                pow2[i] = (int)Math.Pow(2, i);
                if (pow2[i] >= biggerThan)
                    return pow2[i];
            }
            return 2048;
        }
        public static BinaryReader GetBinReader(string path)
        {
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            BinaryReader reader = new BinaryReader(stream);
            return reader;
        }
        public static BinaryWriter GetBinWriter(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            return writer;
        }

        public static List<FileInfo> GetFiles(string path, string ext)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
            {
                if (file.Extension == "." + ext)
                {
                    files.Add(file);
                }
            }
            return files;
        }


        /// <summary>
        /// Does NOT shuffle in place
        /// </summary>
        public static List<T> GetShuffled<T>(this List<T> me)
        {
            List<T> randomList = new List<T>();

            Random r = new Random();
            int randomIndex = 0;
            while (me.Count > 0)
            {
                randomIndex = r.Next(0, me.Count); //Choose a random object in the list
                randomList.Add(me[randomIndex]); //add it to the new, random list
                me.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
    }

    public sealed class DirectoryStructure : Dictionary<string, string>
    {
        string BaseDirectory;
        public DirectoryStructure(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
        }
        public new string this[string dirKey]
        {
            get
            {
                return BaseDirectory + "/" + base[dirKey];
            }
        }
        public string this[string dirKey, string fileName]
        {
            get
            {
                return BaseDirectory + "/" + base[dirKey] + "/" + fileName;
            }
        }
    }
    public sealed class FileStructure : Dictionary<string, string>
    {
        public static DirectoryStructure ds;
        public new string this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                    return base[key];
                else
                    if(key.Contains('.'))
                        return ds["descr"] + "/" + key;
                    else
                        return ds["descr"] + "/" + key + ".txt";
            }
        }
    }
    
    
}
