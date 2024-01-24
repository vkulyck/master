using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WageTable = GmWeb.Logic.Utility.Web.Html.Table<decimal>;

namespace GmWeb.Logic.Importing.Demographics
{
    public class LivingWageImporter : IDataImporter
    {
        public static string ConfiguredDirectory => $@"{Dataset.Directory}\sf-living-wage\all-state-county-tables";
        public IDemographicsContext Cache { get; private set; }
        public string SourceDirectory { get; private set; }
        public LivingWageImporter(IDemographicsContext cache) : this(cache, ConfiguredDirectory) { }
        public LivingWageImporter(IDemographicsContext cache, string sourceDirectory)
        {
            this.Cache = cache;
            this.SourceDirectory = sourceDirectory;
        }
        public async Task ImportAsync() => await this.ProcessDirectoryAsync(this.SourceDirectory);

        protected async Task ProcessDirectoryAsync(string sourceDirectory)
        {
            var files = Directory.EnumerateFiles(sourceDirectory, "*.htm");
            await this.Cache.TruncateAsync<LivingWageEstimate>();
            var models = new List<LivingWageEstimate>();
            foreach (string file in files)
            {
                var tables = this.extractTables(file);
                var livingWageTable = tables[0];
                var location = this.extractLocation(file);
                var entities = this.convertTableToEntities(livingWageTable, location);
                models.AddRange(entities);
            }
            await this.Cache.BulkInsertAsync(models);
        }

        protected (string StateCode, string CountyCode) extractLocation(string file)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string sc = filename.Substring(0, 2);
            string cc = filename.Substring(2);
            return (sc, cc);
        }

        protected List<LivingWageEstimate> convertTableToEntities(WageTable table, (string StateCode, string CountyCode) location)
        {
            var values = new List<LivingWageEstimate>();
            foreach (var cell in table.Cells)
            {
                var estimate = new LivingWageEstimate
                {
                    WageEstimate = cell.Data
                };
                switch (cell.Headers[0])
                {
                    case "1 ADULT": estimate.EarnerType = EarnerType.OneAdult; break;
                    case "2 ADULTS(1 WORKING)": estimate.EarnerType = EarnerType.TwoAdultsOneWorking; break;
                    case "2 ADULTS(BOTH WORKING)": estimate.EarnerType = EarnerType.TwoAdultsTwoWorking; break;
                }
                var ncMatch = Regex.Match(cell.Headers[1], @"^(\d+)");
                estimate.NumberOfChildren = Convert.ToInt32(ncMatch.Value);
                var wtMatch = Regex.Match(cell.Keys[0], @"^(\w+)\b");
                estimate.WageThresholdType = (WageThresholdType)Enum.Parse(typeof(WageThresholdType), wtMatch.Value);
                estimate.StateCode = location.StateCode;
                estimate.CountyCode = location.CountyCode;
                values.Add(estimate);
            }
            return values;
        }

        protected List<WageTable> extractTables(string file)
        {
            var tables = new List<WageTable>();
            string content;
            using (var reader = new StreamReader(file))
                content = reader.ReadToEnd();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            // TODO: Fix for .NET Standard
            // var nodes = doc.DocumentNode.QuerySelectorAll("table .results_table");
            var nodes = new List<HtmlNode>();
            foreach (var node in nodes)
            {
                var table = WageTable.FromNode(node);
                tables.Add(table);
            }
            return tables;
        }

        public void Dispose() => this.Cache.Dispose();
    }
}
