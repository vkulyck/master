using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Identity
{
    // You can add profile data for the user by adding more properties to your AppIdentity class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class GmIdentity : IdentityUser<Guid>
    {
        private Dictionary<TwoFactorProviderType, string> _keys = new Dictionary<TwoFactorProviderType, string>();
        [NotMapped]
        public virtual string AuthenticatorKey
        {
            get => this._keys.TryGetValue(TwoFactorProviderType.Authenticator, out string value) ? value : null;
            set => this._keys[TwoFactorProviderType.Authenticator] = value;
        }

        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string LastName { get; set; }

        [NotMapped]
        public DateTime BirthDate
        {
            get;
            set;
        }

        [NotMapped]
        public Guid AccountID => this.Id;
        [NotMapped]
        public bool HasPassword => !string.IsNullOrWhiteSpace(this.PasswordHash);
        [NotMapped]
        public IList<string> RecoveryCodes { get; set; }
    }
}
