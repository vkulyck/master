using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Utility.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Importing.Demographics
{
    public class HouseholdIncomeImporter : DemographicImporter<HouseholdIncomeRecord, HouseholdIncomeRecordMap>
    {
        public override bool HasBins => true;
        public HouseholdIncomeImporter(IDemographicsContext cache, Category category) : base(cache, category) { }

        protected static readonly Regex RxLower = new Regex($@"Estimate; Total:\s*-\s*(?<description>Less than \$(?<amount>[\d,]+))");
        protected static readonly Regex RxInner = new Regex($@"Estimate; Total:\s*-\s*(?<description>\$(?<amount>[\d,]+) to \$(?<amount>[\d,]+))");
        protected static readonly Regex RxUpper = new Regex($@"Estimate; Total:\s*-\s*(?<description>\$(?<amount>[\d,]+) or more)");
        protected static readonly List<Regex> Rxs = new List<Regex> { RxLower, RxInner, RxUpper };
        protected override Bin CreateBinFromMetadata(MetadataRecord record)
        {
            for (int i = 0; i < Rxs.Count; i++)
            {
                var rx = Rxs[i];
                if (rx.TryMatch(record.ColumnDescription, out var match))
                {
                    var bin = new Bin(this.Category);
                    var amounts = match.GetCurrencies("amount");
                    if (i == 0)
                    {
                        bin.Identifier = $"0_{amounts[0]}";
                        bin.MinValue = 0;
                        bin.MaxValue = amounts[0];
                    }
                    else if (i == 1)
                    {
                        bin.Identifier = $"{amounts[0]}_{amounts[1]}";
                        bin.MinValue = amounts[0];
                        bin.MaxValue = amounts[1];
                    }
                    else
                    {
                        bin.Identifier = $"{amounts[0]}_inf";
                        bin.MinValue = amounts[0];
                        bin.MaxValue = null;
                    }
                    bin.Description = match.Groups["description"].Value;
                    bin.ColumnID = record.ColumnID;
                    return bin;
                }
            }
            return null;
        }
    }
}
