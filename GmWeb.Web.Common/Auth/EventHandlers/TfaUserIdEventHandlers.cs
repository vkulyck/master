using GmWeb.Web.Common.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using GmWeb.Web.Common.Auth.Services.Passport;

namespace GmWeb.Web.Common.Auth.EventHandlers;
public class TfaUserIdEventHandlers : CookieAuthEventHandlers
{
    public TfaUserIdEventHandlers(UserManager<GmIdentity> manager, SignInManager<GmIdentity> signin, IHttpContextAccessor accessor, IPassportService passportService, IOptions<JwtAuthOptions> jwtAuthOptions)
        : base(manager, signin, accessor, passportService, jwtAuthOptions)
    { }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        await base.ValidatePrincipal(context);
        if (this.UserEmail == null)
            await this.RejectPrincipal(context);
    }
}
