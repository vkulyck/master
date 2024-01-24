using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpPrecincts")]
    public class PrecinctShape : GeoRegion
    {
        [Key]
        [Column("PrecinctID")]
        public override int ID { get; set; }
        [Column("PrecName")]
        public override string Name { get; set; }
    }
}