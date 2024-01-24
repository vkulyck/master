using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiService _apiService;
        public ResetPasswordModel(ApiUserManager userManager, ApiService apiService)
        {
            this._userManager = userManager;
            this._apiService = apiService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return this.BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                this.Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var result = await this._apiService.ResetPasswordAsync(this.Input.Email, this.Input.Code, this.Input.Password);
            if (result.Succeeded)
            {
                return this.RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
            return this.Page();
        }
    }
}
