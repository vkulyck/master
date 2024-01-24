using NetTopologySuite.Geometries;

namespace GmWeb.Logic.Importing.Regions
{
    public class ShapeData
    {
        public string GeoID { get; set; }
        public Geometry Shape { get; set; }
    }
}
