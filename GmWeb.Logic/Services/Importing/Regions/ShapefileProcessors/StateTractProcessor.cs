using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Geography;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Regions.ShapefileProcessors
{
    public class StateTractProcessor : ShapefileProcessor<CensusTractShape>
    {
        public IDemographicsContext Cache { get; protected set; }
        public int StateID { get; private set; }
        public override string ArchiveFilenamePattern => @"tl_(?<year>\d{4})_(?<state_id>\d{2})_tract.zip";
        protected override void ParseFilename(System.Text.RegularExpressions.Match match)
        {
            base.ParseFilename(match);
            this.StateID = int.Parse(match.Groups["state_id"].Value);
        }

        public override async Task DeleteShapesAsync()
        {
            using (var cache = this.Cache.CreateNew())
            {
                await cache.TruncateAsync<CensusTractShape>();
            }
        }

        public override async Task ProcessShapefileAsync(ShapeFile shapefile)
        {
            while (shapefile.HasShapes)
            {
                // TODO: Migrate reader functions to async alternatives.
                var model = shapefile.ReadMetadata<CensusTractShape>();
                var shape = shapefile.ReadShape();
                shape = GeometryUtility.SanitizeShape(shape);
                model.GEOM = shape;
                this.Models.Add(model);
            }
            await Task.CompletedTask;
        }

        public override async Task ImportAsync()
        {
            using (var cache = this.Cache.CreateNew())
            {
                await cache.BulkInsertAsync(this.Models);
            }
        }
    }
}
