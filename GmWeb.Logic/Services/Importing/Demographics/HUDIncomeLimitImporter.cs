using EFCore.BulkExtensions;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Data.Models.Lookups;
using GmWeb.Logic.Utility.Csv;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Demographics
{
    public class HUDIncomeLimitImporter : CsvImporter<HUDIncomeLimitRecord, HUDIncomeLimitRecordMap, IDemographicsContext>
    {
        // TODO: PDF data is available back to 1999, but parsing these files will require significantly more work.
        // 2014 and 2015 datasets are missing the county FIPS codes, so we're skipping those as well.
        public static int StartingYear => 2016;
        public static int LatestAvailableYear => DateTime.Now.AddMonths(-4).Year;
        public static string ConfiguredDirectory => $@"{Dataset.Directory}\hud-income-limits";
        public override int HeaderCount => 1;
        public HUDIncomeLimitImporter(IDemographicsContext cache, string sourcePath) : base(cache, sourcePath) { }

        public override async Task ImportAsync()
        {
            int year = this.extractYear(this.SourceFile.Name);
            var hiLevels = await this.Cache.HUDIncomeLevels
                .ToAsyncEnumerable()
                .Where(x => x.Year == year)
                .ToListAsync()
            ;
            var lookup = hiLevels.Select(x => x.HUDIncomeLevelID).ToHashSet();
            var prevModels = await this.Cache.HUDIncomeLevels
                .Where(x => lookup.Contains(x.HUDIncomeLevelID))
                .ToListAsync()
            ;
            await this.Cache.BulkDeleteAsync(prevModels);


            var records = this.IterateRecordsAsync();
            var models = new List<HUDIncomeLevel>();
            await foreach (var record in records)
            {
                foreach (var kvp in record.IncomeAmounts)
                {
                    var model = new HUDIncomeLevel
                    {
                        StateCode = record.StateCode,
                        CountyCode = record.CountyCode,
                        Year = year,
                        IncomeLevel = kvp.Key.IncomeLevel,
                        NumberInFamily = kvp.Key.NumberInFamily,
                        TotalFamilyIncome = kvp.Value
                    };
                    models.Add(model);
                }
            }
            await this.Cache.BulkInsertAsync(models);
        }

        protected int extractYear(string file)
        {
            var match = Regex.Match(file, @"\d+(?=\.csv$)");
            int value = Convert.ToInt32(match.Value);
            return value;
        }

        public static void Download() => Download(ConfiguredDirectory);
        public static void Download(string targetDirectory)
        {
            var uris = new List<string>();
            for (int year = StartingYear; year <= LatestAvailableYear; year++)
                uris.Add($"https://files.hudexchange.info/reports/published/HOME_IncomeLmts_Natl_{year}.xlsx");
            var downloader = new FileDownloader(Uris: uris, TargetDirectory: targetDirectory, AllowOverwrite: true);
            var task = downloader.Run();
            task.Wait();
        }

        public static async Task ImportAllAsync<TCache>() where TCache : IDemographicsContext, new()
        {
            using (var cache = new TCache())
            {
                var files = new List<string>();
                for (int year = StartingYear; year <= LatestAvailableYear; year++)
                    files.Add($@"{ConfiguredDirectory}\HOME_IncomeLmts_Natl_{year}.csv");
                foreach (string file in files)
                {
                    var importer = new HUDIncomeLimitImporter(cache, file);
                    await importer.ImportAsync();
                }
            }
        }
    }
}
