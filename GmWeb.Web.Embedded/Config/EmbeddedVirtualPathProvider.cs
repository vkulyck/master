using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Optimization;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Enums;
using System.Diagnostics;

namespace GmWeb.Web.Embedded.Config
{

    public class EmbeddedVirtualPathProvider : VirtualPathProvider
    {
        protected Assembly Assembly { get; private set; }
        public DirectoryInfo SourceAssemblyDirectory => Directory.GetParent(new Uri(this.Assembly.GetName().CodeBase).LocalPath);
        public IEnumerable<ResourceDesignation> ValidResourceDesignations
        {
            get
            {
                yield return ResourceDesignation.Content;
                yield return ResourceDesignation.Scripts;
            }
        }
        protected string RootNamespace => this.Assembly.GetName().Name;
        protected string RootPattern => $@"^(?:\~?\/)?(?<designation>{string.Join("|", this.ValidResourceDesignations)})";
        protected HashSet<string> FileResources { get; private set; }
        protected HashSet<string> DirectoryResources { get; private set; } = new HashSet<string>();
        protected Dictionary<string, string> ResourceNameToPath { get; private set; }
        protected Dictionary<string, string> PathToResourceName { get; private set; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        protected Dictionary<string, List<string>> DirectoryFiles { get; private set; }
        protected Dictionary<string, List<string>> DirectorySubDirectories { get; private set; }

        private string getResourceDirectory(string rsName)
        {
            var parent = this.DirectoryResources
                .Where(x => rsName.StartsWith(x))
                .Where(x => x != rsName)
                .OrderByDescending(x => x.Length)
                .FirstOrDefault()
            ;
            return parent;
        }
        protected Dictionary<string, List<string>> buildDirectoryTree()
        {
            var fileMap = mapChildren(this.DirectoryResources);
            return fileMap;
        }
        protected Dictionary<string, List<string>> buildFileTree()
        {
            var items = this.Assembly.GetManifestResourceNames().ToList();
            var fileMap = mapChildren(items);
            return fileMap;
        }
        protected Dictionary<string, List<string>> mapChildren(IEnumerable<string> items)
        {
            var childMap = items
                .GroupBy(name => getResourceDirectory(name))
                .Where(grp => grp.Key != null)
                .ToDictionary(
                    grp => grp.Key,
                    grp => grp.ToList()
                )
            ;
            foreach (var item in items)
                if (!childMap.ContainsKey(item))
                    childMap[item] = new List<string>();
            return childMap;
        }

        protected enum EntryType { File, Directory };
        protected List<string> ReadEmbeddedEntries(EntryType type)
        {
            string listFile = $@"{this.SourceAssemblyDirectory}\Config\Embedded{type}List.txt";
            List<string> entries = new List<string>();
            using (var reader = new StreamReader(listFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    entries.Add(line);
                }
            }
            return entries;
        }
        protected void mapPathsToEmbeddingNames()
        {
            // Map directory embedding names to relative physical directory paths
            var dirList = ReadEmbeddedEntries(EntryType.Directory);
            var fileList = ReadEmbeddedEntries(EntryType.File);
            var fileMap = fileList
                .GroupBy(x => Path.GetDirectoryName(x))
                .ToDictionary(x => x.Key, x => x.ToList())
            ;
            foreach (var pDir in dirList)
            {
                var pdPath = NormalizeResourcePath(pDir);
                var pdResourceName = UriToResourceName(pdPath);
                this.PathToResourceName[pdPath] = pdResourceName;
                this.DirectoryResources.Add(pdResourceName);
                if (!fileMap.TryGetValue(pDir, out List<string> physicalFiles))
                    continue;
                foreach (var pFile in physicalFiles)
                {
                    var pfPath = NormalizeResourcePath(pFile);
                    var pname = Path.GetFileName(pFile);
                    var relFilePath = $"{pDir}/{pname}";
                    var pfResourceName = UriToResourceName(pfPath);
                    this.PathToResourceName[pfPath] = pfResourceName;
                }
            }
            this.ResourceNameToPath = this.PathToResourceName.Invert();
        }

        public EmbeddedVirtualPathProvider() : this(typeof(EmbeddedVirtualPathProvider)) { }
        private EmbeddedVirtualPathProvider(Type rootType)
        {
            this.Assembly = Assembly.GetAssembly(rootType);
            this.FileResources = this.Assembly.GetManifestResourceNames().ToHashSet();
            mapPathsToEmbeddingNames(); // also builds this.DirectoryResources
            this.DirectorySubDirectories = buildDirectoryTree();
            this.DirectoryFiles = buildFileTree();
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart)
        {
            if (BundleTable.Bundles.Any(b => b.Path == virtualPath))
            {
                return null;
            }
            if (this.IsEmbeddedPath(virtualPath))
            {
                return null;
            }
            else
            {
                return this.Previous
                           .GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
        }

        public override bool FileExists(string virtualPath)
        {
            var virtualExists = this.Previous?.FileExists(virtualPath) ?? false;
            var embeddedExists = this.IsEmbeddedFilePath(virtualPath);
            var result = virtualExists || embeddedExists;
            return result;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            VirtualFile vf = null;
            if(this.IsEmbeddedFilePath(virtualPath))
            {
                string name = UriToResourceName(virtualPath);
                if (string.IsNullOrEmpty(name))
                    return null;

                vf = new EmbeddedVirtualFile(Path: virtualPath, ResourceName: name, EmbeddingAssembly: this.Assembly);
            }
            else if (this.Previous != null)
            {
                vf = this.Previous.GetFile(virtualPath);
            }
            return vf;
        }

        public override bool DirectoryExists(string virtualPath)
        {
            var virtualExists = this.Previous?.DirectoryExists(virtualPath) ?? false;
            var embeddedExists = this.IsEmbeddedDirectoryPath(virtualPath);
            var result = virtualExists || embeddedExists;
            return result;
        }

        public override VirtualDirectory GetDirectory(string virtualPath)
        {
            VirtualDirectory vdir = null;
            if (this.IsEmbeddedDirectoryPath(virtualPath))
            {
                string key = UriToResourceName(virtualPath);
                if (string.IsNullOrEmpty(key))
                    return null;

                vdir = new EmbeddedVirtualDirectory(Path: virtualPath, ResourceName: key, EmbeddingAssembly: this.Assembly);
            }
            else if (this.Previous != null)
            {
                vdir = this.Previous.GetDirectory(virtualPath);
            }
            return vdir;
        }

        public List<string> GetChildFiles(string directory)
        {
            if (this.DirectoryFiles.TryGetValue(directory, out List<string> fileNames))
            {
                return fileNames;
            }
            throw new ArgumentException($"No subdirectory entry found for resource directory: {directory}");
        }
        public List<string> GetChildDirectories(string directory)
        {
            if (this.DirectorySubDirectories.TryGetValue(directory, out List<string> directoryNames))
            {
                return directoryNames;
            }
            throw new ArgumentException($"No subdirectory entry found for resource directory: {directory}");
        }

        private string JoinNames(params string[] names)
        {
            var nonEmpty = names.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            if (nonEmpty.Count == 0)
                return string.Empty;
            string joined = string.Join(".", nonEmpty);
            return joined;
        }
        private string JoinPaths(params string[] paths)
        {
            var nonEmpty = paths.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            if (nonEmpty.Count == 0)
                return string.Empty;
            string joined = string.Join("/", nonEmpty);
            return joined;
        }
        public string ResourceNameToUri(string resourceName, string directoryResourceName)
        {
            var directoryPath = this.NormalizeResourcePath(this.ResourceNameToPath[directoryResourceName]);
            var resourceFile = Regex.Replace(resourceName, $@"^{Regex.Escape(directoryResourceName)}\.", string.Empty);
            var uri = this.JoinPaths("~", directoryPath, resourceFile);
            return uri;
        }

        private string UriToResourceName(string uri)
        {
            uri = uri.Trim();
            var path = NormalizeResourcePath(uri);
            if (this.PathToResourceName.TryGetValue(path, out string mappedResourceName))
            {
                return mappedResourceName;
            }
            var file = Path.GetFileName(path);
            string directory = null;
            if (!string.IsNullOrWhiteSpace(file))
                directory = Path.GetDirectoryName(path)
                    .Replace("/", ".")
                    .Replace("\\", ".")
                    .Replace("-", "_") // C# silently replaces hyphens with underscores in embedded directory names
                ;
            if (file == directory)
                throw new Exception($"Error: Invalid directory/filename deduced from embedded resource uri: Directory: '{directory}', File: '{file}'");

            string resourceName = this.JoinNames(this.RootNamespace, directory, file);
            return resourceName;
        }

        private string NormalizeResourcePath(string path)
        {
            // Replace windows slashes with uri slashes
            path = path.Replace("\\", "/");

            // Remove protocol.
            path = path.RegexReplace(@"^(file|https?)://", string.Empty);

            // Remove leading tilde and/or slash.
            path = path.RegexReplace(@"^~?/?|/?$", string.Empty);

            return path;
        }

        public bool IsFile(string name) => this.FileResources.Contains(name);
        public bool IsDirectory(string name) => this.DirectoryResources.Contains(name);
        private bool IsEmbeddedPath(string path)
        {
            var isMatch = Regex.IsMatch(path, RootPattern);
            if (!isMatch)
                return false;
            var normalizedPath = this.NormalizeResourcePath(path);
            var isMapped = this.PathToResourceName.ContainsKey(normalizedPath);
            return isMapped;
        }
        private bool IsEmbeddedFilePath(string path)
        {
            if (!this.IsEmbeddedPath(path))
                return false;
            var resourceName = this.UriToResourceName(path);
            return this.IsFile(resourceName);
        }        
        private bool IsEmbeddedDirectoryPath(string path)
        {
            if (!this.IsEmbeddedPath(path))
                return false;
            var resourceName = this.UriToResourceName(path);
            return this.IsDirectory(resourceName);
        }
    }
}
