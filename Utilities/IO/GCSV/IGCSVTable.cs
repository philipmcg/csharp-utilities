using System;
using System.Collections.Generic;
namespace Utilities.GCSV
{
    public interface GCSVTable<T> : IList<T> where T : IGCSVLine
    {
        IGCSVHeader Header { get; }
        string Name { get; }
    }
}
