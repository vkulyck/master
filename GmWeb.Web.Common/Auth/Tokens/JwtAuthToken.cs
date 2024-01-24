using System.Collections.Generic;
using System.Security.Claims;

namespace GmWeb.Web.Common.Auth
{
    public class JwtAuthToken : JwtRevocableToken
    {
        public static string CookieKey => "auth-token";
        public override string TokenType => "auth";
        public JwtAuthToken(IEnumerable<Claim> claims, JwtAuthOptions options)
            : base(claims, options) { }
        public JwtAuthToken(GmIdentity identity, JwtAuthOptions options, string ip, IEnumerable<string> roles)
            : base(JwtRevocableToken.CreateTokenDescriptor(identity, options, ip, options.AuthLifetime, roles), options) { }

        public static JwtAuthToken Deserialize(string jwtEncodedString, JwtAuthOptions options, bool validate = false)
            => JwtRevocableToken.Deserialize<JwtAuthToken>(jwtEncodedString, options, validate: validate);
    }
}
