using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class FunctionHandler
    {
        Dictionary<string, Func<string, int>> IntFunctions;
        Dictionary<string, Func<string, string>> StrFunctions;
        Dictionary<string, Func<string, object>> ObjFunctions;
        Dictionary<string, Action<string>> Actions;

        public FunctionHandler()
        {
            IntFunctions = new Dictionary<string, Func<string, int>>();
            StrFunctions = new Dictionary<string, Func<string, string>>();
            ObjFunctions = new Dictionary<string, Func<string, object>>();
            Actions = new Dictionary<string, Action<string>>();
        }

        public void Add(string key, Func<string, int> function)
        {
            IntFunctions.Add(key, function);
        }

        public void Add(string key, Func<string, string> function)
        {
            StrFunctions.Add(key, function);
        }

        public void Add(string key, Action<string> function)
        {
            Actions.Add(key, function);
        }
        public void Add(string key, Func<string, object> function)
        {
            ObjFunctions.Add(key, function);
        }
        public int Int(string key)
        {
            return IntFunctions[key]("");
        }
        public int Int(string key, string param)
        {
            return IntFunctions[key](param);
        }
        public string Str(string key)
        {
            return StrFunctions[key]("");
        }
        public string Str(string key, string param)
        {
            return StrFunctions[key](param);
        }
        public void Void(string key)
        {
            Actions[key]("");
        }
        public void Void(string key, string param)
        {
            Actions[key](param);
        }
        public object Obj(string key)
        {
            return ObjFunctions[key]("");
        }
        public object Obj(string key, string param)
        {
            return ObjFunctions[key](param);
        }
    }

    
    public sealed class Tester2
    {
        FunctionHandler Func;

        public Tester2()
        {
            Func = new FunctionHandler();

            Func.Add("#int", p => p.ToInt() + 100);
            Func.Add("#int", p => p.ToInt() + 100);
        }
    }
}
