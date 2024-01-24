using Microsoft.AspNetCore.Authorization;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModelBase
    {
        public void OnGet()
        {
        }
    }
}
