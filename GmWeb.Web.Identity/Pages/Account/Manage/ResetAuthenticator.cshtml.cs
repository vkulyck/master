using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.UserManager;
using ProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;

namespace GmWeb.Web.Identity.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiSignInManager _signInManager;
        private readonly ILogger<ResetAuthenticatorModel> _logger;

        public ResetAuthenticatorModel(
            ApiUserManager userManager,
            ApiSignInManager signInManager,
            ILogger<ResetAuthenticatorModel> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
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

            var result = await this._userManager.ResetAuthenticatorKeyAsync(user);
            if (result.Succeeded)
            {
                this._logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

                await this._signInManager.RefreshSignInAsync(user);
                this.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

                return this.RedirectToPage("./EnableAuthenticator");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
            return this.Page();
        }
    }
}