using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Demographics
{
    [Table("tblCategoryValues", Schema = "dmg")]
    public class CategoryValue : CensusMetricEntity
    {
        [Key]
        public int CategoryValueID { get; set; }
        public int CategoryID { get; set; }
        /// <summary>
        /// The demographic category that is measured within 
        /// this entity's census tract to produce the Value
        /// property.
        /// </summary>
        public virtual Category Category { get; set; }
    }
}
