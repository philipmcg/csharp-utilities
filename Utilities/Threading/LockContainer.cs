using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class LockContainer<T>
    {
        readonly Dictionary<T, System.Threading.ReaderWriterLockSlim> locks = new Dictionary<T, System.Threading.ReaderWriterLockSlim>();
        readonly object locksLock = new object();
        Action<Exception> exceptionHandler;

        public LockContainer(Action<Exception> exceptionHandler = null)
        {
            this.exceptionHandler = exceptionHandler;
        }

        public void TryUsing(T id, Action action)
        {
            EnterWriteLock(id);

            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                    exceptionHandler(ex);
                else
                    throw;
            }
            finally
            {
                ExitWriteLock(id);
            }
        }

        public void EnterReadLock(T id)
        {
            GetLock(id).EnterReadLock();
        }
        public void EnterWriteLock(T id)
        {
            GetLock(id).EnterWriteLock();
        }
        public void ExitReadLock(T id)
        {
            GetLock(id).ExitReadLock();
        }
        public void ExitWriteLock(T id)
        {
            GetLock(id).ExitWriteLock();
        }

        // disposable lock.
        public WriteLock EnterWrite(T id)
        {
            return GetLock(id).EnterWrite();
        }

        System.Threading.ReaderWriterLockSlim GetLock(T id)
        {
            System.Threading.ReaderWriterLockSlim l;

            if (locks.TryGetValue(id, out l))
                return l;
            else
            {
                lock (locksLock)
                {
                    if (!locks.ContainsKey(id))
                        locks.Add(id, new System.Threading.ReaderWriterLockSlim());
                    return locks[id];
                }
            }
        }

        public T[] GetCurrentlyHeldWriteLocks()
        {
            lock (locksLock)
            {
                return locks.Where(l => l.Value.IsWriteLockHeld).Select(l => l.Key).ToArray();
            }
        }

        public T[] GetCurrentlyHeldReadLocks()
        {
            lock (locksLock)
            {
                return locks.Where(l => l.Value.IsReadLockHeld).Select(l => l.Key).ToArray();
            }
        }
    }
}
