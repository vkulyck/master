using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Identity;

namespace GmWeb.Logic.Utility.Identity;
public class GmClaimsFactory : UserClaimsPrincipalFactory<GmIdentity>
{
    protected UserManager<GmIdentity> Manager { get; private set; }
    protected IHttpContextAccessor Accessor { get; private set; }
    protected HttpContext Context => this.Accessor.HttpContext;
    protected CookieAuthenticationOptions CookieAuthenticationOptions { get; private set; }
    public GmClaimsFactory(UserManager<GmIdentity> manager, IOptions<IdentityOptions> idOptions, IOptionsSnapshot<CookieAuthenticationOptions> cookieOptions, IHttpContextAccessor accessor) : base(manager, idOptions)
    {
        this.Manager = manager;
        this.Accessor = accessor;
        this.CookieAuthenticationOptions = cookieOptions.Get(IdentityConstants.ApplicationScheme);
    }
    protected virtual IEnumerable<Claim> GetClaims(GmIdentity user)
        => new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String)
            };
    public override async Task<ClaimsPrincipal> CreateAsync(GmIdentity user)
    {
        var principal = new ClaimsPrincipal();
        var claims = this.GetClaims(user).ToList();

        if (this.UserManager.SupportsUserSecurityStamp)
        {
            string stamp = await this.UserManager.GetSecurityStampAsync(user);
            claims.Add(new Claim(ClaimTypes.Sid, stamp, ClaimValueTypes.String));
        }

        if (this.UserManager.SupportsUserRole)
        {
            var roles = await this.UserManager.GetRolesAsync(user);

            foreach (string roleName in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName, ClaimValueTypes.String));
            }
        }

        if (this.UserManager.SupportsUserClaim)
        {
            var userClaims = await this.UserManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
        }
        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        principal.AddIdentity(identity);
        return principal;
    }
}