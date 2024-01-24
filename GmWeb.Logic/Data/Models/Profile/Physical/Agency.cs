using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblAgency")]
    public class Agency : BaseDataModel
    {
        [Key]
        [Column("ID")]
        public int AgencyID { get; set; }
        public string Name { get; set; }
    }
}