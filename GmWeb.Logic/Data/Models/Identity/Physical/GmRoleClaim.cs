using Microsoft.AspNetCore.Identity;

namespace GmWeb.Logic.Data.Models.Identity
{
    public class GmRoleClaim : IdentityRoleClaim<System.Guid>
    {
        public virtual GmRole Role { get; set; }
    }
}
