using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.FileSystems;
using System.Web.Hosting;

namespace GmWeb.Web.Embedded.Config
{
    /// <summary>
    /// Represents a virtual file system.
    /// A wrapper around <see cref="MyCustomVirtualPathProvider"/> implementing
    /// IFileSystem for use in Owin StaticFiles.
    /// </summary>
    public class EmbeddedVirtualFileSystem : IFileSystem
    {
        public static VirtualPathProvider VPP => HostingEnvironment.VirtualPathProvider;
        public static EmbeddedVirtualPathProvider EVPP => VPP as EmbeddedVirtualPathProvider;
        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            if(!VPP.DirectoryExists(subpath))
            {
                contents = null;
                return false;
            }
            var directory = VPP.GetDirectory(subpath);
            contents = directory.Children.OfType<EmbeddedVirtualFileInfo>();
            return true;
        }

        /// <summary>
        /// Locate the path in the virtual path provider
        /// </summary>
        /// <param name="subpath">The path that identifies the file</param>
        /// <param name="fileInfo">The discovered file if any</param>
        /// <returns>
        /// True if a file was located at the given path
        /// </returns>
        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            if (!VPP.FileExists(subpath))
            {
                fileInfo = null;
                return false;
            }
            var vFile = VPP.GetFile(subpath);
            var evFile = vFile as EmbeddedVirtualFile;
            if (evFile == null)
                fileInfo = new VirtualFileInfo(vFile);
            else
                fileInfo = new EmbeddedVirtualFileInfo(evFile);
            return true;
        }
    }
}