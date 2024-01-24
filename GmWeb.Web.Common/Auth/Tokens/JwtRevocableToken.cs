using GmWeb.Logic.Utility.Extensions.Chronometry;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using GmWeb.Logic.Utility.Redis;
using AuthClaimNames = GmWeb.Logic.Utility.Identity.AuthClaimNames;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace GmWeb.Web.Common.Auth
{
    public abstract class JwtRevocableToken : JwtSecurityToken, IAccountToken
    {
        protected JwtAuthOptions Settings { get; private set; }
        protected static JwtSecurityTokenHandler GetHandler(JwtAuthOptions options)
            => options
            .SecurityTokenValidators
            .OfType<JwtSecurityTokenHandler>()
            .SingleOrDefault()
        ;
        protected JwtSecurityTokenHandler Handler => GetHandler(this.Settings);
        [JsonIgnore]
        public string UserId => this.Subject;
        [JsonIgnore]
        public abstract string TokenType { get; }
        [JsonIgnore]
        public DateTime Expires => this.ValidTo;
        [JsonIgnore]
        public DateTime Created => this.GetClaimValue<long>(JwtRegisteredClaimNames.Iat).FromUnixTimeSeconds();
        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow >= this.Expires;
        [JsonIgnore]
        public bool IsRevoked => this.GetClaimValue<bool>(AuthClaimNames.IsRevoked);
        [JsonIgnore]
        public bool IsValid => !this.IsRevoked && !this.IsExpired;

        #region IAccountToken
        [JsonIgnore]
        private Guid? _AccountID;
        public Guid AccountID => _AccountID ?? (_AccountID = new Guid(this.UserId)).Value;
        [JsonIgnore]
        public string TokenID => this.Id;
        [JsonIgnore]
        public TimeSpan Lifetime => this.ValidTo - this.ValidFrom;

        [JsonIgnore]
        public string Audience => this.GetClaimValue(JwtRegisteredClaimNames.Aud);
        #endregion
        public JwtRevocableToken(IEnumerable<Claim> claims, JwtAuthOptions options)
            : base(
                claims: claims,
                signingCredentials: new SigningCredentials(options.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
            )
        { this.Settings = options; }
        public JwtRevocableToken(SecurityTokenDescriptor desc, JwtAuthOptions options)
            : base(
                issuer: desc.Issuer, audience: desc.Audience, claims: desc.Claims.Select(x => new Claim(x.Key, x.Value?.ToString())),
                notBefore: desc.NotBefore, expires: desc.Expires,
                signingCredentials: desc.SigningCredentials
        )
        { this.Settings = options; }
        protected static SecurityTokenDescriptor CreateTokenDescriptor(
            GmIdentity identity, JwtAuthOptions options,
            string ip, TimeSpan lifetime, IEnumerable<string> roles = null
        )
        {
            IEnumerable<KeyValuePair<string, object>> claims = GenerateDefaultClaims(identity, options, ip, lifetime);
            if (roles != null)
                claims = claims.Union(roles.Select(x => KeyValuePair.Create<string, object>("roles", x)));

            var claimMap = claims.ToDictionary(x => x.Key, x => x.Value);
            var descriptor = new SecurityTokenDescriptor
            {
                Claims = claimMap,
                Expires = DateTime.Now + lifetime + options.ClockSkew,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now - options.ClockSkew,
                SigningCredentials = new SigningCredentials(options.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
            };
            return descriptor;
        }

        protected static List<KeyValuePair<string, object>> GenerateDefaultClaims(GmIdentity identity, JwtAuthOptions options, string ip, TimeSpan lifetime)
        {
            var claims = new List<KeyValuePair<string, object>>
            {
                CreateClaim(JwtRegisteredClaimNames.Sub, identity.Id),
                CreateClaim(JwtRegisteredClaimNames.Jti, Guid.NewGuid()),
                CreateClaim(JwtRegisteredClaimNames.Email, identity.Email),
                CreateClaim(AuthClaimNames.ClientIpAddress, ip),
                CreateClaim(AuthClaimNames.IsRevoked, false),
                CreateClaim(JwtRegisteredClaimNames.Iat, DateTime.Now),
                CreateClaim(JwtRegisteredClaimNames.Iss, options.Issuer),
                CreateClaim(JwtRegisteredClaimNames.Aud, options.Audience),
                CreateClaim(JwtRegisteredClaimNames.Exp, DateTime.Now + lifetime + options.ClockSkew),
                CreateClaim(JwtRegisteredClaimNames.Nbf, DateTime.Now - options.ClockSkew),

            };
            return claims;
        }

        protected static KeyValuePair<string, object> CreateClaim(string claimType, DateTime claimValue)
            => CreateClaim(claimType, claimValue.ToUnixTimeSeconds());
        protected static KeyValuePair<string, object> CreateClaim<T>(string claimType, T claimValue)
            => new KeyValuePair<string, object>(claimType, claimValue);

        protected string GetClaimValue(string claimType) => GetClaimValue(claimType, this.Claims);
        protected static string GetClaimValue(string claimType, IEnumerable<Claim> claims)
        {
            var claim = claims.SingleOrDefault(x => x.Type == claimType);
            if (claim == null)
                throw new Exception($"Claim missing from token: {claimType}");
            if (string.IsNullOrWhiteSpace(claim.Value))
                throw new Exception($"Empty claim value found in token: {claimType}");
            return claim.Value;
        }
        protected T GetClaimValue<T>(string claimType)
            where T : struct
        {
            string claimValue = this.GetClaimValue(claimType);
            if (claimValue.TryParse(out T result))
                return result;
            throw new Exception($"Claim type '{claimType}' in {this.TokenType} token could not be parsed as '{typeof(T).Name}'.");
        }

        public string Serialize()
        {
            string serialized = this.Handler.WriteToken(this);
            return serialized;
        }

        protected static TToken Deserialize<TToken>(string jwtEncodedString, JwtAuthOptions options, bool validate = false)
            where TToken : JwtRevocableToken
        {
            var handler = JwtRevocableToken.GetHandler(options);
            IEnumerable<Claim> claims;

            if (validate)
            {
                var principal = handler.ValidateToken(jwtEncodedString, options.TokenValidationParameters, out var validated);
                claims = principal.Claims;
            }
            else
            {
                var decoded = handler.ReadJwtToken(jwtEncodedString);
                claims = decoded.Claims;
            }
            var token = (TToken)Activator.CreateInstance(typeof(TToken), claims, options);
            return token;
        }
    }
}
