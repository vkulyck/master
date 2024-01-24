using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GMaps.Mvc;

namespace GmWeb.Web.Demographics.Helpers
{
    public class GeoLocations
    {
        public static readonly Action<CenterFactory> SanFrancisco = cf => cf.Latitude(37.774546).Longitude(-122.433523);
    }
}