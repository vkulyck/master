using GmWeb.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GmWeb.Logic.Utility.Extensions.Reflection;

namespace GmWeb.Logic.Utility.Extensions
{
    public static class ModelExtensions
    {
        private static readonly HashSet<string> DataAssemblyNames = new HashSet<string> { "GmWeb.Logic" };
        private static readonly HashSet<string> ViewAssemblyNames = new HashSet<string> { "GmWeb.Web", "GmWeb.Web.Common", "GmWeb.Web.Demographics" };
        public static IEnumerable<Assembly> DataAssemblies => AppDomain.CurrentDomain.GetAssemblies().Where(x => DataAssemblyNames.Contains(x.FullName));
        public static IEnumerable<Assembly> ViewAssemblies => AppDomain.CurrentDomain.GetAssemblies().Where(x => ViewAssemblyNames.Contains(x.FullName));
        public enum TypeCollection
        {
            Unspecified,
            ViewModel,
            DataModel
        };
        private static Dictionary<(string, TypeCollection), Type> TypeCache { get; } = new Dictionary<(string, TypeCollection), Type>();
        private static IEnumerable<Assembly> GetTypeAssemblies(TypeCollection typeCollection)
        {
            switch (typeCollection)
            {
                case TypeCollection.DataModel:
                    return DataAssemblies;
                case TypeCollection.ViewModel:
                    return ViewAssemblies;
                default:
                    return AppDomain.CurrentDomain.GetAssemblies();
            }
        }
        public static Type GetModelType(string typeName, TypeCollection typeCollection = TypeCollection.Unspecified)
        {
            lock (TypeCache)
            {
                var key = (typeName, typeCollection);
                if (TypeCache.TryGetValue(key, out var t))
                    return t;

                var assemblies = GetTypeAssemblies(typeCollection);
                foreach (var assembly in assemblies)
                {
                    var allTypes = assemblies.SelectMany(x => x.DefinedTypes);
                    if (typeCollection == TypeCollection.ViewModel)
                        allTypes = allTypes.Where(x => typeof(IViewModel).IsAssignableFrom(x));
                    var modelNamespaces = allTypes.Where(x => x.FullName.Contains(".Models.")).Select(x => x.Namespace).Distinct().ToList();
                    foreach (string prefix in modelNamespaces)
                    {
                        string targetPrefix = $"{prefix}.{typeName}";
                        t = allTypes.SingleOrDefault(x => x.FullName == targetPrefix);
                        if (t != null)
                        {
                            TypeCache[key] = t;
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        public static string GenerateGuid()
        {
            string guid = System.Guid.NewGuid().ToString("N");
            return guid;
        }

        public static IList<string> InferDatabaseProperties(this Type modelType)
        {
            var properties = modelType.GetProperties()
                .Where(x => x.SetMethod != null)
                .Where(x => x.SetMethod.IsPublic)
                .Where(x => x.IsMapped())
                .Where(x => !x.IsNavigation())
                .Where(x => !x.IsRelationalProperty())
                .Select(x => x.Name)
                .ToList()
            ;
            return properties;
        }
        public static IList<string> InferDatabaseProperties<T>(this T model) where T : class
            => typeof(T).InferDatabaseProperties()
        ;
    }
}
