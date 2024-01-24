using AutoMapper;
using GmWeb.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GmWeb.Logic.Utility.Extensions
{
    public static class SerializationExtensions
    {
        public static Dictionary<Type, MapperConfiguration> AutoConfigs { get; } = new Dictionary<Type, MapperConfiguration>();

        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public static T RemoveProxy<T>(this T proxyObject) where T : class
        {
            if (!AutoConfigs.TryGetValue(typeof(T), out var config))
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<T, T>());
                AutoConfigs.Add(typeof(T), config);
            }
            var mapper = config.CreateMapper();
            var mapped = mapper.Map<T>(proxyObject);
            return mapped;
        }

        public static T UnProxy<T>(this DbContext context, T proxyObject) where T : class
        {
            var poco = context.Entry(proxyObject).CurrentValues.ToObject() as T;
            return poco;
        }

        public static U DeepCopyConversion<T, U>(this T original)
        {
            string serialized = JsonConvert.SerializeObject(original, JsonSerializerSettings);
            var copy = JsonConvert.DeserializeObject<U>(serialized, JsonSerializerSettings);
            return copy;
        }

        public static string Serialize<T>(this T model) where T : IViewModel
        {
            string json = JsonConvert.SerializeObject(model, JsonSerializerSettings);
            return json;
        }

        public static T Deserialize<T>(this string json) where T : IViewModel
        {
            var model = JsonConvert.DeserializeObject<T>(json);
            return model;
        }
    }
}
