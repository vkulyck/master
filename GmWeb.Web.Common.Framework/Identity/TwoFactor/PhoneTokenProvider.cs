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
using BaseProviders = Microsoft.AspNet.Identity;

namespace GmWeb.Web.Common.Identity
{
    public class PhoneTokenProvider<TUser> : BaseProviders.PhoneNumberTokenProvider<TUser>
        where TUser : class, IUser<string>
    {
        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser, string> manager, TUser user)
        {
            var result = await base.GenerateAsync(purpose, manager, user);
            return result;
        }

        public override async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser, string> manager, TUser user)
        {
            var result = await base.GetUserModifierAsync(purpose, manager, user);
            return result;
        }

        public override async Task<bool> IsValidProviderForUserAsync(UserManager<TUser, string> manager, TUser user)
        {
            var result = await base.IsValidProviderForUserAsync(manager, user);
            return result;
        }

        public override async Task NotifyAsync(string token, UserManager<TUser, string> manager, TUser user)
        {
            await base.NotifyAsync(token, manager, user);
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, string> manager, TUser user)
        {
            var result = await base.ValidateAsync(purpose, token, manager, user);
            return result;
        }
    }
}