// ArraySet.cs
// Copyright (c) Philip McGarvey 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public interface IIndexed
    {
        int ID { get; set; }
    }

    public class IndexedArraySet<T> : ArraySet<T> where T : class, IIndexed
    {
        public new void Add(T item)
        {
            base.Add(item);
            item.ID = base.m_lastAddedIndex;
        }
    }

     //From the benchmarks I did, accessing by key was 1/3 faster than on
     //a Dictionary<int, T>.  Iterating with foreach was also slightly
     //faster.

     //Inserting can be slightly slower than with a Dictionary, depending
     //on the key variance.  If the keys are mostly clustered closest to
     //0, inserting will be as fast as a Dictionary.  If there are large
     //empty spaces in the array, inserting will be slower, as the unused
     //indices are stored in a hashset.

     //The purpose is for when we want to have a collection where each
     //item has an index, and we do extremely frequent accessing by
     //index, and when we do remove and add new items regularly.  By
     //inserting new items in unused space early in the array, we can
     //keep the array small, without having to resize it, and keep the
     //accessing speed as fast as an array.

    /// <summary>
    /// Represents a collection of zero-based, integer-indexed objects 
    /// with decent performance for inserting, removing, accessing by index and iterating.
    /// </summary>
    /// <typeparam name="T">The type of object in the collection</typeparam>
    public class ArraySet<T> : ICollection<T>, IEnumerable<T> where T : class
    {
        /// <summary>
        /// The actual array.
        /// </summary>
        T[] m_innerArray;

        /// <summary>
        /// The capacity of the current array.
        /// </summary>
        int m_capacity;

        /// <summary>
        /// The length of the array that has been used.  The maximum possible index used is m_length - 1.
        /// </summary>
        int m_length;

        /// <summary>
        /// A stack containing all indices that are unused, and less than m_length.
        /// </summary>
        HashSet<int> m_unusedIndices;

        /// <summary>
        /// The number of non-null elements in the array.
        /// </summary>
        int m_count;

        /// <summary>
        /// The index of the last item added with the Add or Insert methods.
        /// </summary>
        protected int m_lastAddedIndex;

        public ArraySet()
            : this(8)
        {
        }

        public ArraySet(int expectedCapacity)
        {
            m_capacity = expectedCapacity;
            m_innerArray = new T[m_capacity];
            m_unusedIndices = new HashSet<int>();
        }

        /// <summary>
        /// Doubles the size of m_innerArray.
        /// </summary>
        private void Reallocate()
        {
            int newCapacity = m_capacity * 2;
            T[] newArray = new T[newCapacity];
            Array.Copy(m_innerArray, newArray, m_length);

            m_capacity = newCapacity;
            m_innerArray = newArray;
        }

        public bool ContainsKey(int index)
        {
            if (index >= m_length)
                return false;
            else if (m_innerArray[index] == null)
                return false;
            else
                return true;
        }

        public bool TryGetValue(int key, out T value)
        {
            if (key >= m_length)
            {
                value = null;
                return false;
            }
            else
            {
                value = m_innerArray[key];
                return m_innerArray != null;
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            for (int i = 0; i < m_length; i++)
            {
                if (item.Equals(m_innerArray[i]))
                    return i; 
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            while (index >= m_capacity)
            {
                Reallocate();
            }

            if (m_innerArray[index] == null)
            {
                m_innerArray[index] = item;
                m_count++;
            }
            else
            {
                throw new ArgumentException("Index already in use by ArraySet.");
            }

            for (int i = m_length; i < index; i++)
            {
                m_unusedIndices.Add(i);
            }

            if (index >= m_length)
                m_length = index + 1;

            m_lastAddedIndex = index;
            m_unusedIndices.Remove(index);
        }

        public void RemoveAt(int index)
        {
            if (index >= m_capacity)
            {
                throw new ArgumentOutOfRangeException("Index out of range of ArraySet.");
            }

            if (m_innerArray[index] == null)
            {
                throw new ArgumentException("There is no item with this index in ArraySet.");
            }

            m_innerArray[index] = null;
            m_count--;

            if (index < m_length)
                m_unusedIndices.Remove(index);
        }

        public T this[int index]
        {
            get
            {
                return m_innerArray[index];
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            int itemIndex = 0;
            if (m_unusedIndices.Count != 0)
            {
                itemIndex = m_unusedIndices.First();
                m_unusedIndices.Remove(itemIndex);
            }
            else
            {
                itemIndex = m_length;

                if (m_length == m_capacity)
                    Reallocate();

                m_length++;
            }

            this.Insert(itemIndex, item);
        }

        public void Clear()
        {
            Array.Clear(m_innerArray, 0, m_length);
            m_length = 0;
            m_unusedIndices.Clear();
            m_count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < m_length; i++)
            {
                if (item.Equals(m_innerArray[i]))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] dest, int arrayIndex)
        {
            if (dest == null)
                throw new ArgumentNullException("dest");
            if (dest.Rank != 1)
                throw new ArgumentException("dest", "Destination array cannot be multidimensional.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "Number was less than the array's lower bound in the first dimension.");
            if (arrayIndex >= dest.Length)
                throw new ArgumentException("arrayIndex", "Index cannot be greater than length of destination array.");
            if (m_count > dest.Length - arrayIndex)
                throw new ArgumentException("dest", "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");

            int k = arrayIndex;
            for (int i = 0; i < m_length; i++)
            {
                if (m_innerArray[i] != null)
                    dest[k++] = m_innerArray[i];
            }
        }

        public int Count
        {
            get { return m_count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if the item was removed
        /// </summary>
        public bool Remove(T item)
        {
            int index = this.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            else
            {
                this.RemoveAt(index);
                return true;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m_length; i++)
            {
                if (m_innerArray[i] != null)
                    yield return m_innerArray[i];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

}
