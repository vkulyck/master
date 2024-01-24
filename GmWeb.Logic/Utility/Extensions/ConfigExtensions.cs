using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Utility.Extensions
{
    public static class ConfigExtensions
    {
        public static string ClassConfig<T>(this T instance, string key) where T : class
        {
            string typeName = typeof(T).Name;
            if (typeName.Contains('`'))
                typeName = typeName.Substring(0, typeName.IndexOf("`"));
            string ns = typeof(T).Namespace;
            string baseKey = $"{ns}.{typeName}";
            string fqKey = $"{baseKey}.{key}";
            return ConfigurationManager.AppSettings[fqKey]?.ToString();
        }

        public static T ChangeType<T>(this string text)
        {
            if (text == null)
                return default(T);
            return (T)Convert.ChangeType(text, typeof(T));
        }

        public static List<TData> ClassConfigs<TInst, TData>(this TInst instance, string key) where TInst : class
        {
            string collectionStr = instance.ClassConfig(key);
            if (string.IsNullOrWhiteSpace(collectionStr))
                return null;
            var pieces = Regex.Split(collectionStr, @"\s*,\s*").Select(x => x.Trim());
            var converted = pieces.Select(x => x.ChangeType<TData>()).ToList();
            return converted;
        }

        public static List<int> IntClassConfigs<TInst>(this TInst instance, string key) where TInst : class
        {
            string collectionStr = instance.ClassConfig(key);
            if (string.IsNullOrWhiteSpace(collectionStr))
                return null;
            var pieces = Regex.Split(collectionStr, @"\s*,\s*").Select(x => x.Trim());
            var converted = pieces.Select(x => x.ChangeType<int>()).ToList();
            return converted;
        }
        public static List<string> StrClassConfigs<TInst>(this TInst instance, string key) where TInst : class
        {
            string collectionStr = instance.ClassConfig(key);
            if (string.IsNullOrWhiteSpace(collectionStr))
                return null;
            var pieces = Regex.Split(collectionStr, @"\s*,\s*").Select(x => x.Trim());
            var converted = pieces.ToList();
            return converted;
        }

        public static string UnifiedConnectionLookup(string connectionName)
        {
            object value = null;
            if ((value = ConfigurationManager.AppSettings[connectionName]) != null)
                return value.ToString();
            if ((value = ConfigurationManager.ConnectionStrings[connectionName]) != null)
                return value.ToString();
            return null;
        }
    }
}
