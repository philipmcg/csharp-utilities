using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public interface IProvider<K, V>
    {
        bool ContainsKey(K key);
        V this[K key] { get; }
    }

    public class Provider<V> : IProvider<int, V> where V : struct, IEquatable<V>
    {
        public readonly V[] Array;
        V defaultValue;
        Func<int, V> getValue;

        public Provider(int capacity, Func<int, V> getValue, V defaultValue)
        {
            Array = new V[capacity];
            this.defaultValue = defaultValue;
            this.getValue = getValue;
        }

        public bool ContainsKey(int key)
        {
            if (key >= Array.Length)
                return false;

            return !Array[key].Equals(defaultValue);
        }

        public V this[int key]
        {
            get
            {
                if (key >= Array.Length)
                    return getValue(key);

                if (Array[key].Equals(defaultValue))
                    Array[key] = getValue(key);
                return Array[key];
            }
        }
    }
}
