using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.TestHost;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using GmWeb.Tests.Api.Mocking;
using GmWeb.Tests.Api.Extensions;
using LoginModel = GmWeb.Web.Common.Auth.Models.LoginViewModel;
using TokenModel = GmWeb.Web.Common.Auth.Tokens.TokenResult;
using AuthToken = GmWeb.Web.Common.Auth.JwtAuthToken;
using RefreshToken = GmWeb.Web.Common.Auth.JwtRefreshToken;
using BaseToken = GmWeb.Web.Common.Auth.JwtRevocableToken;
using AuthClaimNames = GmWeb.Logic.Utility.Identity.AuthClaimNames;
using JwtRefreshRequest = GmWeb.Web.Common.Auth.Requests.JwtRefreshRequest;
using GmWeb.Logic.Utility.Extensions.Chronometry;
using RedisCache = GmWeb.Logic.Utility.Redis.RedisCache;
using JwtAuthOptions = GmWeb.Web.Common.Options.JwtAuthOptions;
using UserDTO = GmWeb.Web.Common.Models.Carma.UserDTO;
using TokenDataSource = GmWeb.Logic.Utility.Identity.TokenDataSource;
using Microsoft.AspNetCore.Mvc.Testing;
using GmWeb.Web.Common.Auth;

namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class AuthControllerTests : ControllerTestBase<AuthControllerTests>
    {
        protected JwtAuthOptions JwtOptions { get; private set; }
        protected IdentityOptions IdOptions { get; private set; }
        protected RedisCache RedisCache { get; private set; }
        public AuthControllerTests(TestApplicationFactory factory) : base(factory)
        {
            this.IdOptions = this.Scope.ServiceProvider.GetService<IOptions<IdentityOptions>>()?.Value;
            this.RedisCache = this.Scope.ServiceProvider.GetService<RedisCache>();
        }

        protected override void GetServices(WebApplicationFactory<FakeStartup> factory)
        {
            base.GetServices(factory);
            (this.Client, this.CookieContainer) = this.Factory.CreateJwtCookieClient();
            this.JwtOptions = this.Scope.ServiceProvider.GetService<IOptionsSnapshot<JwtAuthOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme)
            ;
        }

        #region Utility
        protected StringContent Serialize<T>(T data)
            => new(JsonConvert.SerializeObject(data), Encoding.UTF8, MediaTypeNames.Application.Json);
        protected RefreshToken GetCurrentRefreshToken()
        {
            var cookieCollection = this.CookieContainer.GetCookies(this.Client.BaseAddress);
            var refreshCookie = cookieCollection[RefreshToken.DataSourceKey];
            var refreshToken = RefreshToken.Deserialize(refreshCookie.Value, this.JwtOptions);
            return refreshToken;
        }
        #endregion

        #region Validation Methods
        protected void validateToken(BaseToken token, GmIdentity identity)
        {
            TimeSpan lifetime;
            if (token is RefreshToken)
                lifetime = this.JwtOptions.RefreshLifetime;
            else if (token is AuthToken)
                lifetime = this.JwtOptions.AuthLifetime;
            else throw new Exception($"Invalid token type: {token.GetType().Name}");

            var groups = token.Claims.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            var duplicateGroups = groups.Where(x => x.Value.Count > 1).ToDictionary(x => x.Key, x => x.Value);

            this.validateClaim(token, JwtRegisteredClaimNames.Sub, identity.Id);
            this.validateClaim(token, JwtRegisteredClaimNames.Jti, token.Id);
            this.validateClaim(token, JwtRegisteredClaimNames.Email, identity.Email);
            this.validateClaim(token, AuthClaimNames.ClientIpAddress, IPAddress.None);
            this.validateClaim(token, AuthClaimNames.IsRevoked, false);
            this.validateClaim(token, JwtRegisteredClaimNames.Iat, DateTime.UtcNow);
            this.validateClaim(token, JwtRegisteredClaimNames.Iss, this.JwtOptions.Issuer);
            this.validateClaim(token, JwtRegisteredClaimNames.Aud, this.JwtOptions.Audience);
            this.validateClaim(token, JwtRegisteredClaimNames.Exp, DateTime.UtcNow + lifetime + this.JwtOptions.ClockSkew);
            this.validateClaim(token, JwtRegisteredClaimNames.Nbf, DateTime.UtcNow - this.JwtOptions.ClockSkew);
        }
        protected void validateClaim<T>(BaseToken token, string claimName, T expectedValue)
            => this.validateClaim(token, claimName, expectedValue.ToString());
        protected void validateClaim(BaseToken token, string claimName, DateTime expectedValue)
        {
            var claim = token.Claims.Single(x => x.Type == claimName);
            if (!long.TryParse(claim.Value, out long epochSeconds))
                Assert.False(true, $"Expected epoch seconds, got: {claim.Value}");
            var actualDate = epochSeconds.FromUnixTimeSeconds();
            var (min, max) = expectedValue.ToWindow(TimeSpan.FromMinutes(1));
            Assert.InRange(actualDate, min, max);
        }
        protected void validateClaim(BaseToken token, string claimName, string expectedValue)
        {
            var claim = token.Claims.Single(x => x.Type == claimName);
            var actualValue = claim.Value;
            Assert.Equal(expectedValue, actualValue);
        }

        #endregion

        #region API Request/Response Methods
        protected async Task<TResult> ValidateCreateTokenError<TResult>(
            string Email, string Password, bool? RememberMe = null, bool? LockoutOnFailure = null,
            HttpStatusCode? ExpectedStatus = null
        ) where TResult : IJwtAuthResult
        {
            LoginModel model;
            if (RememberMe.HasValue && LockoutOnFailure.HasValue)
                model = new LoginModel(Email, Password, RememberMe.Value, LockoutOnFailure.Value);
            else if (RememberMe.HasValue)
                model = new LoginModel(Email, Password, RememberMe.Value);
            else
                model = new LoginModel(Email, Password);
            return await this.ValidateRequestDataErrorAsync<TResult>(
                Controller: "Auth", Action: "token", Method: HttpMethod.Post,
                RequestData: model,
                ExpectedStatus: ExpectedStatus
            );
        }
        protected async Task<TResult> ValidateRefreshTokenError<TResult>(
            HttpStatusCode? ExpectedStatus = null
        ) where TResult: IJwtAuthResult
            => await this.ValidateRequestDataErrorAsync<TResult>(
                Controller: "auth", Action: "retoken", Method: HttpMethod.Post,
                ExpectedStatus: ExpectedStatus
        );
        protected async Task ConfirmExpirationAsync(BaseToken token)
        {
            var isTokenRegistered = await this.RedisCache.ValidateAccountTokenAsync(token);
            Assert.False(isTokenRegistered);
        }
        protected async Task ConfirmRegistrationAsync(BaseToken token)
        {
            var isTokenRegistered = await this.RedisCache.ValidateAccountTokenAsync(token);
            Assert.True(isTokenRegistered);
        }
        protected async Task<T> CreateTokenAsync<T>(string Email, string Password)
        {
            var model = new LoginModel(Email, Password);
            var tokenModel = await this.RequestDataAsync<T>(
                Controller: "auth", Action: "token", Method: HttpMethod.Post,
                RequestData: model, ExpectedStatus: HttpStatusCode.OK
            );
            return tokenModel;
        }
        protected async Task<T> RefreshTokenAsync<T>(JwtRefreshRequest RequestModel = null, HttpStatusCode? ExpectedStatus = null)
            => await this.RequestDataAsync<T>(
                Controller: "auth", Action: "retoken", Method: HttpMethod.Post,
                RequestData: RequestModel,
                ExpectedStatus: ExpectedStatus ?? HttpStatusCode.OK
        );

        protected async Task ValidateGetUserErrorAsync(Guid AccountID, string ExpectedErrorMessage = null, HttpStatusCode? ExpectedStatus = null)
            => await this.ValidateGetUserErrorAsync(
                AccountID: AccountID,
                ExpectedErrorMessages: ExpectedErrorMessage == null
                    ? default
                    : new string[] { ExpectedErrorMessage },
                ExpectedStatus: ExpectedStatus
            );

        protected async Task ValidateGetUserErrorAsync(Guid AccountID, string[] ExpectedErrorMessages = null, HttpStatusCode? ExpectedStatus = null)
            => await this.ValidateRequestDataErrorAsync("User", string.Empty, HttpMethod.Get,
                RequestData: new { AccountID },
                ExpectedErrorMessages: ExpectedErrorMessages,
                ExpectedStatus: ExpectedStatus
            );
        protected async Task<UserDTO> GetUserAsync(Guid AccountID)
            => await this.GetUserAsync(new LookupUserDTO { AccountID = AccountID });
        protected async Task<UserDTO> GetUserAsync(LookupUserDTO lookup)
        {
            var user = await this.RequestDataAsync<UserDTO>(
                Controller: "User", Action: "", Method: HttpMethod.Get,
                RequestData: lookup,
                ExpectedStatus: HttpStatusCode.OK
            );
            return user;
        }

        protected async Task ValidateLogoutErrorAsync(string ExpectedErrorMessage, HttpStatusCode? ExpectedStatus = null)
        {
            var apiResponse = await this.RequestDataAsync<ErrorResponse>(
                Controller: "Auth", Action: "Logout", Method: HttpMethod.Post,
                ExpectedStatus: ExpectedStatus
            );
            Assert.NotEmpty(apiResponse.Errors);
            Assert.NotNull(apiResponse.Errors[0]);
            Assert.False(string.IsNullOrWhiteSpace(ExpectedErrorMessage));
            Assert.Equal(ExpectedErrorMessage, apiResponse.Errors[0].Message);
        }
        protected async Task LogoutAsync()
            => await this.RequestDataAsync(
                Controller: "Auth", Action: "Logout", Method: HttpMethod.Post,
                ExpectedStatus: HttpStatusCode.OK
            );

        #endregion

        #region Tests
        [Fact]
        public async Task TestInvalidLoginAttempt()
        {
            var result = await this.ValidateCreateTokenError<JwtSignInResult>(
                Email: this.Entities.AdminEmail, Password: "invalid password",
                ExpectedStatus: HttpStatusCode.BadRequest
            );
            Assert.False(result.Succeeded);

            var identity = await this.RefreshIdentityAsync();
            identity.LockoutEnd = null;
            identity.AccessFailedCount = 0;
            await this.Manager.UpdateAsync(identity);
            identity = await this.RefreshIdentityAsync();
            Assert.Equal(0, identity.AccessFailedCount);
            Assert.Null(identity.LockoutEnd);

            await this.TestAdminAuth();
        }
        [Fact]
        public async Task TestEmailVerificationRequirement()
        {
            GmIdentity identity = await this.RefreshIdentityAsync();
            identity.EmailConfirmed = false;
            await this.Manager.UpdateAsync(identity);

            identity = await this.RefreshIdentityAsync();
            Assert.False(identity.EmailConfirmed);
            var result = await this.ValidateCreateTokenError<JwtSignInResult>(
                Email: this.Entities.AdminEmail, Password: this.Entities.AdminPassword,
                ExpectedStatus: HttpStatusCode.BadRequest
            );
            Assert.False(result.Succeeded);

            identity.EmailConfirmed = true;
            await this.Manager.UpdateAsync(identity);
            identity = await this.RefreshIdentityAsync();
            Assert.True(identity.EmailConfirmed);

            await this.TestAdminAuth();
        }
        [Fact]
        public async Task TestAccountLockout()
        {
            GmIdentity identity = null;
            for (int i = 0; i < this.IdOptions.Lockout.MaxFailedAccessAttempts; i++)
            {
                var loginFailureResult = await this.ValidateCreateTokenError<JwtSignInResult>(
                    Email: this.Entities.AdminEmail, Password: "incorrect password",
                    RememberMe: true, LockoutOnFailure: true,
                    ExpectedStatus: HttpStatusCode.BadRequest
                );
                Assert.False(loginFailureResult.Succeeded);
                identity = await this.RefreshIdentityAsync();
                // Note: Upon reaching the maximum AccessFailedCount, the lockout period is started and the count is then reset to 0.
                Assert.Equal((i + 1) % this.IdOptions.Lockout.MaxFailedAccessAttempts, identity.AccessFailedCount);
            }

            DateTimeOffset
                min = DateTimeOffset.UtcNow,
                max = DateTimeOffset.UtcNow + this.IdOptions.Lockout.DefaultLockoutTimeSpan
            ;
            Assert.NotNull(identity);
            Assert.NotNull(identity.LockoutEnd);
            Assert.InRange(identity.LockoutEnd.Value, min, max);

            var lockoutResult = await this.ValidateCreateTokenError<JwtSignInResult>(
                Email: this.Entities.AdminEmail, Password: this.Entities.AdminPassword,
                ExpectedStatus: HttpStatusCode.BadRequest
            );
            Assert.True(lockoutResult.IsLockedOut);

            identity = await this.RefreshIdentityAsync();
            identity.LockoutEnd = null;
            identity.AccessFailedCount = 0;
            await this.Manager.UpdateAsync(identity);
            identity = await this.RefreshIdentityAsync();
            Assert.Equal(0, identity.AccessFailedCount);
            Assert.Null(identity.LockoutEnd);
        }
        [Fact]
        public async Task TestAdminAuth()
        {
            var tokenModel = await this.CreateTokenAsync<TokenModel>(
                Email: this.Entities.AdminEmail,
                Password: this.Entities.AdminPassword
            );
            var token = AuthToken.Deserialize(tokenModel.AuthToken, this.JwtOptions);
            this.validateToken(token, this.Entities.AdminIdentity);
        }
        [Fact]
        public async Task TestRefreshSources()
        {
            void clearRefreshCookie()
            {
                var cookieCollection = this.CookieContainer.GetCookies(this.Client.BaseAddress);
                var refreshCookie = cookieCollection[RefreshToken.DataSourceKey];
                if (refreshCookie != null)
                {
                    refreshCookie.Expired = true;
                    refreshCookie.Expires = DateTime.UtcNow.AddMinutes(-1.0D);
                }
            }
            var sources = new List<TokenDataSource> { TokenDataSource.Cookie, TokenDataSource.Header, TokenDataSource.Parameter };
            foreach(var requestedSource in sources)
            {
                foreach (var enabledSource in sources)
                {
                    await this.InitializeServicesAsync(this.Factory
                        .WithWebHostBuilder(builder =>
                        {
                            builder.ConfigureTestServices(services =>
                            {
                                // Set the auth token lifetime to a few seconds to speed up testing.
                                // Note: Setting this value too small may cause failures with internal API sanity checks.
                                // Setting this value to 0 will cause an exception within the Redis setex implementation.
                                services.PostConfigureAll<JwtAuthOptions>(opts =>
                                {
                                    opts.RefreshSource = enabledSource;
                                });
                            });
                        })
                    );

                    var tokenModel = await this.CreateTokenAsync<TokenModel>(
                        Email: this.Entities.AdminEmail,
                        Password: this.Entities.AdminPassword
                    );
                    var authToken = AuthToken.Deserialize(tokenModel.AuthToken, this.JwtOptions);
                    this.validateToken(authToken, this.Entities.AdminIdentity);
                    var isTokenValid = await this.RedisCache.ValidateAccountTokenAsync(authToken);
                    Assert.True(isTokenValid);

                    this.Client.Authorize(authToken);
                    string serializedRefreshToken = null;
                    switch (enabledSource)
                    {
                        case TokenDataSource.Cookie:
                            var cookieCollection = this.CookieContainer.GetCookies(this.Client.BaseAddress);
                            var refreshCookie = cookieCollection[RefreshToken.DataSourceKey];
                            Assert.NotNull(refreshCookie);
                            serializedRefreshToken = refreshCookie.Value;
                            break;
                        case TokenDataSource.Header:
                            Assert.True(this.HeaderContainer.TryGetValues(RefreshToken.DataSourceKey, out var headerValues));
                            Assert.NotNull(headerValues);
                            Assert.Collection(headerValues, headerValue =>
                            {
                                Assert.Null(serializedRefreshToken);
                                serializedRefreshToken = headerValue;
                            });
                            break;
                        case TokenDataSource.Parameter:
                            serializedRefreshToken = tokenModel.RefreshToken;
                            break;
                    }
                    Assert.NotNull(serializedRefreshToken);
                    var oldRefreshToken = RefreshToken.Deserialize(serializedRefreshToken, this.JwtOptions);
                    
                    bool expSuccess = requestedSource == enabledSource;
                    HttpStatusCode expectedStatus = expSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                    TokenModel newTokenModel = null;
                    string newSerializedRefreshToken = null;
                    switch (requestedSource)
                    {
                        case TokenDataSource.Header:
                            bool contains = this.Client.DefaultRequestHeaders.Contains(RefreshToken.DataSourceKey);
                            if (contains)
                                this.Client.DefaultRequestHeaders.Remove(JwtRefreshToken.DataSourceKey);
                            this.Client.DefaultRequestHeaders.Add(JwtRefreshToken.DataSourceKey, oldRefreshToken.Serialize());
                            clearRefreshCookie();
                            newTokenModel = await this.RefreshTokenAsync<TokenModel>(ExpectedStatus: expectedStatus);
                            this.Client.DefaultRequestHeaders.Remove(JwtRefreshToken.DataSourceKey);

                            var hasHeader = this.HeaderContainer.TryGetValues(RefreshToken.DataSourceKey, out var headerValues);
                            Assert.Equal(expSuccess, hasHeader);
                            if (expSuccess)
                            {
                                Assert.NotNull(headerValues);
                                Assert.Collection(headerValues, headerValue =>
                                {
                                    Assert.Null(newSerializedRefreshToken);
                                    newSerializedRefreshToken = headerValue;
                                });
                            }
                            break;
                        case TokenDataSource.Parameter:
                            var requestModel = new JwtRefreshRequest { RefreshToken = oldRefreshToken.Serialize() };
                            clearRefreshCookie();
                            newTokenModel = await this.RefreshTokenAsync<TokenModel>(RequestModel: requestModel, ExpectedStatus: expectedStatus);
                            newSerializedRefreshToken = newTokenModel.RefreshToken;
                            break;
                        case TokenDataSource.Cookie:
                            newTokenModel = await this.RefreshTokenAsync<TokenModel>(ExpectedStatus: expectedStatus);
                            var cookieCollection = this.CookieContainer.GetCookies(this.Client.BaseAddress);
                            var refreshCookie = cookieCollection[RefreshToken.DataSourceKey];
                            if (expSuccess)
                            {
                                Assert.NotNull(refreshCookie);
                                newSerializedRefreshToken = refreshCookie.Value;
                            }
                            else
                                Assert.Null(refreshCookie);
                            break;
                    }
                    if (!expSuccess)
                        continue;
                    RefreshToken newRefreshToken = RefreshToken.Deserialize(newSerializedRefreshToken, this.JwtOptions);
                    this.validateToken(newRefreshToken, this.Entities.AdminIdentity);
                    var newToken = AuthToken.Deserialize(newTokenModel.AuthToken, this.JwtOptions);
                    this.validateToken(newToken, this.Entities.AdminIdentity);

                    isTokenValid = await this.RedisCache.ValidateAccountTokenAsync(authToken);
                    Assert.False(isTokenValid);
                    var isNewTokenValid = await this.RedisCache.ValidateAccountTokenAsync(newToken);
                    Assert.True(isNewTokenValid);
                }
            }
        }
        [Fact]
        public async Task TestRefreshValidAuthToken() 
        {
            var tokenModel = await this.CreateTokenAsync<TokenModel>(
                Email: this.Entities.AdminEmail,
                Password: this.Entities.AdminPassword
            );
            var authToken = AuthToken.Deserialize(tokenModel.AuthToken, this.JwtOptions);
            this.validateToken(authToken, this.Entities.AdminIdentity);
            var isTokenValid = await this.RedisCache.ValidateAccountTokenAsync(authToken);
            Assert.True(isTokenValid);

            this.Client.Authorize(authToken);
            var newTokenModel = await this.RefreshTokenAsync<TokenModel>();
            var cookieCollection = this.CookieContainer.GetCookies(this.Client.BaseAddress);
            var refreshCookie = cookieCollection[RefreshToken.DataSourceKey];
            var refreshToken = RefreshToken.Deserialize(refreshCookie.Value, this.JwtOptions);
            this.validateToken(refreshToken, this.Entities.AdminIdentity);
            var newToken = AuthToken.Deserialize(newTokenModel.AuthToken, this.JwtOptions);
            this.validateToken(newToken, this.Entities.AdminIdentity);

            isTokenValid = await this.RedisCache.ValidateAccountTokenAsync(authToken);
            Assert.False(isTokenValid);
            var isNewTokenValid = await this.RedisCache.ValidateAccountTokenAsync(newToken);
            Assert.True(isNewTokenValid);
        }
        [Fact]
        public async Task TestRefreshExpiredAuthToken()
        {
            var authLifetime = new TimeSpan(0, 0, 10);
            await this.InitializeServicesAsync(this.Factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Set the auth token lifetime to a few seconds to speed up testing.
                        // Note: Setting this value too small may cause failures with internal API sanity checks.
                        // Setting this value to 0 will cause an exception within the Redis setex implementation.
                        services.PostConfigureAll<JwtAuthOptions>(opts =>
                        {
                            opts.ClockSkew = new TimeSpan(0L);
                            opts.AuthLifetime = authLifetime;
                            opts.ValidateLifetime = false;
                        });
                    });
                })
            );

            var tokenModel = await this.CreateTokenAsync<TokenModel>(
                Email: this.Entities.AdminEmail,
                Password: this.Entities.AdminPassword
            );
            var startAuthToken = AuthToken.Deserialize(tokenModel.AuthToken, this.JwtOptions);
            var startRefreshToken = this.GetCurrentRefreshToken();

            await Task.Delay(authLifetime);
            await this.ConfirmExpirationAsync(startAuthToken);
            await this.ConfirmRegistrationAsync(startRefreshToken);

            this.Client.Authorize(startAuthToken);
            var newTokenModel = await this.RefreshTokenAsync<TokenModel>();
            var finalAuthToken = AuthToken.Deserialize(newTokenModel.AuthToken, this.JwtOptions);
            var finalRefreshToken = this.GetCurrentRefreshToken();

            await Task.Delay(authLifetime);
            await this.ConfirmExpirationAsync(finalAuthToken);
            await this.ConfirmExpirationAsync(startRefreshToken);
            await this.ConfirmRegistrationAsync(finalRefreshToken);
            Assert.NotEqual(startRefreshToken.Id, finalRefreshToken.Id);
        }
        [Fact]
        public async Task TestRefreshExpiration()
        {
            var refreshLifetime = new TimeSpan(0, 0, 10);
            await this.InitializeServicesAsync(this.Factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Set the auth token lifetime to a few seconds to speed up testing.
                        // Note: Setting this value too small may cause failures with internal API sanity checks.
                        // Setting this value to 0 will cause an exception within the Redis setex implementation.
                        services.PostConfigureAll<JwtAuthOptions>(opts =>
                        {
                            opts.ClockSkew = new TimeSpan(0L);
                            opts.RefreshLifetime = refreshLifetime;
                            opts.RefreshCookie.MaxAge = refreshLifetime + TimeSpan.FromMinutes(10);
                            opts.ValidateLifetime = false;
                        });
                    });
                })
            );

            var startTokenModel = await this.CreateTokenAsync<TokenModel>(
                Email: this.Entities.AdminEmail,
                Password: this.Entities.AdminPassword
            );
            this.Client.Authorize(startTokenModel);
            var finalTokenModel = await this.RefreshTokenAsync<TokenModel>();
            var refreshToken = this.GetCurrentRefreshToken();

            await Task.Delay(refreshLifetime);
            this.Client.Authorize(finalTokenModel);
            var result = await this.ValidateRefreshTokenError<JwtRefreshResult>(
                ExpectedStatus: HttpStatusCode.BadRequest
            );
            Assert.True(result.IsRefreshTokenExpired);
            await this.ConfirmExpirationAsync(refreshToken);
        }

        [Fact]
        public async Task TestLogout()
        {
            var tokenModel = await this.CreateTokenAsync<TokenModel>(
                Email: this.Entities.AdminEmail,
                Password: this.Entities.AdminPassword
            );
            var authToken = AuthToken.Deserialize(tokenModel.AuthToken, this.JwtOptions);
            var refreshToken = this.GetCurrentRefreshToken();
            await this.ConfirmRegistrationAsync(authToken);
            await this.ConfirmRegistrationAsync(refreshToken);

            await this.ValidateGetUserErrorAsync(AccountID: this.Entities.MemberIdentity.Id,
                ExpectedErrorMessage: "Client request lacks authorized credentials.", ExpectedStatus: HttpStatusCode.Unauthorized
            );

            this.Client.Authorize(authToken);
            var userInfo = await this.GetUserAsync(this.Entities.MemberIdentity.Id);
            Assert.Equal(this.Entities.MemberIdentity.Id, userInfo.AccountID);
            await this.ConfirmRegistrationAsync(authToken);
            await this.ConfirmRegistrationAsync(refreshToken);

            await this.LogoutAsync();
            await this.ConfirmExpirationAsync(authToken);
            await this.ConfirmExpirationAsync(refreshToken);
            await this.ValidateGetUserErrorAsync(AccountID: this.Entities.MemberIdentity.Id,
                ExpectedErrorMessages: new string[]{
                    "The request is not authorized because the supplied access token is no longer valid.",
                    "Client request lacks authorized credentials."
                },
                ExpectedStatus: HttpStatusCode.Unauthorized
            );
        }
        #endregion
    }
}
