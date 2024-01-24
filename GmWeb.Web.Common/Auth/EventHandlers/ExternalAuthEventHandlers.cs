using GmWeb.Web.Common.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using GmWeb.Web.Common.Auth.Services.Passport;

namespace GmWeb.Web.Common.Auth.EventHandlers;
    public class ExternalAuthEventHandlers : CookieAuthEventHandlers
    {
    public ExternalAuthEventHandlers(UserManager<GmIdentity> manager, SignInManager<GmIdentity> signin, IHttpContextAccessor accessor, IPassportService passportService, IOptions<JwtAuthOptions> jwtAuthOptions)
        : base(manager, signin, accessor, passportService, jwtAuthOptions)
        { }
        public override Task ValidatePrincipal(CookieValidatePrincipalContext context) => base.ValidatePrincipal(context);
}
