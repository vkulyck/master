using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Importing.Demographics
{
    public class RacialDensityImporter : DemographicImporter<RacialDensityRecord, RacialDensityRecordMap>
    {
        public override bool HasBins => true;
        public RacialDensityImporter(IDemographicsContext cache, Category category) : base(cache, category) { }

        protected override Bin CreateBinFromMetadata(MetadataRecord record)
        {
            var regexes = new List<Regex>{
                new Regex(@"^\s*Population of one race: - (?<description>\S.+\S) alone\s*$"),
                new Regex(@"^\s*Two or More Races: - Population of (?:two|three|four|five) races: - (?<description>\S.+\S)\s*$")
            };

            foreach (var regex in regexes)
            {
                var match = regex.Match(record.ColumnDescription);
                if (!match.Success) continue;

                string description = match.Groups["description"].Value;
                var bin = new Bin(this.Category)
                {
                    Description = description,
                    Identifier = record.ColumnID,
                    ColumnID = record.ColumnID
                };
                return bin;
            }
            return null;
        }
    }
}
