using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.ComponentModel;

namespace Utilities
{
        public delegate void Load();
        public interface ILoadingScreen
        {
            void Reset(int length);
            int Progress { set; get; }
            string Message { set; }
        }
        public class LoadingEngine
        {
            public bool Quit = false;
            BackgroundWorker m_worker;

            ILoadingScreen m_loadingScreen;
            List<Load> m_routines;
            List<string> m_messages;
            public LoadingEngine(ILoadingScreen loadingScreen)
            {
                m_loadingScreen = loadingScreen;
                Clear();
            }
            public void Add(Load routine, string message)
            {
                m_routines.Add(routine);
                m_messages.Add(message);
            }
            public void Clear()
            {
                m_routines = new List<Load>();
                m_messages = new List<string>();
            }
            public void Load()
            {
                m_loadingScreen.Reset(m_routines.Count);
                for (int k = 0; k < m_routines.Count; k++)
                {
                    if (Quit)
                        return;
                    m_loadingScreen.Message = m_messages[k];
                    SW.Start(14);
                    m_routines[k]();
                    SW.Stop(14);
                    Console.WriteLine(m_messages[k]);
                    m_loadingScreen.Progress++;
                }
            }
            public void Load3()
            {
                m_loadingScreen.Reset(m_routines.Count);
                for (int k = 0; k < m_routines.Count; k++)
                {
                    if (Quit)
                        return;
                    m_worker = new BackgroundWorker();
                    m_loadingScreen.Message = m_messages[k];
                    
                    m_worker.DoWork += (s, e) => m_routines[k]();
                    m_worker.RunWorkerAsync();
                    while (m_worker.IsBusy)
                    {
                        Application.DoEvents();
                    }
                    m_loadingScreen.Progress++;
                }
            }
        }
    
}
