using GmWeb.Logic.Utility.Extensions.UserManager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Security.Claims;
using GmWeb.Logic.Utility.Identity;

namespace GmWeb.Web.Identity.Api;
public class ApiSignInManager : CompleteSignInManager
{
    protected ApiUserManager Manager { get; private set; }
    protected ApiService Api { get; private set; }
    public ApiSignInManager(
        ApiService api, 
        ApiUserManager manager, 
        IHttpContextAccessor contextAccessor, 
        ApiClaimsFactory claimsFactory, 
        IOptions<IdentityOptions> optionsAccessor, 
        ILogger<SignInManager<GmIdentity>> logger, 
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<GmIdentity> confirmation
    )
    : base(manager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        this.Api = api;
        this.Manager = manager;
    }

    public override async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
    {
        var email = this.Context.User.FindFirst(ClaimTypes.Email)?.Value;
        var result = await this.Api.TwoFactorAuthenticatorSignInAsync(email, code, isPersistent, rememberClient);
        if (result.SignInResult.Succeeded)
        {
            var user = await this.Manager.FindByEmailAsync(email);
            await this.SignInAsync(user, isPersistent);
        }
        return result.SignInResult;
    }
    public override async Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
        var result = await this.Api.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        if (result.SignInResult.Succeeded)
        {
            var user = await this.Manager.FindByIdAsync(result.AccountID);
            await this.SignInAsync(user, isPersistent: false);
        }
        return result.SignInResult;
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await this.Api.PasswordSignInAsync(email: userName, password: password, isPersistent, lockoutOnFailure);
        if (result.SignInResult.Succeeded)
        {
            var user = await this.Manager.FindByEmailAsync(userName);
            await this.SignInAsync(user, isPersistent);
        }
        return result.SignInResult;
    }
}