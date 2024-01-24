using Provider = GmWeb.Logic.Enums.TwoFactorProviderType;
namespace GmWeb.Logic.Utility.Identity;
public class AuthClaimNames
{
    /// <summary>
    /// The client's IP address at the time the token was requested.
    /// </summary>
    public const string ClientIpAddress = "ip";
    /// <summary>
    /// Whether the token was revoked during the validation process.
    /// </summary>
    public const string IsRevoked = "rev";
    /// <summary>
    /// Whether the token was revoked during the validation process.
    /// </summary>
    public const string EncodedRefreshToken = "jwe-ref";
    public const string EncodedAuthToken = "jwe-auth";
    public const string EncodedPassport = "jwe-auth";

    public const string RecoveryCodes = "tfa-codes";
    public static string TwoFactorProviderToken(Provider provider)
    {
        return $"tfa-key;{provider}";
    }
}