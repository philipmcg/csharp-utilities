using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

using System.Threading;

using System.Windows.Forms;

namespace Utilities.Windows.SimpleForms
{
    public class SimpleFormsController
    {
        public string ApplicationPath { get { return System.Windows.Forms.Application.ExecutablePath; } }
        public string ApplicationDirectory { get { return System.IO.Path.GetDirectoryName(ApplicationPath); } }

        Dictionary<string, ISimpleFormsModule> modules;

        public Form Form { get; private set; }

        bool executing = false;


        public event Action<string> ActionStarted;
        public event Action<string> ActionFinished;

        public SimpleFormsController(Form form)
        {
            modules = new Dictionary<string, ISimpleFormsModule>();
            this.Form = form;
        }

        ISimpleFormsModule GetModule<T>() where T : ISimpleFormsModule, new()
        {
            string name = typeof(T).Name;

            if (!modules.ContainsKey(name))
            {
                ISimpleFormsModule module = new T();
                module.Initialize(this, name);
                modules.Add(name, module);
            }

            return modules[name];
        }

        public void RunAction(Action action)
        {
            if (!executing)
            {
                executing = true;
                RunActionAsync(action);
            }
        }

        public void RunModule<T>() where T : ISimpleFormsModule, new()
        {
            if (!executing)
            {
                executing = true;
                RunModuleAsync(GetModule<T>());
            }
        }

        void RunActionAsync(Action action)
        {
            thread = new Thread(() => RunActionWrapper(action));
            thread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            this.ActionStarted("Action");

            thread.Start();
            while (executing)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            thread.Abort();

            this.ActionFinished("Action");

        }

        void RunModuleAsync(ISimpleFormsModule module)
        {
            thread = new Thread(() => RunModuleWrapper(module));
            thread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            
            this.ActionStarted(module.Name);

            thread.Start();
            while (executing)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            thread.Abort();

            this.ActionFinished(module.Name);

        }

        void RunActionWrapper(Action action)
        {
            action();
            executing = false;
        }

        void RunModuleWrapper(ISimpleFormsModule module)
        {
            module.BeforeJob();
            module.DoYourJob();
            executing = false;
        }

        public void Quit()
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }

        Thread thread;
    }
}
