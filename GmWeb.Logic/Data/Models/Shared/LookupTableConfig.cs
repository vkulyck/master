namespace GmWeb.Logic.Data.Models.Shared
{
    public class LookupTableConfig
    {
        public string TableName { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public string DescriptionColumn { get; set; }

        public override string ToString() => $"{this.TableName} PK:{this.PrimaryKeyColumn}, Desc:{this.DescriptionColumn}";
    }
}
