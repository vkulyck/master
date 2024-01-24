namespace GmWeb.Logic.Importing.Demographics
{
    public sealed class MeanIncomeRecordMap : DemographicRecordMap<MeanIncomeRecord, MeanIncomeRecordMap>
    {
        public MeanIncomeRecordMap()
        {
            //Map(x => x.Value).ConvertUsing((IReaderRow row) =>
            //{
            //    var valueStr = row.GetField("HC03_EST_VC02");
            //    valueStr = Regex.Replace(valueStr, @"\+$", string.Empty);
            //    if(decimal.TryParse(valueStr, System.Globalization.NumberStyles.AllowTrailingSign))
            //});
        }
    }
}
