using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Identity;
using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;

namespace GmWeb.Logic.Utility.Extensions.UserStore;
public static class UserStoreExtensions
{
    public static async Task SetTfaKeyClaimAsync<TUser>(this ICompleteUserStore<TUser> store, TUser user, TwoFactorProviderType provider, string key, CancellationToken cancellationToken)
        where TUser : class
    {
        var claims = await store.GetClaimsAsync(user, cancellationToken);
        var claimType = AuthClaimNames.TwoFactorProviderToken(TwoFactorProviderType.Authenticator);
        var keyClaim = claims.Where(x => x.Type == claimType).SingleOrDefault();
        if (keyClaim == null)
            await store.AddClaimsAsync(user, new Claim[] { new Claim(claimType, key) }, cancellationToken);
        else
            await store.ReplaceClaimAsync(user, keyClaim, new Claim(claimType, key), cancellationToken);
    }
    public static async Task<string> GetTfaKeyClaimAsync<TUser>(this ICompleteUserStore<TUser> store, TUser user, TwoFactorProviderType provider, CancellationToken cancellationToken)
        where TUser : class
    {
        var claims = await store.GetClaimsAsync(user, cancellationToken);
        var claimType = AuthClaimNames.TwoFactorProviderToken(TwoFactorProviderType.Authenticator);
        var keyClaim = claims.Where(x => x.Type == claimType).SingleOrDefault();
        return keyClaim?.Value;
    }

    public static CancellationToken Uncancellable => CancellationToken.None;
    public static async Task<IdentityResult> CreateAsync<TUser>(this IUserStore<TUser> store, TUser user) where TUser : class
        => await store.CreateAsync(user, Uncancellable);
    public static async Task<IdentityResult> DeleteAsync<TUser>(this IUserStore<TUser> store, TUser user) where TUser : class
        => await store.DeleteAsync(user, Uncancellable);
    public static async Task<TUser> FindByNameAsync<TUser>(this IUserStore<TUser> store, string name) where TUser : class
        => await store.FindByNameAsync(name, Uncancellable);
    public static async Task<TUser> FindByEmailAsync<TUser>(this IUserEmailStore<TUser> store, string email) where TUser : class
        => await store.FindByEmailAsync(email, Uncancellable);
    public static async Task<TUser> FindByIdAsync<TUser>(this IUserStore<TUser> store, string id) where TUser : class
        => await store.FindByIdAsync(id, Uncancellable);
}