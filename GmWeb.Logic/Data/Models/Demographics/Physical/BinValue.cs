using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Demographics
{
    [Table("tblBinValues", Schema = "dmg")]
    public class BinValue : CensusMetricEntity
    {
        /// <summary>
        /// The primary key for this entity.
        /// </summary>
        [Key]
        public int BinValueID { get; set; }
        [ForeignKey("Bin")]
        public int BinID { get; set; }
        public virtual Bin Bin { get; set; }
    }
}
