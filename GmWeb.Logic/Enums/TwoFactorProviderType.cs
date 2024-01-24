using GmWeb.Logic.Utility.Extensions.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GmWeb.Logic.Enums
{
    public enum TwoFactorProviderType
    {
        None = 0,
        [Display(Name = "SMS Code")]
        Phone,
        [Display(Name = "Email Verification")]
        Email,
        [Display(Name = "Google Authenticator")]
        Authenticator,
        [Display(Name = "FIDO Security Key")]
        SecurityKey,
        [Display(Name = "Yubico Security Key")]
        Yubikey
    }

    public static class InformationTypeExtensions
    {
        public static bool IsTwoFactor(this TwoFactorProviderType infoType)
        {
            switch (infoType)
            {
                case TwoFactorProviderType.Authenticator:
                case TwoFactorProviderType.SecurityKey:
                case TwoFactorProviderType.Yubikey:
                case TwoFactorProviderType.Phone:
                case TwoFactorProviderType.Email:
                    return true;
                default:
                    return false;
            }
        }

        public static IEnumerable<TwoFactorProviderType> TwoFactorTypes() => EnumExtensions
            .GetEnumValues<TwoFactorProviderType>()
            .Where(x => x.IsTwoFactor())
        ;

        public static bool EmploysSharedTokenGenerators(this TwoFactorProviderType infoType)
        {
            switch (infoType)
            {
                case TwoFactorProviderType.Authenticator:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsUserProperty(this TwoFactorProviderType infoType)
        {
            switch (infoType)
            {
                case TwoFactorProviderType.Email:
                case TwoFactorProviderType.Phone:
                    return true;
                default:
                    return false;
            }
        }
    }
}
