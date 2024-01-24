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
#if AUTOLOGIN
    public class DevAutoLoginProvider<TUser> : SyncTokenProvider<TUser>
        where TUser : class, IUser<string>
    {
        protected override InformationType InformationType => InformationType.DevAutoLogin;

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, string> manager, TUser user)
        {
            return await Task.FromResult(true);
        }

        public override async Task<bool> IsValidProviderForUserAsync(UserManager<TUser, string> manager, TUser user)
        {
            return await Task.FromResult(true);
        }
    }
#endif
}