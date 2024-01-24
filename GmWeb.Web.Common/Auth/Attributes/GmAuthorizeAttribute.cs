using GmWeb.Common.Identity;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using Microsoft.AspNetCore.Authorization;

namespace GmWeb.Web.Common.Auth.Attributes;

public class GmAuthorizeAttribute : AuthorizeAttribute
{
    public GmAuthorizeAttribute()
    {
        this.AuthenticationSchemes = string.Join(",", IdentityConstants.ApplicationScheme, BypassAuthExtensions.AuthenticationScheme);
    }
}
