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

namespace GmWeb.Web.Identity.Api;
public class ApiClaimsFactory : GmClaimsFactory
{
    protected ApiService Api { get; private set; }
    public ApiClaimsFactory(ApiService api, UserManager<GmIdentity> manager, IOptions<IdentityOptions> idOptions, IOptionsSnapshot<CookieAuthenticationOptions> cookieOptions, IHttpContextAccessor accessor)
        : base(manager, idOptions, cookieOptions, accessor)
    {
        this.Api = api;
    }
    protected override IEnumerable<Claim> GetClaims(GmIdentity user) => base.GetClaims(user).Union(
        new Claim[] { new Claim(AuthClaimNames.EncodedPassport, JsonConvert.SerializeObject(this.Api.Passport), ClaimValueTypes.String) }
    ).ToList();
}