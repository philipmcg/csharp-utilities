using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;

namespace Utilities
{


    public class Async
    {
        private bool[] Finished;
        private object Lock;

        private void InternalRun(params Action[] actions)
        {
            int count = actions.Length;

            Finished = new bool[count];
            Lock = new object();

            for (int s = 0; s < count; s++)
            {
                int k = s;
                StartAsyncSilent(() => { lock (Lock) { Finished[k] = true; } }, actions[k]);
            }

            while (Finished.Any(f => !f))
                Application.DoEvents();
        }

        /// <summary>
        /// Runs all actions in separate threads, returning when all are finished.
        /// </summary>
        public static void Run(params Action[] actions)
        {
            Async ops = new Async();
            ops.InternalRun(actions);
        }

        /// <summary>
        /// Runs action in separate thread, returns immediately.  Does not keep track of thread or progress.
        /// </summary>
        public static void RunSilentlyWithoutCallback(Action action)
        {
            Thread thread = new Thread(new ThreadStart(action));
            thread.Start();
        }

        private static void StartAsyncSilent(Action complete, Action work)
        {
            System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();

            bg.DoWork += (s, e) =>
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                work();
            };


            if (complete != null)
                bg.RunWorkerCompleted += (s, e) => complete();

            bg.RunWorkerAsync();
        }
    }

}
