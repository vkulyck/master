using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using GmWeb.Logic.Data.Context;
using System.Data.Entity;

namespace GmWeb.Web.Common.Identity
{
    public class GmIdentityContext : GmIdentityContext<GmIdentity>
    {
        private GmIdentityContext() : base() { }
        public static GmIdentityContext Create()
        {
            return new GmIdentityContext();
        }
    }

    public class GmIdentityContext<TUser> : IdentityDbContext<TUser>
        where TUser : IdentityUser
    {
        protected GmIdentityContext() : base("IDENTITY_DB", throwIfV1Schema: false)
        {
        }
    }
}