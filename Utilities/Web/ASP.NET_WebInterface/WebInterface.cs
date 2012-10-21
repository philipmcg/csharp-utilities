using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;


using System.Threading;

namespace Utilities.MSWebInterface
{

    public class WebInterface
    {
        protected int Timeout;

        public string Url { get; private set; }

        XmlObjectSerializer Serializer;


        protected AsyncWebInterface AsyncInterface;

        public WebInterface(string url)
        {
            this.Url = url;
            this.Serializer = new XmlObjectSerializer();
            AsyncInterface = new AsyncWebInterface(this, 30);
        }

        public virtual void UploadFile(string localFile, string remoteFile)
        {
            SendFiles("File", "PostFiles", new[] { new KeyValuePair<string, string>(localFile, remoteFile) });
        }


        public virtual void DownloadFile(string remoteFile, string localFile)
        {
            DownloadFile("File", "Download", localFile, "file", remoteFile);
        }


        public virtual string GetUrl(string controller, string request, params object[] arguments)
        {
            string args = WebServer.CreateArguments(arguments);
            return Url + controller + "/" + request + "/" + args;
        }

        public virtual TResult RequestStruct<TResult, TPass>(string controller, string request, TPass pass)
        {
            string requestUrl = GetUrl(controller, request);

            var str = Web.GetPostRequestToString(requestUrl, Serializer.Serialize(pass));
            
            return Serializer.Deserialize<TResult>(str);
        }

        public virtual TResult GetRequestStruct<TResult>(string controller, string request, object pass)
        {
            string requestUrl = GetUrl(controller, request, "arg", pass);

            var str = Web.GetRequestToString(requestUrl);

            return Serializer.Deserialize<TResult>(str);
        }

        public virtual bool RequestBoolean<TPass>(string controller, string request, TPass pass)
        {
            string requestUrl = GetUrl(controller, request);

            string result = Web.GetPostRequestToString(requestUrl, Serializer.Serialize(pass));
            return result == "1" || result == "success";
        }

        public virtual string RequestString<TPass>(string controller, string request, TPass pass)
        {
            string requestUrl = GetUrl(controller, request);

            return Web.GetPostRequestToString(requestUrl, Serializer.Serialize(pass));
        }

        protected string GetRequest(string controller, string request, params object[] arguments)
        {
            string requestUrl = GetUrl(controller, request, arguments);
            return Web.GetRequestToString(requestUrl);
        }

        protected IResultSet GetComplexRequest(string controller, string request, params object[] arguments)
        {
            return new ResultSet(GetRequest(controller, request, arguments));
        }


        void SendFiles(string controller, string request, IEnumerable<KeyValuePair<string, string>> files)
        {
            string requestUrl = GetUrl(controller, request);
            
            var s = Web.GetPostRequestToString(requestUrl, LoadFiles(files));
        }

        void DownloadFile(string controller, string request, string localFile, params object[] arguments)
        {
            Web.DownloadRequestToFile(GetUrl(controller, request, arguments), localFile);
        }

        /// <summary>
        /// Writes the files to a stream of bytes.  Pairs should have Key = LocalPath, Value = RemotePath.
        /// 
        /// The sequence is as follows:
        /// 
        /// Int32 Number of files
        /// 
        /// foreach file
        ///   Int32 Destination Path Length
        ///   byte[] Destination Path
        ///   Int32 File Length
        ///   byte[] File Contents
        /// </summary>
        IEnumerable<byte[]> LoadFiles(IEnumerable<KeyValuePair<string,string>> files)
        {
            yield return ConvertInt32ToBytes(files.Count());

            foreach (var file in files)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(file.Key, FileMode.Open)))
                {
                    byte[] filename = Encoding.UTF8.GetBytes(file.Value);
                    yield return ConvertInt32ToBytes(filename.Length);
                    yield return filename;

                    int fileLength = (int)reader.BaseStream.Length;
                    yield return ConvertInt32ToBytes(fileLength);
                    yield return reader.ReadBytes(fileLength);
                }
	        }
        }

        byte[] ConvertInt32ToBytes(int integer)
        {
            byte[] intBytes = BitConverter.GetBytes(integer);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }


    }

}
