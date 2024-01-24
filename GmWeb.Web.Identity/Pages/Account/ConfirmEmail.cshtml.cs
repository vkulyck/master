using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;

        public ConfirmEmailModel(ApiUserManager userManager)
        {
            this._userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string code)
        {
            if (id == null || code == null)
            {
                return this.RedirectToPage("/Index");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await this._userManager.ConfirmEmailAsync(new Guid(id), code);
            if (result.Succeeded)
            {
                this.ModelState.AddModelError(string.Empty, "Thank you for confirming your email");
                this.StatusMessage = "Thank you for confirming your email";
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Error confirming your email.");
                this.StatusMessage = "Error confirming your email.";
                foreach (var error in result.Errors)
                    this.ModelState.AddModelError(string.Empty, error.Description);
            }
            return this.Page();
        }
    }
}
