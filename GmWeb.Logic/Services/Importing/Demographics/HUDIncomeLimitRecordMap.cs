using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;

namespace GmWeb.Logic.Importing.Demographics
{
    public sealed class HUDIncomeLimitRecordMap : CsvRecordMap<HUDIncomeLimitRecord, HUDIncomeLimitRecordMap>
    {
        public HUDIncomeLimitRecordMap()
        {
            AutoMap();
            Map(x => x.StateCode).ConvertUsing(x => x.GetField<int>("State").ToString("D2"));
            Map(x => x.CountyCode).ConvertUsing(x => x.GetField<int>("County").ToString("D3"));
            Map(x => x.IncomeAmounts).ConvertUsing((IReaderRow row) =>
            {
                var amounts = new Dictionary<(int NumberInFamily, IncomeLevel IncomeLevel), int>();
                var dataHeaders = row.Context.HeaderRecord.Skip(6).ToList();
                foreach (var header in dataHeaders)
                {
                    var match = Regex.Match(header, @"^lim(?<Percentile>\d+)_(\d+)p(?<NumberInFamily>\d)", RegexOptions.IgnoreCase);
                    int percentile = int.Parse(match.Groups["Percentile"].Value);
                    int nfamily = int.Parse(match.Groups["NumberInFamily"].Value);
                    IncomeLevel level;
                    switch (percentile)
                    {
                        case 30: level = IncomeLevel.ExtremeLow; break;
                        case 50: level = IncomeLevel.Low; break;
                        case 60: level = IncomeLevel.Moderate; break;
                        case 80: level = IncomeLevel.AboveModerate; break;
                        default: throw new Exception($"Unable to parse indexing data from HUD data column: {header}");
                    }
                    if (row.TryGetField<int>(header, out int amount))
                    {
                        amounts[(nfamily, level)] = amount;
                    }
                    else throw new Exception($"Unable to parse income amount from HUD data with header = '{header}'");
                }
                return amounts;

            });
        }
    }
}
