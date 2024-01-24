using CsvHelper.Configuration.Attributes;
namespace GmWeb.Logic.Importing.Demographics
{
    public class HouseholdMakeupRecord : DemographicRecord
    {
        [Name("HC01_EST_VC01")]
        public decimal TotalHouseholdCount { get; set; }
        [Name("HC02_EST_VC01")]
        public decimal FamilyCount { get; set; }
        public decimal FamilyPercent { get; set; }
        [Name("HC03_EST_VC01")]
        public decimal MarriedCoupleFamilyCount { get; set; }
        public decimal MarriedCoupleFamilyPercent => this.MarriedCoupleFamilyCount / this.TotalHouseholdCount;
        [Name("HC04_EST_VC01")]
        public decimal NonFamilyCount { get; set; }
        public decimal NonFamilyPercent => this.NonFamilyCount / this.TotalHouseholdCount;
    }
}
