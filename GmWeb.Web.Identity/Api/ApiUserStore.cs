using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;
using Newtonsoft.Json;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions.UserStore;
using AuthClaimNames = GmWeb.Logic.Utility.Identity.AuthClaimNames;

namespace GmWeb.Web.Identity.Api;
public class ApiUserStore : ICompleteUserStore<GmIdentity>
{
    private readonly ApiService _api;
    public ApiUserStore(ApiService service)
    {
        this._api = service;
    }

    #region User CRUD

    public Task<IdentityResult> CreateAsync(GmIdentity user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> DeleteAsync(GmIdentity user, CancellationToken cancellationToken)
    {
        // TODO: We aren't going to delete user accounts, so we need to either disable this completely or determine alternative behavior.
        throw new NotImplementedException();
    }

    #endregion

    #region User Lookup

    public async Task<GmIdentity> FindByEmailAsync(string normalizedEmail)
        => await this._api.FindUserByEmailAsync(normalizedEmail);
    public async Task<GmIdentity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this._api.FindUserByEmailAsync(normalizedEmail);
    }
    public async Task<GmIdentity> FindByIdAsync(string userId)
        => await this._api.FindUserByIdAsync(userId);
    public async Task<GmIdentity> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this._api.FindUserByIdAsync(userId);
    }
    public async Task<GmIdentity> FindByNameAsync(string normalizedUserName)
        => await this._api.FindUserByUsernameAsync(normalizedUserName);
    public async Task<GmIdentity> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this._api.FindUserByUsernameAsync(normalizedUserName);
    }

    #endregion

    #region Property Getters

    public async Task<string> GetEmailAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.Email);
    public async Task<bool> GetEmailConfirmedAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.EmailConfirmed);
    public async Task<string> GetNormalizedEmailAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.NormalizedEmail);
    public async Task<string> GetNormalizedUserNameAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.NormalizedUserName);
    public async Task<string> GetPasswordHashAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.PasswordHash);
    public async Task<string> GetPhoneNumberAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.PhoneNumber);
    public async Task<bool> GetPhoneNumberConfirmedAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.PhoneNumberConfirmed);
    public async Task<string> GetSecurityStampAsync(GmIdentity user, CancellationToken cancellationToken)
    {
        if (user.SecurityStamp == null)
            user = await this._api.GetUser(user.AccountID);
        return user.SecurityStamp;
    }
    public Task<bool> GetTwoFactorEnabledAsync(GmIdentity user, CancellationToken cancellationToken)
        => Task.FromResult(user.TwoFactorEnabled);
    public async Task<string> GetUserNameAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.UserName);
    public async Task<string> GetUserIdAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.Id.ToString());
    public async Task<bool> HasPasswordAsync(GmIdentity user, CancellationToken cancellationToken)
        => await Task.FromResult(user.HasPassword);

    #endregion

    #region Property Setters

    public async Task SetEmailAsync(GmIdentity user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetEmailConfirmedAsync(GmIdentity user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetNormalizedEmailAsync(GmIdentity user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetNormalizedUserNameAsync(GmIdentity user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetPasswordHashAsync(GmIdentity user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetPhoneNumberAsync(GmIdentity user, string phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetPhoneNumberConfirmedAsync(GmIdentity user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetSecurityStampAsync(GmIdentity user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        await this._api.UpdateUserAsync(user);
    }
    public async Task SetTwoFactorEnabledAsync(GmIdentity user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        await _api.UpdateUserAsync(user);
    }
    public async Task SetUserNameAsync(GmIdentity user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        await this._api.UpdateUserAsync(user);
    }

    #endregion

    #region Auth

    public async Task<string> GetAuthenticatorKeyAsync(GmIdentity user, CancellationToken cancellationToken)
    {
        if (user.AuthenticatorKey == null)
        {
            var key = await this.GetTfaKeyClaimAsync(user, TwoFactorProviderType.Authenticator, cancellationToken);
            user.AuthenticatorKey = key;
        }
        return user.AuthenticatorKey;
    }
    public async Task SetAuthenticatorKeyAsync(GmIdentity user, string key, CancellationToken cancellationToken)
    {
        await this.SetTfaKeyClaimAsync(user, TwoFactorProviderType.Authenticator, key, cancellationToken);
        user.AuthenticatorKey = key;
    }
    public async Task<IdentityResult> UpdateAsync(GmIdentity user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await this._api.UpdateUserAsync(user);
        return null;
    }
    private async Task<Claim> RefreshRecoveryCodes(GmIdentity user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var claims = await this.GetClaimsAsync(user, cancellationToken);
        var recoveryClaim = claims.SingleOrDefault(x => x.Type == AuthClaimNames.RecoveryCodes);
        if (recoveryClaim?.Value == null)
            return null;
        var codes = JsonConvert.DeserializeObject<List<string>>(recoveryClaim.Value);
        user.RecoveryCodes = codes;
        return recoveryClaim;
    }
    public async Task<int> CountCodesAsync(GmIdentity user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user.RecoveryCodes == null)
            await this.RefreshRecoveryCodes(user, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.FromResult(user.RecoveryCodes?.Count ?? 0);
    }
    public async Task<bool> RedeemCodeAsync(GmIdentity user, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user.RecoveryCodes == null)
            await this.RefreshRecoveryCodes(user, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.FromResult(user.RecoveryCodes.Remove(code));
    }
    public async Task ReplaceCodesAsync(GmIdentity user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        var codes = user.RecoveryCodes = recoveryCodes.ToList();
        var recoveryClaim = await this.RefreshRecoveryCodes(user, cancellationToken);
        var newClaim = new Claim(AuthClaimNames.RecoveryCodes, JsonConvert.SerializeObject(codes));
        if (recoveryClaim == null)
            await this.AddClaimsAsync(user, new Claim[] { newClaim }, cancellationToken);
        else
            await this.ReplaceClaimAsync(user, recoveryClaim, newClaim, cancellationToken);
    }

    #endregion

    #region Claims

    public Task<IList<Claim>> GetClaimsAsync(GmIdentity user, CancellationToken cancellationToken)
        => this._api.GetClaimsAsync(user, cancellationToken);
    public Task AddClaimsAsync(GmIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        => this._api.AddClaimsAsync(user, claims, cancellationToken);
    public Task ReplaceClaimAsync(GmIdentity user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        => this._api.ReplaceClaimAsync(user, claim, newClaim, cancellationToken);
    public Task RemoveClaimsAsync(GmIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        => this._api.RemoveClaimsAsync(user, claims, cancellationToken);
    public Task<IList<GmIdentity>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        => this._api.GetUsersForClaimAsync(claim, cancellationToken);

    #endregion

    public void Dispose() { }
}