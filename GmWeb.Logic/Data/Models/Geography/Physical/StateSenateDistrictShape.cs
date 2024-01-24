using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpStSenateDist")]
    public class StateSenateDistrictShape : GeoRegion
    {
        [Key]
        [Column("StSenateDistID")]
        public override int ID { get; set; }
        [Column("CaseN")]
        public override string Name { get; set; }
    }
}