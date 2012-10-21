using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;

using Utilities;


namespace Utilities.MSWebInterface
{
    public interface IResultSet : IData
    {
        string Name { get; }

        bool ContainsSet(string setKey);
        IResultSet GetSet(string setKey);

        IEnumerable<IResultSet> InnerSets { get; }
        IEnumerable<KeyValuePair<string,string>> Results { get; }
    }

    class ResultSet : IResultSet
    {
        public string Name { get; private set; }
        Dictionary<string, string> values;
        Dictionary<string, ResultSet> innerSets;

        public ResultSet(string xml) : this(XElement.Parse(xml))
        {
        }

        public ResultSet(XElement element)
        {
            values = new Dictionary<string, string>();
            innerSets = new Dictionary<string, ResultSet>();
            Name = element.Name.LocalName;

            foreach (var childElement in element.Elements())
            {
                if(!string.IsNullOrEmpty(childElement.Value))
                    values.Add(childElement.Name.LocalName, childElement.Value);

                if (childElement.Elements().Any())
                    innerSets.Add(childElement.Name.LocalName, new ResultSet(childElement));
            }
        }
    
        public bool ContainsKey(string key)
        {
 	        return values.ContainsKey(key);
        }

        public string this[string key]
        {
	        get { return values[key]; }
        }

        public bool ContainsSet(string setKey)
        {
            return innerSets.ContainsKey(setKey);
        }

        public IResultSet GetSet(string setKey)
        {
            return innerSets[setKey];
        }

        public IEnumerable<IResultSet> InnerSets
        {
            get 
            {
                foreach (var set in innerSets.Values)
                {
                    yield return set;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Results
        {
            get { return values; }
        }
    }


}
