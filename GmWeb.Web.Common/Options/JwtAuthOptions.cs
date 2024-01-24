using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using GmWeb.Logic.Utility.Identity;

namespace GmWeb.Web.Common.Options
{
    public class JwtAuthOptions : JwtBearerOptions
    {
        private string _Key;
        public string Key
        {
            get => this._Key;
            set
            {
                this._Key = value;
                this.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Key));
            }
        }

        public TokenDataSource RefreshSource { get; set; } = TokenDataSource.Cookie;
        public bool EnableDetailedErrorResults { get; set; } = false;
        public SecurityKey IssuerSigningKey
        {
            get => this.TokenValidationParameters.IssuerSigningKey;
            set => this.TokenValidationParameters.IssuerSigningKey = value;
        }
        public string Issuer
        {
            get => this.TokenValidationParameters.ValidIssuer;
            set => this.TokenValidationParameters.ValidIssuer = value;
        }
        public bool ValidateIssuer
        {
            get => this.TokenValidationParameters.ValidateIssuer;
            set => this.TokenValidationParameters.ValidateIssuer = value;
        }
        public bool ValidateAudience
        {
            get => this.TokenValidationParameters.ValidateAudience;
            set => this.TokenValidationParameters.ValidateAudience = value;
        }
        public bool ValidateLifetime
        {
            get => this.TokenValidationParameters.ValidateLifetime;
            set => this.TokenValidationParameters.ValidateLifetime = value;
        }
        public bool ValidateIssuerSigningKey
        {
            get => this.TokenValidationParameters.ValidateIssuerSigningKey;
            set => this.TokenValidationParameters.ValidateIssuerSigningKey = value;
        }
        public TimeSpan AuthLifetime { get; set; }
        public TimeSpan RefreshLifetime { get; set; }
        public TimeSpan ClockSkew
        {
            get => this.TokenValidationParameters.ClockSkew;
            set => this.TokenValidationParameters.ClockSkew = value;
        }

        public CookieOptions RefreshCookie { get; } = new CookieOptions();

        public JwtAuthOptions() { }
    }
}
