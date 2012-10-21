// Copyright (c) Philip McGarvey 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public interface IQueue<T>
    {
        T Dequeue();
        int NumberRemaining { get; }
    }

    /// <summary>
    /// Represents a group of ShuffledCycledLists.  Items are dequeued randomly, 
    /// with each item from the total pool of all items in all lists having
    /// an equal chance of being dequeued.
    /// </summary>
    public class ShuffledCycledListGroup<T> : IQueue<T>
    {
        ShuffledCycledList<T>[] lists;

        public ShuffledCycledListGroup(IEnumerable<ShuffledCycledList<T>> lists)
        {
            this.lists = lists.ToArray();
        }

        /// <summary>
        /// Number of items remaining before it must be reshuffled
        /// </summary>
        public int NumberRemaining 
        { 
            get 
            {
                return lists.Sum(o => o.NumberRemaining);
            }
        }

        /// <summary>
        /// Dequeues a random item from aggregate of the lists.
        /// </summary>
        /// <returns>The item dequeued</returns>
        public T Dequeue()
        {
            int sum = 0;
            int rand = Rand.Int(NumberRemaining);

            for (int k = 0; k < lists.Length; k++)
            {
                sum += lists[k].NumberRemaining;
                if (rand < sum)
                    return lists[k].Dequeue();
            }

            throw new NotSupportedException("ShuffledCycledList should never be empty.");
        }
    }

    /// <summary>
    /// Represents a queue of values that are shuffled.  After the last item has been
    /// dequeued, the list is reshuffled.
    /// </summary>
    public class ShuffledCycledList<T> : IQueue<T>
    {
        List<T> list;
        int place;

        /// <summary>
        /// Number of items remaining before it must be reshuffled
        /// </summary>
        public int NumberRemaining { get { return list.Count - place; } }

        public ShuffledCycledList(List<T> list)
        {
            SetList(list);
        }

        public void SetList(List<T> list)
        {
            place = 0;
            this.list = list.GetShuffled();
        }

        /// <summary>
        /// Dequeues a random item from the list.
        /// </summary>
        /// <returns>The item dequeued</returns>
        public T Dequeue()
        {
            T item = list[place];

            place++;

            if (place == list.Count)
                SetList(list);

            return item;
        }
    }
}
