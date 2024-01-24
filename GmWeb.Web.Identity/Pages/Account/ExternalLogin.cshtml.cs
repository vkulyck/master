using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModelBase
    {
        private readonly ApiSignInManager _signInManager;
        private readonly ApiUserManager _userManager;
        private readonly ApiService _apiService;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            ApiSignInManager signInManager,
            ApiUserManager userManager,
            ApiService apiService,
            ILogger<ExternalLoginModel> logger)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._apiService = apiService;
            this._logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync() => this.RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            string redirectUrl = this.Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (remoteError != null)
            {
                this.ErrorMessage = $"Error from external provider: {remoteError}";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.ErrorMessage = "Error loading external login information.";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this._signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                this._logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return this.Redirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                this.ReturnUrl = returnUrl;
                this.ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    this.Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.ErrorMessage = "Error loading external login information during confirmation.";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (this.ModelState.IsValid)
            {
                var user = new GmIdentity { UserName = this.Input.Email, Email = this.Input.Email };

                var result = await this._userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this._userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        this._logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (this._userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return this.RedirectToPage("./RegisterConfirmation", new { Email = this.Input.Email });
                        }

                        await this._signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return this.Redirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            this.ProviderDisplayName = info.ProviderDisplayName;
            this.ReturnUrl = returnUrl;
            return this.Page();
        }
    }
}
