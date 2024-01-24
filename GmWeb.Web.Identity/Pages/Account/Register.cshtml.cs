using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModelBase
    {
        private readonly ApiSignInManager _signInManager;
        private readonly ApiUserManager _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApiService _api;

        public RegisterModel(
            ApiUserManager userManager,
            ApiSignInManager signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApiService apiService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailSender = emailSender;
            this._api = apiService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }


            [Required]
            [Display(Name = "BirthDate ")]
            [DataType(DataType.Date,ErrorMessage = "Date is required")]
            public System.DateTime BirthDate
            {
                get;
                set; 
            }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");
            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!this.ModelState.IsValid)
                return this.Page();

            var user = new GmIdentity
            {
                UserName = this.Input.Email,
                Email = this.Input.Email,
                FirstName = this.Input.FirstName,
                LastName = this.Input.LastName,
                BirthDate = this.Input.BirthDate//System.DateOnly.FromDateTime(this.Input.BirthDate)  
            };
            var result = await this._userManager.CreateAsync(user, this.Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    this.ModelState.AddModelError(string.Empty, error.Description);
                return this.Page();
            }
            this._logger.LogInformation("User created a new account with password.");
            if (this._userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return this.RedirectToPage("RegisterConfirmation", new { email = this.Input.Email, returnUrl = returnUrl });
            }
            else
            {
                var resultLoggingIn = await this._signInManager.PasswordSignInAsync(this.Input.Email, this.Input.Password, true, lockoutOnFailure: false);
                if (resultLoggingIn.Succeeded)
                    return this.Redirect(returnUrl);
                else
                    return this.RedirectToPage("./Login");
            }

        }
    }
}
