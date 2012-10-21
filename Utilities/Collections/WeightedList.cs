// Copyright (c) Philip McGarvey 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    /// <summary>
    /// Represents a collection of values each with an integer weight.
    /// Provides functions for randomly selecting a value with each value's chance
    /// of being selected proportional to its weight.  Values are not removed from
    /// the list when selected.
    /// </summary>
    public class WeightedList<T>
    {
        long[] m_previousWeight;
        bool m_optimized = false;
        List<WeightedItem> m_list;
        int m_totalWeight;

        public  List<WeightedItem> List
        {
            get { return m_list; }
            set { m_list = value; }
        }

        public int Count { get { return m_list.Count; } }

        public WeightedList()
        {
            m_list = new List<WeightedItem>();
            m_totalWeight = 0;
        }

        /// <summary>
        /// Caches data about the items to improve performance of the GetRandom() method.
        /// </summary>
        public void Optimize()
        {
            if (m_optimized)
                return;
            if (m_totalWeight <= 0)
                throw new InvalidOperationException("Total weight on a weighted list must be > 0");

            m_optimized = true;
            m_previousWeight = new long[m_list.Count];
            int progress = 0;
            int k = 0;
            foreach (var item in m_list)
            {
                progress += item.Weight;
                m_previousWeight[k] = progress;
                k++;
            }
        }

        /// <summary>
        /// Adds a weighted item to the list.
        /// </summary>
        /// <param name="value">Item to add to the list</param>
        /// <param name="weight">Weight of the added item</param>
        public void Add(T item, int weight)
        {
            m_optimized = false;
            m_previousWeight = null;

            m_list.Add(new WeightedItem(item, weight));
            m_totalWeight += weight;
        }

        /// <summary>
        /// Returns a random item from the list, with each item's chance
        /// of being returned proportional to its weight.
        /// </summary>
        public T GetRandom()
        {
            int key = Rand.Int(m_totalWeight);

            if (m_optimized)
                return GetRandomFast(key);

            int progress = 0;
            foreach (var item in m_list)
            {
                progress += item.Weight;
                if (key < progress)
                    return item.Value;
            }
            return default(T);
        }

        /// <summary>
        /// Performs a binary search on the weighted items.
        /// </summary>
        T GetRandomFast(int key)
        {
            return GetRandomFast(0, m_list.Count - 1, key);
        }

        /// <summary>
        /// Performs a binary search on the weighted items between min and max.
        /// </summary>
        T GetRandomFast(int min, int max, int key)
        {
            if (min == max)
                return m_list[min].Value;

            int mid = ((max - min) / 2) + min;

            if (m_previousWeight[mid] <= key)
                return GetRandomFast(mid + 1, max, key);
            else
                return GetRandomFast(min, mid, key);
        }

        /// <summary>
        /// Represents an item and its weight.
        /// </summary>
        public struct WeightedItem
        {
            public readonly T Value;
            public readonly int Weight;
            public WeightedItem(T value, int weight)
            {
                Value = value;
                Weight = weight;
            }
        }

        static class Rand
        {
            static Random m_random = new Random();
            public static int Int(int max)
            {
                return m_random.Next() % max;
            }
        }
    }
}
