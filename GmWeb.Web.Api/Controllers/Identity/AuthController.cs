using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Web.Api.Models.Identity;
using GmWeb.Logic.Utility.Email;
using GmWeb.Web.Common.Auth;
using GmWeb.Web.Common.Auth.Models;
using GmWeb.Logic.Utility.Extensions.UserManager;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JwtRefreshRequest = GmWeb.Web.Common.Auth.Requests.JwtRefreshRequest;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using GmWeb.Logic.Utility.Redis;
using GmWeb.Logic.Utility.Identity.DTO;
using GmWeb.Web.Common.Auth.Services.JwtAuth;
using GmWeb.Web.Common.Auth.Tokens;

namespace GmWeb.Web.Api.Controllers.Identity;

public partial class AuthController : GmApiController
{
    private readonly SignInManager<GmIdentity> _signin;
    private readonly JwtAuthTokenService _auth;
    private readonly RedisCache _cache;
    private readonly IEmailService _emailService;
    private readonly GmWebOptions _options;
    private readonly CookieAuthenticationOptions _tfarmCookieOptions;
    private readonly ILogger<AuthController> _logger;
    public AuthController(
        UserManager<GmIdentity> userManager,
        SignInManager<GmIdentity> signin,
        RedisCache cache,
        IEmailService emailService,
        JwtAuthTokenService auth,
        IOptions<GmWebOptions> options,
        IOptionsSnapshot<CookieAuthenticationOptions> tfarmCookieOptions,
        ILoggerFactory loggerFactory
    ) : base(userManager)
    {
        this._signin = signin;
        this._auth = auth;
        this._cache = cache;
        this._emailService = emailService;
        this._options = options.Value;
        this._logger = loggerFactory.CreateLogger<AuthController>();
        this._tfarmCookieOptions = tfarmCookieOptions.Get(IdentityConstants.TwoFactorRememberMeScheme);
    }

    /// <summary>
    /// Verify a TFA token
    /// </summary>
    /// <param name="model">VerifyTfaViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> VerifyTwoFactorToken(VerifyTfaViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.Manager.FindByIdAsync(model.AccountID);
        if (user == null)
            return this.Unauthorized();

        var verified = await this.Manager.VerifyTwoFactorTokenAsync(user, model.Provider.ToString(), model.Token);
        if (verified)
            return this.Ok(IdentityResult.Success);
        else
            return this.IdentityErrors("Two-factor verification failed.");
    }


    /// <summary>
    /// Register an account
    /// </summary>
    /// <param name="model">RegisterViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    [AllowAnonymous]
    public async Task<ObjectResult> Register([FromBody] RegisterDTO model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = Mapper.Map<RegisterDTO, GmIdentity>(model);
        IdentityResult result;
        if(model.Password == null)
            result = await this.Manager.CreateAsync(user);
        else
            result = await this.Manager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return this.IdentityErrors(result);
        await this.SendVerificationEmail(new VerifyEmailViewModel { Email = user.Email });
        return this.Ok(IdentityResult.Success);
    }

    /// <summary>
    /// Log in with TFA 
    /// </summary>
    /// <param name="model">LoginWith2faViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(TokenResult), 200)]
    [ProducesResponseType(typeof(IdentitySignInResult), 400)]
    [AllowAnonymous]
    public async Task<ObjectResult> TwoFactorTokenSignIn(TfaLoginViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var identity = await this.Manager.FindByEmailAsync(model.Email);
        if (identity == null)
            return this.BadRequest(IdentitySignInResult.Failed);

        var verified = await this.Manager.VerifyTwoFactorTokenAsync(identity, model.Provider.ToString(), model.Code);
        if (verified)
        {
            var rmToken = new TwoFactorRememberMeToken
            {
                AccountID = identity.AccountID,
                Lifetime = _tfarmCookieOptions.ExpireTimeSpan
            };
            await this._cache.RegisterAccountTokenAsync(rmToken);
            var tokenResult = await this._auth.CreateAuthTokenAsync(identity);
            var tokenVM = TokenResult.Create(tokenResult);
            return this.Ok(tokenVM);
        }
        return this.BadRequest(IdentitySignInResult.Failed);
    }

    /// <summary>
    /// Log in with TFA 
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TokenResult), 200)]
    [ProducesResponseType(typeof(IdentitySignInResult), 400)]
    [AllowAnonymous]
    public async Task<ObjectResult> TwoFactorRecoveryCodeSignIn(string RecoveryCode)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var signinResult = await this._signin.TwoFactorRecoveryCodeSignInAsync(RecoveryCode);
        if (!signinResult.Succeeded)
            return this.BadRequest(signinResult);

        var identity = await this.Manager.GetUserAsync(this.HttpContext.User);
        var tokenResult = await this._auth.CreateAuthTokenAsync(identity);
        var tokenVM = TokenResult.Create(tokenResult);
        if (signinResult.Succeeded || signinResult.RequiresTwoFactor)
            return this.Ok(tokenVM);
        return this.BadRequest(tokenVM);
    }

    /// <summary>
    /// Create a JWT access token using the provided user credentials.
    /// </summary>
    /// <param name="model">LoginViewModel</param>
    /// <returns>The token and its metadata.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TokenResult), 200)]
    [ProducesResponseType(typeof(JwtSignInResult), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    [ActionName("token")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
    {
        try
        {
            if (!this.ModelState.IsValid)
                return this.ModelStateErrors();
            var result = await this._auth.CreateAuthTokenAsync(model);
            if (result.Succeeded || result.RequiresTwoFactor)
            {
                var tokenVM = TokenResult.Create(result);
                return this.Ok(tokenVM);
            }
            return this.BadRequest(result);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            return this.BadRequest(JwtSignInResult.Failed);
        }
    }

    /// <summary>
    /// Request a renewed token.
    /// </summary>
    /// <returns>A new token with metadata.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TokenResult), 200)]
    [ProducesResponseType(typeof(JwtRefreshResult), 400)]
    [ActionName("retoken")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] JwtRefreshRequest request)
    {
        try
        {
            var jwtResponse = await this._auth.RefreshAuthTokenAsync(request?.RefreshToken);
            if (jwtResponse.Succeeded)
            {
                var tokenVM = TokenResult.Create(jwtResponse);
                return this.Ok(tokenVM);
            }
            return this.BadRequest(jwtResponse);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            return this.BadRequest(JwtRefreshResult.RefreshFailed);
        }
    }

    /// <summary>
    /// Log out of account
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(JwtLogoutResult), 200)]
    [ProducesResponseType(typeof(JwtLogoutResult), 400)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var identity = await this.GetUserAsync();
            var result = await this._auth.RevokeTokensAsync(identity);
            if (result.Succeeded)
                return this.Ok(result);
            return this.BadRequest(result);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            return this.BadRequest(JwtLogoutResult.Failed);
        }
    }

    /// <summary>
    /// Sends an email with a link to the email confirmation flow.
    /// </summary>
    /// <param name="model">ForgotPasswordViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [AllowAnonymous]
    public async Task<IActionResult> SendVerificationEmail(VerifyEmailViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var user = await this.Manager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (user == null)
            return this.Success();

        var rawToken = await this.Manager.GenerateEmailConfirmationTokenAsync(user);
        var encToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

        var callback = this._options.ConfirmEmail.WithQuery(new
        {
            Id = user.Id,
            Code = encToken
        });
        await this._emailService.SendEmailConfirmationAsync(model.Email, callback.Url);

        return this.Success();
    }
}
