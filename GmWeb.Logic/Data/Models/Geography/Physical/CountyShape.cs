using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpCounty")]
    public class CountyShape : GeoRegion
    {
        [Key]
        [Column("CountyID")]
        public override int ID { get; set; }
        [NotMapped]
        public override string Name { get => this.NAMELSAD; set => this.NAMELSAD = value; }

        public string STATEFP { get; set; }
        public string COUNTYFP { get; set; }
        public string COUNTYNS { get; set; }
        public string GEOID { get; set; }
        public string NAME { get; set; }
        public string NAMELSAD { get; set; }
        public string LSAD { get; set; }
        public string CLASSFP { get; set; }
        public string MTFCC { get; set; }
        public string CSAFP { get; set; }
        public string CBSAFP { get; set; }
        public string METDIVFP { get; set; }
        public string FUNCSTAT { get; set; }
        public long ALAND { get; set; }
        public long AWATER { get; set; }
        public decimal INTPTLAT { get; set; }
        public decimal INTPTLON { get; set; }
    }
}