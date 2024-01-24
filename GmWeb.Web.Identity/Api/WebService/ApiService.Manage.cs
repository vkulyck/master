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

namespace GmWeb.Web.Identity.Api.WebService;
public partial class ApiService
{
    public async Task<IdentityResult> ResetAuthenticatorKeyAsync(GmIdentity user)
    {
        var response = await this.PostAsync<IdentityResult>($"manage/reset-authenticator?AccountID={user.AccountID}");
        return response;
    }
    public async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(GmIdentity user, int number)
    {
        var response = await this.PostAsync<RecoveryCodesDTO>("manage/generate-new-two-factor-recovery-codes", new RecoveryCodesDTO 
        { 
            AccountID = user.AccountID.ToString(), 
            Number = number 
        });
        return response.RecoveryCodes;
    }
    public async Task<IdentityResult> ChangePasswordAsync(string OldPassword, string NewPassword)
    {
        var response = await this.PostAsync<IdentityResult>("manage/change-password", new { OldPassword, NewPassword, ConfirmPassword = NewPassword });
        return response;
    }
    public async Task<IdentityResult> SetPasswordAsync(string NewPassword)
        => await this.SetPasswordAsync(NewPassword, NewPassword);
    public async Task<IdentityResult> SetPasswordAsync(string NewPassword, string ConfirmPassword)
    {
        var response = await this.PostAsync<IdentityResult>("manage/set-password", new { NewPassword, ConfirmPassword });
        return response;
    }
    public async Task SendConfirmEmailChangeAsync(string NewEmail)
        => await this.Anonymous.PostAsync(MgrEndpoint(nameof(SendConfirmEmailChangeAsync)), new { NewEmail });
    public async Task SendConfirmEmailAsync(string NewEmail)
        => await this.SendVerificationEmailAsync(NewEmail);
    public async Task SendPasswordResetEmailAsync(string Email)
        => await this.Anonymous.PostAsync(MgrEndpoint(nameof(SendPasswordResetEmailAsync)), new { Email });
    public async Task<IdentityResult> ResetPasswordAsync(string Email, string Token, string NewPassword)
        => await this.Anonymous.PostAsync<IdentityResult>(MgrEndpoint(nameof(ResetPasswordAsync)), new { Email, Token, NewPassword });
    public async Task<IdentityResult> ChangeEmailAsync(Guid AccountID, string Email, string Code)
        => await this.PostAsync<IdentityResult>(MgrEndpoint(nameof(ChangeEmailAsync)), new { AccountID, Email, Code });
    public async Task<IdentityResult> ConfirmEmailAsync(Guid AccountID, string Code)
        => await this.Anonymous.PostAsync<IdentityResult>(MgrEndpoint(nameof(ConfirmEmailAsync)), new { AccountID, Code });
}