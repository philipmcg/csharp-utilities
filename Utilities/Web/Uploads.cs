using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;

using System.Net;

using System.IO;

namespace Utilities
{
    public class Uploads
    {
        /// <summary>
        /// Temp directory to use, default "temp"
        /// </summary>
        public string TempDir = "temp";

        /// <summary>
        /// Temp zip file to use, default "temp.zip"
        /// </summary>
        public string TempZipFile = "temp.zip";
        public string UploadURL = "upload.php";
        public string FilesManagerURL = "files_manager.php";
        public string BaseURL;

        public Uploads(string BaseURL, string UploadURL, string TempZipFile, string TempDir, string FilesManagerURL)
        {
            this.BaseURL = BaseURL;
            this.UploadURL = UploadURL;
            this.TempZipFile = TempZipFile;
            this.TempDir = TempDir;
            this.FilesManagerURL = FilesManagerURL;
        }

        /// <summary>
        /// composes a php argument _GET string
        /// for example, CreateArguments("a", 1, "b", 2) returns "?a=1&b=2"
        /// </summary>
        static string CreateArguments(params object[] p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("?");
            for (int k = 0; k < p.Length; k += 2)
            {
                sb.Append(p[k]);
                sb.Append('=');
                sb.Append(p[k + 1]);
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        /// <summary>
        /// Slower than UploadFiles, but doesn't use zipping
        /// </summary>
        public void UploadFileUnzip(string localSource, string onlineDest)
        {
            var args = CreateArguments
                (
                    "unzip", "0"
                    , "dest", onlineDest
                );

            var url = Path.Combine(BaseURL, UploadURL + args);

            System.Net.WebClient Client = new System.Net.WebClient();
            byte[] result = Client.UploadFile(url, "POST", localSource);
            string s = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
        }


    }
}
