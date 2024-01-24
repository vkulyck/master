using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblClientCategoryDate")]
    public class ClientCategoryDate : BaseDataModel
    {
        [Key]
        public int ClientCategoryDateID { get; set; }
        [ForeignKey("ClientCategory")]
        public int ClientCategoryID { get; set; }
        public virtual ClientCategory ClientCategory { get; set; }
        public bool Assigned { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}