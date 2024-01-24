using GmWeb.Logic.Utility.Csv;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace GmWeb.Logic.Importing.Demographics
{
    public class DemographicRecord : CsvRecord
    {
        [Name("GEO.id")]
        public string InternationalGeoID { get; set; }
        [Name("GEO.id2")]
        public string DomesticGeoID { get; set; }
        public string LocalGeoID { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public Dictionary<string, decimal> FieldValues { get; set; }
    }
}
