using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.GCSV
{
    public class GCSVMain
    {
        public const char InitialCharacter = '~';

        public static GCSVTable ReadFromFile(DelimReader delimReader, string filePath)
        {
            GCSVReader reader = new GCSVReader();

            List<string[]> lines = delimReader.ReadToStringArray(filePath);

            int position = 0;

            var result = reader.ReadGCSVFromLines(lines, ref position);
            if (result == null)
                throw new ArgumentException(string.Format("No Gcsv header found in input file {0}", filePath), "filePath");
            return result;
        }

        public static GCSVCollection ReadMultipleFromFile(DelimReader delimReader, string filePath)
        {
            GCSVReader reader = new GCSVReader();
            GCSVCollection collection = new GCSVCollection();

            List<string[]> lines = delimReader.ReadToStringArray(filePath);

            int position = 0;

            while (position < lines.Count)
            {
                GCSVTable single = reader.ReadGCSVFromLines(lines, ref position);
                if (single == null)
                {
                    throw new ArgumentException(string.Format("No Gcsv headers found in input file {0}", filePath), "filePath");
                }
                collection.Add(single.Name, single);
            }

            return collection;
        }

        public static void WriteToFile(string path, IEnumerable<GCSVTable> gcsvs)
        {
            GCSVWriter writer = new GCSVWriter(',');
            writer.WriteToFile(path, gcsvs);
        }

        public static void WriteToFile(string path, GCSVTable gcsv)
        {
            WriteToFile(path, new[]{gcsv});
        }

        public static IGCSVHeader CreateHeader(string[] fields)
        {
            if (fields.Length < 7)
                return new GCSVArrayHeader(fields);
            else
                return new GCSVHeader(fields);
        }

        public static GCSVTable Create(string name, string[] fields)
        {
            return new GCSVTable(name, CreateHeader(fields));
        }

        public static string CreateCSV(string[] items, char delimiter)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < items.Length; k++)
            {
                sb.Append(items[k]);
                sb.Append(delimiter);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string CreateCSV(string[] items)
        {
            return CreateCSV(items, ',');
        }
    }


}
