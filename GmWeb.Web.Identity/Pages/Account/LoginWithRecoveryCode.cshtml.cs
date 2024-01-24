using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : PageModelBase
    {
        private readonly ApiSignInManager _signInManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
        private readonly ApiService _api;
        private readonly ApiUserManager _userManager;
        public LoginWithRecoveryCodeModel(ApiSignInManager signInManager, ApiService apiService, ApiUserManager userManager, ILogger<LoginWithRecoveryCodeModel> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._api = apiService;
            this._logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public static string Code { get; set; }

        public class InputModel
        {
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string code, string email, string returnUrl = null)
        {
            var user = await this._api.FindUserByEmailAsync(email);
            if (user == null)
            {
                this.ModelState.AddModelError(string.Empty, "Unable to load two-factor authentication user");
            }

            this.ReturnUrl = returnUrl;
            Code = code;
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string email, string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
                return this.Page();


            var user = await this._userManager.FindByEmailAsync(email);
            if (user == null)
            {
                this.ModelState.AddModelError(string.Empty, "Unable to load two-factor authentication user");
                return this.Page();
            }

            string recoveryCode = this.Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await this._signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            if (result.Succeeded)
            {
                await this._signInManager.RefreshSignInAsync(user);
                this._logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
                return this.Redirect(returnUrl ?? this.Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                this._logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                this._logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                this.ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return this.Page();
            }
        }
    }
}
