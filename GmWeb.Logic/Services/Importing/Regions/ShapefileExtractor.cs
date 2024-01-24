using GmWeb.Logic.Importing.Regions.ShapefileProcessors;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GmWeb.Logic.Importing.Regions
{
    public class ShapeFileExtractor<T> : IDisposable where T : ShapefileProcessor, new()
    {
        protected string ArchiveSourceDirectory { get; private set; }
        protected string TempOutputDirectory { get; set; }
        private List<ShapefileProcessor> _ArchiveFiles { get; set; }
        public IReadOnlyList<ShapefileProcessor> ArchiveFiles => this._ArchiveFiles;
        private List<ShapeFile> ShapeFiles { get; } = new List<ShapeFile>();
        public ShapeFileExtractor(string archiveDirectory)
        {
            this.ArchiveSourceDirectory = archiveDirectory;
            this._ArchiveFiles = Directory
                .GetFiles(archiveDirectory, "*.zip")
                .Select(x => ShapefileProcessorFactory.Create<T>(x))
                .ToList()
            ;
        }

        public IEnumerable<ShapeFile> ExtractShapefiles(ShapefileProcessor archive)
        {
            if (this.TempOutputDirectory == null)
                this.TempOutputDirectory = this.GetTemporaryDirectory();
            string subDir = archive.Filename;
            string subPath = Path.Combine(this.TempOutputDirectory, subDir);
            Directory.CreateDirectory(subPath);
            ZipFile.ExtractToDirectory(archive.Path, subPath);
            string[] paths = Directory.GetFiles(subPath, "*.shp");

            foreach (string path in paths)
            {
                var shapeFile = new ShapeFile(path);
                this.ShapeFiles.Add(shapeFile);
                yield return shapeFile;
            }
        }

        protected IEnumerable<ShapeFile> ExtractShapefiles(IEnumerable<ShapefileProcessor> archives)
        {
            if (this.TempOutputDirectory == null)
                this.TempOutputDirectory = this.GetTemporaryDirectory();
            foreach (var archive in archives)
            {
                var extracted = this.ExtractShapefiles(archive);
                foreach (var shapeFile in extracted)
                    yield return shapeFile;
            }
        }

        protected string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), $"shapefiles-{Path.GetRandomFileName()}");
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public void Dispose()
        {
            foreach (var shapeFile in this.ShapeFiles)
            {
                shapeFile.Dispose();
            }
            if (!string.IsNullOrEmpty(this.TempOutputDirectory) && Directory.Exists(this.TempOutputDirectory))
            {
                Directory.Delete(this.TempOutputDirectory, true);
            }
        }
    }
}
