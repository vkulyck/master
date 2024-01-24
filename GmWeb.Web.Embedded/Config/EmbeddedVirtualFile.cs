using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;
using System.Reflection;

namespace GmWeb.Web.Embedded.Config
{
    // Nested class representing the "virtual file"
    public class EmbeddedVirtualFile : VirtualFile
    {
        public static VirtualPathProvider VPP => HostingEnvironment.VirtualPathProvider;
        public static EmbeddedVirtualPathProvider EVPP => VPP as EmbeddedVirtualPathProvider;
        public Assembly EmbeddingAssembly { get; private set; }
        public string ResourceName { get; private set; }
        public string Path { get; private set; }
        public long Length { get; private set; }
        //public string PhysicalPath => this.EmbeddingAssembly.Location;
        //public DateTime LastModified => File.GetCreationTime(this.EmbeddingAssembly.Location);

        public EmbeddedVirtualFile(string Path, string ResourceName, Assembly EmbeddingAssembly) : base(Path)
        {
            this.Path = Path;
            this.ResourceName = ResourceName;
            this.EmbeddingAssembly = EmbeddingAssembly;
            using (var stream = this.EmbeddingAssembly.GetManifestResourceStream(this.ResourceName))
            {
                this.Length = stream.Length;
            }
        }

        public override Stream Open()
        {
            return this.EmbeddingAssembly.GetManifestResourceStream(this.ResourceName);
        }

        public override string ToString() => this.Path;
    }
}