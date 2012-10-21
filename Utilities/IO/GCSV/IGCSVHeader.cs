using System;
namespace Utilities.GCSV
{
    public interface IGCSVHeader
    {
        bool TryGetValue(string key, out int index);
        bool ContainsKey(string key);
        bool Equals(object obj);
        string FirstField { get; }
        int GetHashCode();
        System.Collections.Generic.IEnumerable<string> Keys { get; }
        int Length { get; }
        int this[string key] { get; }
        string ToString();
        string ToString(char delimiter);
    }
}
