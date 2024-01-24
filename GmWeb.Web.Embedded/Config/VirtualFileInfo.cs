using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Owin.FileSystems;
using System.Web.Hosting;
using System.Reflection;

namespace GmWeb.Web.Embedded.Config
{
    public class VirtualFileInfo : IFileInfo
    {
        public static VirtualPathProvider VPP => HostingEnvironment.VirtualPathProvider;
        public static EmbeddedVirtualPathProvider EVPP => VPP as EmbeddedVirtualPathProvider;
        public VirtualFile VirtualFile { get; private set; }
        public long Length { get; private set; }
        public string PhysicalPath { get; private set; }
        public string Name => this.VirtualFile.Name;
        public DateTime LastModified => File.GetCreationTime(this.PhysicalPath);
        public bool IsDirectory => throw new NotImplementedException();

        public Stream CreateReadStream()
        {
            return this.VirtualFile.Open();
        }

        public VirtualFileInfo(VirtualFile virtualFile)
        {
            this.VirtualFile = virtualFile;
            using(var fstream = VirtualFile.Open() as FileStream)
            {
                if (fstream == null)
                    throw new Exception($"{this.GetType().Name} only supports physical files references.");
                this.Length = fstream.Length;
                this.PhysicalPath = fstream.Name;
            }
        }

        public override string ToString() => this.PhysicalPath;
    }
}