﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiSignInManager _signInManager;

        public ConfirmEmailChangeModel(ApiUserManager userManager, ApiSignInManager signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return this.RedirectToPage("/Index");
            }

            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await this._userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                this.StatusMessage = "Error changing email.";
                return this.Page();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await this._userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                this.StatusMessage = "Error changing user name.";
                return this.Page();
            }
            await this._signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Thank you for confirming your email change.";
            return this.Page();
        }
    }
}
