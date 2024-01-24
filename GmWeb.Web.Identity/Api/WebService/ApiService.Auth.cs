using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthHeader = System.Net.Http.Headers.AuthenticationHeaderValue;
using HttpMethod = System.Net.Http.HttpMethod;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using StatusCode = System.Net.HttpStatusCode;
using GmWeb.Web.Common.Auth.Tokens;

namespace GmWeb.Web.Identity.Api.WebService;
public partial class ApiService
{
    public async Task<IdentityResult> RegisterAsync(GmIdentity User, string Password)
    {
        var dto = MappingFactory.Instance.Map<GmIdentity, RegisterDTO>(User);
        dto.Password = Password;
        dto.ConfirmPassword = Password;
        var response = await this.Anonymous.PostAsync<IdentityResult>(AuthEndpoint(nameof(RegisterAsync)), dto);
        return response;
    }

    public async Task<TokenResult> TwoFactorAuthenticatorSignInAsync(string Email, string Code, bool IsPersistent, bool RememberClient)
    {
        var result = await this.Anonymous.PostAsync<TokenResult>("auth/two-factor-token-sign-in", new 
        { 
            Email,
            Code, 
            Provider = TfaProviderType.Authenticator, 
            IsPersistent,
            RememberClient
        });
        this.Passport = result;
        return result;
    }

    public async Task<IdentityResult> VerifyTwoFactorTokenAsync(GmIdentity user, TfaProviderType provider, string token)
    {
        var result = await this.PostAsync<IdentityResult>(AuthEndpoint(nameof(VerifyTwoFactorTokenAsync)), data: new
        {
            AccountID = user.AccountID,
            Provider = provider,
            Token = token
        });
        return result;
    }

    public async Task<TokenResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        => await this.Anonymous.PostAsync<TokenResult>("auth/two-factor-recovery-code-sign-in", new { RecoveryCode = recoveryCode });

    public async Task<TokenResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await this.Anonymous.PostAsync<TokenResult>("auth/token", new { email, password, isPersistent, lockoutOnFailure });
        if (result == null)
        {
            return new TokenResult
            {
                SignInResult = JwtSignInResult.Failed
            };
        }
        
        if (result.RequiresTwoFactor)
            return result;
        this.Passport = result;
        return result;

    }

    public async Task SendVerificationEmailAsync(string Email)
    => await this.Anonymous.PostAsync(AuthEndpoint(nameof(SendVerificationEmailAsync)), new { Email });
}