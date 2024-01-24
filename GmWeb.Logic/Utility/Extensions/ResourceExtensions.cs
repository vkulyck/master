using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Utility.Extensions.Resources
{
    public static class ResourceExtensions
    {
        private static Dictionary<string, Assembly> ResourceAssemblyMap { get; } = new Dictionary<string, Assembly>();
        private static HashSet<Assembly> ResourceAssemblies { get; } = new HashSet<Assembly>();

        static ResourceExtensions()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.IsAnonymousAssembly())
                    continue;
                if (assembly.IsSystemAssembly())
                    continue;
                foreach (string name in assembly.GetManifestResourceNames())
                {
                    ResourceAssemblyMap.Add(name, assembly);
                    ResourceAssemblies.Add(assembly);
                }
            }
        }
        private static bool IsDescendantOf(this string path, string directory)
        {
            var parentDir = new DirectoryInfo(directory);
            var childDir = new DirectoryInfo(path);
            bool isDescendant = false;
            while (childDir.Parent != null)
            {
                if (childDir.Parent.FullName.ToLower() == parentDir.FullName.ToLower())
                {
                    isDescendant = true;
                    break;
                }
                else childDir = childDir.Parent;
            }
            return isDescendant;
        }

        public static bool IsAnonymousAssembly(this Assembly assembly)
        {
            if (assembly.FullName.StartsWith("Anonymously Hosted"))
                return true;
            try
            {
                string location = assembly.Location;
            }
            catch
            {
                return true;
            }
            return false;
        }

        public static bool IsSystemAssembly(this Assembly assembly)
        {
            if (assembly.FullName.StartsWith("GmWeb"))
                return false;
            if (assembly.Location.IsSystemLocation())
                return true;
            var systemNamespaces = new List<string>
            {
                "Microsoft",
                "System"
            };
            foreach (string sysNs in systemNamespaces)
            {
                if (assembly.FullName.ToLower().StartsWith(sysNs.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSystemLocation(this string path)
        {
            var systemDirectories = new List<string>
            {
                @"C:\Windows\Microsoft.NET"
            };
            foreach (string sysDir in systemDirectories)
            {
                if (path.IsDescendantOf(sysDir))
                    return true;
            }
            return false;
        }
        private static string FullyQualifiedResourceName(Assembly assembly, string resourceName)
        {
            string assemblyName = assembly.GetName().Name;
            string sanitizedResourceName = resourceName
                .Replace(" ", "_")
                .Replace("\\", ".")
                .Replace("/", ".")
            ;
            string result = $"{assemblyName}.{sanitizedResourceName}";
            return result;
        }

        private static StreamReader GetEmbeddedResourceStream(string resourceName) => GetEmbeddedResourceStream(resourceName, null);
        private static StreamReader GetEmbeddedResourceStream(string resourceName, Assembly assembly)
        {
            if (assembly == null)
                assembly = GetEmbeddedResourceAssembly(resourceName);
            string fqResourceName = FullyQualifiedResourceName(assembly, resourceName);
            var resourceStream = assembly.GetManifestResourceStream(fqResourceName);
            return new StreamReader(resourceStream);
        }

        private static Assembly GetEmbeddedResourceAssembly(string resourceName)
        {
            foreach (var assembly in ResourceAssemblies)
            {
                string fqName = FullyQualifiedResourceName(assembly, resourceName);
                if (ResourceAssemblyMap.ContainsKey(fqName))
                    return assembly;
            }
            throw new ArgumentException($"Resource not found in any referenced assemblies: {resourceName}");
        }

        public static string GetEmbeddedResource(string ResourceName, Assembly Assembly)
        {
            if (Assembly == null)
                Assembly = GetEmbeddedResourceAssembly(ResourceName);
            using (var stream = GetEmbeddedResourceStream(ResourceName, Assembly))
            {
                return stream.ReadToEnd();
            }
        }

        public static string GetEmbeddedResource(string resourceName) => GetEmbeddedResource(resourceName, null);

        public static IEnumerable<string> IterateEmbeddedResourceLines(string resourceName)
        {
            using (var stream = GetEmbeddedResourceStream(resourceName))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    yield return line;
                }
            }
            yield break;
        }

        public static string RegexReplace(this string input, string pattern, string replacement)
        {
            string result = Regex.Replace(input, pattern, replacement);
            return result;
        }
        public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options)
        {
            string result = Regex.Replace(input, pattern, replacement, options);
            return result;
        }
    }
}
