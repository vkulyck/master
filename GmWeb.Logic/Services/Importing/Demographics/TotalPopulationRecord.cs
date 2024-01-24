using CsvHelper.Configuration.Attributes;
using NS = System.Globalization.NumberStyles;

namespace GmWeb.Logic.Importing.Demographics
{
    public class TotalPopulationRecord : DemographicRecord
    {
        [Name("HD01_VD01")]
        [NumberStyles(NS.AllowTrailingSign | NS.AllowThousands)]
        public int? Value { get; set; }
    }
}
