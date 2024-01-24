using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.Owin.FileSystems;
using System.Web.Hosting;

namespace GmWeb.Web.Embedded.Config
{
    public class EmbeddedVirtualFileInfo : IFileInfo
    {
        public static VirtualPathProvider VPP => HostingEnvironment.VirtualPathProvider;
        public static EmbeddedVirtualPathProvider EVPP => VPP as EmbeddedVirtualPathProvider;
        public Stream CreateReadStream() => VirtualFile.Open();
        public long Length => VirtualFile.Length;
        public string Name => VirtualFile.Name;
        public DateTime LastModified => File.GetCreationTime(this.VirtualFile.EmbeddingAssembly.Location);
        public string PhysicalPath => null;
        public bool IsDirectory => VirtualFile.IsDirectory;
        private EmbeddedVirtualFile VirtualFile { get; set; }

        /// <summary>
        /// Construct using a <see cref="EmbeddedResourceVirtualFile"/>
        /// </summary>
        /// <param name="virtualFile"></param>
        public EmbeddedVirtualFileInfo(EmbeddedVirtualFile virtualFile)
        {
            this.VirtualFile = virtualFile;
        }

        public override string ToString() => this.PhysicalPath;
    }
}