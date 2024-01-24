using GmWeb.Logic.Data.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblUser")]
    public class UserAccount : BaseAccount
    {
        [Key]
        [Column("UserKeyID")]
        public override int AccountID { get; set; }
        [Column("UserEmailAddress")]
        public override string Email { get; set; }
        [Column("UserPassword")]
        public override string PasswordHash { get; set; }
        [Column("AgencyID")]
        public override int? AgencyID { get; set; }
        [Column("UserPhone")]
        public override string Phone { get; set; }
        [Column("UserFName")]
        [EncryptedStringColumn]
        public override string FirstName { get; set; }
        [Column("UserLName")]
        [EncryptedStringColumn]
        public override string LastName { get; set; }
    }
}
