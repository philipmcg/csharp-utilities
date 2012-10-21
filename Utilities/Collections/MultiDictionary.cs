using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
                Add(key, new List<TValue>());
            this[key].Add(value);
        }

        public void Add(TKey key, IEnumerable<TValue> value)
        {
            if (!ContainsKey(key))
                base.Add(key, new List<TValue>());

            this[key].AddRange(value);
        }

        public List<TValue> GetValuesForKey(TKey key)
        {
            return this[key];
        }

        public IEnumerable<TValue> GetAllValues()
        {
            foreach (var keyValPair in this)
                foreach (var val in keyValPair.Value)
                    yield return val;
        }
    }
}
