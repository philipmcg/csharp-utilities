using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public static class WeightedListEx
    {
        public static WeightedList<T> ReadToWeightedList<T>(this DelimReader me, string file, Func<string, T> selector)
        {
            bool m = me.IgnoreLinesStartingWithDelimiter;
            me.IgnoreLinesStartingWithDelimiter = false;
            var lines = me.ReadToStringArray(file);
            var wl = new WeightedList<T>();
            foreach (var line in lines)
            {
                wl.Add(selector(line[0]), int.Parse(line[1]));
            }
            me.IgnoreLinesStartingWithDelimiter = m;
            return wl;
        }


        public static WeightedList<string> ReadToWeightedList(this DelimReader me, string file)
        {
            return me.ReadToWeightedList(file, s => s);
        }
    }
}
