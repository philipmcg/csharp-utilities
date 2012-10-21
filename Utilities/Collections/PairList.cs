using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class PairList<K,V> : List<KeyValuePair<K,V>>
    {
        public void Add(K key, V value)
        {
            base.Add(new KeyValuePair<K, V>(key, value));
        }
    }
}
