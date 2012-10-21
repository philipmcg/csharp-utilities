using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class CollectionsExt
    {
        public static IEnumerable<T> Valid<T>(this IEnumerable<T> source)
        {
            return source.Where(n => n != null);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, TValue def)
        {
            TValue val;
            bool found = me.TryGetValue(key, out val);
            return found ? val : def;
        }

        /// <summary>
        /// Reverses the order of elements in the stack
        /// </summary>
        public static Stack<T> FlipStack<T>(this Stack<T> me)
        {
            if (me == null)
                return null;

            Stack<T> copy = new Stack<T>(me.Count);
            while (me.Count > 0)
            {
                copy.Push(me.Pop());
            }
            return copy;
        }

        public static Stack<T> Clone<T>(this Stack<T> stack)
        {
            return new Stack<T>(new Stack<T>(stack));
        }


        public static IEnumerable<T> Include<T>(this IEnumerable<T> me, params T[] others)
        {
            return me.Union(others);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            foreach (T i in source)
                yield return i;

            yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
        {
            yield return item;

            foreach (T i in source)
                yield return i;
        }


        public static IEnumerable<KeyValuePair<T, T>> GetPairs<T>(this T[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                for (int k = i + 1; k < list.Length; k++)
                {
                    yield return new KeyValuePair<T, T>(list[i], list[k]);
                }
            }
        }

        public static IEnumerable<KeyValuePair<T, T>> GetPairs<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int k = i + 1; k < list.Count; k++)
                {
                    yield return new KeyValuePair<T, T>(list[i], list[k]);
                }
            }
        }

        public static KeyValuePair<T, T> GetRandomPair<T>(this IList<T> list)
        {
            var a = list.GetRandom();
            var b = list.GetRandom();
            return new KeyValuePair<T,T>(a, b);
        }

        public static int IndexOf<T>(this T[] me, T item) where T : IComparable
        {
            for (int k = 0; k < me.Length; k++)
            {
                if (me[k].Equals(item))
                    return k;
            }
            return -1;
        }

        public static void FillArrayRandomly<T, T2>(T[] array, WeightedList<T2> weightedList, bool allUnique, Func<T2, T> getItem)
        {
            if (allUnique && array.Length > weightedList.Count)
                throw new ArgumentException("(array.Length > weightedList.Count):  If the array must be filled with unique values, there must not be more space in the array than there are values to choose from.");
            
            T item;
            for (int k = 0; k < array.Length; k++)
            {
                if (allUnique)
                {
                    do
                    {
                        item = getItem(weightedList.GetRandom());
                    }
                    while (array.Contains(item));
                }
                else
                    item = getItem(weightedList.GetRandom());

                array[k] = item;
            }
        }


        public static string ToCSV<T>(this IEnumerable<T> me,char delimiter)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in me)
            {
                if(item != null)
                    sb.Append(item);
                sb.Append(delimiter);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
