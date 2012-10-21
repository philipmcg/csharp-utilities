using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilities.GCSV
{
    class GCSVWriter
    {
        char m_delimiter;
        public GCSVWriter(char delimiter)
        {
            m_delimiter = delimiter;
        }

        public void WriteToFile(string path, GCSVTable gcsv)
        {
            WriteToFile(path, new[] { gcsv });
        }

        public void WriteToFile(string path, IEnumerable<GCSVTable> gcsvs)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                foreach (var gcsv in gcsvs)
                {
                    WriteToStream(stream, gcsv);
                }
            }
        }

        void WriteToStream(StreamWriter writer, GCSVTable gcsv)
        {
            WriteHeaderLine(writer, gcsv);
            foreach (var line in gcsv)
            {
                writer.WriteLine(line.ToString(m_delimiter));
            }
        }

        void WriteHeaderLine(StreamWriter writer, GCSVTable gcsv)
        {
            writer.WriteLine(GCSVMain.InitialCharacter + gcsv.Name + Environment.NewLine + gcsv.Header.Keys.Implode(m_delimiter));
        }
    }
}
