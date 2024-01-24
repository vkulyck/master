using GmWeb.Logic.Data.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblUser")]
    public class User : BaseDataModel
    {
        [Key]
        public int UserKeyID { get; set; }
        public int? AgencyID { get; set; }
        [EncryptedStringColumn]
        public string FirstName { get; set; }
        [EncryptedStringColumn]
        public string LastName { get; set; }
        [Column("UserPhone")]
        public string Phone { get; set; }
    }
}
