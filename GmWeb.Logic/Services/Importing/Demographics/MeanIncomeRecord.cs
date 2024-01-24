using CsvHelper.Configuration.Attributes;
using NS = System.Globalization.NumberStyles;

namespace GmWeb.Logic.Importing.Demographics
{
    public class MeanIncomeRecord : DemographicRecord
    {
        [Name("HC03_EST_VC02")]
        [NumberStyles(NS.AllowTrailingSign | NS.AllowThousands)]
        public int? Value { get; set; }
    }
}
