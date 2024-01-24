using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblCategory")]
    public class ProfileCategory : BaseDataModel
    {
        [Key]
        public int CategoryID { get; set; }
        [ForeignKey("WorkPlan")]
        public int WorkPlanID { get; set; }
        public virtual WorkPlan WorkPlan { get; set; }
    }
}