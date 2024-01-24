﻿using Microsoft.AspNetCore.Identity;

namespace GmWeb.Logic.Data.Models.Identity
{
    public class GmUserLogin : IdentityUserLogin<System.Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
