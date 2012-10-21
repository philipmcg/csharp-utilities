using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.GCSV
{
    public class GCSVTable : List<GCSVLine>, GCSVTable<GCSVLine>
    {
        public string Name { get; private set; }

        private Dictionary<string, GCSVLine> HashTable;

        private IGCSVHeader m_header;

        public IGCSVHeader Header
        {
            get
            {
                return m_header;
            }
        }

        public string HashTableKey { get; private set;}

        public new void Add(GCSVLine line)
        {
            if(HashTable != null)
                HashTable.Add(line[HashTableKey], line);

            base.Add(line);
        }

        public GCSVLine this[string key]
        {
            get 
            {
                if (HashTable == null)
                    StartHashTable();

                return HashTable[key]; 
            }
        }

        public bool ContainsKey(string key)
        {
            if (HashTable == null)
                StartHashTable();

            return HashTable.ContainsKey(key);
        }

        private void StartHashTable()
        {
            HashTableKey = m_header.FirstField;
            HashTable = new Dictionary<string, GCSVLine>();
            foreach (var line in this)
            {
                HashTable.Add(line[HashTableKey], line);
            }
        }

        public GCSVTable(string name, IGCSVHeader header)
        {
            Name = name;
            m_header = header;
        }
    }


}
