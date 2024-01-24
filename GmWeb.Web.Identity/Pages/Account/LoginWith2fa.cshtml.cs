using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Security.Claims;

namespace GmWeb.Web.Identity.Pages.Account
{
    [Authorize(AuthenticationSchemes = "Identity.TwoFactorUserId")]
    public class LoginWith2faModel : PageModelBase
    {
        private readonly ApiSignInManager _signInManager;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly ApiUserManager _userManager;
        public LoginWith2faModel(ApiSignInManager signInManager, ILogger<LoginWith2faModel> logger, ApiUserManager userManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public IActionResult OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
            this.RememberMe = rememberMe;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            string authenticatorCode = this.Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await this._signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, this.Input.RememberMachine);

            var claim = this.HttpContext.User.FindFirst(ClaimTypes.Email);
            var user = await this._userManager.FindByEmailAsync(claim.Value);

            if (result.Succeeded)
            {
                this._logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return this.Redirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                this._logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                this._logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                this.ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return this.Page();
            }
        }
    }
}
