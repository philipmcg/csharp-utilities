using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Shorthand for Dictionary<string, string>
    /// </summary>
    public class Dict : Dictionary<string, string> 
    {
        /// <summary>
        /// Create a new associative array.  There should be an even number of arguments, alternating key/value.
        /// </summary>
        public static Dict Create(params object[] p)
        {
            if (p.Length < 2)
                return null;

            var d = new Dict();
            for (int k = 0; (k + 1) < p.Length; k += 2)
            {
                d.Add(p[k].ToString(), p[k + 1].ToString());
            }
            return d;
        }
    }

    /// <summary>
    /// List of associative arrays of strings.  
    /// Useful for quickly putting together associative arrays and searching or accessing them.
    /// </summary>
    public class Dicts : List<Dict>
    {
        /// <summary>
        /// Start a new list of dicts with an optional first item
        /// </summary>
        public static Dicts Start(params string[] p)
        {
            Dicts d = new Dicts();
            if (p.Length >= 2)
                d.Add(p);
            return d;
        }

        /// <summary>
        /// Add a new associative array to the list
        /// </summary>
        public Dict Add(params string[] p)
        {
            var d = Dict.Create(p);
            base.Add(d);
            return d;
        }

        /// <summary>
        /// Get the first associative array in the list that has a pair with key = entry and value = match
        /// </summary>
        public Dict this[string entry, string match]
        {
            get
            {
                return this.FirstOrDefault(d => d.ContainsKey(entry) && d[entry] == match);
            }
        }
    }

    
    public static class DictsExt
    {
        public static Dicts ToDicts(this List<Dict> me)
        {
            return (Dicts)me;
        }
    }
}
