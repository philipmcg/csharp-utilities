using System;
namespace Utilities
{
    public interface IData
    {
        bool ContainsKey(string key);
        string this[string key] { get; }
    }
}

namespace Utilities.GCSV
{
    public interface IGCSVLine : IData
    {
        bool TryGetValue(string key, out string value);
        string[] FieldArray { get; }
        IGCSVHeader Header { get; }
        int Length { get; }
        new string this[string key] { get; set; }
        string ToString();
        string ToString(char delimiter);
    }
}
