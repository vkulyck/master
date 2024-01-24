using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Identity;
public class GmSignInManager : CompleteSignInManager
{
    protected GmUserManager Manager { get; private set; }
    public GmSignInManager(
        GmUserManager manager, 
        IHttpContextAccessor contextAccessor, 
        IUserClaimsPrincipalFactory<GmIdentity> claimsFactory, 
        IOptions<IdentityOptions> optionsAccessor, 
        ILogger<SignInManager<GmIdentity>> logger, 
        IAuthenticationSchemeProvider schemes, 
        IUserConfirmation<GmIdentity> confirmation)
    : base(manager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        this.Manager = manager;
    }
}