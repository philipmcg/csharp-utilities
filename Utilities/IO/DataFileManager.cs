using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Utilities
{
    public class DataFileManager<TKey, T> : Dictionary<TKey, T>
    {
        protected string Directory;

        /// <summary>
        /// Function to load the object from file.  First parameter is the directory name, second is the key.
        /// </summary>
        public Func<string, TKey, T> LoadFile { get; set; }
        object loadLock;

        public DataFileManager(string directory)
        {
            Directory = directory;
            loadLock = new object();
        }

        public new T this[TKey key]
        {
            get
            {
                T ret;

                if (!base.TryGetValue(key, out ret))
                    lock (loadLock)
                        if (!base.TryGetValue(key, out ret))
                            return LoadNewFile(key);
                    
                return ret;
            }
            set
            {
                base[key] = value;
            }
        }
        protected T LoadNewFile(TKey key)
        {
            return LoadFile(Directory, key);
        }

    }

    public class DataFileManager<T> : DataFileManager<string, T>
    {
        public DataFileManager(string directory) : base(directory)
        {
        }
    }
}
