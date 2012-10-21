using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Data;

namespace Utilities
{
    public class WebServer
    {
        public string URL;
        WebClient WebClient = new WebClient();

        /// <summary>
        /// Returns "URL/path"
        /// </summary>
        public string SubURL(string path)
        {
            return Path.Combine(URL, path);
        }

        /// <summary>
        /// requests "URL/suburl" + arguments, returning a stream
        /// </summary>
        public StreamReader GetRequestToStream(string suburl, string arguments)
        {
            return Utilities.Web.GetRequestToStream(SubURL(suburl) + arguments);
        }

        /// <summary>
        /// requests "URL/suburl" + arguments, returning a string
        /// </summary>
        public string GetRequestToString(string suburl, string arguments)
        {
            return Utilities.Web.GetRequestToString(SubURL(suburl) + arguments);
        }

        /// <summary>
        /// composes a php argument _GET string
        /// for example, CreateArguments("a", 1, "b", 2) returns "?a=1&b=2"
        /// </summary>
        public static string CreateArguments(params object[] p)
        {
            if (p.Length == 0)
                return string.Empty;

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
        /// Launches "URL/suburl" in the client's internet browser
        /// </summary>
        public string LaunchURL(string suburl)
        {
            try
            {
                var full = SubURL(suburl);
                System.Diagnostics.Process.Start(full);
                return full;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                System.Windows.Forms.MessageBox.Show("Could not locate your Internet Browser.", "Browser not found");
                return "error";
            }
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public void DownloadFile(string subUrl, string destinationPath)
        {
            WebClient.Proxy = new WebProxy();
            WebClient.Credentials = null;

            string fullOnlineUrl = Path.Combine(URL, subUrl);
            try
            {
                WebClient.DownloadFile(fullOnlineUrl, destinationPath);
            }
            catch (WebException ex)
            {
                var response = ex.Response;
                throw;
            }
        }
    }
}
