using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Csv;
using System.Collections.Generic;

namespace GmWeb.Logic.Importing.Demographics
{
    public class HUDIncomeLimitRecord : CsvRecord
    {
        public Dictionary<(int NumberInFamily, IncomeLevel IncomeLevel), int> IncomeAmounts { get; set; }
        public string StateCode { get; set; }
        public string CountyCode { get; set; }
        public string statename { get; set; }
        public string areaname { get; set; }
        public override string ToString() => this.areaname;
    }
}
