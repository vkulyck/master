using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace GmWeb.Web.Common.Identity
{
    public class GmRoleManager : RoleManager<IdentityRole>
    {
        public GmRoleManager(IRoleStore<IdentityRole, string> roleStore)
        : base(roleStore)
        { }

        public static GmRoleManager Create(
            IdentityFactoryOptions<GmRoleManager> options,
            IOwinContext context)
        {
            var manager = new GmRoleManager(
                new RoleStore<IdentityRole>(context.Get<GmIdentityContext>()));
            return manager;
        }
    }
}