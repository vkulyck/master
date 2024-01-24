using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Web.Demographics.ViewModels.Geo;

namespace GmWeb.Web.Demographics.ViewModels
{
    public class CommunityViewModel
    {
        public IList<MarkerData> Clients { get; set; } = new List<MarkerData>();
        public IList<RegionData> Regions { get; set; } = new List<RegionData>();
    }
}