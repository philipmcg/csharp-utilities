using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Windows
{
    /// <summary>
    /// Provides functions for manipulating this process and other processes of the same application.
    /// </summary>
    public static class Processes
    {
        /// <summary>
        /// Kills all other running processes that have the same process name and same executable file path.
        /// </summary>
        public static void KillOtherProcessesOfSameApplication()
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            foreach (var process in System.Diagnostics.Process.GetProcessesByName(thisProcess.ProcessName))
            {
                if (process.Id != thisProcess.Id && process.MainModule.FileName == thisProcess.MainModule.FileName)
                {
                    process.Kill();
                }
            }
        }

        /// <summary>
        /// Returns true if there is another running process of the same application and file path.
        /// </summary>
        public static bool OtherProcessOfSameApplicationIsRunning()
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            return System.Diagnostics.Process.GetProcessesByName(thisProcess.ProcessName)
                .Any(p => p.Id != thisProcess.Id 
                    && p.MainModule.FileName == thisProcess.MainModule.FileName);
        }


        /// <summary>
        /// Returns true if there is another running process of the same name running.  It may not have the same file path.
        /// </summary>
        public static bool OtherProcessOfSameNameIsRunning()
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            return System.Diagnostics.Process.GetProcessesByName(thisProcess.ProcessName)
                .Any(p => p.Id != thisProcess.Id);
        }

        /// <summary>
        /// Starts another instance of this application.  
        /// The current instance should be given a command 
        /// to close first, such as a Form.Close() call.
        /// </summary>
        public static void RestartThisApplication()
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            System.Diagnostics.Process.Start(thisProcess.MainModule.FileName);
        }


        /// <summary>
        /// Kills the current process.
        /// </summary>
        public static void KillThisProcess()
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            thisProcess.Kill();
        }
    }
}
