using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.Utilities
{
    public static class JsonSerialization
    {
        public static string Serialize(object source)
        {
            return Serialize(source, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static string Serialize(object source, bool isIgnoreJsonProperty)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            if (isIgnoreJsonProperty)
            {
                jsonSerializerSettings.ContractResolver = new IgnoreJsonPropertyResolver();
            }

            return Serialize(source, jsonSerializerSettings);
        }

        public static string Serialize(object source, JsonSerializerSettings settings)
        {
            if (source == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(source, settings);
        }

        public static TData Deserialize<TData>(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                return JsonConvert.DeserializeObject<TData>(source);
            }

            return default(TData);
        }

        public static TData Deserialize<TData>(string source, bool isIgnoreJsonProperty)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                return JsonConvert.DeserializeObject<TData>(source, isIgnoreJsonProperty ? new JsonSerializerSettings
                {
                    ContractResolver = new IgnoreJsonPropertyResolver()
                } : null);
            }

            return default(TData);
        }
    }

    internal class IgnoreJsonPropertyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
            foreach (JsonProperty item in list)
            {
                item.PropertyName = item.UnderlyingName;
            }

            return list;
        }
    }
}
