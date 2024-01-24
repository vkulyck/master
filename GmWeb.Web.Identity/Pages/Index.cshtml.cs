using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace GmWeb.Web.Identity.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            this._logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
