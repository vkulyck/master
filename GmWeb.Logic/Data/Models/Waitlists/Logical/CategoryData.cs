using GmWeb.Logic.Data.Models.Shared;
using Newtonsoft.Json;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class CategoryData : DataReference
    {
        public override string VariablePrefix => "Category";
        protected override string ParentTableName => "CAT";
        [JsonIgnore]
        public DynamicFieldValue ConfiguredValue => base.LookupValue;
        [JsonIgnore]
        public int? CategoryID
        {
            get => this.DataSource?.RowID;
            set
            {
                if (this.DataSource != null)
                    this.DataSource.RowID = value;
            }
        }
    }
}
