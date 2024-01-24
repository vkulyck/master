using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GmWeb.Web.Identity.Pages
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class PageModelBase : PageModel
    {
    }
}
