using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using User = GmWeb.Logic.Data.Models.Carma.User;
using MutableSignInResult = GmWeb.Logic.Utility.Identity.Results.MutableSignInResult;

namespace GmWeb.Web.Common.Auth
{
    public class JwtSignInResult : MutableSignInResult, IJwtAuthResult
    {
        public static JwtSignInResult Failed => new JwtSignInResult(SignInResult.Failed);
        public static JwtSignInResult LockedOut => new JwtSignInResult(SignInResult.LockedOut);
        public static JwtSignInResult NotAllowed => new JwtSignInResult(SignInResult.NotAllowed);
        public static JwtSignInResult TwoFactorRequired => new JwtSignInResult(SignInResult.TwoFactorRequired);

        [JsonIgnore]
        public User User { get; protected set; }
        [JsonIgnore]
        public GmIdentity Identity { get; protected set; }
        [JsonIgnore]
        public JwtAuthToken AuthToken { get; set; }
        [JsonIgnore]
        public JwtRefreshToken RefreshToken { get; set; }

        private JwtSignInResult(SignInResult result) : base(result) { }
        public JwtSignInResult(GmIdentity identity, User user) : this(SignInResult.Success)
        {
            this.User = user;
            this.Identity = identity;
        }
    }
}
