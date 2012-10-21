using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.GCSV
{

    public class IndexedGCSVTable : GCSVTable
    {
        private string m_indexKey;

        private Dictionary<string, int> m_index;

        public IndexedGCSVTable(string name, IGCSVHeader header, string mappingKey)
            : base(name, header)
        {
            m_index = new Dictionary<string, int>();
            m_indexKey = mappingKey;
        }

        public IndexedGCSVTable(string name, IGCSVHeader header)
            : base(name, header)
        {
            m_index = new Dictionary<string, int>();
            m_indexKey = header.FirstField;
        }

        /// <summary>
        /// Adds the line to the list, and adds it to the index.
        /// </summary>
        public new void Add(GCSVLine line)
        {
            base.Add(line);
            if (!m_index.ContainsKey(line[m_indexKey]))
                m_index.Add(line[m_indexKey], base.Count - 1);
        }

    }
}
