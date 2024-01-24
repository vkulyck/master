using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using GmWeb.Web.Common.Identity;
using System.Data.Entity.SqlServer;

namespace GmWeb.Web.Common.Identity
{
    public class ClaimsIdentityFactory<TUser> : Microsoft.AspNet.Identity.ClaimsIdentityFactory<TUser>
        where TUser : GmIdentity
    {
        public override async Task<ClaimsIdentity> CreateAsync(UserManager<TUser, string> manager, TUser user, string authenticationType)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var id = new ClaimsIdentity(authenticationType, UserNameClaimType, RoleClaimType);
            id.AddClaim(new Claim(UserIdClaimType, ConvertIdToString(user.Id), ClaimValueTypes.String));
            var thang = new Claim(ClaimTypes.NameIdentifier, user.Id, ClaimValueTypes.String);
            id.AddClaim(new Claim(UserNameClaimType, user.UserName, ClaimValueTypes.String));
            //id.AddClaim(new Claim(IdentityProviderClaimType, DefaultIdentityProviderClaimValue, ClaimValueTypes.String));
            id.AddClaim(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String));

            if (manager.SupportsUserSecurityStamp)
            {
                var stamp = await manager.GetSecurityStampAsync(user.Id);
                id.AddClaim(new Claim(SecurityStampClaimType, stamp));
            }

            if (manager.SupportsUserRole)
            {
                IList<string> roles = await manager.GetRolesAsync(user.Id);

                foreach (string roleName in roles)
                {
                    id.AddClaim(new Claim(RoleClaimType, roleName, ClaimValueTypes.String));
                }
            }

            if (manager.SupportsUserClaim)
            {
                id.AddClaims(await manager.GetClaimsAsync(user.Id));
            }

            return id;
        }
    }
}