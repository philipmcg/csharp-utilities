using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class DebugException : ApplicationException
    {
        public DebugException(string message)
            : base(message)
        {
        }
         
    }
}
