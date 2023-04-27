using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Services.Services;
using Services.Utiltities;
using Shared.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Caching
{
    public class InMemoryCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceConfig _serviceConfig;
        private ConcurrentDictionary<string, List<string>> SavedItems = new ConcurrentDictionary<string, List<string>>();
        public InMemoryCache(IMemoryCache memoryCache, IServiceConfig serviceConfig)
        {
            _memoryCache = memoryCache;
            _serviceConfig = serviceConfig;
        }

        public T TryGet1Async<T>(string key, Func<T> findObject)
        {
            if (_serviceConfig.LMB.EnableCache && SavedItems.ContainsKey(key))
            {
                if (_memoryCache.TryGetValue(key, out object obj))
                {
                    if (obj is T obj1)
                    {
                        return obj1;
                    }
                }
            }

            var cachedValue = findObject.Invoke();
            if (cachedValue != null && _serviceConfig.LMB.EnableCache)
            {
                _memoryCache.Set(key, cachedValue, _serviceConfig.LMB.CacheExpireTime);
                if (SavedItems.ContainsKey(key))
                {
                    if (!SavedItems[key].Contains(key))
                    {
                        SavedItems[key].Add(key);
                    }
                }
                else
                {
                    SavedItems.TryAdd(key, new List<string> { key });
                }
            }

            //return cachedValue;
            return findObject.Invoke();
        }

        public T TryGetAsync<T>(string key, Func<T> findObject)
        {
            //var bulkKeys = GetBulkKey<T>();
            //if (SavedItems.ContainsKey(bulkKeys) && SavedItems[bulkKeys].Contains(key))
            //{
            //    if (_memoryCache.TryGetValue(key, out object obj))
            //    {
            //        if(obj is T obj1)
            //        {
            //            return obj1;
            //        }
            //    }
            //}

            //var cachedValue = findObject.Invoke();
            //if (cachedValue != null)
            //{
            //    _memoryCache.Set(key, cachedValue, CacheExpireTime);
            //    if (SavedItems.ContainsKey(bulkKeys))
            //    {
            //        if (!SavedItems[bulkKeys].Contains(key))
            //        {
            //            SavedItems[bulkKeys].Add(key);
            //        }
            //    }
            //    else
            //    {
            //        SavedItems.TryAdd(bulkKeys, new List<string> { key });
            //    }
            //}

            //return cachedValue;
            return findObject.Invoke();
        }

        public void Remove<T>(string key)
        {
            var bulkKeys = GetBulkKey<T>();
            if (SavedItems.ContainsKey(bulkKeys))
            {
                _memoryCache.Remove(key);
                SavedItems[bulkKeys].Remove(key);
            }
        }

        public void Remove(Type type, string key)
        {
            var bulkKeys = GetBulkKey(type);
            if (SavedItems.ContainsKey(bulkKeys))
            {
                _memoryCache.Remove(key);
                SavedItems[bulkKeys].Remove(key);
            }
        }

        public void Invalidate<T>()
        {
            var bulkKeys = GetBulkKey<T>();
            if (SavedItems.ContainsKey(bulkKeys))
            {
                foreach (var key in SavedItems[bulkKeys])
                {
                    _memoryCache.Remove(key);
                }

                SavedItems[bulkKeys].Clear();
            }
        }

        public void Invalidate(Type type, List<string> sessionCacheTypesCleared)
        {
            var bulkKeys = GetBulkKey(type);

            if (SavedItems.ContainsKey(bulkKeys))
            {
                foreach (var key in SavedItems[bulkKeys])
                {
                    _memoryCache.Remove(key);
                }

                SavedItems[bulkKeys].Clear();
            }

            var navationProperties = type.GetNavigationProperties(string.Empty);
            var enumarableType = typeof(IEnumerable);
            foreach (var navationProperty in navationProperties)
            {
                //if (sessionCacheTypesCleared.Contains(navationProperty.PropertyType.Name))
                //{
                //    continue;
                //}

                //if (enumarableType.IsAssignableFrom(navationProperty.PropertyType))
                //{
                //    var realType = navationProperty.PropertyType.GenericTypeArguments[0];
                //    sessionCacheTypesCleared.Add(realType.Name);
                //    Invalidate(realType, sessionCacheTypesCleared);
                //}
                //else
                //{
                //    sessionCacheTypesCleared.Add(navationProperty.PropertyType.Name);
                //    Invalidate(navationProperty.PropertyType, sessionCacheTypesCleared);
                //}
            }
        }

        public string GetBulkKey<T>()
        {
            return ToPluralize(typeof(T).Name);
        }

        public void ForceSet<T>(string key, Dictionary<T, int> objects)
        {
            _memoryCache.Set(key, objects);
        }

        public Dictionary<T, int> TryGet<T>(string key)
        {
            if (!_memoryCache.TryGetValue(key, out Dictionary<T, int> value))
            {
                value = new Dictionary<T, int>();
            }

            return value;
        }

        private string GetBulkKey(Type type)
        {
            return ToPluralize(type.Name);
        }

        private string ToPluralize(string src)
        {
            return src.Pluralize();
        }
    }
}
