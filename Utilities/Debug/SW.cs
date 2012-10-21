using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Utilities
{
    public static class SW
    {
        static Dictionary<int, Stopwatch> dict = new Dictionary<int,Stopwatch>();

        [Conditional("LOG")]
        public static void Start(int id, string message)
        {
            Console.WriteLine(message);
            if (dict.ContainsKey(id))
                dict.Remove(id);
            dict.Add(id, Stopwatch.StartNew());
        }

        [Conditional("LOG")]
        public static void Start(int id)
        {
            if (dict.ContainsKey(id))
                dict.Remove(id);

            dict.Add(id, Stopwatch.StartNew());
        }

        [Conditional("LOG")]
        public static void Add(int id)
        {
            if (!dict.ContainsKey(id))
                dict.Add(id, Stopwatch.StartNew());
            else
                dict[id].Start();
        }

        public static long Stop(int id)
        {
#if LOG
            dict[id].Stop();
            var ms = dict[id].ElapsedMilliseconds;
            Console.WriteLine(id + ": " + ms);
            return ms;
#else
            return 0;
#endif
        }
    }

    public static class SWR
    {
        static Dictionary<int, Stopwatch> dict = new Dictionary<int, Stopwatch>();

        public static void Start(int id, string message)
        {
            Console.WriteLine(message);
            if (dict.ContainsKey(id))
                dict.Remove(id);
            dict.Add(id, Stopwatch.StartNew());
        }
        public static void Start(int id)
        {
            if (dict.ContainsKey(id))
                dict.Remove(id);

            dict.Add(id, Stopwatch.StartNew());
        }
        /// <summary>
        /// Returns the number of milliseconds elapsed on the timer
        /// </summary>
        public static long Check(int id)
        {
            if (!dict.ContainsKey(id))
                Start(id);

            return dict[id].ElapsedMilliseconds;
        }

        public static void Add(int id)
        {
            if (!dict.ContainsKey(id))
                dict.Add(id, Stopwatch.StartNew());
            else
                dict[id].Start();
        }
        /// <summary>
        /// Returns the number of milliseconds elapsed on the timer
        /// </summary>
        public static long Stop(int id)
        {
            dict[id].Stop();
            var ms = dict[id].ElapsedMilliseconds;
            if(ms > 0)
                Console.WriteLine(id + ": " + ms);
            return ms;
        }
    }
}
