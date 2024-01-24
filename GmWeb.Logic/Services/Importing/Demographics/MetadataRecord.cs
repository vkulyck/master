using GmWeb.Logic.Utility.Csv;

namespace GmWeb.Logic.Importing.Demographics
{
    public class MetadataRecord : CsvRecord
    {
        public string ColumnID { get; set; }
        public string ColumnDescription { get; set; }
        public override string ToString() => $"{this.ColumnID}: {this.ColumnDescription}";
    }
}
