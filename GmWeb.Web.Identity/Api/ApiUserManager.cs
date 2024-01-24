using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Identity;
using GmWeb.Logic.Utility.Config;

namespace GmWeb.Web.Identity.Api;

public class ApiUserManager : CompleteUserManager
{
    private readonly ApiService _api;
    public ApiService Api => this._api;
    new protected ApiUserStore Store => (ApiUserStore)base.Store;
    public GmWebOptions WebOptions => this.Api.WebOptions;
    public ApiUserManager(
        ApiService api,
        ApiUserStore store,
        IOptions<IdentityOptions> optionsAccessor,
        GmPasswordHasher passwordHasher,
        IEnumerable<IUserValidator<GmIdentity>> userValidators,
        IEnumerable<IPasswordValidator<GmIdentity>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<GmIdentity>> logger
    ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        this._api = api;
    }

    private IdentityResult CreateIdentityResult(bool Success, string Description = null)
    {
        if (Success)
            return IdentityResult.Success;
        return IdentityResult.Failed(new IdentityError
        {
            Description = Description ?? "The requested operation could not be completed."
        });
    }

    public override Task<GmIdentity> FindByEmailAsync(string email)
        => _api.FindUserByEmailAsync(email);
    public override Task<GmIdentity> FindByIdAsync(string userId)
        => _api.FindUserByIdAsync(userId);
    public override Task<GmIdentity> FindByNameAsync(string userName)
        => _api.FindUserByUsernameAsync(userName);
    public override async Task<IdentityResult> ChangePasswordAsync(GmIdentity user, string currentPassword, string newPassword)
        => await _api.ChangePasswordAsync(currentPassword, newPassword);
    public override async Task<IdentityResult> ChangeEmailAsync(GmIdentity user, string newEmail, string token)
        => await this.Api.ChangeEmailAsync(user.AccountID, newEmail, token);

    public override async Task<IdentityResult> AddPasswordAsync(GmIdentity user, string password)
    {
        if (user.HasPassword)
            return IdentityResult.Success;
        var result = await this._api.SetPasswordAsync(password);
        return result;
    }

    public override Task<IdentityResult> CreateAsync(GmIdentity user)
        => this.CreateAsync(user, null);
    public override async Task<IdentityResult> CreateAsync(GmIdentity user, string password)
        => await this._api.RegisterAsync(user, password);
    public async Task<IdentityResult> ConfirmEmailAsync(Guid AccountID, string Code)
        => await this._api.ConfirmEmailAsync(AccountID, Code);
    public override async Task<IdentityResult> ResetPasswordAsync(GmIdentity user, string token, string newPassword)
        => await _api.ResetPasswordAsync(user.Email, token, newPassword);
    public override async Task<IdentityResult> SetTwoFactorEnabledAsync(GmIdentity user, bool enabled)
    {
        await this.Store.SetTwoFactorEnabledAsync(user, enabled, CancellationToken.None);
        return this.CreateIdentityResult(user.TwoFactorEnabled == enabled);
    }
    public override async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(GmIdentity user, int number)
        => await this._api.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

    public override async Task<string> GetAuthenticatorKeyAsync(GmIdentity user)
    {
        return await this.Store.GetAuthenticatorKeyAsync(user, CancellationToken.None);
    }

    public override async Task<IdentityResult> ResetAuthenticatorKeyAsync(GmIdentity user)
    {
        var result = await this._api.ResetAuthenticatorKeyAsync(user);
        return result;
    }

    public override async Task<IdentityResult> SetPhoneNumberAsync(GmIdentity user, string phoneNumber)
    {
        await this.Store.SetPhoneNumberAsync(user, phoneNumber, CancellationToken.None);
        return this.CreateIdentityResult(user.PhoneNumber == phoneNumber);
    }

    public override async Task<bool> VerifyTwoFactorTokenAsync(GmIdentity user, string tokenProvider, string token)
    {
        var result = await this.Api.VerifyTwoFactorTokenAsync(user, tokenProvider.ToProvider(), token);
        return result.Succeeded;
    }

    public override async Task<IdentityResult> DeleteAsync(GmIdentity user)
        => await this.Store.DeleteAsync(user, CancellationToken.None);

    public override async Task<bool> CheckPasswordAsync(GmIdentity user, string password)
    {
        var result = await this.PasswordHasher.ValidateAsync(this, user, password);
        return result.Succeeded;
    }
}
