using Microsoft.AspNetCore.Identity;

namespace GmWeb.Logic.Data.Models.Identity
{
    public class GmUserToken : IdentityUserToken<System.Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
