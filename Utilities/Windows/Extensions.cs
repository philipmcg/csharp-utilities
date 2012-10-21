using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Utilities.Windows
{
    public static class WindowsExtensions
    {
        /// <summary>
        /// Invokes the action if necessary, otherwise calls it directly.
        /// </summary>
        public static void InvokeIfRequired(this Form me, Action action)
        {
            if (me.InvokeRequired)
            {
                me.Invoke(action);
            }
            else
            {
                action();
            }
        }


        /// <summary>
        /// Sets the window size and position to that specified in the variable bin.
        /// </summary>
        public static void ApplySavedFormLayout(this Form me, IVariableBin var, string prefix)
        {
            if (var.Int.ContainsKey(prefix+"width"))
                me.Width = var.Int[prefix + "width"];

            if (var.Int.ContainsKey(prefix + "height"))
                me.Height = var.Int[prefix + "height"];

            if (var.Int.ContainsKey(prefix + "left"))
                me.Left = var.Int[prefix + "left"];

            if (var.Int.ContainsKey(prefix + "top"))
                me.Top = var.Int[prefix + "top"];
        }

        /// <summary>
        /// Saves the window size and position to values in the variable bin.
        /// </summary>
        public static void SaveFormLayout(this Form me, IVariableBin var, string prefix)
        {
            var.Int[prefix + "width"] = me.Width;
            var.Int[prefix + "height"] = me.Height;
            var.Int[prefix + "left"] = me.Left;
            var.Int[prefix + "top"] = me.Top;
        }
    }
}
