using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Models.Identity;
using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Logic.Utility.Extensions.UserManager;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using GmWeb.Logic.Utility.Email;
using GmRole = GmWeb.Logic.Data.Models.Identity.GmRole;

namespace GmWeb.Web.Api.Controllers.Identity;

public partial class ManageController : GmApiController
{
    private readonly IEmailService _emailService;
    private readonly GmWebOptions _options;

    public ManageController(
        UserManager<GmIdentity> userManager,
        IEmailService emailService,
        IOptions<GmWebOptions> options
    ) : base(userManager)
    {
        this._emailService = emailService;
        this._options = options.Value;
    }

    /// <summary>
    /// Get user information
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(UserInfoViewModel), 200)]
    public async Task<IActionResult> UserInfo()
    {
        var user = await this.Manager.FindByIdAsync(this.GetUserId()).ConfigureAwait(false);

        var userModel = new UserInfoViewModel
        {
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            LockoutEnabled = user.LockoutEnabled,
            Roles = await this.Manager.GetRolesAsync(user).ConfigureAwait(false)
        };

        return this.Ok(userModel);
    }

    /// <summary>
    /// Get TFA stats
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(TwoFactorAuthenticationViewModel), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await this.Manager.FindByIdAsync(this.GetUserId()).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find user!");

        var model = new TwoFactorAuthenticationViewModel
        {
            HasAuthenticator = await this.Manager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false) != null,
            Is2faEnabled = user.TwoFactorEnabled,
            RecoveryCodesLeft = await this.Manager.CountRecoveryCodesAsync(user).ConfigureAwait(false)
        };

        return this.Ok(model);
    }

    /// <summary>
    /// Reset TFA (This will reset and disable TFA)
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> ResetAuthenticator([FromQuery]string AccountID)
    {
        var user = await this.Manager.FindByIdAsync(AccountID).ConfigureAwait(false);
        if (user == null)
            return this.DataNotFound().AsIdentityResult();

        var resetResult = await this.Manager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        if (!resetResult.Succeeded)
            return this.IdentityErrors(resetResult);

        var key = await this.Manager.GetAuthenticatorKeyAsync(user);
        await this.Manager.SetTfaKeyClaimAsync(user, Logic.Enums.TwoFactorProviderType.Authenticator, key);

        var disableResult = await this.Manager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
        if (!disableResult.Succeeded)
            return this.IdentityErrors(disableResult);
        return Ok(IdentityResult.Success);
    }

    /// <summary>
    /// Generate {Number} new recovery codes for the specified user.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(RecoveryCodesDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> GenerateNewTwoFactorRecoveryCodesAsync([FromBody] RecoveryCodesDTO model)
    {
        var user = await this.Manager.FindByIdAsync(model.AccountID).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("User not found.");
        if (!user.TwoFactorEnabled)
            return this.BadRequest($"Recovery code generation failed; user {model.AccountID} has not enabled two-factor authentication.");
        var codes = await this.Manager.GenerateNewTwoFactorRecoveryCodesAsync(user, model.Number);
        if (codes == null || codes.Count() != model.Number)
            return this.BadRequest("Error generating recovery codes.");
        model.RecoveryCodes = codes.ToList();
        return Ok(model);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="model">ChangePasswordViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<ObjectResult> ChangePassword([FromBody] ChangePasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.Manager.FindByIdAsync(this.GetUserId()).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find user!").AsIdentityResult();

        var passwordValidator = new PasswordValidator<GmIdentity>();
        var result = await passwordValidator.ValidateAsync(this.Manager, null, model.NewPassword).ConfigureAwait(false);

        if (result.Succeeded)
        {
            var changePasswordResult = await this.Manager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword).ConfigureAwait(false);
            if (!changePasswordResult.Succeeded)
                return this.IdentityErrors(changePasswordResult);

            return this.Success(changePasswordResult);
        }
        else
        {
            return this.IdentityErrors(result);
        }
    }


    /// <summary>
    /// Set a password if the user doesn't have one already
    /// </summary>
    /// <param name="model">SetPasswordViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<ObjectResult> SetPassword([FromBody] SetPasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.Manager.FindByIdAsync(this.GetUserId()).ConfigureAwait(false);
        if (user == null)
            return this.DataNotFound().AsIdentityResult();

        var addPasswordResult = await this.Manager.AddPasswordAsync(user, model.NewPassword).ConfigureAwait(false);

        if (addPasswordResult.Succeeded)
            return this.Success(addPasswordResult);

        return this.IdentityErrors(addPasswordResult);
    }

    /// <summary>
    /// Reset account password with reset token
    /// </summary>
    /// <param name="model">ResetPasswordViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    [AllowAnonymous]
    public async Task<ObjectResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.Manager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (user == null)
        {
            return this.DataNotFound().AsIdentityResult();
        }
        var result = await this.Manager.ChangePasswordAsync(user, model.Code, model.Password).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Success(result);
        }
        return this.IdentityErrors(result);
    }

    /// <summary>
    /// Sends an email with a link to the email-change confirmation flow.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> SendConfirmEmailChange([FromBody] ChangeEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return this.ModelStateErrors();

        var user = await this.GetUserAsync();
        if (user == null)
            return this.Unauthorized();

        var rawToken = await this.Manager.GenerateChangeEmailTokenAsync(user, model.Email);
        var encToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

        var callback = this._options.ConfirmEmailChange.WithQuery(new
        {
            user.Id,
            Code = encToken
        });
        await this._emailService.SendEmailConfirmationAsync(model.Email, callback.Url);

        return this.Success();
    }

    /// <summary>
    /// Sends an email with a link containing reset token
    /// </summary>
    /// <param name="model">ForgotPasswordViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [AllowAnonymous]
    public async Task<ObjectResult> SendPasswordResetEmail(ForgotPasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var user = await this.Manager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (user == null || !(await this.Manager.IsEmailConfirmedAsync(user).ConfigureAwait(false)))
            return this.DataNotFound();

        string token = await this.Manager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        string encToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var callback = this._options.ResetPassword.WithQuery(new
        {
            Code = encToken
        });
        await this._emailService.SendPasswordResetAsync(model.Email, callback.Url).ConfigureAwait(false);
        return this.Success();
    }

    /// <summary>
    /// Changes a user email address
    /// </summary>
    /// <param name="model">ChangeEmailViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<ObjectResult> ChangeEmail([FromBody] ChangeEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.GetUserAsync();
        if (user == null)
            return this.DataNotFound().AsIdentityResult();

        var result = await this.Manager.ChangeEmailAsync(user, model.Email, model.Code).ConfigureAwait(false);
        if (result.Succeeded)
            return this.Success(result);

        return this.IdentityErrors(result);
    }

    /// <summary>
    /// Confirms a user email address
    /// </summary>
    /// <param name="model">ConfirmEmailViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    [AllowAnonymous]
    public async Task<ObjectResult> ConfirmEmail([FromBody] ConfirmEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.Manager.FindByIdAsync(model.AccountID).ConfigureAwait(false);
        if (user == null)
            return this.DataNotFound().AsIdentityResult();

        var result = await this.Manager.ConfirmEmailAsync(user, model.Code).ConfigureAwait(false);
        if (result.Succeeded)
            return Ok(IdentityResult.Success);

        return this.IdentityErrors(result);
    }
}
