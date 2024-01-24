using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpStAsmblyDists")]
    public class AssemblyDistrictShape : GeoRegion
    {
        [Key]
        [Column("StAsmblyDistsID")]
        public override int ID { get; set; }
        [Column("Caassmd")]
        public override string Name { get; set; }
    }
}