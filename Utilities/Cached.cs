
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utilities
{
    /// <summary>
    /// A simple wrapper around an object that can be recached.
    /// </summary>
    public class SimpleCached<T>
    {
        protected T item;
        protected object lockObject = new object();

        /// <summary>
        /// If the value is not cached, this may take some time!
        /// </summary>
        public virtual T Value
        {
            get
            {
                lock (lockObject)
                {
                    return item;
                }
            }
        }

        /// <summary>
        /// Recaches this item from a provided value
        /// </summary>
        public void Recache(T value)
        {
            lock (lockObject)
            {
                item = value;
            }
        }

        public bool HasValue 
        { 
            get
            {
                lock (lockObject)
                {
                    return item != null; 
                }
            } 
        }
    }

    /// <summary>
    /// A simple wrapper around an object where the recaching function is provided.
    /// Items can recached based on the time since they were last cached.
    /// </summary>
    public class Cached<T> : SimpleCached<T> where T : class
    {
        DateTime lastCache;
        Func<T> recache;

        /// <summary>
        /// If the value is not cached, this may take some time!
        /// </summary>
        public override T Value
        {
            get
            {
                lock (lockObject)
                {
                    if (item == null)
                        item = recache();
                    return item;
                }
            }
        }

        public Cached(Func<T> recache)
        {
            this.recache = recache;
        }

        void RecacheBlocking(int minutes = 0)
        {
            lock (lockObject)
            {
                if (HasExpired(minutes))
                {
                    item = recache();
                    lastCache = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Recaches this item in the background
        /// </summary>
        public void RecacheAsync(int minutes = 0)
        {
            Async.RunSilentlyWithoutCallback(() => RecacheBlocking(minutes));
        }

        /// <summary>
        /// Recaches this cache so it must be recached next time it is requested
        /// </summary>
        public void Invalidate()
        {
            lock (lockObject)
            {
                item = null;
            }
        }

        TimeSpan TimeSinceLastCache()
        {
            if (HasValue)
                return DateTime.UtcNow.Subtract(lastCache);
            else
                return TimeSpan.MaxValue;
        }

        /// <summary>
        /// Returns true if the item has not been recached in the last X minutes
        /// </summary>
        bool HasExpired(int minutes)
        {
            if (HasValue)
                return TimeSinceLastCache().TotalMinutes >= minutes;
            else
                return true;
        }
    }

}
