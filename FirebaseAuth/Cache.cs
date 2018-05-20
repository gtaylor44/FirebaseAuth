using Microsoft.Extensions.Caching.Memory;
using System;

namespace FirebaseAuth
{
    internal sealed class Cache
    {
        private static volatile Cache instance; //  Locks var until assignment is complete for double safety
        private static MemoryCache memoryCache;
        private static object syncRoot = new Object();
        private Cache() { }

        /// <summary>
        /// Singleton cache instance
        /// </summary>
        internal static Cache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Cache();
                            memoryCache = new MemoryCache(new MemoryCacheOptions());
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Sets key/value pair to cahce
        /// </summary>
        /// <param name="Key">Key to associate Value with in Cache</param>
        /// <param name="Value">Value to be stored in Cache associated with Key</param>
        internal void Set(string key, object value, DateTimeOffset offset)
        {
            memoryCache.Set(key, value, offset);
        }

        /// <summary>
        /// Returns value stored in cache.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>Value stored in cache</returns>
        internal T Get<T>(string key)
        {
            if (memoryCache.TryGetValue(key, out var result))
            {
                return (T)result;
            }
            return default(T);
        }
    }
}
