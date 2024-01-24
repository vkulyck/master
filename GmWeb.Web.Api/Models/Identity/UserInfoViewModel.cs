using System.Collections.Generic;

namespace GmWeb.Web.Api.Models.Identity
{
    public class UserInfoViewModel
    {
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public IList<string> Roles { get; set; }
    }
}
