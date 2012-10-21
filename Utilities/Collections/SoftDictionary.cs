using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public sealed class DefaultsDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        Func<TKey, TValue> GetDefault;
        public DefaultsDictionary(Func<TKey, TValue> getDefault = null)
        {
            this.GetDefault = getDefault;
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue ret;
                if (! base.TryGetValue(key, out ret))
                {
                    ret = GetDefault(key);
                    base[key] = ret;
                }
                return ret;
            }
            set
            {
                if (base.ContainsKey(key))
                    base[key] = value;
                else
                    base.Add(key, value);
            }
        }
    }

    /// <summary>
    /// This is the same as a Dictionary, with two differences:
    /// SoftDictionary[key] will return default(TValue) if the dictionary does contain key.
    /// And SoftDictionary[key] = value can be used in place of Add().
    /// </summary>
    public sealed class SoftDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new TValue this[TKey key]
        {
            get
            {
                TValue ret;
                bool found = base.TryGetValue(key, out ret);
                if (found)
                    return ret;
                else
                    return default(TValue);
            }
            set
            {
                if (base.ContainsKey(key))
                    base[key] = value;
                else
                    base.Add(key, value);
            }
        }
    }

    public sealed class CloneDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : ICloneable
    {
        public new TValue this[TKey key]
        {
            get
            {
                return (TValue)base[key].Clone();
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
