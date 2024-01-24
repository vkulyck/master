using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblWorkPlans")]
    public class WorkPlan : BaseDataModel
    {
        [Key]
        public int Work_ID { get; set; }
        [ForeignKey("WorkProject")]
        public int WorkProjectID { get; set; }
        public virtual Project WorkProject { get; set; }
        [ForeignKey("Activity")]
        public string ActivityID { get; set; }
        public virtual Activity Activity { get; set; }
        public int? WorkActivityType { get; set; }

    }
}