using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class LazyClassLoader
    {
        Dictionary<Type, object> Objects;

        public LazyClassLoader()
        {
            Objects = new Dictionary<Type, object>();
        }

        public T Get<T>() where T : class, new()
        {
            if (!Objects.ContainsKey(typeof(T)))
                Objects.Add(typeof(T), new T());
            return Objects[typeof(T)] as T;
        }
    }

    public class Singleton<T> where T : class, new()
    {
        T instance;
        object _lock = new object();
        public T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }
    }
} 
