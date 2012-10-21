using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Utilities
{

    public class LazyConcurrentDictionary<K, V>
    {
        System.Collections.Concurrent.ConcurrentDictionary<K, V> dictionary;
        object dictionaryLock;
        Func<K, V> creator;

        public LazyConcurrentDictionary(Func<K, V> creator)
        {
            dictionary = new System.Collections.Concurrent.ConcurrentDictionary<K, V>();
            dictionaryLock = new object();
            this.creator = creator;
        }

        public V this[K key]
        {
            get
            {
                if (!dictionary.ContainsKey(key))
                {
                    lock (dictionaryLock)
                    {
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.TryAdd(key, creator(key));
                        }
                    }
                }

                return dictionary[key];
            }
        }

        public KeyValuePair<K, V>[] Clear()
        {
            lock (dictionaryLock)
            {
                var array = dictionary.ToArray();
                dictionary.Clear();
                return array;
            }
        }
    }
}
