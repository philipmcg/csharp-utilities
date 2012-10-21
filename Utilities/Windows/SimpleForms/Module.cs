using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Windows.SimpleForms
{
    public interface ISimpleFormsModule
    {
        void Initialize(SimpleFormsController controller, string name);
        string Name { get; }
        void BeforeJob();
        void DoYourJob();
    }

    public abstract class SimpleFormsModule : ISimpleFormsModule
    {

        protected SimpleFormsController Controller { get; private set; }
        public string Name { get; private set; }

        public void Initialize(SimpleFormsController controller, string name)
        {
            Controller = controller;
            Name = name;
        }

        public virtual void BeforeJob() { }

        public abstract void DoYourJob();
    }
}
