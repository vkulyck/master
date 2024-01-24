using CsvHelper.Configuration;

namespace GmWeb.Logic.Utility.Csv
{
    public abstract class CsvRecordMap<TRecord, TRecordMap> : ClassMap<TRecord>
        where TRecord : CsvRecord
        where TRecordMap : CsvRecordMap<TRecord, TRecordMap>
    {
        public virtual TRecordMap Init() => (TRecordMap)this;
    }
}
