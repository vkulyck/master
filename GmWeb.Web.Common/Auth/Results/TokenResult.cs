using MutableSignInResult = GmWeb.Logic.Utility.Identity.Results.MutableSignInResult;

namespace GmWeb.Web.Common.Auth.Tokens;

public class TokenResult : JwtPassport
{
    public MutableSignInResult SignInResult { get; set; }
    public bool RequiresTwoFactor => this.SignInResult?.RequiresTwoFactor ?? false;
    public static TokenResult Create(IJwtAuthResult result)
    {
        var signinResult = result as MutableSignInResult;
        if (signinResult != null && signinResult.RequiresTwoFactor)
            return new TokenResult { SignInResult = signinResult };
        return new TokenResult
        {
            AuthToken = result.AuthToken.Serialize(),
            RefreshToken = result.RefreshToken?.Serialize(),
            UtcExpiration = result.AuthToken.ValidTo,
            AccountID = result.User.AccountID.Value,
            UserID = result.User.UserID,
            LookupID = result.User.LookupID,
            SignInResult = result as MutableSignInResult
        };
    }
}