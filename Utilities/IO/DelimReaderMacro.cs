using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

namespace Utilities
{
    public class DelimReaderMacro : IStringProcessor
    {
        Dictionary<string, string> Definitions;
        char[] Delimiter;

        public DelimReaderMacro(params char[] delim)
        {
            Delimiter = delim;
        }

        public void RefreshForNewFile(string path)
        {
            Definitions = new Dictionary<string, string>();
        }

        public string ProcessString(string str)
        {
            if (str.StartsWith("#define"))
            {
                var pairs = str.Split(Delimiter).Where(s => s.Contains('=')).Select(s => s.Split('='));
                foreach (var pair in pairs)
                {
                    Definitions[pair[0]] = pair[1];
                }
                return "";
            }

            if (Definitions.Count == 0)
                return str;

            StringBuilder result = new StringBuilder();

            StringBuilder defi = new StringBuilder();

            for (int k = 0; k < str.Length; k++)
            {
                if (str[k] == '{')
                {
                    defi = new StringBuilder();
                    while (str[k + 1] != '}')
                    {
                        k++;
                        defi.Append(str[k]);
                    }
                    k++;
                    var token = defi.ToString();
                    if (Definitions.ContainsKey(token))
                        result.Append(Definitions[token]);
                    else
                    {
                        result.Append('{');
                        result.Append(token);
                        result.Append('}');
                    }
                }
                else
                    result.Append(str[k]);
            }

            return result.ToString();
        }
    }
}
