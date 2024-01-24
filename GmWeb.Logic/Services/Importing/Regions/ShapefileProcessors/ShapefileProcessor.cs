using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Regions.ShapefileProcessors
{
    public abstract class ShapefileProcessor
    {
        public FileInfo FileInfo { get; protected set; }
        public string Filename => this.FileInfo.Name;
        public string Directory => this.FileInfo.DirectoryName;
        public string Path => this.FileInfo.FullName;
        public abstract string ArchiveFilenamePattern { get; }
        private Regex _Regex { get; set; }
        public Regex FilenameRegex => this._Regex ?? (this._Regex = new Regex(this.ArchiveFilenamePattern));
        public int Year { get; protected set; }

        public void Initialize(string archivePath)
        {
            this.FileInfo = new FileInfo(archivePath);
            this.ParseFilename(this.Filename);
        }

        protected void ParseFilename(string filename)
        {
            var match = this.FilenameRegex.Match(this.Filename);
            if (!match.Success)
                throw new Exception($"Invalid tract filename found: {this.Filename}");
            this.ParseFilename(match);
        }

        protected virtual void ParseFilename(Match match) => this.Year = Convert.ToInt32(match.Groups["year"].Value);

        public abstract Task ProcessShapefileAsync(ShapeFile shapefile);
        public abstract Task DeleteShapesAsync();
        public abstract Task ImportAsync();
    }
    public abstract class ShapefileProcessor<TModel> : ShapefileProcessor where TModel : class
    {
        protected List<TModel> Models { get; } = new List<TModel>();
    }
}
