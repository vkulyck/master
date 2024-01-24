using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Enums;
using GmWeb.Web.Demographics.Logic.Data.Context;
using GmWeb.Web.Demographics.Logic.DataModels;
using GmWeb.Logic.Data.Context.Profile;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Logic.Utility.Math;
using GmWeb.Web.Demographics.ViewModels.Geo;
using Point = NetTopologySuite.Geometries.Point;
using WeightedTractList = System.Collections.Generic.List<(GmWeb.Logic.Data.Models.Geography.CensusTractShape Tract, double Weight)>;

namespace GmWeb.Web.Demographics.Logic
{
    using Tract = GmWeb.Logic.Data.Models.Geography.CensusTractShape;
    using TractMapping = Dictionary<GeoRegion, WeightedTractList>;
    public class GeoAnalyzer : IConditionallyDisposable
    {
        protected DemographicsContext Context { get; set; }
        public MedianIncomeDescriptor Descriptor { get; private set; }
        public MultiGradientBrush<MedianIncomeDescriptor> MedianIncomeBrush { get; private set; }
        public bool EnableDispose { get; private set; } = false;
        public GeoAnalyzer() : this(new DemographicsContext())
        {
            this.EnableDispose = true;
        }
        public GeoAnalyzer(DemographicsContext context) 
        {
            this.Context = context;
            this.Descriptor = new MedianIncomeDescriptor();
            this.MedianIncomeBrush = new KnownHeatmapBrush<MedianIncomeDescriptor>(this.Descriptor);
        }

        public IList<RegionData> AnalyzeRegions(GeoRegionType rtype, IEnumerable<ClientMarker> markers)
        {
            var regions = new List<RegionData>();
            var local = this.GetLocalRegions(rtype);
            TractMapping containedTracts = GroupSubTracts(local, rtype);
            using (var cache = new ProfileCache())
            {
                var category = cache.Categories.Single(x => x.Name == "Median Income");
                foreach (var region in local)
                {
                    double? medianIncome = 0;
                    var wSubTracts = containedTracts[region];
                    int nClients = 0;
                    foreach (var wSubTract in wSubTracts)
                    {
                        var catValue = cache.CategoryValues
                            .Where(x => x.CategoryID == category.CategoryID)
                            .Where(x => x.TractID == wSubTract.Tract.GEOID)
                            .Select(x => x.Value)
                            .SingleOrDefault()
                        ;
                        var wValue = (double?)catValue * wSubTract.Weight;
                        medianIncome += wValue;
                        var nc = wSubTract.Tract.ClientCount(this.Context.Clients);
                        nClients += (int)(nc * wSubTract.Weight);
                    }
                    var totalWeight = wSubTracts.Sum(x => x.Weight);
                    medianIncome /= totalWeight;

                    regions.Add(new RegionData
                    {
                        GEOM = region.GEOM,
                        Weight = this.Descriptor.Project(medianIncome).Value,
                        Title = $"{region.ID}: {region.Name}",
                    });

                    var stats = regions.Last().Stats;
                    stats.Name = region.Name;
                    stats.Region_Type = rtype;
                    stats.ID = region.ID;
                    stats.ClientCount = nClients;
                    stats.Latitude = region.Latitude;
                    stats.Longitude = region.Longitude;
                    stats.Median_Income = medianIncome;

                    var colorData = this.MedianIncomeBrush.Interpolate(stats.Median_Income);
                    stats.ColorData = colorData;
                    stats.Color = colorData.Item1;
                }
            }
            return regions;
        }

        protected IEnumerable<GeoRegion> GetLocalRegions(GeoRegionType rtype)
        {
            switch (rtype)
            {
                case GeoRegionType.Tract:
                    return this.Context.CensusTractShapes
                        .Where(x => x.STATEFP == "06" && x.COUNTYFP == "075")
                        .Where(x => x.ID != 14586 && x.ID != 14234 && x.ID != 12802 && x.ID != 10986) // These tracts don't make sense
                        .Where(x => x.ID != 13888) // These tracts don't make sense
                        .ToList()
                    ;
                case GeoRegionType.Neighborhood:
                    return this.Context.NeighborhoodShapes.Where(x => x.CITY == "San Francisco").ToList();
                case GeoRegionType.CongressionalDistrict:
                    return this.Context.CongressionalDistrictShapes
                        .Where(x => x.STATEFP == "06")
                        .Where(x => Convert.ToInt32(x.CD116FP) >= 9 && Convert.ToInt32(x.CD116FP) <= 19)
                        .ToList()
                    ;
            }
            throw new ArgumentException($"Region type is not enabled: {rtype}");
        }

        protected TractMapping GroupSubTracts(IEnumerable<GeoRegion> parents, GeoRegionType rtype)
        {
            IEnumerable<CensusTractShape> tracts;
            if (rtype == GeoRegionType.Tract)
                tracts = parents.Cast<CensusTractShape>();
            else
                tracts = GetLocalRegions(GeoRegionType.Tract).Cast<CensusTractShape>();

            TractMapping containedTracts;
            if (rtype == GeoRegionType.Tract)
                containedTracts = tracts.ToDictionary(
                    x => (GeoRegion)x,
                    x => new WeightedTractList { (Tract: x, Weight: 1.0f) }
                );
            else
            {
                containedTracts = new TractMapping();
                foreach (var parent in parents)
                {
                    containedTracts[parent] = GroupSubTracts(parent, tracts);
                }
            }
            return containedTracts;
        }
        protected WeightedTractList GroupSubTracts(GeoRegion parent, IEnumerable<CensusTractShape> tracts)
        {
            Func<CensusTractShape, (CensusTractShape Tract, double Weight)> selector = (CensusTractShape t) =>
            {
                var intersection = t.GEOM.Intersection(parent.GEOM);
                return (Tract: t, Weight: intersection.Area);
            };
            var intersections = tracts.Select(selector).Where(x => x.Weight > 0).ToList();
            var totalWeight = intersections.Sum(x => x.Weight);
            // Normalize the weights
            if (totalWeight > 0)
            {
                intersections = intersections.Select(x => (x.Tract, x.Weight / totalWeight)).ToList();
            }
            return intersections;
        }

        public void Dispose()
        {
            if (!this.EnableDispose)
                return;
            this.Context.Dispose();
        }
    }
}