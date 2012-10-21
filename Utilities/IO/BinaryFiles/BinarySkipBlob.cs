using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Utilities.IO
{


    public interface IBinarySkipBlobItem
    {
        void Write(BinaryWriter writer);
        void Read(BinaryReader reader);
        int AddressOfPrevious { get; set; }
    }

    public class BinarySkipBlob<T> where T : struct, IBinarySkipBlobItem
    {
        ReaderWriterLockSlim Lock;
        string directory;
        string headFile;
        string blobFile;
        string counterFile;
        BinaryFileIntegerArray headPointers;

        CounterFile counter;
        int itemSize;
        int possibleHeads;

        int position;

        public BinarySkipBlob(string directory, int possibleHeads, int itemSize)
        {
            this.directory = directory;
            this.itemSize = itemSize;
            this.possibleHeads = possibleHeads;
            Lock = new ReaderWriterLockSlim();
            headFile = Path.Combine(directory, "heads");
            counterFile = Path.Combine(directory, "count");
            blobFile = Path.Combine(directory, "blob");
            Initialize();
        }

        void Initialize()
        {
            position = 0;
            headPointers = new BinaryFileIntegerArray(headFile, possibleHeads);
            counter = new CounterFile(counterFile);

            if (!File.Exists(blobFile))
                File.WriteAllBytes(blobFile, new byte[itemSize]);
        }

        public void Clear()
        {
            using (Lock.EnterWrite())
            {
                DirectoryEx.DeleteContents(directory);
            }
        }

        public void Reset()
        {
            using (Lock.EnterWrite())
            {
                DirectoryEx.DeleteContents(directory);
                Initialize();
            }
        }

        public void WriteItems(T[] items, int[] keys)
        {
            using (Lock.EnterWrite())
            {
                position = counter.Allocate(items.Length);
                using (var context = headPointers.EnterWrite())
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(blobFile, FileMode.Open, FileAccess.Write)))
                    {
                        writer.Seek(position * itemSize, SeekOrigin.Begin);

                        for (int i = 0; i < items.Length; i++)
                        {
                            items[i].AddressOfPrevious = context[keys[i]];
                            context[keys[i]] = (int)(writer.BaseStream.Position / itemSize);
                            items[i].Write(writer);
                        }
                    }
                }
            }
        }

        public T[] ReadItems(int key)
        {
            using (Lock.EnterRead())
            {
                if (key < 0)
                    return new T[0];

                int position = headPointers[key];

                if (position == 0)
                    return new T[0];
                else
                    return PrivateReadItems(key);
            }
        }

        T[] PrivateReadItems(int key)
        {
            List<T> items = new List<T>();
            
            using (BinaryReader reader = new BinaryReader(File.Open(blobFile, FileMode.Open, FileAccess.Read)))
            {
                int pos = headPointers[key];
                while (pos != 0)
                {
                    reader.BaseStream.Seek(pos * itemSize, SeekOrigin.Begin);
                    T item = new T();
                    item.Read(reader);
                    items.Add(item);
                    pos = item.AddressOfPrevious;
                }
            }

            return items.ToArray();
        }
    }
}
