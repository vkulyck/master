using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace GmWeb.Web.Identity.Pages
{
    [AllowAnonymous]
    public class PrivacyModel : PageModelBase
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            this._logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
