using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using GmWeb.Common.Crypto;
using GmWeb.Web.Common.Models;
using GmWeb.Web.Common.Identity;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using Google.Authenticator;

namespace GmWeb.Web.Common.Utility
{
    public static class TwoFactorExtensions
    {
        public static SetupCode GenerateQrCode(string email, string secretKey = null)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                secretKey = SymmetricEncryptor.GenerateKey32();
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("goodmojo", email, secretKey, secretIsBase32: true, QRPixelsPerModule: 32);
            return setupInfo;
        }

        public static async Task<bool> TfaValidateAsync<TUser>(this UserManager<TUser, string> manager, string purpose, TUser user, InformationType infoType, string token)
            where TUser : class, IUser<string>
        {
            var provider = manager.GetTfaProvider(infoType);
            bool result = false;
            try
            {
                result = await provider.ValidateAsync(purpose, token, manager, user);
            }
#if RELEASE
            catch { } // TODO: Log the exception       
#else
            finally { }
#endif
            return result;
        }

        public static TotpSecurityStampBasedTokenProvider<TUser, string> GetTfaProvider<TUser>(this UserManager<TUser, string> manager, InformationType infoType)
            where TUser : class, IUser<string>
        {
            var provider = manager.TwoFactorProviders[infoType.ToString()];
            var stampProvider = provider as TotpSecurityStampBasedTokenProvider<TUser, string>;
            if (stampProvider == null)
                throw new ArgumentException($"No such provider is configured: '{infoType}'");
            return stampProvider;
        }

        public static async Task<string> GetUserTfaSecretAsync<TUser>(this UserManager<TUser, string> manager, TUser user, InformationType infoType)
            where TUser : class, IUser<string>
        {
            var claims = await manager.GetClaimsAsync(user.Id);
            var secretClaim = await claims.FindAsync(TwoFactorClaimTypes.TokenProviderSecret, infoType.ToString());
            return secretClaim?.Value;
        }

        public static async Task SetUserTfaSecretAsync<TUser>(this UserManager<TUser, string> manager, TUser user, InformationType infoType, string secret)
            where TUser : class, IUser<string>
        => await manager._UpdateUserTfaClaimAsync(user, value: secret, TwoFactorClaimTypes.TokenProviderSecret, infoType.ToString());

        public static async Task DeleteUserTfaSecretAsync<TUser>(this UserManager<TUser, string> manager, TUser user, InformationType infoType)
            where TUser : class, IUser<string>
        => await manager._DeleteUserTfaClaimAsync(user, TwoFactorClaimTypes.TokenProviderSecret, infoType.ToString());

        public static async Task<bool> GetUserTfaEnabledAsync<TUser>(this UserManager<TUser, string> manager, TUser user, InformationType infoType)
            where TUser : class, IUser<string>
        {
            var claims = await manager.GetClaimsAsync(user.Id);
            var secretClaim = await claims.FindAsync(TwoFactorClaimTypes.TokenProviderEnabled, infoType.ToString());
            if (bool.TryParse(secretClaim?.Value, out bool result))
                return result;
            return false;
        }

        public static async Task EnableUserTfaAsync<TUser>(this UserManager<TUser, string> manager, TUser user, InformationType infoType)
            where TUser : class, IUser<string>
        => await manager._UpdateUserTfaClaimAsync(user, value: true, TwoFactorClaimTypes.TokenProviderEnabled, infoType.ToString());

        public static async Task DisableUserTfaAsync<TUser>(this UserManager<TUser, string> manager, TUser user, InformationType infoType)
            where TUser : class, IUser<string>
        => await manager._UpdateUserTfaClaimAsync(user, value: false, TwoFactorClaimTypes.TokenProviderEnabled, infoType.ToString());

        public static async Task _UpdateUserTfaClaimAsync<TUser, TValue>(
            this UserManager<TUser, string> manager, TUser user, TValue value, params string[] typeElements
        )
            where TUser : class, IUser<string>
            where TValue : struct
        => await manager._UpdateUserTfaClaimAsync<TUser>(user, value.ToString(), typeElements);

        public static async Task _UpdateUserTfaClaimAsync<TUser>(
            this UserManager<TUser, string> manager, TUser user, string value, params string[] typeElements
        )
            where TUser : class, IUser<string>
        {
            var claims = await manager.GetClaimsAsync(user.Id);
            var type = string.Join(";", typeElements);
            var secretClaim = await claims.FindAsync(type);
            if (secretClaim != null)
            {
                if (secretClaim.Value == value)
                    return;
                await manager.RemoveClaimAsync(user.Id, secretClaim);
            }
            secretClaim = new Claim(type, value);
            await manager.AddClaimAsync(user.Id, secretClaim);
        }
        public static async Task _DeleteUserTfaClaimAsync<TUser>(
            this UserManager<TUser, string> manager, TUser user, params string[] typeElements
        )
            where TUser : class, IUser<string>
        {
            var claims = await manager.GetClaimsAsync(user.Id);
            var type = string.Join(";", typeElements);
            var secretClaim = await claims.FindAsync(type);
            if (secretClaim != null)
            {
                await manager.RemoveClaimAsync(user.Id, secretClaim);
            }
        }
    }
}