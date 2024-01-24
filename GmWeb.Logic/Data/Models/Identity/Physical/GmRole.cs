using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GmWeb.Logic.Data.Models.Identity
{
    public class GmRole : IdentityRole<System.Guid>
    {
        public virtual ICollection<GmRoleClaim> RoleClaims { get; set; }
    }
}
