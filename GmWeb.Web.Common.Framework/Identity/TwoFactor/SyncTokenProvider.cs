using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using OtpSharp;
using Base32;
using GmWeb.Web.Common.Utility;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Common.Identity
{
    public abstract class SyncTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser, string>
        where TUser : class, IUser<string>
    {
        protected abstract InformationType InformationType { get; }
        protected string Name => this.InformationType.GetDisplayName();
        protected string Key => this.InformationType.ToString();

        public override async Task<bool> IsValidProviderForUserAsync(UserManager<TUser, string> manager, TUser user)
        {
            var secretKey = await manager.GetUserTfaSecretAsync(user, this.InformationType);
            var providerEnabled = await manager.GetUserTfaEnabledAsync(user, this.InformationType);
            bool keyExists = string.IsNullOrWhiteSpace(secretKey);
            return keyExists && providerEnabled;
        }
    }
}