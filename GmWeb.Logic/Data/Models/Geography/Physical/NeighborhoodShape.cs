using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Geography
{
    [Table("tblShpNeighborhood")]
    public class NeighborhoodShape : GeoRegion
    {
        [Key]
        public override int ID { get; set; }
        [NotMapped]
        public override string Name { get => this.NAME; set => this.NAME = value; }

        public string STATE { get; set; }
        public string COUNTY { get; set; }
        public string CITY { get; set; }
        public string NAME { get; set; }
        public double REGIONID { get; set; }
    }
}
