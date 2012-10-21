// Copyright (c) Philip McGarvey 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilities
{
    public interface IVariableBin
    {
        IVariableMap<string> Str { get; }
        IVariableMap<double> Double { get; }
        IVariableMap<int> Int { get; }
        IVariableMap<bool> Bool { get; }

    }

    interface IStringSerializable
    {
        void LoadFromFile(string path);
        void LoadFromLines(IEnumerable<string> lines);
        void SaveToFile(string path);
        IEnumerable<string> SaveToLines();
    }

    public interface IVariableMap<T>
    {
        /// <summary>
        /// Gets or sets this variable.
        /// </summary>
        T this[string key] { get; set; }

        /// <summary>
        /// Gets or sets this variable.
        /// </summary>
        T this[string key, T defaultValue] { get; }

        /// <summary>
        /// Returns true if a variable exists with this key.
        /// </summary>
        bool ContainsKey(string key);
    }
    
    /// <summary>
    /// Presents an interface for storing and retrieving variables by name.  
    /// Integer and boolean variables are stored as Int32, strings and doubles are stored as string.
    /// </summary>
    public class VariableBin : IVariableBin, IStringSerializable
    {
        const char StringPrefix = '$';
        const char IntegerPrefix = '#';


        #region Variable Maps

        /// <summary>
        /// The set of string variables in this variable bin.
        /// </summary>
        public IVariableMap<string> Str { get { return stringMap; } }

        /// <summary>
        /// The set of integer variables in this variable bin.
        /// </summary>
        public IVariableMap<int> Int { get { return integerMap; } }

        /// <summary>
        /// The set of double variables in this variable bin.
        /// </summary>
        public IVariableMap<double> Double { get; private set; }

        /// <summary>
        /// The set of boolean variables in this variable bin.
        /// </summary>
        public IVariableMap<bool> Bool { get; private set; }

        #endregion

        IntegerMap integerMap;
        StringMap stringMap;

        Func<string, IEnumerable<string>> readFileFunction;

        public VariableBin()
            : this(path => File.ReadAllLines(path))
        {

        }

        /// <summary>
        /// Constructs a VariableBin with the given function for reading files.
        /// </summary>
        /// <param name="readFileFunction">A function taking a file path and returning a list of strings read from the file.</param>
        public VariableBin(Func<string, IEnumerable<string>> readFileFunction, Action onStateChange = null)
        {
            this.readFileFunction = readFileFunction;

            if (onStateChange == null)
            {
                stringMap = new StringMap();
                integerMap = new IntegerMap();
            }
            else
            {
                stringMap = new EventStringMap();
                (stringMap as EventStringMap).StateChanged += onStateChange;
                integerMap = new EventIntegerMap();
                (integerMap as EventIntegerMap).StateChanged += onStateChange;
            }

            Double = new DoubleMap(Str);
            Bool = new BoolMap(Int);
        }

        /// <summary>
        /// Loads all variables from the specified file.
        /// </summary>
        public void LoadFromFile(string path)
        {
            var lines = readFileFunction(path);
            LoadFromLines(lines);
        }

        /// <summary>
        /// Loads all variables from the strings provided.
        /// </summary>
        public void LoadFromLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var parts = line.Substring(1).Split('=').Select(s => s.Trim()).ToArray();
                if (parts.Length != 2)
                    continue;

                var pair = new { Type = line[0], Key = parts[0], Value = parts[1] };

                if (pair.Type == StringPrefix)
                    Str[pair.Key] = pair.Value;
                else if (pair.Type == IntegerPrefix)
                    Int[pair.Key] = int.Parse(pair.Value);
            }
        }

        /// <summary>
        /// Saves all variables to the specified file.
        /// </summary>
        public void SaveToFile(string path)
        {
            SaveToFile(path, SaveToLines());
        }

        /// <summary>
        /// Saves all variables whose keys start with any of the provided prefixes to the specified file.
        /// </summary>
        public void SaveToFileByPrefix(string path, params string[] prefixes)
        {
            SaveToFile(path, key => prefixes.Any(p => key.StartsWith(p)));
        }

        /// <summary>
        /// Yields all variables as lines for an ini file.
        /// </summary>
        public IEnumerable<string> SaveToLines()
        {
            return SaveToLines(s => true);
        }

        /// <summary>
        /// Yields all variables whose keys start with any of the provided prefixes as lines for an ini file.
        /// </summary>
        public IEnumerable<string> SaveToLinesByPrefix(params string[] prefixes)
        {
            return SaveToLines(s => prefixes.Any(p => s.StartsWith(p)));
        }

        private IEnumerable<string> SaveToLines(Func<string, bool> keyFilter)
        {
            return stringMap.GetPairs().Where(p => keyFilter(p.Key)).Select(p => Concat(p.Key[0] == StringPrefix ? "" : StringPrefix.ToString(), p.Key, "=", p.Value))
                .Concat(integerMap.GetPairs().Where(p => keyFilter(p.Key)).Select(p => Concat(p.Key[0] == IntegerPrefix ? "" : IntegerPrefix.ToString(), p.Key, "=", p.Value)));
        }

        private void SaveToFile(string path, Func<string, bool> keyFilter)
        {
            SaveToFile(path, SaveToLines(keyFilter));
        }

        private void SaveToFile(string path, IEnumerable<string> lines)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private string Concat(params string[] parts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var part in parts)
            {
                sb.Append(part);
            }
            return sb.ToString();
        }

        public IEnumerable<KeyValuePair<string, string>> GetVariablesByPrefix(string prefix)
        {
            return GetPairs(key => key.StartsWith(prefix));
        }

        private IEnumerable<KeyValuePair<string, string>> GetPairs(Func<string, bool> keyFilter)
        {
            return stringMap.GetPairs().Where(p => keyFilter(p.Key))
                .Concat(integerMap.GetPairs().Where(p => keyFilter(p.Key)));
        }

        private abstract class VariableProxy
        {
            protected char prefix;

            protected string FilterKey(string key)
            {
                if (key[0] == prefix)
                    return key.Substring(1);
                return key;
            }
        }

        private class BoolMap : VariableProxy, IVariableMap<bool>
        {
            IVariableMap<int> Base;

            public BoolMap(IVariableMap<int> inner)
            {
                prefix = IntegerPrefix;
                Base = inner;
            }

            public bool this[string key, bool defaultValue]
            {
                get
                {
                    return Base[FilterKey(key), defaultValue ? 1 : 0] != 0;
                }
            }
            public bool this[string key]
            {
                get
                {
                    return Base[FilterKey(key)] != 0;
                }
                set
                {
                    Base[FilterKey(key)] = value ? 1 : 0;
                }
            }

            public bool ContainsKey(string key)
            {
                return Base.ContainsKey(FilterKey(key));
            }
        }

        private class DoubleMap : VariableProxy, IVariableMap<double>
        {
            IVariableMap<string> Base;

            public DoubleMap(IVariableMap<string> inner)
            {
                prefix = StringPrefix;
                Base = inner;
            }


            public double this[string key, double defaultValue]
            {
                get
                {
                    return double.Parse(Base[FilterKey(key), defaultValue.ToString()]);
                }
            }
            public double this[string key]
            {
                get
                {
                    return double.Parse(Base[FilterKey(key)]);
                }
                set
                {
                    Base[FilterKey(key)] = value.ToString();
                }
            }

            public bool ContainsKey(string key)
            {
                return Base.ContainsKey(FilterKey(key));
            }
        }

        private class EventStringMap : StringMap
        {
            public event Action StateChanged;

            public override string this[string key, string defaultValue]
            {
                get
                {
                    key = FilterKey(key);
                    string value;
                    if (Base.TryGetValue(key, out value))
                        return value;
                    else
                    {
                        Base[key] = defaultValue;
                        StateChanged();
                        return defaultValue;
                    }
                }
            }
            public override string this[string key]
            {
                get
                {
                    return Base[FilterKey(key)];
                }
                set
                {
                    Base[FilterKey(key)] = value;
                    StateChanged();
                }
            }
        }

        private class StringMap : VariableProxy, IVariableMap<string>
        {
            protected Dictionary<string, string> Base;

            public StringMap()
            {
                prefix = StringPrefix;
                Base = new Dictionary<string, string>();
            }

            public virtual string this[string key, string defaultValue]
            {
                get
                {
                    key = FilterKey(key);
                    string value;
                    if (Base.TryGetValue(key, out value))
                        return value;
                    else
                    {
                        Base[key] = defaultValue;
                        return defaultValue;
                    }
                }
            }
            public virtual string this[string key]
            {
                get
                {
                    return Base[FilterKey(key)];
                }
                set
                {
                    Base[FilterKey(key)] = value;
                }
            }

            public bool ContainsKey(string key)
            {
                return Base.ContainsKey(FilterKey(key));
            }

            public IEnumerable<KeyValuePair<string, string>> GetPairs()
            {
                return Base;
            }
        }

        private class EventIntegerMap : IntegerMap
        {
            public event Action StateChanged;

            public override int this[string key, int defaultValue]
            {
                get
                {
                    key = FilterKey(key);
                    int value;
                    if (Base.TryGetValue(key, out value))
                        return value;
                    else
                    {
                        Base[key] = defaultValue;
                        StateChanged();
                        return defaultValue;
                    }
                }
            }
            public override int this[string key]
            {
                get
                {
                    return Base[FilterKey(key)];
                }
                set
                {
                    Base[FilterKey(key)] = value;
                    StateChanged();
                }
            }
        }
        private class IntegerMap : VariableProxy, IVariableMap<int>
        {
            protected Dictionary<string, int> Base;

            public IntegerMap()
            {
                prefix = IntegerPrefix;
                Base = new Dictionary<string, int>();
            }

            public virtual int this[string key, int defaultValue]
            {
                get
                {
                    key = FilterKey(key);
                    int value;
                    if (Base.TryGetValue(key, out value))
                        return value;
                    else
                    {
                        Base[key] = defaultValue;
                        return defaultValue;
                    }
                }
            }
            public virtual int this[string key]
            {
                get
                {
                    return Base[FilterKey(key)];
                }
                set
                {
                    Base[FilterKey(key)] = value;
                }
            }

            public bool ContainsKey(string key)
            {
                return Base.ContainsKey(FilterKey(key));
            }

            public IEnumerable<KeyValuePair<string, string>> GetPairs()
            {
                foreach (var pair in Base)
                {
                    yield return new KeyValuePair<string, string>(pair.Key, pair.Value.ToString());
                }
            }
        }
    }
}