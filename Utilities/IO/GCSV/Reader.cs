using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.GCSV
{

    class GCSVReader
    {
        private GCSVTable m_gcsv;
        private string m_name;
        private string[] m_headerLine;
        private IGCSVHeader m_header;
        private bool m_started;

        public GCSVReader()
        {
        }

        private void StartGCSV()
        {
            m_headerLine = m_headerLine.Where(key => key != string.Empty).ToArray();
            m_header = GCSVMain.CreateHeader(m_headerLine);
            m_gcsv = new GCSVTable(m_name, m_header);
            m_started = true;
            m_headerLine = null;
            m_name = null;
        }

        public GCSVTable ReadGCSVFromLines(List<string[]> lines, ref int position)
        {
            m_gcsv = null;
            m_name = null;
            m_headerLine = null;
            m_started = false;

            for (; position < lines.Count; position++)
            {
                string[] line = lines[position];

                // If this is the start of a gcsv
                if (line[0][0] == GCSVMain.InitialCharacter)
                {
                    // If we had already started a gcsv, this must be the next one,
                    // so we return the one we were filling up.
                    if (m_started == true)
                        return m_gcsv;

                    m_name = line[0].Substring(1);

                    // If the gcsv header is defined on the same line as the name
                    // the line will have multiple delimited values and the second
                    // value, which is the first field name in the header, will
                    // not be empty.
                    if (line.Length > 1 && !string.IsNullOrEmpty(line[1]))
                    {
                        m_headerLine = new string[line.Length - 1];
                        Array.Copy(line, 1, m_headerLine, 0, line.Length - 1);
                        StartGCSV();
                    }
                    continue;
                }

                // If we've started the gcsv, we can add this line to it.
                if (m_started)
                {
                    // If the line is not long enough, create a new line 
                    // with empty strings to pad the end.
                    if (line.Length != m_header.Length)
                    {
                        string[] newLine = new string[m_header.Length];
                        Array.Copy(line, newLine, Math.Min(newLine.Length, line.Length));
                        for (int i = line.Length; i < newLine.Length; i++)
                        {
                            newLine[i] = string.Empty;
                        }
                        line = newLine;
                    }

                    m_gcsv.Add(new GCSVLine(m_header, line));

                }
                else
                {
                    // If we've read the name of the gcsv, but not the header
                    // then this line is the header.
                    if (m_name != null)
                    {
                        m_headerLine = line;
                        StartGCSV();
                    }
                }
            }

            GCSVTable result = m_gcsv;
            m_gcsv = null;
            return result;
        }
    }
}
