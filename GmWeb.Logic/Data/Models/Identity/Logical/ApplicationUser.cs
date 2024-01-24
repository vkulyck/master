using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using GenderType = GmWeb.Logic.Enums.Gender;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using UserRole = GmWeb.Logic.Enums.UserRole;

namespace GmWeb.Logic.Data.Models.Identity
{
    public class ApplicationUser : GmIdentity
    {
        public Guid LookupID { get; set; }
        public UserRole UserRole { get; set; } = UserRole.Client;
        public GenderType Gender { get; set; } = GenderType.Unspecified;
        public string LanguageCode { get; set; } = PrimaryLanguages.English.Value;

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
