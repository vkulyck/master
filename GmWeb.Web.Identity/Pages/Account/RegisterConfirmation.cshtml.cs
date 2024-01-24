using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(ApiUserManager userManager, IEmailSender sender)
        {
            this._userManager = userManager;
            this._sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public IActionResult OnGet(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return this.RedirectToPage("/Index");
            }


            this.Email = email;
            this.DisplayConfirmAccountLink = false;
            return this.Page();
        }
    }
}
