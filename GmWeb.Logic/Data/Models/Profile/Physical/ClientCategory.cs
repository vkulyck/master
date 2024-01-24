using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblClientCategory")]
    public class ClientCategory : BaseDataModel
    {
        [Key]
        public int ClientCategoryID { get; set; }
        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public virtual ProfileCategory Category { get; set; }

        [InverseProperty("ClientCategory")]
        public virtual ICollection<ClientCategoryDate> ClientCategoryDates { get; set; } = new List<ClientCategoryDate>();
    }
}