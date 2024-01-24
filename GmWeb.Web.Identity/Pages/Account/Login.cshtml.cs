using GmWeb.Web.Identity.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiSignInManager _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApiService _api;
        private readonly DebugSettings _settings;

        public LoginModel(ApiSignInManager signInManager,
            ILogger<LoginModel> logger,
            ApiUserManager userManager, ApiService apiService, IOptions<DebugSettings> settings)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._api = apiService;
            this._settings = settings?.Value;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; } = true;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.StatusMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.StatusMessage);
            }

            this.Input = new InputModel
            {
                Email = this._settings.DefaultLoginEmail,
                Password = this._settings.DefaultLoginPassword,
                RememberMe = false
            };

            returnUrl ??= this.Url.Content("~/");

            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(this.Input.Password) && this.Input.Email == this._settings.DefaultLoginEmail)
                this.Input.Password = this._settings.DefaultLoginPassword;
            returnUrl ??= this.Url.Content("~/");

            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (this.ModelState.IsValid)
            {
                var result = await this._signInManager.PasswordSignInAsync(this.Input.Email, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    this._logger.LogInformation("User logged in.");
                    return this.Redirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme,
                        new ClaimsPrincipal(new ClaimsIdentity(
                            new List<Claim> 
                            { 
                                new Claim (ClaimTypes.Email, this.Input.Email) 
                            },
                            IdentityConstants.TwoFactorUserIdScheme
                        )),
                        new AuthenticationProperties 
                        { 
                            IsPersistent = true 
                        }
                    );
                    this._logger.LogInformation("User performed password sign-in; redirecting to two-factor sign-in.");
                    return this.LocalRedirect($"~/Account/LoginWith2fa?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
                }
                if (result.IsLockedOut)
                {
                    this._logger.LogWarning("User account locked out.");
                    return this.RedirectToPage("./Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
