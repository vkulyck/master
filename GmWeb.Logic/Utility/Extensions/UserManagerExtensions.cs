using TotpConfig = GmWeb.Logic.Services.QRCode.TotpConfig;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using GmWeb.Logic.Utility.Identity.DTO;
using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;

namespace GmWeb.Logic.Utility.Extensions.UserManager;
public static class UserManagerExtensions
{
    public static async Task<TUser> FindByIdAsync<TUser>(this UserManager<TUser> manager, Guid? userId)
        where TUser : class
        => userId.HasValue ? await manager.FindByIdAsync(userId.Value) : default;
    public static async Task<TUser> FindByIdAsync<TUser>(this UserManager<TUser> manager, Guid userId)
        where TUser : class 
        => await manager.FindByIdAsync(userId.ToString());
    public static async Task<IdentityResult> AddClaimsAsync<TUser>(this UserManager<TUser> manager, TUser user, IEnumerable<ClaimDTO> dtoClaims)
        where TUser : class
    {
        var claims = dtoClaims.Select(x => new Claim(
            type: x.Type,
            value: x.Value,
            valueType: x.ValueType
        )).ToList();
        var result = await manager.AddClaimsAsync(user, claims);
        return result;
    }
    public static async Task<IdentityResult> RemoveClaimsAsync<TUser>(this UserManager<TUser> manager, TUser user, IEnumerable<ClaimDTO> dtoClaims)
        where TUser : class
    {
        var claims = dtoClaims.Select(x => new Claim(
            type: x.Type,
            value: x.Value,
            valueType: x.ValueType
        )).ToList();
        var result = await manager.RemoveClaimsAsync(user, claims);
        return result;
    }

    public static async Task<IdentityResult> ReplaceClaimAsync<TUser>(this UserManager<TUser> manager, TUser user, ClaimDTO claimDto, ClaimDTO newClaimDto)
        where TUser : class
    {
        var claim = new Claim(claimDto.Type, claimDto.Value, claimDto.ValueType);
        var newClaim = new Claim(newClaimDto.Type, newClaimDto.Value, newClaimDto.ValueType);
        return await manager.ReplaceClaimAsync(user, claim, newClaim);
    }

    public static async Task<IList<TUser>> GetUsersForClaimAsync<TUser>(this UserManager<TUser> manager, ClaimDTO claimDto)
        where TUser : class
    {
        var claim = new Claim(claimDto.Type, claimDto.Value, claimDto.ValueType);
        return await manager.GetUsersForClaimAsync(claim);
    }

    public static string GetTfaKey<TUser>(this UserManager<TUser> manager, TwoFactorProviderType provider)
        where TUser : class
    => AuthClaimNames.TwoFactorProviderToken(TwoFactorProviderType.Authenticator);
    public static string GetTfaKey<TUser>(this UserManager<TUser> manager, string provider)
        where TUser : class
    => AuthClaimNames.TwoFactorProviderToken(TwoFactorProviderType.Authenticator);

    public static async Task SetTfaKeyClaimAsync<TUser>(this UserManager<TUser> manager, TUser user, TwoFactorProviderType provider, string key)
        where TUser : class
    {
        var claims = await manager.GetClaimsAsync(user);
        var claimType = manager.GetTfaKey(TwoFactorProviderType.Authenticator);
        var keyClaim = claims.Where(x => x.Type == claimType).SingleOrDefault();
        if (keyClaim == null)
            await manager.AddClaimAsync(user, new Claim(claimType, key));
        else
            await manager.ReplaceClaimAsync(user, keyClaim, new Claim(claimType, key));
    }
    public static async Task<string> GetTfaKeyClaimAsync<TUser>(this UserManager<TUser> manager, TUser user, TwoFactorProviderType provider)
        where TUser : class
    {
        var claims = await manager.GetClaimsAsync(user);
        var claimType = manager.GetTfaKey(TwoFactorProviderType.Authenticator);
        var keyClaim = claims.Where(x => x.Type == claimType).SingleOrDefault();
        return keyClaim?.Value;
    }

    public static async Task<TotpConfig> GenerateAuthenticatorConfig<TUser>(this UserManager<TUser> manager, TUser user)
        where TUser : class
    {
        var key = await manager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await manager.ResetAuthenticatorKeyAsync(user);
            key = await manager.GetAuthenticatorKeyAsync(user);
        }

        var email = await manager.GetEmailAsync(user);
        var config = new TotpConfig(key, email);
        return config;
    }
}