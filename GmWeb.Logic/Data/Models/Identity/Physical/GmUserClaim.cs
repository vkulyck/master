using Microsoft.AspNetCore.Identity;

namespace GmWeb.Logic.Data.Models.Identity
{
    public class GmUserClaim : IdentityUserClaim<System.Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
