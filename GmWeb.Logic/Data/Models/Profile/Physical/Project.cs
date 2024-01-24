using GmWeb.Logic.Data.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblProjects")]
    public class Project : BaseDataModel
    {
        [Key]
        public int ProjectID { get; set; }
        [ForeignKey("Agency")]
        public int AgencyID { get; set; }
        public virtual Agency Agency { get; set; }
        [SqlDataType(System.Data.SqlDbType.Char)]
        public int ProjectYear { get; set; }
        public string Program { get; set; }
    }
}