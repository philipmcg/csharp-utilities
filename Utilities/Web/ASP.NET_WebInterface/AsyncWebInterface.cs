using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;


using System.Threading;

namespace Utilities.MSWebInterface
{
    public class AsyncWebInterface
    {
        Thread bg;
        Queue<Action> queue;
        WebInterface web;

        int msPauseBetweenActions;

        public AsyncWebInterface(WebInterface web, int msPauseBetweenActions)
        {
            this.web = web;
            this.msPauseBetweenActions = msPauseBetweenActions;

            queue = new Queue<Action>();
            bg = new Thread(RunBackgroundThread);
            bg.Start();
        }


        public void Enqueue(Action action)
        {
            lock (queue)
            {
                queue.Enqueue(action);
            }
        }

        void RunBackgroundThread()
        {
            Action action;
            while (true)
            {
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        action = queue.Dequeue();
                    }
                    else
                    {
                        action = null;
                    }
                }

                if (action != null)
                    action();

                Thread.Sleep(msPauseBetweenActions);
            }
        }
    }
}
