using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    [Table("tblDataSources", Schema = "wtl")]
    public class DataSource : BaseDataModel
    {
        public int DataSourceID { get; set; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();
        public string Description { get; set; }
        public string Table { get; set; }
        public string Field { get; set; }
        [SqlDataType(System.Data.SqlDbType.Int)]
        public EntityType EntityType { get; set; }
        [SqlDataType(System.Data.SqlDbType.Int)]
        public DataType DataType { get; set; }
        [NotMapped]
        public int? RowID { get; set; }

        public DataSource WithParentID(int id)
        {
            var source = new DataSource { Table = this.Table, Field = this.Field, RowID = id };
            return source;
        }
    }
}
