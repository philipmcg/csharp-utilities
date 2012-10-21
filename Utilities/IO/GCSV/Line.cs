using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;


namespace Utilities.GCSV
{


    /// <summary>
    /// Represents a delimited line of a GCSV.
    /// </summary>
    public class GCSVLine : IGCSVLine
    {
        private IGCSVHeader m_header;

        public IGCSVHeader Header
        {
            get { return m_header; }
        }

        protected string[] m_values;

        public int Length
        {
            get
            {
                return m_values.Length;
            }
        }

        /// <summary>
        /// Returns the inner field array.
        /// </summary>
        public string[] FieldArray
        {
            get
            {
                return m_values;
            }
        }

        /// <summary>
        /// Copy this GCSVLine to a new format with the given header.
        /// Will fill the empty slots with the default values from
        /// the given header.
        /// </summary>
        public GCSVLine ChangeHeader(IGCSVHeader newHeader)
        {
            GCSVLine newLine = new GCSVLine(newHeader);
            foreach (var column in newHeader.Keys)
            {
                string key = column;
                if (this.m_header.ContainsKey(key))
                    newLine[key] = this[key];
                else
                    newLine[key] = "";
            }
            return newLine;
        }

        /// <summary>
        /// Creates a GCSVLine where each field is the empty string.
        /// </summary>
        public GCSVLine(IGCSVHeader header)
        {
            m_header = header;
            // Initialize the array to empty strings
            m_values = Enumerable.Range(0, header.Length).Select(i => string.Empty).ToArray();
        }

        /// <summary>
        /// Creates a GCSVLine with the given fields.
        /// </summary>
        public GCSVLine(IGCSVHeader header, string[] fields)
        {
            m_header = header;

            if (fields.Length != m_header.Length)
            {
                m_values = new string[m_header.Length];
                Array.Copy(fields, m_values, fields.Length);
                for (int i = fields.Length; i < m_header.Length; i++)
                {
                    m_values[i] = string.Empty;
                }
            }
            else
            {
                m_values = fields;
            }

        }

        public bool TryGetValue(string key, out string value)
        {
            int index = 0;
            if (m_header.TryGetValue(key, out index))
            {
                value = m_values[index];
                if (value != string.Empty)
                    return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Returns true if the given field has a non-empty value.
        /// </summary>
        public bool ContainsKey(string key)
        {
            if (m_header.ContainsKey(key))
            {
                int index = m_header[key];
                if (m_values[index] != string.Empty)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the value of the given field.
        /// </summary>
        public string this[string key]
        {
            get
            {
                if (m_header.ContainsKey(key))
                    return m_values[m_header[key]];
                else
                    return string.Empty;
            }
            set
            {
                m_values[m_header[key]] = value;
            }
        }

        /// <summary>
        /// Packs the GCSVLine into a delimited string with the given delimiter.
        /// </summary>
        public string ToString(char delimiter)
        {
            return GCSVMain.CreateCSV(m_values, delimiter);
        }

        /// <summary>
        /// Packs the GCSVLine into a comma-delimited string.
        /// </summary>
        public override string ToString()
        {
            return GCSVMain.CreateCSV(m_values);
        }
    }
}
