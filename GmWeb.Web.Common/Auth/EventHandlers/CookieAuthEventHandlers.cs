using GmWeb.Web.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthClaimNames = GmWeb.Logic.Utility.Identity.AuthClaimNames;
using GmWeb.Web.Common.Auth.Services.Passport;
using JwtPassport = GmWeb.Web.Common.Auth.Tokens.JwtPassport;

namespace GmWeb.Web.Common.Auth.EventHandlers;
public class CookieAuthEventHandlers : CookieAuthenticationEvents
{
    protected UserManager<GmIdentity> Manager { get; private set; }
    protected SignInManager<GmIdentity> SignIn { get; private set; }
    protected IHttpContextAccessor Accessor { get; private set; }
    protected IPassportService PassportService { get; private set; }
    protected ClaimsPrincipal Principal { get; private set; }
    protected GmIdentity User { get; set; }
    protected JwtAuthToken AuthToken { get; private set; }
    protected JwtRefreshToken RefreshToken { get; private set; }
    protected JwtAuthOptions JwtAuthOptions { get; private set; }

    protected string UserId
        => this.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    protected string UserEmail
        => this.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
    protected string UserSecurityStamp
        => this.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
    protected string EncodedUserAuthToken
        => this.Principal.Claims.FirstOrDefault(claim => claim.Type == AuthClaimNames.EncodedAuthToken)?.Value;
    protected string EncodedUserRefreshToken
        => this.Principal.Claims.FirstOrDefault(claim => claim.Type == AuthClaimNames.EncodedRefreshToken)?.Value;
    protected string EncodedPassport
        => this.Principal.Claims.FirstOrDefault(claim => claim.Type == AuthClaimNames.EncodedPassport)?.Value;

    public CookieAuthEventHandlers(UserManager<GmIdentity> manager, SignInManager<GmIdentity> signin, IHttpContextAccessor accessor, IPassportService passportService, IOptions<JwtAuthOptions> jwtAuthOptions)
    {
        this.Manager = manager;
        this.SignIn = signin;
        this.Accessor = accessor;
        this.PassportService = passportService;
        this.JwtAuthOptions = jwtAuthOptions.Value;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        this.Principal = context.Principal;
        await base.ValidatePrincipal(context);
    }

    protected async Task<bool> RejectPrincipal(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await this.SignIn.SignOutAsync();
        return await Task.FromResult(false);
    }
    public override Task RedirectToLogout(RedirectContext<CookieAuthenticationOptions> context) => base.RedirectToLogout(context);
}
