using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace GmWeb.Web.Common.Auth
{
    [Owned]
    public class JwtRefreshToken : JwtRevocableToken
    {
        public const string DataSourceKey = "RefreshToken";
        public override string TokenType => "refresh";

        public JwtRefreshToken(IEnumerable<Claim> claims, JwtAuthOptions settings)
            : base(claims, settings) { }
        public JwtRefreshToken(GmIdentity identity, JwtAuthOptions settings, string ip)
            : base(JwtRevocableToken.CreateTokenDescriptor(identity, settings, ip, settings.RefreshLifetime), settings) { }
        public static JwtRefreshToken Deserialize(string jwtEncodedString, JwtAuthOptions settings, bool validate = false)
            => JwtRevocableToken.Deserialize<JwtRefreshToken>(jwtEncodedString, settings, validate: validate);
    }
}
