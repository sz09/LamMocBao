using Services.Caching;
using Services.DbContexts;
using Services.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Services.Utiltities
{
    public static class QueryUtilities
    {
        private static ConcurrentDictionary<string, List<string>> typeIncludeMappings = new ConcurrentDictionary<string, List<string>>();
        private static Type EnumerableType = typeof(IEnumerable);

        public static List<string> GetNavigationProperties(this Type type, string baseType)
        {
            var baseType1 = string.IsNullOrEmpty(baseType) ? string.Empty : $"{baseType}.";
            var propertyInfos = type.GetProperties().Where(propertyInfo => (EnumerableType.IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(string)) || propertyInfo.PropertyType.Namespace == type.Namespace);
            var result = propertyInfos.Select(d => $"{baseType1}{d.Name}").ToList();
            foreach (var propertyInfo in propertyInfos.Where(propertyInfo => EnumerableType.IsAssignableFrom(propertyInfo.PropertyType)))
            {
                result.Add($"{baseType1}{propertyInfo.Name}");
                var genericArguments = propertyInfo.PropertyType.GetGenericArguments();
                if (genericArguments.Any())
                {
                    result.AddRange(genericArguments[0].GetNavigationProperties($"{baseType1}{propertyInfo.Name}"));
                }
            }

            foreach (var propertyInfo in propertyInfos.Where(propertyInfo => !EnumerableType.IsAssignableFrom(propertyInfo.PropertyType)))
            {
                result.Add($"{baseType1}{propertyInfo.Name}");
            }

            return result;
        }

        private static List<string> GetNavigationProperties(Type baseType)
        {
            var navigationProperties = baseType.GetNavigationProperties(string.Empty);
            var navigationProperties1 = navigationProperties.GroupBy(d => d).Select(d => d.First()).ToList();
            return navigationProperties1;
        }

        public static IDbContext ResolveDbContext(this IServiceConfig serviceConfig, InMemoryCache memoryCache)
        {
            if (serviceConfig.IsTestMode)
            {
                return new SqlLiteContext(memoryCache, serviceConfig);
            }

            return new ApplicationDbContext(memoryCache, serviceConfig);
        }
    }
}
