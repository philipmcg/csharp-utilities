using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.GCSV
{

    /// <summary>
    /// Represents the delimited header line of a GCSV.
    /// </summary>
    public class GCSVArrayHeader : IGCSVHeader
    {
        string[] m_fields;

        public string FirstField { get; private set; }

        public GCSVArrayHeader(string[] fields)
        {
            m_fields = new string[fields.Length];
            Array.Copy(fields, m_fields, fields.Length);
            FirstField = fields[0];
        }

        public int this[string key]
        {
            get
            {
                for (int i = 0; i < m_fields.Length; i++)
                {
                    if (key == m_fields[i])
                        return i;
                }
                throw new ArgumentException("Key not found in GCSVArrayHeader", "key");
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return m_fields;
            }
        }

        public bool TryGetValue(string key, out int value)
        {
            for (int i = 0; i < m_fields.Length; i++)
            {
                if (key == m_fields[i])
                {
                    value = i;
                    return true;
                }
            }
            value = 0;
            return false;
        }


        public bool ContainsKey(string key)
        {
            for (int i = 0; i < m_fields.Length; i++)
            {
                if (key == m_fields[i])
                    return true;
            }
            return false;
        }

        public int Length
        {
            get
            {
                return m_fields.Length;
            }
        }

        /// <summary>
        /// Returns true if the two GCSVHeaders have all the same fields.  
        /// The fields can be in different order.
        /// </summary>
        public override bool Equals(object obj)
        {
            IGCSVHeader other = obj as IGCSVHeader;

            foreach (var field in m_fields)
            {
                if (!other.ContainsKey(field))
                    return false;
            }

            return other.Length == Length;
        }

        public override int GetHashCode()
        {
            StringBuilder fields = new StringBuilder();
            m_fields.OrderBy(s => s).ForEach(s => fields.Append(s));
            return fields.ToString().GetHashCode();
        }

        /// <summary>
        /// Packs the GCSVArrayHeader into a delimited string with the given delimiter.
        /// </summary>
        public string ToString(char delimiter)
        {
            return GCSVMain.CreateCSV(m_fields, delimiter);
        }

        /// <summary>
        /// Packs the GCSVArrayHeader into a comma-delimited string.
        /// </summary>
        public override string ToString()
        {
            return GCSVMain.CreateCSV(m_fields);
        }
    }


    /// <summary>
    /// Represents the delimited header line of a GCSV.
    /// </summary>
    public class GCSVHeader : IGCSVHeader
    {
        Dictionary<string, int> m_fields;

        public string FirstField { get; private set; }

        public GCSVHeader(string[] fields)
        {
            m_fields = new Dictionary<string, int>(fields.Length);
            FirstField = fields[0];
            for (int i = 0; i < fields.Length; i++)
            {
                m_fields.Add(fields[i], i);
            }
        }

        public int this[string key]
        {
            get
            {
                return m_fields[key];
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return m_fields.Keys;
            }
        }

        public bool TryGetValue(string key, out int value)
        {
            return m_fields.TryGetValue(key, out value);
        }

        public bool ContainsKey(string key)
        {
            return m_fields.ContainsKey(key);
        }

        public int Length
        {
            get
            {
                return m_fields.Count;
            }
        }

        /// <summary>
        /// Returns true if the two GCSVHeaders have all the same fields.  
        /// The fields can be in different order.
        /// </summary>
        public override bool Equals(object obj)
        {
            IGCSVHeader other = obj as IGCSVHeader;

            foreach (var field in m_fields.Keys)
            {
                if (!other.ContainsKey(field))
                    return false;
            }

            return other.Length == Length;
        }

        public override int GetHashCode()
        {
            StringBuilder fields = new StringBuilder();
            m_fields.OrderBy(s => s.Value).ForEach(s => fields.Append(s));
            return fields.ToString().GetHashCode();
        }

        /// <summary>
        /// Packs the GCSVHeader into a delimited string with the given delimiter.
        /// </summary>
        public string ToString(char delimiter)
        {
            return GCSVMain.CreateCSV(m_fields.Keys.ToArray(), delimiter);
        }

        /// <summary>
        /// Packs the GCSVHeader into a comma-delimited string.
        /// </summary>
        public override string ToString()
        {
            return GCSVMain.CreateCSV(m_fields.Keys.ToArray());
        }
    }
}
