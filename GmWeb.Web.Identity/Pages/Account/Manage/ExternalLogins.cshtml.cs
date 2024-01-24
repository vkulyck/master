using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiSignInManager _signInManager;

        public ExternalLoginsModel(
            ApiUserManager userManager,
            ApiSignInManager signInManager
        )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID 'user.Id'.");
            }

            this.CurrentLogins = await this._userManager.GetLoginsAsync(user);
            this.OtherLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => this.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            this.ShowRemoveButton = user.PasswordHash != null || this.CurrentLogins.Count > 1;
            return this.Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID 'user.Id'.");
            }

            var result = await this._userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                this.StatusMessage = "The external login was not removed.";
                return this.RedirectToPage();
            }

            await this._signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "The external login was removed.";
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Request a redirect to the external login provider to link a login for the current user
            string redirectUrl = this.Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, this._userManager.GetUserId(this.User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID 'user.Id'.");
            }

            var info = await this._signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
            if (info == null)
            {
                throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await this._userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                this.StatusMessage = "The external login was not added. External logins can only be associated with one account.";
                return this.RedirectToPage();
            }

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            this.StatusMessage = "The external login was added.";
            return this.RedirectToPage();
        }
    }
}
