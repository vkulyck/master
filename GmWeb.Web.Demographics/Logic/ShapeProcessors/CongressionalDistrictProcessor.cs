using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Importing.Regions;
using GmWeb.Logic.Importing.Regions.ShapefileProcessors;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Web.Demographics.Logic.Data.Context;

namespace GmWeb.Web.Demographics.Logic.ShapeProcessors
{
    public class CongressionalDistrictProcessor : ShapefileProcessor
    {
        public int Generation { get; private set; }
        public List<CongressionalDistrictShape> Models { get; } = new List<CongressionalDistrictShape>();
        public override string ArchiveFilenamePattern => @"tl_(?<year>\d{4})_us_cd(?<generation>\d+).zip";
        protected override void ParseFilename(System.Text.RegularExpressions.Match match)
        {
            base.ParseFilename(match);
            this.Generation = int.Parse(match.Groups["generation"].Value);
        }

        public override void ClearExistingShapes()
        {
            using (var cache = new DemographicsContext())
            {
                var eset = cache.CongressionalDistrictShapes;
                eset.RemoveRange(eset);
            }
        }

        public override void ProcessShapefile(ShapeFile shapefile)
        {
            while (shapefile.HasShapes)
            {
                var model = shapefile.ReadMetadata<CongressionalDistrictShape>();
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
                cache.CongressionalDistrictShapes.AddRange(this.Models);
                cache.SaveChanges();
            }
        }
    }
}
