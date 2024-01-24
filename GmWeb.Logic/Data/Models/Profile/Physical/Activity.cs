using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("lkpActivity")]
    public class Activity : BaseDataModel
    {
        [Key]
        public string ActivityID { get; set; }
        public string ActivityDescription { get; set; }
        public string HUDCode { get; set; }
    }
}