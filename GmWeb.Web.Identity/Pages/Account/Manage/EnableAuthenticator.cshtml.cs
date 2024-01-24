using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;
using GmWeb.Logic.Utility.Extensions.UserManager;
using GmWeb.Logic.Utility.Identity;
using GmWeb.Logic.Services.QRCode;

namespace GmWeb.Web.Identity.Pages.Account.Manage
{
    public class EnableAuthenticatorModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly IQRCodeGeneratorService _qrService;
        private readonly ILogger<EnableAuthenticatorModel> _logger;

        public EnableAuthenticatorModel(
            ApiUserManager userManager,
            IQRCodeGeneratorService qrService,
            ILogger<EnableAuthenticatorModel> logger)
        {
            _userManager = userManager;
            _qrService = qrService;
            _logger = logger;
        }

        public TotpConfig AuthConfig { get; set; }
        public string SharedKey => this.AuthConfig.SharedKey;
        public string AuthenticatorUri => this.AuthConfig.AuthenticatorUri;
        public string AuthenticatorQRCodeImage => this._qrService.GenerateHtmlImageNode(this.AuthConfig);

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            this.AuthConfig = await this._userManager.GenerateAuthenticatorConfig(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                this.AuthConfig = await this._userManager.GenerateAuthenticatorConfig(user);
                return Page();
            }

            // Strip spaces and hypens
            var token = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var provider = _userManager.Options.Tokens.AuthenticatorTokenProvider;
            var isTokenVerified = await _userManager.VerifyTwoFactorTokenAsync(user, provider, token);

            if (!isTokenVerified)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                this.AuthConfig = await this._userManager.GenerateAuthenticatorConfig(user);
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await _userManager.GetUserIdAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

            StatusMessage = "Your authenticator app has been verified.";

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                RecoveryCodes = recoveryCodes.ToArray();
                return RedirectToPage("./ShowRecoveryCodes");
            }
            else
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }
        }
    }
}
