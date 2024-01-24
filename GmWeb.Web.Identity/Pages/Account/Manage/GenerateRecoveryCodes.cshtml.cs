﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account.Manage
{
    public class GenerateRecoveryCodesModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ILogger<GenerateRecoveryCodesModel> _logger;

        public GenerateRecoveryCodesModel(
            ApiUserManager userManager,
            ILogger<GenerateRecoveryCodesModel> logger)
        {
            this._userManager = userManager;
            this._logger = logger;
        }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            bool isTwoFactorEnabled = await this._userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                string userId = await this._userManager.GetUserIdAsync(user);
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            bool isTwoFactorEnabled = await this._userManager.GetTwoFactorEnabledAsync(user);
            string userId = await this._userManager.GetUserIdAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await this._userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            this.RecoveryCodes = recoveryCodes.ToArray();

            this._logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
            this.StatusMessage = "You have generated new recovery codes.";
            return this.RedirectToPage("./ShowRecoveryCodes");
        }
    }
}