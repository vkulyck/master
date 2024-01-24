using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Importing.Regions;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Importing.Regions.ShapefileProcessors;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Web.Demographics.Logic.Data.Context;
using EFCore.BulkExtensions;

namespace GmWeb.Web.Demographics.Logic.ShapeProcessors
{
    public class CountyProcessor : ShapefileProcessor
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<CountyShape> Models { get; } = new List<CountyShape>();
        public override string ArchiveFilenamePattern => @"tl_(?<year>\d{4})_us_county.zip";
        protected override void ParseFilename(System.Text.RegularExpressions.Match match)
        {
            base.ParseFilename(match);
        }

        public override void ClearExistingShapes()
        {
            using (var cache = new DemographicsContext())
            {
                var deleted = cache.CountyShapes.BatchDelete();
                _logger.Info($"Batch delete completed for County shapes; {deleted} records removed.");
            }
        }

        public override void ProcessShapefile(ShapeFile shapefile)
        {
            while (shapefile.HasShapes)
            {
                var model = shapefile.ReadMetadata<CountyShape>();
                var shape = shapefile.ReadShape();
                shape = GeometryUtility.SanitizeShape(shape);
                model.GEOM = shape;
                this.Models.Add(model);
            }
        }
        public override void ImportShapes()
        {
            using (var cache = new DemographicsContext())
            {
                var properties = typeof(CountyShape)
                    .InferDatabaseProperties()
                    .Except("ID", "GEOM")
                    .ToList()
                ;
                // TODO: Fix GEOM issues so that this field can be bulk updated.
                _logger.Info($"Bulk updating {this.Models.Count} County Shapes on properties: {string.Join(",", properties)}");
                cache.ChangeTracker.AutoDetectChangesEnabled = false;
                cache.BulkInsert(this.Models, new BulkConfig
                    {
                        BatchSize = 250,
                        CalculateStats = true,
                        PreserveInsertOrder = true,
                        PropertiesToInclude = properties
                    }
                );
                var geoidMap = cache.CountyShapes.ToDictionary(x => x.GEOID, x => x.ID);
                this.Models.ForEach(x => x.ID = geoidMap[x.GEOID]);
            }
            var updates = this.Models.Select(x => new CountyShape { GEOM = x.GEOM, ID = x.ID }).ToList();
            var chunks = updates.ChunkBy(100);
            for(int i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                CountyShape first = chunk.FirstOrDefault(), last = chunk.LastOrDefault();
                _logger.Info($"Performing final geom updates on county shapes {first?.ID} to {last?.ID}");
                using (var cache = new DemographicsContext())
                {
                    cache.CountyShapes.AttachRange(chunk);
                    foreach (var item in chunk)
                        cache.Entry(item).Property(x => x.GEOM).IsModified = true;
                    cache.SaveChanges();
                }
            }
        }
    }
}
