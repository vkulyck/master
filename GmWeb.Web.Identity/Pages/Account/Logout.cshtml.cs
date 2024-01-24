using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModelBase
    {
        private readonly ApiSignInManager _signInManager;
        private readonly ApiService _apiService;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ApiSignInManager signInManager, ApiService apiService, ILogger<LogoutModel> logger)
        {
            this._signInManager = signInManager;
            this._apiService = apiService;
            this._logger = logger;
        }

        public async Task<IActionResult> OnGet(bool confirm = true)
        {
            if (confirm)
                return this.Page();
            else
            {
                await this._signInManager.SignOutAsync();
                this._logger.LogInformation("User logged out.");
                return this.Redirect("~/");
            }
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await this._signInManager.SignOutAsync();
            this._logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToPage();
            }
        }
    }
}
