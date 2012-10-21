using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Utilities;
using System.Net;


namespace Utilities.Updater
{

    public struct UpdateVersion : IEquatable<UpdateVersion>
    {
        public static readonly UpdateVersion Empty = new UpdateVersion(null);

        public readonly string Number;
        public readonly string Date;

        public int VersionNumber { get { return IsEmpty ? 0 : int.Parse(Number); } }

        public bool IsEmpty
        {
            get { return Number == ""; }
        }

        // VersionInfo comes in as a string, with the version number, newline, and date.
        public UpdateVersion(string versionInfo)
        {
            if (versionInfo == null)
            {
                Number = "";
                Date = "";
            }
            else
            {
                string[] lines = versionInfo.SplitToLines();
                Number = lines[0];
                Date = lines[1];
            }

        }

        public bool Equals(UpdateVersion other)
        {
            return Date == other.Date && Number == other.Number;
        }

        public override string ToString()
        {
            return Number + "\r\n" + Date;
        }

    }

    public struct UpdaterSettings
    {
        public string UpdaterBak;
        public string UpdaterExe;
        public string VersionFile;
        public string IndexFile;
        public string UpdaterDirectory;
        public string OnlinePath;
        public string RestartExecutable;
        public bool UpdateOnly;
        public bool ConfirmBeforeUpdate;
        public string RelativeDestinationPath;



        public UpdaterSettings(string restartExecutable, string onlinePath)
        {
            UpdaterBak = "Updater.bak";
            UpdaterExe = "Updater.exe";
            VersionFile = "version.txt";
            IndexFile = "index.txt";
            UpdaterDirectory = "updater";
            OnlinePath = onlinePath;
            RestartExecutable = restartExecutable;
            UpdateOnly = false;
            ConfirmBeforeUpdate = false;
            RelativeDestinationPath = "-";
        }

    }

    public class RunUpdater
    {
        public static bool Run(UpdaterSettings settings, out UpdateVersion version)
        {
            string restartExe = settings.UpdateOnly ? "" : settings.RestartExecutable;

            string[] args = new string[]
            {
                settings.OnlinePath,
                settings.IndexFile,
                settings.RelativeDestinationPath,
                restartExe,
                settings.UpdaterDirectory,
            };

            string versionString = null;

            string localVersionFile = Path.Combine(settings.UpdaterDirectory, settings.VersionFile);
            if (System.IO.File.Exists(localVersionFile))
                versionString = File.ReadAllText(localVersionFile);
            version = new UpdateVersion(versionString);


            bool quit = false;

            Utilities.Updater.Updater updater = new Utilities.Updater.Updater(version, settings.VersionFile, settings.UpdaterExe, settings.UpdaterBak, args, () => System.Windows.Forms.Application.ExitThread());
            updater.ConfirmBeforeUpdate = !settings.UpdateOnly && settings.ConfirmBeforeUpdate;
            bool updated = updater.CheckForUpdates(settings.UpdateOnly, () => quit = true);

            return quit;
        }

        public static bool Run(bool updateOnly, string onlinePath, string restartExecutable, out UpdateVersion version, bool confirmBeforeUpdate)
        {
            UpdaterSettings settings = new UpdaterSettings(restartExecutable, onlinePath);
            settings.UpdateOnly = updateOnly;
            settings.ConfirmBeforeUpdate = confirmBeforeUpdate;

            return Run(settings, out version);
        }

        /// <summary>
        /// Returns true if the application should now exit.
        /// </summary>
        /// <param name="updateOnly">If true, the update will be run without user confirmation, and the application will not be restarted after the update.</param>
        /// <param name="onlinePath">The http:// path where the updater index.txt and version.txt are located.</param>
        /// <param name="restartExecutable">The name of the executable to be run after the update is complete.</param>
        /// <returns>True if the application should exit after Run() returns.</returns>
        public static bool Run(bool updateOnly, string onlinePath, string restartExecutable, bool confirmBeforeUpdate, out UpdateVersion version)
        {
            return Run(updateOnly, onlinePath, restartExecutable, out version, confirmBeforeUpdate);
        }
    }

    public class Updater
    {
        static StreamReader GetRequestToStream(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader input = new StreamReader(response.GetResponseStream());
            return input;
        }

        /// <summary>
        /// Makes an http request and returns the result as a string
        /// </summary>
        static string GetRequestToString(string url)
        {
            StreamReader input = GetRequestToStream(url);
            return input.ReadToEnd();
        }


