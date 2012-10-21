using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class ReaderSplitterEx
    {
        public static Dictionary<string, ShuffledCycledList<string>> ConvertToShuffledLists(this ReaderSplitter me)
        {
            var Map = new Dictionary<string, ShuffledCycledList<string>>();
            foreach (var item in me.Sections)
            {
                Map.Add(item.Key, new ShuffledCycledList<string>(item.Value));
            }

            return Map;
        }
    }

    /// <summary>
    /// Splits the strings into separate lists starting at bracketed identifiers such as "[Identifier]"
    /// </summary>
    public class ReaderSplitter
    {
        public Dictionary<string, List<string>> Sections;

        List<string> CurrentSection;
        string CurrentSectionIdentifier;

        public bool RefreshForFile(string path)
        {
            Sections = new Dictionary<string, List<string>>();
            return true;
        }

        public string ProcessString(string str)
        {
            string section = IsNewSection(str);
            if (section == "")
                CurrentSection.Add(str);
            else
            {
                if (CurrentSection != null)
                    Sections.Add(CurrentSectionIdentifier, CurrentSection);

                CurrentSectionIdentifier = section;

                if (Sections.ContainsKey(section))
                    CurrentSection = Sections[section];
                else
                    CurrentSection = new List<string>();
            }
            return str;
        }

        private string IsNewSection(string str)
        {
            if (str[0] == '[')
                return str.Substring(1, str.Length - 2);
            else return "";
        }
    }
}
