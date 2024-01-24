using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Models.Identity;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ApiService _apiService;

        public ForgotPasswordModel(ApiUserManager userManager, ApiService apiService)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
                return this.Page();
             await this._apiService.SendPasswordResetEmailAsync(this.Input.Email);
            return this.RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
