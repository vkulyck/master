using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Redis;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Extensions.UserManager;
using GmWeb.Logic.Utility.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LoginViewModel = GmWeb.Web.Common.Auth.Models.LoginViewModel;

namespace GmWeb.Web.Common.Auth.Services.JwtAuth;
public partial class JwtAuthTokenService
{
    private readonly JwtAuthOptions _jwtOptions;
    public JwtAuthOptions JwtOptions => this._jwtOptions;
    private readonly RedisCache _cache;
    private readonly UserManager<GmIdentity> _manager;
    private readonly IdentityOptions _idOptions;
    private readonly CarmaCache _carma;
    private readonly IHttpContextAccessor _accessor;
    private readonly CookieAuthenticationOptions _tfarmCookieOptions;

    public JwtAuthTokenService(
        IOptionsSnapshot<JwtAuthOptions> jwtOptions,
        IOptionsSnapshot<CookieAuthenticationOptions> tfarmCookieOptions,
        IOptions<IdentityOptions> idOptions,
        RedisCache cache,
        UserManager<GmIdentity> manager,
        CarmaContext context,
        IHttpContextAccessor accessor
    )
    {
        this._jwtOptions = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme);
        this._tfarmCookieOptions = tfarmCookieOptions.Get(IdentityConstants.TwoFactorRememberMeScheme);
        this._idOptions = idOptions.Value;
        this._cache = cache;
        this._manager = manager;
        this._carma = new CarmaCache(context);
        this._accessor = accessor;
    }

    protected string GetClientIpAddress()
    {
        var headers = this._accessor.HttpContext.Request.Headers;
        IPAddress ip;

        if (headers.ContainsKey("X-Forwarded-For"))
            return headers["X-Forwarded-For"];
        else
        {
            var conn = this._accessor.HttpContext.Connection;
            var address = conn.RemoteIpAddress ?? conn.LocalIpAddress;
            if (address == null)
                // TODO: This can occur during unit testing. Replace with Mocking:
                // https://stackoverflow.com/questions/60723816/net-core-unit-test-how-to-mock-httpcontext-remoteipaddress
                ip = IPAddress.None;
            else
                ip = address.MapToIPv4();
        }
        return ip.ToString();
    }

    public async Task<JwtAccessResult> ValidateRequestTokenAsync(JwtSecurityToken token)
    {
        var authToken = new JwtAuthToken(token.Claims, this._jwtOptions);
        bool isValid = await this._cache.ValidateAccountTokenAsync(authToken);
        if (isValid)
            return JwtAccessResult.Valid;
        else
            return JwtAccessResult.Revoked;
    }

    public async Task<JwtSignInResult> ValidateCredentials(LoginViewModel model)
    {
        var identity = await this._manager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (identity == null)
            return JwtSignInResult.Failed;

        if (!identity.EmailConfirmed)
            return JwtSignInResult.NotAllowed;

        // Used as user lock
        if (await this._manager.IsLockedOutAsync(identity))
            return JwtSignInResult.LockedOut;

        bool success = await this._manager.CheckPasswordAsync(identity, model.Password).ConfigureAwait(false);
        if (success)
        {
            if (this._manager.SupportsUserLockout)
                await this._manager.ResetAccessFailedCountAsync(identity);
            if (identity.TwoFactorEnabled)
            {
                var rmToken = new TwoFactorRememberMeToken
                {
                    AccountID = identity.AccountID
                };
                var remembered = await this._cache.ValidateAccountTokenAsync(rmToken);
                if (!remembered)
                    return JwtSignInResult.TwoFactorRequired;
            }
            var user = await this._carma.Users
                .Where(x => x.AccountID == identity.Id)
                .SingleOrDefaultAsync()
            ;
            return new JwtSignInResult(identity, user);
        }
        else if (this._manager.SupportsUserLockout && identity.LockoutEnabled && model.LockoutOnFailure)
            await this._manager.AccessFailedAsync(identity);
        return JwtSignInResult.Failed;
    }

    public async Task<JwtSignInResult> CreateAuthTokenAsync(LoginViewModel model)
    {
        var result = await this.ValidateCredentials(model);
        if (!result.Succeeded)
            return result;
        return await this.CreateAuthTokenAsync(result);
    }
    public async Task<JwtSignInResult> CreateAuthTokenAsync(GmIdentity identity)
    {
        var user = await this._carma.Users
            .Where(x => x.AccountID == identity.Id)
            .SingleOrDefaultAsync()
        ;
        return await this.CreateAuthTokenAsync(new JwtSignInResult(identity, user));
    }
    private async Task<JwtSignInResult> CreateAuthTokenAsync(JwtSignInResult result)
    {
        // authentication successful so generate jwt and refresh tokens
        bool revokeResult = await this.RevokeAccountTokensAsync(result.Identity);
        if (!revokeResult)
            throw new Exception($"Could not revoke existing account tokens.");
        result.AuthToken = await this.GenerateAuthTokenAsync(result.Identity);
        bool isAuthValid = await this.ValidateAuthTokenAsync(result.AuthToken);
        if (!isAuthValid)
            throw new Exception($"Could not generate a valid auth token.");
        var refreshToken = await this.UpdateRefreshToken(result);
        bool isRefreshValid = await this.ValidateRefreshTokenAsync(refreshToken);
        if (!isRefreshValid)
            throw new Exception($"Could not generate a valid refresh token.");

        return result;
    }

    public async Task<JwtRefreshResult> RefreshAuthTokenAsync(string serializedRefreshToken = null)
    {
        var (result, identity, user) = await this.ParseRefreshToken(serializedRefreshToken);
        if (!result.Succeeded)
            return result;

        bool isCurrentRefreshValid = await this.ValidateRefreshTokenAsync(result.RefreshToken);
        if (!isCurrentRefreshValid)
            return JwtRefreshResult.BadRefreshToken;

        await this.RevokeAccountTokensAsync(identity);
        bool isRevokedRefreshValid = await this.ValidateRefreshTokenAsync(result.RefreshToken);
        if (isRevokedRefreshValid)
            return JwtRefreshResult.BadRefreshToken;

        result.RefreshToken = await this.GenerateRefreshTokenAsync(identity);
        bool isNewRefreshValid = await this.ValidateRefreshTokenAsync(result.RefreshToken);
        if (!isNewRefreshValid)
            return JwtRefreshResult.RefreshFailed;
        var refreshToken = await this.UpdateRefreshToken(result);

        result.AuthToken = await this.GenerateAuthTokenAsync(identity);
        bool isAuthValid = await this.ValidateAuthTokenAsync(result.AuthToken);
        if (!isAuthValid)
            return JwtRefreshResult.RefreshFailed;
        return result;
    }

    protected async Task<(JwtAuthToken Token, GmIdentity Identity, User User)> ParseAuthToken(string serializedAuthToken)
    {
        var token = JwtAuthToken.Deserialize(serializedAuthToken, this._jwtOptions);
        var userData = await this.LookupTokenUser(token);
        return (token, userData.Identity, userData.User);
    }
    protected async Task<(JwtRefreshResult Token, GmIdentity Identity, User User)> ParseRefreshToken(string serializedRefreshToken = null)
    {
        var refreshResult = this.ReadRefreshToken(serializedRefreshToken);
        var userData = await this.LookupTokenUser(refreshResult.RefreshToken);
        refreshResult.User = userData.User;
        refreshResult.Identity = userData.Identity;
        return (refreshResult, userData.Identity, userData.User);
    }
    protected async Task<(GmIdentity Identity, User User)> LookupTokenUser(JwtRevocableToken token)
    {
        var identity = await this._manager.FindByIdAsync(token.AccountID);
        User user = null;
        if (identity != null)
            user = await this._carma.Users
                .Where(x => x.AccountID == identity.Id)
                .SingleOrDefaultAsync()
            ;
        return (identity, user);
    }
    public async Task<JwtLogoutResult> RevokeTokensAsync(GmIdentity identity)
    {
        await this._cache.RevokeAccountTokensAsync(identity.AccountID);
        return JwtLogoutResult.Success;
    }

    private async Task<JwtAuthToken> GenerateAuthTokenAsync(GmIdentity identity)
    {
        var roles = await this._manager.GetRolesAsync(identity).ConfigureAwait(false);
        var token = new JwtAuthToken(identity, this._jwtOptions, this.GetClientIpAddress(), roles);
        await this._cache.RegisterAccountTokenAsync(token);
        return token;
    }

    private async Task<JwtRefreshToken> GenerateRefreshTokenAsync(GmIdentity identity)
    {
        var token = new JwtRefreshToken(identity, this._jwtOptions, this.GetClientIpAddress());
        await this._cache.RegisterAccountTokenAsync(token);
        return token;
    }
    private async Task<bool> ValidateAuthTokenAsync(JwtAuthToken token)
    {
        if (token == null)
            throw new Exception($"Cannot validate a null auth token.");
        bool result = await this._cache.ValidateAccountTokenAsync(token);
        return result;
    }
    private async Task<bool> ValidateRefreshTokenAsync(JwtRefreshToken token)
    {
        if (token == null)
            throw new Exception($"Cannot validate a null auth token.");
        bool result = await this._cache.ValidateAccountTokenAsync(token);
        return result;
    }
    private async Task<bool> RevokeAuthTokenAsync(JwtAuthToken token)
    {
        if (token == null)
            throw new Exception($"Cannot revoke a null auth token.");
        bool result = await this._cache.RevokeAccountTokenAsync(token);
        return result;
    }
    private async Task<bool> RevokeRefreshTokenAsync(JwtRefreshToken token)
    {
        if (token == null)
            throw new Exception($"Cannot revoke a null refresh token.");
        bool result = await this._cache.RevokeAccountTokenAsync(token);
        return result;
    }
    private async Task<bool> RevokeAccountTokensAsync(GmIdentity identity)
    {
        if (identity == null)
            throw new Exception($"Cannot revoke tokens without an identity argument.");
        bool result = await this._cache.RevokeAccountTokensAsync(identity.Id);
        return result;
    }

    public JwtRefreshResult ReadRefreshToken(string serializedRefreshToken = null)
    {
        var refreshSource = this._jwtOptions.RefreshSource;
        string serialized = null;
        if (refreshSource.HasFlag(TokenDataSource.Parameter))
            serialized = serializedRefreshToken;
        if (serialized == null && refreshSource.HasFlag(TokenDataSource.Header))
        {
            var headers = this._accessor.HttpContext.Request.Headers;
            if (headers.ContainsKey(JwtRefreshToken.DataSourceKey))
                serialized = headers[JwtRefreshToken.DataSourceKey];
        }
        if (serialized == null && refreshSource.HasFlag(TokenDataSource.Cookie))
        {
            var cookies = this._accessor.HttpContext.Request.Cookies;
            if (cookies.ContainsKey(JwtRefreshToken.DataSourceKey))
                serialized = cookies[JwtRefreshToken.DataSourceKey];
        }

        if (string.IsNullOrWhiteSpace(serialized))
            return JwtRefreshResult.BadRefreshToken;
        var refreshToken = JwtRefreshToken.Deserialize(serialized, this._jwtOptions);
        var result = new JwtRefreshResult(refreshToken);
        return result;
    }

    public async Task<JwtRefreshToken> UpdateRefreshToken(IJwtAuthResult result)
    {
        var refreshToken = await this.GenerateRefreshTokenAsync(result.Identity);
        var refreshSource = this._jwtOptions.RefreshSource;
        if (refreshSource.HasFlag(TokenDataSource.Parameter))
            result.RefreshToken = refreshToken;
        if (refreshSource.HasFlag(TokenDataSource.Header))
        {
            var headers = this._accessor.HttpContext.Response.Headers;
            headers[JwtRefreshToken.DataSourceKey] = refreshToken.Serialize();
        }
        if (refreshSource.HasFlag(TokenDataSource.Cookie))
        {
            this._accessor.HttpContext.Response.Cookies.Append(JwtRefreshToken.DataSourceKey, refreshToken.Serialize(), this._jwtOptions.RefreshCookie);
        }
        return refreshToken;
    }

    public void DeleteRefreshToken()
    {
        var refreshSource = this._jwtOptions.RefreshSource;
        if (refreshSource.HasFlag(TokenDataSource.Header))
        {
            var headers = this._accessor.HttpContext.Response.Headers;
            headers.Remove(JwtRefreshToken.DataSourceKey);
        }
        if (refreshSource.HasFlag(TokenDataSource.Cookie))
        {
            var cookies = this._accessor.HttpContext.Response.Cookies;
            cookies.Delete(JwtRefreshToken.DataSourceKey);
        }
    }
}
