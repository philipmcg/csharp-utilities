using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Utilities.GCSV
{

    public class GCSVManager : DataFileManager<GCSVTable>, IGCSVCollection
    {
        DelimReader Reader;
        object LoaderLock = new object();

        public GCSVManager(DelimReader reader, string directory)
            : base(directory)
        {
            Reader = reader;

            base.LoadFile = OverrideLoadFile;
        }
        protected GCSVTable OverrideLoadFile(string directory, string key)
        {
            string path = Path.Combine(directory, key + ".csv");

            LoadMasterFileWithPath(path);
            if(!base.ContainsKey(key))
                throw new ArgumentException("GCSV File named " + key + " does not contain GCSV with name " + key);
            return base[key];
        }

        public void LoadMasterFile(string key)
        {
            lock (LoaderLock)
            {
                string path = Path.Combine(Directory, key + ".csv");
                LoadMasterFileWithPath(path);
            }
        }

        public void LoadMasterFileWithPath(string path)
        {
            lock (LoaderLock)
            {
                GCSVCollection gcsvs = GCSVMain.ReadMultipleFromFile(Reader, path);
                foreach (var gcsv in gcsvs)
                {
                    if(!base.ContainsKey(gcsv.Key))
                        base.Add(gcsv.Key, gcsv.Value);
                }
            }
        }

        public IGCSVCollection AsCollection()
        
        {
            return this;
        }
    }
}
