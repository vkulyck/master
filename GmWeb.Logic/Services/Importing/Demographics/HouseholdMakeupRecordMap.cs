namespace GmWeb.Logic.Importing.Demographics
{
    public sealed class HouseholdMakeupRecordMap : DemographicRecordMap<HouseholdMakeupRecord, HouseholdMakeupRecordMap>
    {
        public override bool EnableFieldValueMap => true;
        public override int? FieldValueMapEndIndex => 10;
    }
}
