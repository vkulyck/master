using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpCongDist")]
    public class CongressionalDistrictShape : GeoRegion
    {
        [Key]
        [Column("CongDistID")]
        public override int ID { get; set; }
        [NotMapped]
        public override string Name { get => this.NAMELSAD; set => this.NAMELSAD = value; }

        public string STATEFP { get; set; }
        public string CD116FP { get; set; }
        public string GEOID { get; set; }
        public string NAMELSAD { get; set; }
        public string LSAD { get; set; }
        public string CDSESSN { get; set; }
        public string MTFCC { get; set; }
        public string FUNCSTAT { get; set; }
        public long ALAND { get; set; }
        public long AWATER { get; set; }
        public decimal INTPTLAT { get; set; }
        public decimal INTPTLON { get; set; }
    }
}