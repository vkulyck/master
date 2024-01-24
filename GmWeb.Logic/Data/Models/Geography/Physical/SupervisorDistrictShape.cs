using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpSuperDist")]
    public class SupervisorDistrictShape : GeoRegion
    {
        [Key]
        [Column("SupDistID")]
        public override int ID { get; set; }
        [Column("Supdist")]
        public override string Name { get; set; }
        [Column("Supervisor")]
        public int SupervisorID { get; set; }
        [Column("Supname")]
        public string Supervisor { get; set; }
    }
}