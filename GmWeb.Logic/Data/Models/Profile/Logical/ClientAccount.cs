using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblClient")]
    public class ClientAccount : BaseAccount
    {
        [Key]
        [Column("ClientID")]
        public override int AccountID { get; set; }
        [Column("Email")]
        public override string Email { get; set; }
        [Column("Password")]
        public override string PasswordHash { get; set; }
        [Column("AgencyID")]
        public override int? AgencyID { get; set; }
    }
}
