using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using Newtonsoft.Json.Linq;

namespace GmWeb.Logic.Utility.Config
{
    public class ConfigAccessor : IConfigurationRoot
    {
        private readonly IConfigurationRoot _root;

        #region IConfigurationRoot Members
        public IEnumerable<IConfigurationProvider> Providers => this._root.Providers;
        public void Reload() => this._root.Reload();
        #region IConfiguration Members
        public string this[string key] { get => this._root[key]; set => this._root[key] = value; }
        public IEnumerable<IConfigurationSection> GetChildren() => this._root.GetChildren();
        public IChangeToken GetReloadToken() => this._root.GetReloadToken();
        public IConfigurationSection GetSection(string key) => this._root.GetSection(key);
        #endregion
        #endregion

        #region Static Variables
        public static readonly string SaveName = "_BUILD_";
        public static readonly Regex ConfigNamePattern = new Regex(@"\bappsettings(?:\.(?<configName>|\w+(?:\.\w+)*))\.json$", RegexOptions.Compiled);
        public static string HostEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public static bool EnableReloadOnChange
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("DOTNET_hostBuilder__reloadConfigOnChange");
                if (string.IsNullOrWhiteSpace(env))
                    return true;
                if (bool.TryParse(env, out bool result))
                    return result;
                return false;
            }
        }
        public static IEnumerable<string> Defaults = new List<string> { string.Empty, HostEnvironment };
        public static IEnumerable<string> BuildExclusions => new List<string> { SaveName };
        private static List<string> _ConfigDirectoryNames => new() { "Settings" };
        public static string BasePath => AppContext.BaseDirectory;
        public static readonly IReadOnlyList<string> ConfigDirectories = _ConfigDirectoryNames.Select(n => Path.Combine(BasePath, n)).ToList();
        #endregion

        public ConfigAccessor()
        {
            this._root = new ConfigurationBuilder()
               .SetBasePath(BasePath)
               .AddDefaultConfigs()
               .AddExtendedConfigs()
               .Build()
            ;
        }
        public ConfigAccessor(IEnumerable<string> configs)
        {
            this._root = new ConfigurationBuilder()
               .SetBasePath(BasePath)
               .AddConfigs(configs)
               .Build()
            ;
        }
        public ConfigAccessor(params string[] configs) 
            : this(configs.AsEnumerable()) { }

        public JToken Serialize() => this._root.Serialize();

    }

    public static class ConfigAccessorExtensions
    {
        public static bool TryGetEnvironmentVariable(this string name, out string result)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The environment variable name must not be null or whitespace.", nameof(name));
            result = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(result))
                return false;
            return true;
        }
        public static bool TryGetEnvironmentVariable<T>(this string name, out T result)
            where T : struct
        {
            result = default;
            var strValue = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The environment variable name must not be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(strValue))
                return false;
            var success = strValue.TryParse<T>(out result);
            return success;
        }
        public static IConfigurationBuilder Save(this IConfigurationBuilder builder)
        {
            var isSaveDefined = "GMWEB_ENABLE_CONFIG_SAVE".TryGetEnvironmentVariable(out bool isSaveEnabled);
            if (!isSaveDefined || !isSaveEnabled)
                return builder;
            var jsonSources = builder.Sources
                .OfType<Microsoft.Extensions.Configuration.Json.JsonConfigurationSource>()
                .ToList()
            ;
            var jsonBuilder = new ConfigurationBuilder();
            jsonSources.ForEach(x => jsonBuilder.Add(x));
            var jsonConfig = jsonBuilder.Build();
            using var writer = new System.IO.StreamWriter($"{ConfigAccessor.BasePath}/Settings/appsettings.{ConfigAccessor.SaveName}.json");
            var json = jsonConfig.Serialize().ToString(Newtonsoft.Json.Formatting.Indented);
            json = Regex.Replace(json, @"""(True|False|\d+)""", m => m.Groups[1].Value.ToLower(), RegexOptions.IgnoreCase);
            writer.Write(json);
            return builder;
        }
        public static JToken Serialize(this IConfiguration config)
        {
            JObject obj = new JObject();
            foreach (var child in config.GetChildren())
            {
                obj.Add(child.Key, Serialize(child));
            }

            if (!obj.HasValues && config is IConfigurationSection section)
                return new JValue(section.Value);

            return obj;
        }

        private static string GetConfigNameFromPath(this string path)
        {
            var fileName = Path.GetFileName(path);
            var configName = ConfigAccessor.ConfigNamePattern.Replace(fileName, "${configName}");
            return configName;
        }
        private static IEnumerable<string> AvailableConfigs(IEnumerable<string> Exclusions)
            => ConfigAccessor.ConfigDirectories.SelectMany(directory =>
            {
                if (!Directory.Exists(directory))
                    return new string[] { };
                var files = Directory.GetFiles(directory, "appsettings.*.json", SearchOption.AllDirectories)
                    .Select(path => Path.GetRelativePath(directory, path))
                    .ExceptWhere(path => Exclusions.Contains(path.GetConfigNameFromPath()))
                ;
                return files;
            })
        ;

        /// <summary>
        /// Get the list of subdirectories of the provided configuration file, relative to the project's Settings directory.
        /// </summary>
        /// <example>
        /// GetSettingsSubdirectories("Datasets/appsettings.Solution.Local.json") => { "Datasets" }
        /// </example>
        /// <param name="settingsPath">The relative path of the configuration file.</param>
        /// <returns>The list of subdirectories that contain the configuration file.</returns>
        private static List<string> GetSettingsSubdirectories(this string settingsPath) =>
            Regex.Split(settingsPath.ToLower(), @"[\\/]+").ToList()
        ;

        /// <summary>
        /// Get the config file's descriptors, which are encoded as period-delimited strings 
        /// that occur within the config filename between 'appsettings' and the 'json' file 
        /// extension. 
        /// </summary>
        /// <example>
        /// Calling this function with the argument "Datasets/appsettings.Solution.Local.json") 
        /// returns the descriptors { "Solution", "Local" }
        /// </example>
        /// <param name="settingsPath">The relative path of the configuration file.</param>
        /// <returns>The list of components in the name of the configuration file.</returns>
        private static List<string> GetSettingsDescriptors(this string settingsPath) =>
            Regex.Split(
                Path.GetFileNameWithoutExtension(settingsPath).ToLower(),
                @"\."
            )
            .Skip(1) // Remove the initial 'appsettings' portion of the filename
            .ToList()
        ;

        /// <summary>
        /// A string comparer that compares appsettings file paths on the basis of configuration
        /// precedence.
        /// </summary>
        /// <remarks>
        /// The <see cref="ConfigPrecedenceComparer"/> class analyzes the filenames 
        /// of any appsettings files that are used to configure the application and 
        /// produces Compare results to achieve the configuration precedence defined. 
        /// Files with higher precedence are loaded later to ensure that, in the case 
        /// of key collisions, configuration values of higher precedence take effect.
        /// 
        /// Precedence is defined as follows, where file categories are ordered by 
        /// ascending precedence. Equivalently, categories are listed by their load 
        /// order. If two files fall within the same category then their load order
        /// is determined by the standard <b><see cref="String.Compare(string,string)"/></b>
        /// method.
        /// 
        /// <list type="number">
        ///     <item>Datasets (used for enumerations, dropdown lists, etc)</item>
        ///     <item>Solution-wide settings.</item>
        ///     <item>Solution-wide settings with the current environment descriptor.</item>
        ///     <item>Solution-wide settings with the Local descriptor.</item>
        ///     <item>Project-specific settings.</item>
        ///     <item>Project-specific settings with the current environment descriptor.</item>
        ///     <item>Project-specific settings with the Local descriptor.</item>
        ///     <item>User secrets.</item>
        /// </list>
        /// </remarks>
        private class ConfigPrecedenceComparer : IComparer<string>
        {
            private const int X_Before_Y = -1;
            private const int X_Equals_Y = 0;
            private const int X_After_Y = 1;

            private int IncreasePrecedenceIf(Func<List<string>, bool> predicate, List<string> xData, List<string> yData)
                => -1 * ReducePrecedenceIf(predicate, xData, yData);
            private int ReducePrecedenceIf(Func<List<string>,bool> predicate, List<string> xData, List<string> yData)
            {
                if (predicate(xData) && !predicate(yData))
                    return X_Before_Y;
                if (!predicate(xData) && predicate(yData))
                    return X_After_Y;
                return 0;
            }

            public int Compare(string x, string y)
            {
                List<string>
                    xDirs = x.GetSettingsSubdirectories(),
                    yDirs = y.GetSettingsSubdirectories(),
                    xDescriptors = x.GetSettingsDescriptors(),
                    yDescriptors = y.GetSettingsDescriptors()
                ;
                string
                    xName = Path.GetFileNameWithoutExtension(x),
                    yName = Path.GetFileNameWithoutExtension(y)
                ;
                var pathCompareResult = String.Compare(x, y);

                // Identical paths yield equal precedence.
                if (pathCompareResult == X_Equals_Y)
                    return X_Equals_Y;

                // Datasets are the lowest precedence.
                var datasetResult = ReducePrecedenceIf(list => list.Contains("datasets"), xDirs, yDirs);
                if (datasetResult != X_Equals_Y)
                    return datasetResult;

                // Solution-wide settings are lower precedence than project-specific settings.
                var solutionResult = ReducePrecedenceIf(list => list.Contains("solution"), xDescriptors, yDescriptors);
                if (solutionResult != X_Equals_Y)
                    return solutionResult;

                // Environment-specific settings are higher precedence than environment-agnostic settings.
                var envResult = IncreasePrecedenceIf(list => list.Contains(ConfigAccessor.HostEnvironment), xDescriptors, yDescriptors);
                if (envResult != X_Equals_Y)
                    return envResult;

                // Locally-defined settings are higher precedence than general settings.
                var localResult = IncreasePrecedenceIf(list => list.Contains("local"), xDescriptors, yDescriptors);
                if (localResult != X_Equals_Y)
                    return localResult;

                return String.Compare(x, y);
            }
        }
        private static IEnumerable<string> ExtendedConfigs()
            => AvailableConfigs(Exclusions: ConfigAccessor.Defaults)
            .OrderBy(x => x, new ConfigPrecedenceComparer())
        ;

        private static void LogLoadOrder(string configPath)
        {
            var relPath = Path.GetRelativePath(AppContext.BaseDirectory, configPath);
            var message = $"Ordered extended config: {relPath}";
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static IConfigurationBuilder AddConfigs(this IConfigurationBuilder builder, IEnumerable<string> Configs)
        {
            foreach (var config in Configs)
            {
                string fileName, configName;
                var match = ConfigAccessor.ConfigNamePattern.Match(config);
                if (match.Success)
                {
                    fileName = config;
                    configName = match.Groups["configName"].Value;
                }
                else
                {
                    fileName = ".".JoinNonNull("appsettings", config, "json");
                    configName = config;
                }
                if (ConfigAccessor.BuildExclusions.Contains(configName))
                    continue;
                foreach (var directory in ConfigAccessor.ConfigDirectories)
                {
                    string path = Path.Combine(directory, fileName);
                    builder = builder.AddJsonFile(
                        path: path,
                        optional: true,
                        reloadOnChange: ConfigAccessor.EnableReloadOnChange
                    );
                    LogLoadOrder(path);
                }
            }
            return builder;
        }
        public static IConfigurationBuilder AddExtendedConfigs(this IConfigurationBuilder builder)
            => builder.AddConfigs(ExtendedConfigs())
        ;

        public static IConfigurationBuilder AddDefaultConfigs(this IConfigurationBuilder builder)
            => builder
            .AddConfigs(ConfigAccessor.Defaults)
        ;

        public static IConfigurationBuilder AddSecretConfigs(this IConfigurationBuilder builder)
            => builder.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true, reloadOnChange: true)
        ;
    }
}
