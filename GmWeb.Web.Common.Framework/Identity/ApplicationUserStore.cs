using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNet.Identity;
using GmWeb.Common;

namespace GmWeb.Web.Common.Identity
{
    using TKey = System.String;
    public interface IApplicationUserStore<TUser>
        : IUserStore<TUser>
        , IUserEmailStore<TUser>
        , IUserLockoutStore<TUser, TKey>
        , IUserLoginStore<TUser>
        , IUserPasswordStore<TUser>
        , IUserPhoneNumberStore<TUser>
        , IUserSecurityStampStore<TUser>
        , IUserTwoFactorStore<TUser, TKey>
        where TUser : class, IApplicationUser, IUser<string>
    { }
}