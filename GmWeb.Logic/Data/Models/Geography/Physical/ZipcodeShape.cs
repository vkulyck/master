using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpSFZipCodes")]
    public class ZipcodeShape : GeoRegion
    {
        [Key]
        [Column("SFZipCodeID")]
        public override int ID { get; set; }
        [Column("ZipCode")]
        public override string Name { get; set; }
    }
}