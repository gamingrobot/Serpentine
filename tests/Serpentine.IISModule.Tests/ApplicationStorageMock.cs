using System.Collections;

namespace Serpentine.IISModule.Tests
{
    internal class ApplicationStorageMock : IApplicationStorage
    {
        private readonly Hashtable _hashtable;

        public ApplicationStorageMock()
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