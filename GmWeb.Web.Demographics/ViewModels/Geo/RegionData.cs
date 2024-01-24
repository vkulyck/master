using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetTopologySuite.Geometries;
using System.Dynamic;

namespace GmWeb.Web.Demographics.ViewModels.Geo
{
    public class RegionData
    {
        public Geometry GEOM { get; set; }
        public double Latitude => this.GEOM.Centroid.Y;
        public double Longitude => this.GEOM.Centroid.X;
        public double Weight { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public dynamic Stats { get; set; } = new ExpandoObject();
    }
}