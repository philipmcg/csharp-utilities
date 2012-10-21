using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.Net;
using System.IO;

namespace Utilities
{
    public class Web
    {
#if DEBUG
        const int Timeout = 1000000000;
#endif

        static WebProxy EmptyProxy = new WebProxy();

        public static void GetRequestAsync(string url)
        {
            WebClient client = new WebClient();
            client.Proxy = EmptyProxy;
            client.DownloadStringAsync(new Uri(url));
        }

        static HttpWebRequest CreateRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
#if DEBUG
            request.Timeout = Timeout;
#endif
            return request;
        }

        /// <summary>
        /// Gets an http request to StreamReader 
        /// </summary>
        public static StreamReader GetRequestToStream(string url)
        {

            HttpWebRequest request = CreateRequest(url);

            // Disable the automatic proxy detection.  Without this, sometimes the first request took ~15 seconds.
            request.Proxy = EmptyProxy;
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader input = new StreamReader(response.GetResponseStream());
            return input;
        }

        /// <summary>
        /// Gets an http request and saves the content to a local file.
        /// </summary>
        public static void DownloadRequestToFile(string url, string localFile)
        {
            using (StreamReader reader = GetRequestToStream(url))
            {
                using (Stream stream = File.Open(localFile, FileMode.Create, FileAccess.Write))
                {
                    CopyStream(reader.BaseStream, stream);
                }
            }
        }

        static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, read);
        }

        /// <summary>
        /// Gets an http request to string
        /// </summary>
        public static string GetRequestToString(string url)
        {
            StreamReader streamReader = GetRequestToStream(url);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }

        /// <summary>
        /// Gets an http request to string
        /// </summary>
        public static StreamReader GetPostRequestToStream(string url, IEnumerable<byte[]> data)
        {
            url = url.ToLower();  // I was getting 404s on post requests when the url was capitalized..

            HttpWebRequest request = CreateRequest(url);
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ContentLength = data.Sum(d => d.Length);
            request.Proxy = new WebProxy();

            using (Stream dataStream = request.GetRequestStream())
            {
                foreach (var array in data)
                {
                    dataStream.Write(array, 0, array.Length);
                }
            }
            WebResponse response = request.GetResponse();
            StreamReader input = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            return input;
        }

        /// <summary>
        /// Gets an http request to string
        /// </summary>
        public static StreamReader GetPostRequestToStream(string url, string data)
        {
            return GetPostRequestToStream(url, new[]{Encoding.UTF8.GetBytes(data)});
        }

        /// <summary>
        /// Gets an http request to string
        /// </summary>
        public static string GetPostRequestToString(string url, string data)
        {
            StreamReader streamReader = GetPostRequestToStream(url, data);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }

        /// <summary>
        /// Gets an http request to string
        /// </summary>
        public static string GetPostRequestToString(string url, IEnumerable<byte[]> data)
        {
            StreamReader streamReader = GetPostRequestToStream(url, data);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }
    }
}
