using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilities.IO
{
    public class CounterFile
    {
        string path;
        int Count;
        object lockObject;

        public CounterFile(string path, Func<int> getDefault = null)
        {
            lockObject = new object();
            this.path = path;

            if (!File.Exists(path))
            {
                int def = (getDefault ?? new Func<int>(() => 0))();
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
                    writer.Write(def);
            }

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
                Count = reader.ReadInt32();
        }

        public int Allocate(int quantity)
        {
            lock (lockObject)
            {
                Count += quantity;
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
                    writer.Write(Count);
                return Count - quantity;
            }
        }
    }

    public class CounterFileLong
    {
        string path;
        long Count;
        object lockObject;

        public CounterFileLong(string path, Func<long> getDefault = null)
        {
            lockObject = new object();
            this.path = path;

            if (!File.Exists(path))
            {
                long def = (getDefault ?? new Func<long>(() => 0))();
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
                    writer.Write(def);
            }

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
                Count = reader.ReadInt64();
        }

        public long Allocate(int quantity)
        {
            lock (lockObject)
            {
                Count += quantity;
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
                    writer.Write(Count);
                return Count - quantity;
            }
        }
    }
}
