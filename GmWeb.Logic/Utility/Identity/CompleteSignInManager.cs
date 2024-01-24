using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Linq;

namespace GmWeb.Logic.Utility.Identity;
public class CompleteSignInManager : SignInManager<GmIdentity>
{
    public CompleteSignInManager(
        UserManager<GmIdentity> manager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<GmIdentity> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<GmIdentity>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<GmIdentity> confirmation)
    : base(manager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task SignOutAsync()
    {
        await base.SignOutAsync();
        var identities = this.Context.User.Identities.ToList();
        foreach (var identity in identities)
        {
            await this.Context.SignOutAsync(identity.AuthenticationType);
        }
    }
}