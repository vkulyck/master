using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Demographics.ViewModels.Geo
{
    public class MarkerData
    {
        public string Label { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImagePath { get; set; }
        public string InfoWindowContent { get; set; }
    }
}