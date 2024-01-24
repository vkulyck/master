using GmWeb.Logic.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblClientServiceProfile")]
    public class ClientServiceProfile : BaseDataModel
    {
        public int ClientServiceProfileID { get; set; }
        public int? ParentTableID { get; set; }
        public string ParentTableName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DataType DataType { get; set; }
    }
}
