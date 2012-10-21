using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
namespace Utilities
{
    public static partial class Extensions
    {
        public static ReadLock EnterRead(this ReaderWriterLockSlim me)
        {
            return new ReadLock(me);
        }
        public static WriteLock EnterWrite(this ReaderWriterLockSlim me)
        {
            return new WriteLock(me);
        }
    }

    public struct ReadLock : IDisposable
    {
        readonly ReaderWriterLockSlim parent;

        public ReadLock(ReaderWriterLockSlim parent)
        {
            this.parent = parent;
            this.parent.EnterReadLock();
        }

        public void Dispose()
        {
            this.parent.ExitReadLock();
        }
    }

    public struct WriteLock : IDisposable
    {
        readonly ReaderWriterLockSlim parent;

        public WriteLock(ReaderWriterLockSlim parent)
        {
            this.parent = parent;
            this.parent.EnterWriteLock();
        }

        public void Dispose()
        {
            this.parent.ExitWriteLock();
        }
    }

}

namespace Utilities.IO
{

    public class BinaryFileIntegerArray
    {
        ReaderWriterLockSlim Lock;
        string path;
        ReadWriteContext context;

        public class ReadWriteContext : IDisposable
        {
            BinaryWriter writer;
            BinaryReader reader;
            BinaryFileIntegerArray parent;

            public ReadWriteContext(BinaryFileIntegerArray parent)
            {
                this.parent = parent;
            }

            public ReadWriteContext Enter()
            {
                parent.Lock.EnterWriteLock();
                Stream stream = File.Open(parent.path, FileMode.Open, FileAccess.ReadWrite);
                writer = new BinaryWriter(stream);
                reader = new BinaryReader(stream);
                return this;
            }

            public void Dispose()
            {
                writer.Close();
                reader.Close();
                parent.Lock.ExitWriteLock();
            }

            public int this[int index]
            {
                get
                {
                    reader.BaseStream.Seek(index * sizeof(int), SeekOrigin.Begin);
                    return reader.ReadInt32();
                }
                set
                {
                    writer.BaseStream.Seek(index * sizeof(int), SeekOrigin.Begin);
                    writer.Write(value);
                }
            }
        }

        public ReadWriteContext EnterWrite()
        {
            return context.Enter();
        }

        public BinaryFileIntegerArray(string path, int size)
        {
            Lock = new ReaderWriterLockSlim();
            context = new ReadWriteContext(this);
            int fileLength = size * sizeof(int);
            this.path = path;

            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, new byte[fileLength]);
            }
        }

        public int this[int index]
        {
            get
            {
                using (Lock.EnterRead())
                {
                    using (var reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
                    {
                        reader.BaseStream.Seek(index * sizeof(int), SeekOrigin.Begin);
                        return reader.ReadInt32();
                    }
                }
            }
            set
            {
                using (Lock.EnterRead())
                {
                    using (var writer = new BinaryWriter(File.Open(path, FileMode.Open, FileAccess.Write)))
                    {
                        writer.BaseStream.Seek(index * sizeof(int), SeekOrigin.Begin);
                        writer.Write(value);
                    }
                }
            }
        }
    }
}
