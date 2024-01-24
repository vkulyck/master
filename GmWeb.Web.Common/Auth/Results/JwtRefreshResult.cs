using GmWeb.Logic.Data.Models.Carma;
using Newtonsoft.Json;

namespace GmWeb.Web.Common.Auth
{
    public class JwtRefreshResult : IJwtAuthResult
    {
        public static JwtRefreshResult BadRefreshToken => _BadRefreshToken;
        private static readonly JwtRefreshResult _BadRefreshToken = new JwtRefreshResult { IsRefreshTokenValid = false };
        public static JwtRefreshResult BadAuthToken => _BadAuthToken;
        private static readonly JwtRefreshResult _BadAuthToken = new JwtRefreshResult { IsAuthTokenValid = false };
        public static JwtRefreshResult RefreshTokenRevoked => _RefreshTokenRevoked;
        private static readonly JwtRefreshResult _RefreshTokenRevoked = new JwtRefreshResult { IsRefreshTokenRevoked = true };
        public static JwtRefreshResult RefreshTokenExpired => _RefreshTokenExpired;
        private static readonly JwtRefreshResult _RefreshTokenExpired = new JwtRefreshResult { IsRefreshTokenExpired = true };
        public static JwtRefreshResult RefreshFailed => _RefreshFailed;
        private static readonly JwtRefreshResult _RefreshFailed = new JwtRefreshResult { Succeeded = false };


        public bool IsAuthTokenValid { get; set; } = true;
        private bool? _IsRefreshTokenValid;
        public bool IsRefreshTokenValid //=> !(this.IsRefreshTokenExpired || this.IsRefreshTokenRevoked);
        {
            get => !this.IsRefreshTokenExpired && !this.IsRefreshTokenRevoked && (this._IsRefreshTokenValid ?? true);
            set => this._IsRefreshTokenValid = value;
        }
        private bool? _IsRefreshTokenRevoked;
        public bool IsRefreshTokenRevoked
        {
            get => this._IsRefreshTokenRevoked ?? this.RefreshToken?.IsRevoked ?? false;
            set => this._IsRefreshTokenRevoked = value;
        }
        private bool? _IsRefreshTokenExpired;
        public bool IsRefreshTokenExpired
        {
            get => this._IsRefreshTokenExpired ?? this.RefreshToken?.IsExpired ?? false;
            set => this._IsRefreshTokenExpired = value;
        }
        private bool? _Succeeded;
        public bool Succeeded
        {
            get => this.IsRefreshTokenValid && this.IsAuthTokenValid && (this._Succeeded ?? true);
            set => this._Succeeded = value;
        }

        [JsonIgnore]
        public GmIdentity Identity { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public JwtAuthToken AuthToken { get; set; }
        [JsonIgnore]
        public JwtRefreshToken RefreshToken { get; set; }

        protected JwtRefreshResult() { }
        public JwtRefreshResult(JwtRefreshToken refreshToken)
        {
            this.RefreshToken = refreshToken;
        }
    }
}
