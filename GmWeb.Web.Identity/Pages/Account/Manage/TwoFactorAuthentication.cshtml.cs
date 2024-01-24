﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModelBase
    {
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

        private readonly ApiUserManager _userManager;
        private readonly ApiSignInManager _signInManager;
        private readonly ILogger<TwoFactorAuthenticationModel> _logger;

        public TwoFactorAuthenticationModel(
            ApiUserManager userManager,
            ApiSignInManager signInManager,
            ILogger<TwoFactorAuthenticationModel> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool IsTwoFactorEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            this.HasAuthenticator = await this._userManager.GetAuthenticatorKeyAsync(user) != null;
            this.IsTwoFactorEnabled = await this._userManager.GetTwoFactorEnabledAsync(user);
            this.IsMachineRemembered = await this._signInManager.IsTwoFactorClientRememberedAsync(user);
            this.RecoveryCodesLeft = await this._userManager.CountRecoveryCodesAsync(user);

            return this.Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            await this._signInManager.ForgetTwoFactorClientAsync();
            this.StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            return this.RedirectToPage();
        }
    }
}