using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public class Tuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public Tuple(T1 i1, T2 i2, T3 i3)
        {
            Item1 = i1;
            Item2 = i2;
            Item3 = i3;
        }
        public Tuple()
        {

        }
    }
    
    public struct Tuple3<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public Tuple3(T1 i1, T2 i2, T3 i3)
        {
            Item1 = i1;
            Item2 = i2;
            Item3 = i3;
        }
    }
}
