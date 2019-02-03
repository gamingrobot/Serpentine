using System;
using System.Collections;

namespace Serpentine.IISModule
{
    internal interface IApplicationStorage
    {
        void Set<T>(string key, T value);
        T Get<T>(string key);
    }

    //Singleton because multiple httpapplications can exist per worker process
    internal class ApplicationStorage : IApplicationStorage
    {
        private static readonly Lazy<ApplicationStorage> LazyInstance =
            new Lazy<ApplicationStorage>(() => new ApplicationStorage());

        public static ApplicationStorage Instance => LazyInstance.Value;

        private readonly Hashtable _hashtable;

        private ApplicationStorage()
        {
            _hashtable = new Hashtable();
        }

        public void Set<T>(string key, T value)
        {
            _hashtable[key] = value;
        }

        public T Get<T>(string key)
        {
            if (!_hashtable.ContainsKey(key))
            {
                return default(T);
            }

            return (T) _hashtable[key];
        }
    }
}