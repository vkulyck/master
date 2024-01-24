using GmWeb.Logic.Interfaces;
using ShellProgressBar;
using System;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Regions
{
    public class ShapeImporter<T> : IDataImporter where T : ShapefileProcessors.ShapefileProcessor, new()
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ShapeFileExtractor<T> ShapefileExtractor { get; private set; }
        public string ArchiveDirectory { get; private set; }

        public ShapeImporter(string archiveDirectory)
        {
            this.ArchiveDirectory = archiveDirectory;
            this.ShapefileExtractor = new ShapeFileExtractor<T>(archiveDirectory);
        }
        public async Task ImportAsync()
        {
            using (var pBar = new ProgressBar(this.ShapefileExtractor.ArchiveFiles.Count, "Processing shape file archives.", ConsoleColor.Cyan))
            {
                for (int i = 0; i < this.ShapefileExtractor.ArchiveFiles.Count; i++)
                {
                    var archive = this.ShapefileExtractor.ArchiveFiles[i];
                    await archive.DeleteShapesAsync();
                    var shapefiles = this.ShapefileExtractor.ExtractShapefiles(archive);
                    foreach (var shapefile in shapefiles)
                    {
                        await archive.ProcessShapefileAsync(shapefile);
                        await archive.ImportAsync();
                        string filename = archive.Filename;
                        int total = this.ShapefileExtractor.ArchiveFiles.Count;
                        pBar.Tick($"Processed {filename} ({i:N0} / {total:N0})");
                    }
                }
            }
        }

        public void Dispose() => this.ShapefileExtractor?.Dispose();
    }
}
