using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaseProviders = Microsoft.AspNet.Identity;
using BaseIdentities = Microsoft.AspNet.Identity.EntityFramework;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Common.Identity
{
    public static class TwoFactorProviderFactory<TUser>
        where TUser : BaseIdentities.IdentityUser
    {
        public static BaseProviders.TotpSecurityStampBasedTokenProvider<TUser, string> Create(InformationType method)
        {
            switch(method)
            {
                case InformationType.Email:
                    return new EmailTokenProvider<TUser>
                    {
                        Subject = "Security Code",
                        BodyFormat = "Your security code is {0}"
                    };
                case InformationType.Phone:
                    return new PhoneTokenProvider<TUser>
                    {
                        MessageFormat = "Your security code is {0}"
                    };
                case InformationType.GoogleAuthenticator:
                    return new GoogleAuthenticatorTokenProvider<TUser>();
                case InformationType.Yubikey:
                    return new YubikeyTokenProvider<TUser>();
                case InformationType.SecurityKey:
                    return new FidoTokenProvider<TUser>();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}