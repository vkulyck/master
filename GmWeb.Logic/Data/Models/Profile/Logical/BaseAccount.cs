using GmWeb.Logic.Data.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    public abstract class BaseAccount : BaseDataModel
    {
        public abstract int AccountID { get; set; }
        public abstract string Email { get; set; }
        public abstract string PasswordHash { get; set; }
        public virtual string Phone { get; set; }
        [EncryptedStringColumn]
        public virtual string FirstName { get; set; }
        [EncryptedStringColumn]
        public virtual string LastName { get; set; }
        [NotMapped]
        public virtual bool IsEmailConfirmed { get; set; }
        public abstract int? AgencyID { get; set; }
    }
}
