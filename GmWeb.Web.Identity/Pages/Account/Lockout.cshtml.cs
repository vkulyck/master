using Microsoft.AspNetCore.Authorization;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : PageModelBase
    {
        public void OnGet()
        {

        }
    }
}
