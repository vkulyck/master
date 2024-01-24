using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GmWeb.Web.Common.Auth;
using GmWeb.Web.Common.Auth.Services.Passport;
using JwtPassport = GmWeb.Web.Common.Auth.Tokens.JwtPassport;
using Microsoft.AspNetCore.Authentication;

namespace GmWeb.Web.Common.Auth.EventHandlers;
public class ApplicationAuthEventHandlers : CookieAuthEventHandlers
{
    public ApplicationAuthEventHandlers(UserManager<GmIdentity> manager, SignInManager<GmIdentity> signin, IHttpContextAccessor accessor, IPassportService passportService, IOptions<JwtAuthOptions> jwtAuthOptions)
        : base(manager, signin, accessor, passportService, jwtAuthOptions)
        { }
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        await base.ValidatePrincipal(context);
        async Task<bool> validate()
        {
            if (context == null)
                throw new System.ArgumentNullException(nameof(context));
            if (this.EncodedPassport == null)
                return await this.RejectPrincipal(context);
            var decoded = JsonConvert.DeserializeObject<JwtPassport>(this.EncodedPassport);
            JwtPassport passport;
            try
            {
            passport = await this.PassportService.LoginAsync(decoded);
            }
            catch (Exception)
            {
                return await this.RejectPrincipal(context);
            }

            if (passport == null)
                return await this.RejectPrincipal(context);
            this.User = await this.Manager.GetUserAsync(this.Principal);
            if (this.User == null)
                return await this.RejectPrincipal(context);

            if (this.UserId != await this.Manager.GetUserIdAsync(this.User))
                return await this.RejectPrincipal(context);
            else if (this.UserEmail != await this.Manager.GetEmailAsync(this.User))
                return await this.RejectPrincipal(context);
            else if (this.UserSecurityStamp != await this.Manager.GetSecurityStampAsync(this.User))
                return await this.RejectPrincipal(context);
            return await Task.FromResult(true);
        }
        await validate();
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context) => base.RedirectToAccessDenied(context);
    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context) => base.RedirectToLogin(context);
    public override Task RedirectToReturnUrl(RedirectContext<CookieAuthenticationOptions> context) => base.RedirectToReturnUrl(context);
    public override Task SignedIn(CookieSignedInContext context) => base.SignedIn(context);
    public override Task SigningIn(CookieSigningInContext context) => base.SigningIn(context);
    public override Task SigningOut(CookieSigningOutContext context) => base.SigningOut(context);
}
