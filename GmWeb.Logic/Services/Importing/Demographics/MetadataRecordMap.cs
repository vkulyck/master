using GmWeb.Logic.Utility.Csv;

namespace GmWeb.Logic.Importing.Demographics
{
    public class MetadataRecordMap : CsvRecordMap<MetadataRecord, MetadataRecordMap>
    {
        public MetadataRecordMap()
        {
            Map(x => x.ColumnID).Index(0);
            Map(x => x.ColumnDescription).Index(1);
        }
    }
}
