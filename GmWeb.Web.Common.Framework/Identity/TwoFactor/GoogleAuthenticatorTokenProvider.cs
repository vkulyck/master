using System;
using System.Linq;
using System.Threading.Tasks;
using Base32;
using Microsoft.AspNet.Identity;
using OtpSharp;
using System.Security.Claims;
using GmWeb.Web.Common.Utility;
using Google.Authenticator;
using GmWeb.Logic.Enums;
using GmWeb.Common.Crypto;

namespace GmWeb.Web.Common.Identity
{
    public class GoogleAuthenticatorTokenProvider<TUser> : SyncTokenProvider<TUser>, ISharedTokenGeneratorProvider<TUser>
        where TUser : class, IUser<string>
    {
        protected override InformationType InformationType => InformationType.GoogleAuthenticator;

        public async Task<string> ConfigureSharedSecret(UserManager<TUser, string> manager, TUser user)
        {
            var secretKey = SymmetricEncryptor.GenerateKey32();
            await manager.SetUserTfaSecretAsync(user, this.InformationType, secretKey);
            return secretKey;
        }

        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser, string> manager, TUser user)
        {
            string secretKey = await manager.GetUserTfaSecretAsync(user, this.InformationType);
            var otp = new Totp(Base32Encoder.Decode(secretKey));
            return otp.ComputeTotp();
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, string> manager, TUser user)
        {
            var secretKey = await manager.GetUserTfaSecretAsync(user, this.InformationType);
            long timeStepMatched;
            var otp = new Totp(Base32Encoder.Decode(secretKey));
            var valid = otp.VerifyTotp(token, out timeStepMatched, new VerificationWindow(2, 2));
            return valid;
        }
    }
}