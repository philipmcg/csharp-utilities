// Copyright (c) Philip McGarvey 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Utilities
{
    public interface ILog
    {
        void Write(string line);
    }

    /// <summary>
    /// Provides a simple interface for writing to a log file asyncronously.  
    /// </summary>
    public class LogFile : ILog
    {
        static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        #region Fields

        // Thread the log queue is running on
        Thread logThread;
        // Path to the log file being written to
        string path;
        // Position in the log file to write to
        long position;
        object locker;
        // Queue of strings to be written
        List<string> queue;

        // Delay in miliseconds between writes
        int delay;
        // Maximum file length before wrapping around to the beginning
        long fileLength;
        string lineEnding;
        // Format string for the timestamp on each line
        string timeFormat;
        FileMode mode;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the format string used to format the unix timestamp at the beginning of each line of the log file.  Example: "00000000.000 ".  Default is null.
        /// </summary>
        public string TimestampFormat
        {
            get { lock (locker) 
            { return timeFormat; } }
            set { lock (locker) 
            { timeFormat = value; } }
        }

        /// <summary>
        /// Gets or sets the string to append to each line of the log file.  Default is "\r\n".
        /// </summary>
        public string Newline
        {
            get { lock (locker) 
            { return lineEnding; } }
            set { lock (locker) 
            { lineEnding = value; } }
        }

        /// <summary>
        /// Gets or sets the maximum length of the log file, in bytes.  If MaximumLength is 0, there is no limit.  Otherwise, the log will begin overwriting the file each time it reaches the limit.
        /// </summary>
        public long MaximumLength
        {
            get { lock (locker) 
            { return fileLength; } }
            set
            {
                lock (locker)
                {
                    if (value < 0)
                        throw new ArgumentException("MaximumLength must be non-negative");
                    else
                        fileLength = value;
                }
            }
        }

        /// <summary>
        /// The interval, in milliseconds, between writes to the log file.
        /// </summary>
        public int Interval
        {
            get { lock (locker) 
            { return delay; } }
            set
            {
                lock (locker)
                {
                    if (value < 0)
                        throw new ArgumentException("Interval must be non-negative");
                    else
                        fileLength = value;
                }
            }
        }

        /// <summary>
        /// Gets whether the writing thread has been started.
        /// </summary>
        public bool Started { get; private set; }

        #endregion


        /// <summary>
        /// Creates a new LogFile which will write to the given path.
        /// </summary>
        /// <param name="path">The destination path for the log file to be written to.</param>
        public LogFile(string path, FileMode mode = FileMode.Create)
        {
            timeFormat = null;
            lineEnding = "\r\n";
            fileLength = 0;
            delay = 1;
            this.mode = mode;
            this.path = path;
            position = 0;
            queue = new List<string>();
            locker = new object();
        }

        /// <summary>
        /// Starts the writing thread.
        /// </summary>
        public void Start()
        {
            if (Started)
                throw new InvalidOperationException("Log file has already been started");

            if(mode != FileMode.Append)
                File.Delete(path);

            logThread = new Thread(LogLoop);
            logThread.Start();
            Started = true;
        }

        /// <summary>
        /// Stops the writing thread.
        /// </summary>
        public void Stop()
        {
            logThread.Abort();
        }

        /// <summary>
        /// Writes a line to the log file.
        /// </summary>
        /// <param name="str">The line to be written.</param>
        public void Write(string line)
        {
            lock (locker)
            {
                queue.Add(GetTimestamp() + line);
            }
        }

        #region Private Implementation

        /// <summary>
        /// Returns the current timestamp formatted with timeFormat.
        /// </summary>
        private string GetTimestamp()
        {
            if (string.IsNullOrEmpty(timeFormat))
            {
                return "";
            }
            else
            {
                double unixTime = (DateTime.UtcNow - unixEpoch).TotalSeconds;
                return unixTime.ToString(timeFormat);
            }
        }

        /// <summary>
        /// Writes strings to the log file, and closes the file.
        /// </summary>
        private void WriteStrings(string[] strings)
        {
            File.AppendAllLines(path, strings);
        }

        /// <summary>
        /// The main loop for the writing thread.
        /// </summary>
        private void LogLoop()
        {
            while (true)
            {
                string[] current = null;
                lock (locker)
                {
                    if (queue.Count > 0)
                    {
                        current = new string[queue.Count];
                        queue.CopyTo(current);
                        queue.Clear();
                    }
                }
                if (current != null)
                {
                    WriteStrings(current);
                }

                Thread.Sleep(delay);
            }
        }

        #endregion
    }
}
