using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using OtpSharp;
using Base32;
using GmWeb.Web.Common.Utility;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Common.Identity
{
    public class YubikeyTokenProvider<TUser> : SyncTokenProvider<TUser>
        where TUser : class, IUser<string>
    {
        protected override InformationType InformationType => InformationType.Yubikey;
        public string ApiClientID => System.Configuration.ConfigurationManager.AppSettings[$"{this.GetProviderKey()}.ApiClientId"].ToString();
        public string ApiSecretKey => System.Configuration.ConfigurationManager.AppSettings[$"{this.GetProviderKey()}.ApiSecretKey"].ToString();

        public override Task<string> GenerateAsync(string purpose, UserManager<TUser, string> manager, TUser user)
        {
            return Task.FromResult(string.Empty);
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, string> manager, TUser user)
        {
            var secretKey = await manager.GetUserTfaSecretAsync(user, this.InformationType);
            var enabled = await manager.GetUserTfaEnabledAsync(user, this.InformationType);
            var client = new Yubico.YubicoClient(ApiClientID, ApiSecretKey);
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                if (enabled)
                    throw new ArgumentException($"User {user.Id} is misconfigured; Yubikey is enabled but no secret key exists.");
                var verifyResult = await client.VerifyAsync(token);
                if (verifyResult.Status != Yubico.YubicoResponseStatus.Ok)
                    return false;
                secretKey = verifyResult.PublicId;
                await manager.SetUserTfaSecretAsync(user, this.InformationType, secretKey);
                return true;
            }
            else
            {
                if (!enabled)
                    throw new ArgumentException($"User {user.Id} has not enabled Yubikey authentication.");
                var authResult = await client.AuthenticateAsync(secretKey, token);
                return authResult == Yubico.YubicoAuthenticationResponse.AuthenticationSuccess;
            }
        }
    }
}