        UpdateVersion m_version;
        string m_versionFile;
        string m_indexFile;
        string m_updaterApp;
        string m_updaterBak;
        string m_updaterArguments;
        string m_url;
        string m_mainDirectory;
        string m_tempDirectory;
        Action m_close;

        string LocalIndexFile
        {
            get
            {
                return Path.Combine(Path.Combine(m_mainDirectory, m_tempDirectory), m_indexFile);
            }
        }

        public bool ConfirmBeforeUpdate = false;

        public Updater(UpdateVersion version, string versionFile, string updaterApp, string updaterBak, string[] updaterArguments, Action close)
        {
            m_mainDirectory = Environment.CurrentDirectory;
            m_version = version;
            m_versionFile = versionFile;
            m_updaterApp = updaterApp;
            m_updaterBak = updaterBak;
            m_url = updaterArguments[0];
            m_indexFile = updaterArguments[1];
            m_tempDirectory = updaterArguments[4];
            m_updaterArguments = "";
            foreach (var arg in updaterArguments)
                m_updaterArguments += " " + arg;

            m_close = close;
        }
        public bool CheckForUpdates(bool updateOnly, Action quit)
        {
            string date = "";
            UpdateVersion onlineVersion = UpdateVersion.Empty;

           try
           {
                string onlineVersionString = Web.GetRequestToString(Path.Combine(m_url, m_versionFile));
                onlineVersion = new UpdateVersion(onlineVersionString);
           }
           catch // System.Net.WebException
           {
           }

            if (onlineVersion.IsEmpty)
            {
                if (ConfirmBox.ShowDialog("Unable to contact server", "The updater was unable to contact the server.\n\nDo you wish to quit (recommended) or continue offline?", "Quit", "Continue", null))
                    return false;
                else
                {
                    quit();
                    return false;
                }
            }
            
            if (!m_version.Equals(onlineVersion) || !Directory.Exists(Path.GetDirectoryName(LocalIndexFile)) || !File.Exists(LocalIndexFile))
            {
                if (ConfirmBeforeUpdate)
                {

                    string dateDisplay = "";
                    date = date.Replace('-', '/');
                    if (!string.IsNullOrEmpty(date))
                        dateDisplay = string.Format(", {0}", date);

                    string display = "There is an update available for {0}.\nDo you wish to install the update now?\n(You have version {1}, while the latest version is {2}{3}.)";
                    if (m_version.Equals(onlineVersion) || m_version.IsEmpty)
                        display = "There is an update available for {0}.\nDo you wish to install the update now?\n(The update will bring you to version {2}{3}.)";

                    DialogResult choice = MessageBox.Show(string.Format(display, System.Windows.Forms.Application.ProductName, m_version, onlineVersion, dateDisplay), "Update", MessageBoxButtons.YesNoCancel);

                    if (choice == DialogResult.Cancel || choice == DialogResult.No)
                    {
                        quit();
                        return false;
                    }
                
                }
                
                if (File.Exists(m_updaterBak))
                    Utilities.IO.Files.MoveSafely(m_updaterBak, m_updaterApp);

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = m_updaterApp;
                startInfo.Arguments = m_updaterArguments;

                string localVersionFile = Path.Combine(m_tempDirectory, "progress_" + m_versionFile);
                if (!onlineVersion.IsEmpty)
                {
                    EnsureDirectory(localVersionFile);
                    File.WriteAllText(localVersionFile, onlineVersion.ToString());
                }

                if (File.Exists(m_updaterApp))
                    System.Diagnostics.Process.Start(startInfo);
                else
                    MessageBox.Show("Updater.exe not found.  You may need to reinstall this application.");

                quit();
                return true;
            }

            if (updateOnly)
                quit();
            return false;
        }

        void EnsureDirectory(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }


    }


}

namespace Utilities.IO
{
    public static class Files
    {
        public static void MoveSafely(string start, string dest)
        {
            if (File.Exists(start))
            {
                string msg = "For the auto updater to be able to function properly, you may have to run this application with administrative rights.";
                try
                {
                    if (File.Exists(dest))
                        File.Delete(dest);
                }
                catch
                {
                    MessageBox.Show(msg);
                    return;
                }


                if (File.Exists(dest))
                {
                    MessageBox.Show(msg);
                    return;
                }

                try
                {
                    if (!File.Exists(dest))
                    {
                        File.Copy(start, dest);
                        File.Delete(start);
                    }
                }
                catch
                {
                    MessageBox.Show(msg);
                    return;
                }

                if (!File.Exists(dest))
                {
                    MessageBox.Show(msg);
                    return;
                }
            }
        }



    }
}