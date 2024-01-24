using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Demographics.Logic;
using GmWeb.Web.Demographics.Logic.Data.Context;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Web.Demographics.ViewModels;
using GmWeb.Web.Demographics.ViewModels.Geo;

namespace GmWeb.Web.Demographics.Controllers
{
    public class CommunityController : BaseClientServicesController
    {
        public ActionResult Index(GeoRegionType RegionType = GeoRegionType.Tract, int SelectedTab = 1)
        {
            var model = new CommunityViewModel();
            using (var context = new DemographicsContext())
            {
                var localMarkers = context.ClientMarkers
                    .Where(x => x.Latitude > 37.7 && x.Latitude < 37.83)
                    .Where(x => x.Longitude > -122.52 && x.Longitude < -122.36)
                    .ToList()
                ;
                foreach (var marker in localMarkers)
                {
                    model.Clients.Add(new MarkerData
                    {
                        Latitude = marker.Latitude,
                        Longitude = marker.Longitude
                    });
                }
                var analyzer = new GeoAnalyzer(context);
                model.Regions = analyzer.AnalyzeRegions(RegionType, localMarkers);
            }
            ViewBag.SelectedTab = SelectedTab;
            return View(model);
        }

        public ActionResult _SFClient_Heatmap() => PartialView("_SFClient_Heatmap");
        public ActionResult _SFClient_Regions() => PartialView("_SFClient_Regions");
    }
}
