using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Utility.Extensions.Reflection;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper.Configuration.Attributes;

namespace GmWeb.Logic.Importing.Demographics
{
    public class HouseholdMakeupImporter : DemographicImporter<HouseholdMakeupRecord, HouseholdMakeupRecordMap>
    {
        public override bool HasBins => true;
        public HouseholdMakeupImporter(IDemographicsContext cache, Category category) : base(cache, category) { }

        protected override Bin CreateBinFromMetadata(MetadataRecord record)
        {
            if (record.ColumnDescription.Contains("Margin of Error"))
                return null;
            var binIdentifier = typeof(HouseholdMakeupRecord).GetProperties()
                .Select(x => new { Prop = x, Attr = x.GetAttribute<NameAttribute>() })
                .Where(x => x.Attr != null)
                .Where(x => x.Attr.Names.First().Trim() == record.ColumnID)
                .Where(x => x.Prop.PropertyType == typeof(decimal))
                .Select(x => x.Prop.Name)
                .FirstOrDefault()
            ;
            if (binIdentifier == null)
                return null;

            string binDescription = Regex.Replace(record.ColumnDescription, @";[^;]+;[^;]+$", string.Empty);
            switch (binDescription)
            {
                case "Households":
                    binDescription = "Total Household Count";
                    break;
                case "Families":
                    binDescription = "Count of Family Households";
                    break;
                case "Married-couple families":
                    binDescription = "Count of Married-couple Family Households";
                    break;
                case "Nonfamily households":
                    binDescription = "Count of Non-Family Households";
                    break;
            }

            var bin = new Bin(this.Category)
            {
                Description = binDescription,
                Identifier = binIdentifier,
                ColumnID = record.ColumnID
            };
            return bin;
        }
    }
}
