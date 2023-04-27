using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Caching
{
    public class CacheKey
    {
        public static string CacheKeyFor<T>()
        {
            return typeof(T).Name;
        }

        public static string CacheKeyFor<T>(object t1)
        {
            return $"{typeof(T).Name}__{t1}";
        }

        public static string CacheKeyFor<T>(string spectificKey)
        {
            return $"{typeof(T).Name}__{spectificKey}";
        }
    }
}
