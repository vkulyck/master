using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiService _apiService;

        public ResendEmailConfirmationModel(ApiUserManager userManager, ApiService apiService)
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
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            await this._apiService.SendVerificationEmailAsync(this.Input.Email);
            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return this.Page();
        }
    }
}
