using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

namespace GmWeb.Web.Common.Auth.Services.BypassAuth
{
    /// <summary>
    /// Inherited from <see cref="AuthenticationHandler{TOptions}"/> for basic authentication.
    /// </summary>
    public class BypassAuthHandler : AuthenticationHandler<BypassAuthOptions>
    {
        private readonly UserManager<GmIdentity> _manager;

        public BypassAuthHandler(
            UserManager<GmIdentity> manager,
            IOptionsMonitor<BypassAuthOptions> optionsMonitor,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(optionsMonitor, logger, encoder, clock)
        {
            this._manager = manager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Options.Enabled)
            {
                this.Logger.LogInformation("AuthBypass was disabled within appsettings.");
                return AuthenticateResult.NoResult();
            }
            if (this.IgnoreAuthenticationIfAllowAnonymous())
            {
                this.Logger.LogInformation("AllowAnonymous found on the endpoint so request was not authenticated.");
                return AuthenticateResult.NoResult();
            }

            if (this.IgnoreAuthenticationIfAlreadyAuthenticated())
            {
                this.Logger.LogInformation("Already authenticated this request using a different scheme.");
                return AuthenticateResult.NoResult();
            }

            try
            {
                var user = await this._manager.FindByEmailAsync(this.Options.AuthenticationEmail).ConfigureAwait(false);
                var claims = await this._manager.GetClaimsAsync(user).ConfigureAwait(false);
                var identity = new ClaimsIdentity(claims, this.Scheme.Name);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, this.Options.ClaimsIssuer));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String, this.Options.ClaimsIssuer));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String, this.Options.ClaimsIssuer));
                this.Context.User = new ClaimsPrincipal(identity);
                if (this.Options.Events.OnSucceeded != null)
                    await this.Options.Events.OnSucceeded().ConfigureAwait(false);
                return AuthenticateResult.Success(new AuthenticationTicket(this.Context.User, BypassAuthExtensions.AuthenticationScheme));
            }
            catch (Exception exception)
            {
                if (this.Options.Events.OnFailed != null)
                    await this.Options.Events.OnFailed().ConfigureAwait(false);
                return AuthenticateResult.Fail(exception);
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (this.Options.Events.OnChallenge != null)
                await this.Options.Events.OnChallenge().ConfigureAwait(false);
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            if (this.Options.Events.OnForbidden != null)
                await this.Options.Events.OnForbidden().ConfigureAwait(false);
        }

        private bool IgnoreAuthenticationIfAllowAnonymous()
        {
            if (!this.Options.IgnoreAuthenticationIfAllowAnonymous)
                return false;
            var ep = this.Context.GetEndpoint();
            var metadata = ep?.Metadata?.GetMetadata<IAllowAnonymous>();
            bool allowAnon = metadata != null;
            return allowAnon;
        }

        private bool IgnoreAuthenticationIfAlreadyAuthenticated()
        {
            if (!this.Options.IgnoreAuthenticationIfAlreadyAuthenticated)
                return false;

            if (this.Context.User.Identity.IsAuthenticated)
                return true;

            var authHeader = this.Context.Request.Headers[HeaderNames.Authorization];
            if (authHeader.Count > 0 && authHeader[0].StartsWith(JwtBearerDefaults.AuthenticationScheme))
                return true;
            return false;
        }
    }
}
