using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;
using System.Reflection;
using System.Collections;

namespace GmWeb.Web.Embedded.Config
{
    // Nested class representing the "virtual file"
    public class EmbeddedVirtualDirectory : VirtualDirectory
    {
        public static VirtualPathProvider VPP => HostingEnvironment.VirtualPathProvider;
        public static EmbeddedVirtualPathProvider EVPP => VPP as EmbeddedVirtualPathProvider;
        public Assembly EmbeddingAssembly { get; private set; }
        public string ResourceName { get; private set; }
        public string Path { get; private set; }

        protected List<string> ChildFileResourceNames { get; private set; }
        public override IEnumerable Files
        {
            get
            {
                foreach (var name in this.ChildFileResourceNames)
                {
                    var path = EVPP.ResourceNameToUri(name, this.ResourceName);
                    yield return new EmbeddedVirtualFile(Path: path, ResourceName: name, EmbeddingAssembly: this.EmbeddingAssembly);
                }
            }
        }
        protected List<string> ChildDirectoryResourceNames { get; private set; }
        public override IEnumerable Directories
        {
            get
            {
                foreach (var name in this.ChildDirectoryResourceNames)
                {
                    var path = EVPP.ResourceNameToUri(name, this.ResourceName);
                    yield return new EmbeddedVirtualDirectory(Path: path, ResourceName: name, EmbeddingAssembly: this.EmbeddingAssembly);
                }
            }
        }

        public override IEnumerable Children
        {
            get
            {
                foreach(var name in this.ChildFileResourceNames.Union(this.ChildDirectoryResourceNames))
                {
                    var path = EVPP.ResourceNameToUri(name, this.ResourceName);
                    if(EVPP.IsFile(name))
                        yield return new EmbeddedVirtualFile(Path: path, ResourceName: name, EmbeddingAssembly: this.EmbeddingAssembly);
                    else
                        yield return new EmbeddedVirtualDirectory(Path: path, ResourceName: name, EmbeddingAssembly: this.EmbeddingAssembly);
                }
            }
        }

        public EmbeddedVirtualDirectory(string Path, string ResourceName, Assembly EmbeddingAssembly) : base(Path)
        {
            this.Path = Path;
            this.ResourceName = ResourceName;
            this.EmbeddingAssembly = EmbeddingAssembly;

            this.ChildFileResourceNames = EVPP.GetChildFiles(ResourceName);
            this.ChildDirectoryResourceNames = EVPP.GetChildDirectories(ResourceName);
        }

        public override string ToString() => this.Path;
    }
}