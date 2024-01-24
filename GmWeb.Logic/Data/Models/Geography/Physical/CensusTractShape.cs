using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpCensusTract")]
    public class CensusTractShape : GeoRegion
    {
        [Key]
        public override int ID { get; set; }
        [Column("NAME")]
        public override string Name { get; set; }

        public string STATEFP { get; set; }
        public string COUNTYFP { get; set; }
        public string TRACTCE { get; set; }
        public string GEOID { get; set; }
        public string NAMELSAD { get; set; }
        public string MTFCC { get; set; }
        public string FUNCSTAT { get; set; }
        public long ALAND { get; set; }
        public long AWATER { get; set; }
        public decimal INTPTLAT { get; set; }
        public decimal INTPTLON { get; set; }
    }
}