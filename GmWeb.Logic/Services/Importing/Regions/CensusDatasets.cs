using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Regions
{
    public class CensusDatasets
    {
        public static class DatasetUris
        {
            public static class Shapefiles
            {
                private static List<string> _StateArchives(IEnumerable<StateFipsCode> fipsCodes, string suffix)
                {
                    string baseUri = "https://" + $"www2.census.gov/geo/tiger/TIGER2019/{suffix.ToUpper()}";
                    string fileFormat = "tl_2019_{0}_{1}.zip";
                    var fipsModels = EnumExtensions.GetEnumViewModels<StateFipsCode>();
                    if (fipsCodes.Count() > 0)
                        fipsModels = fipsModels.Where(x => fipsCodes.Contains(x.Value)).ToList();
                    var uris = new List<string>();
                    foreach (var fModel in fipsModels)
                    {
                        string fipsCode = $"{fModel.ID:D2}";
                        string filename = string.Format(fileFormat, fipsCode, suffix);
                        string uri = $"{baseUri}/{filename}";
                        uris.Add(uri);
                    }
                    return uris;
                }
                private static List<string> _StateTracts(IEnumerable<StateFipsCode> fipsCodes)
                    => _StateArchives(fipsCodes, "tract");
                public static List<string> StateTracts(IEnumerable<StateFipsCode> fipsCodes) => _StateTracts(fipsCodes);
                public static List<string> StateTracts(params StateFipsCode[] fipsCodes) => _StateTracts(fipsCodes);
                private static List<string> _StateBlockGroups(IEnumerable<StateFipsCode> fipsCodes)
                    => _StateArchives(fipsCodes, "bg");
                public static List<string> StateBlockGroups(IEnumerable<StateFipsCode> fipsCodes) => _StateBlockGroups(fipsCodes);
                public static List<string> StateBlockGroups(params StateFipsCode[] fipsCodes) => _StateBlockGroups(fipsCodes);
                private static List<string> _ConCity(IEnumerable<StateFipsCode> fipsCodes)
                    => _StateArchives(fipsCodes, "concity");
                public static List<string> ConCity(IEnumerable<StateFipsCode> fipsCodes) => _ConCity(fipsCodes);
                public static List<string> ConCity(params StateFipsCode[] fipsCodes) => _ConCity(fipsCodes);
            }

            public static class LivingWageDatasets
            {
                private static List<string> _StateCountyLivingWageDatasets<TCache>(
                    IEnumerable<StateFipsCode> FipsCodes,
                    int? MinCountyID = null,
                    int? MaxCountyID = null
                )
                    where TCache : IDemographicsContext, new()
                {
                    string baseUri = "https://livingwage.mit.edu/counties";
                    var fipsModels = EnumExtensions.GetEnumViewModels<StateFipsCode>();
                    if (FipsCodes.Count() > 0)
                        fipsModels = fipsModels.Where(x => FipsCodes.Contains(x.Value)).ToList();
                    var stateIds = fipsModels.Select(x => x.ID).ToList();
                    var ids = new List<string>();
                    using (var cache = new TCache())
                    {
                        foreach (int stateId in stateIds)
                        {
                            var query = cache.CensusTractShapes.AsQueryable()
                                .Where(x => x.STATEFP == $"{stateId:D2}")
                                .Select(x => x.COUNTYFP)
                                .Distinct()
                                .Select(x => int.Parse(x))
                            ;
                            if (MinCountyID.HasValue)
                                query = query.Where(x => x >= MinCountyID.Value);
                            if (MaxCountyID.HasValue)
                                query = query.Where(x => x <= MaxCountyID.Value);
                            var results = query.Select(x => $"{stateId:D2}{x:D3}").ToList();
                            ids.AddRange(results);
                        }
                    }
                    var uris = new List<string>();
                    foreach (string id in ids)
                    {
                        string uri = $"{baseUri}/{id}";
                        uris.Add(uri);
                    }
                    return uris;
                }
                public static List<string> StateCountyLivingWageDatasets<TCache>(IEnumerable<StateFipsCode> fipsCodes)
                    where TCache : IDemographicsContext, new()
                    => _StateCountyLivingWageDatasets<TCache>(fipsCodes);
                public static List<string> StateCountyLivingWageDatasets<TCache>(params StateFipsCode[] fipsCodes)
                    where TCache : IDemographicsContext, new()
                    => _StateCountyLivingWageDatasets<TCache>(fipsCodes);
            }
        }

        public static Task<FileDownloader> DownloadStateTracts(string TargetDirectory)
            => new FileDownloader(DatasetUris.Shapefiles.StateTracts(), TargetDirectory: TargetDirectory).Run()
        ;

        public static Task<FileDownloader> DownloadStateBlockGroups(string TargetDirectory)
            => new FileDownloader(DatasetUris.Shapefiles.StateBlockGroups(), TargetDirectory: TargetDirectory).Run()
        ;

        public static Task<FileDownloader> DownloadLivingWageDatasets<TCache>(string TargetDirectory)
            where TCache : IDemographicsContext, new()
            => new FileDownloader(DatasetUris.LivingWageDatasets.StateCountyLivingWageDatasets<TCache>(), TargetDirectory: TargetDirectory, AllowOverwrite: false).Run()
        ;
    }
}
