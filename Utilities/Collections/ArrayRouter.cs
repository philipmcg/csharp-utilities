using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class ArrayRouter<T> : Dictionary<string,int>
    {
        private T[] m_array;

        public ArrayRouter(T[] array)
        {
            m_array = array;
        }

        public new T this[string key]
        {
            get
            {
                return m_array[base[key]];
            }
        }
    }
}
