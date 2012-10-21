using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;

using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Forms;

namespace Utilities
{
    public class FTP
    {
        public string Username;
        public string Password;
        public string HttpURL;
        public string FTPURL;
        public NetworkCredential Credentials { get { return new NetworkCredential(Username, Password); } }
        public WebClient Client;

        public FTP()
        {
            Client = new WebClient();
        }

        public void UploadFile(string local, string destination)
        {
            Client.Credentials = Credentials;
            string dest = Path.Combine(FTPURL, destination);

            if (!File.Exists(local))
                throw new FileNotFoundException("File not found to upload: " + local);

            //try
            //{
                Client.UploadFile(new Uri(dest),
                     "STOR",
                     local);
            /*}
            catch (WebException ex)
            {
                var response = ex.Response;
                throw;
            }*/
        }
        public void DownloadFile(string online, string destination)
        {
            Client.Credentials = null;

            string onl = Path.Combine(HttpURL, online);
            try
            {
                Client.DownloadFile(onl, destination);
            }
            catch (WebException ex)
            {
                var response = ex.Response;
                throw;
            }
        }
        
        public void CreateDirectory(string directory)
        {
            try
            {
                string dest = Path.Combine(FTPURL, directory);
                WebRequest request = WebRequest.Create(dest);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = Credentials;
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    //Console.WriteLine(resp.StatusCode);
                }
            }
            catch { }
        }

    }
}
