using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.IO;

namespace Utilities.IO
{
    public interface IFlagArray : IDisposable
    {
        BitArray Bits { get; }
    }

    public class FlagsDictionary<TKey>
    {
        Func<TKey, string> getPath;
        int size;
        LazyConcurrentDictionary<TKey, FlagsDictionary<TKey>.FlagArray> dictionary;

        LockContainer<TKey> locks;

        class FlagArray : IFlagArray
        {
            public FlagArray(string path, int size, Action onDispose)
            {
                int numBytes = (size + sizeof(byte) - 1) / sizeof(byte);
                this.onDispose = onDispose;
                this.path = path;

                if (!File.Exists(path))
                {
                    bytes = new byte[numBytes];
                    File.WriteAllBytes(path, new byte[numBytes]);
                }
                else
                {
                    bytes = File.ReadAllBytes(path);
                }

                Bits = new BitArray(bytes);
                Lock = new ReaderWriterLockSlim();
            }

            readonly Action onDispose;
            readonly string path;
            readonly byte[] bytes;
            public readonly ReaderWriterLockSlim Lock;

            public BitArray Bits { get; private set; }

            public void Dispose()
            {
                Bits.CopyTo(bytes, 0);
                File.WriteAllBytes(path, bytes);
                onDispose();
            }
        }

        /// <summary>
        /// Represents a dictionary of flag arrays.  Each key has one flag array (each flag is 1 bit) of size flags.
        /// </summary>
        /// <param name="getPath">A function that returns the path to the binary file that will be created for the given key.</param>
        /// <param name="size">The number of flags for each key</param>
        public FlagsDictionary(Func<TKey, string> getPath, int size)
        {
            System.Diagnostics.Contracts.Contract.Requires(getPath != null);

            this.getPath = getPath;
            this.size = size;
            this.locks = new LockContainer<TKey>();
            this.dictionary = new LazyConcurrentDictionary<TKey, FlagArray>(key => new FlagArray(this.getPath(key), this.size, () => locks.ExitWriteLock(key)));
        }

        public IFlagArray this[TKey key]
        {
            get
            {
                locks.EnterWriteLock(key);
                var flagArray = dictionary[key];
                return flagArray;
            }
        }

        public void Release()
        {
            foreach (var flag in dictionary.Clear())
            {
                locks.EnterWriteLock(flag.Key);
                flag.Value.Dispose();
            }
        }
    }
}
