using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.GCSV
{

    public interface IGCSVCollection : IDictionary<string, GCSVTable>
    {
    }
    public class GCSVCollection : Dictionary<string, GCSVTable>, IGCSVCollection
    {
        public Dictionary<string, IGCSVHeader> Headers
        {
            get
            {
                return this.ToDictionary(p => p.Key, p => p.Value.Header);
            }
        }
    }

}
