using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{

    public static class Functions
    {
        /// <summary>
        /// Takes a list of string/function pairs, and wraps each function with the given wrapper.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, Func<TParam, TResult>>> Wrap<TParam, TNewParam, TResult>(IEnumerable<KeyValuePair<string, Func<TNewParam, TResult>>> functions, Func<TParam, TNewParam> wrapper)
        {
            foreach (var pair in functions)
            {
                yield return new KeyValuePair<string, Func<TParam, TResult>>(pair.Key, Wrap<TParam, TNewParam, TResult>(pair.Value, wrapper));
            }
        }

        /// <summary>
        /// Wraps a function with another function that changes the parameter
        /// 
        /// Pseudocode might look like this:
        /// func wrap(func function, func wrapper)
        ///     return p => function(wrapper(p))
        /// </summary>
        public static Func<TParam, TResult> Wrap<TParam, TNewParam, TResult>(Func<TNewParam, TResult> function, Func<TParam, TNewParam> wrapper)
        {
            return p => function(wrapper(p));
        }
    }
}
