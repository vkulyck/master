using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Microsoft.Owin.Security.Cookies;
using Owin;
using GmWeb.Logic.Data.Context;
using GmWeb.Web.Common.Config;
using GmWeb.Web.Embedded.Config;
using GmWeb.Web.Common.Identity;
using GmWeb.Web.Common.Utility;
using System.Web.Routing;
using System.Web.Optimization;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Redis;
using System.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;

namespace GmWeb.Web.Common.Identity
{
    public interface IGmClaimManager<TUser,TManager> : IDisposable
    {
        Task ValidateAuthenticationCookie(CookieValidateIdentityContext context);
    }
    public class GmClaimManager<TUser,TManager> : IGmClaimManager<TUser,TManager>
        where TUser : GmIdentity, new()
        where TManager : UserManager<TUser>
    {
        public async Task ValidateAuthenticationCookie(CookieValidateIdentityContext context)
        {
            await this.CreateIdentityExpirationClaim(context);
            await this.CreateUserDataClaims(context);
            var authenticationManager = context.OwinContext.Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant
            (
                new ClaimsPrincipal(context.Identity),
                new AuthenticationProperties() { IsPersistent = true }
            );
            var onValidate = SecurityStampValidator.OnValidateIdentity<TManager, TUser>(
                validateInterval: TimeSpan.FromMinutes(30),
                regenerateIdentity: (manager, user) =>
                {
                    var identity = manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    return identity;
                }
            );
            await onValidate(context);
        }
        public Task CreateUserDataClaims(CookieValidateIdentityContext context)
        {
            var manager = context.OwinContext.Get<GmManager>();
            var user = manager.FindById(context.Identity.GetUserId());
            var identity = context.Identity;
            identity.AddClaim(new Claim($"SwitcherData:UserEmailAddress", identity.GetUserName()));
            if (user.IdentityType == GmWeb.Common.AppIdentityType.Client)
                identity.AddClaim(new Claim($"SwitcherData:ClientID", user.Account.AccountID.ToString()));
            if (user.IdentityType == GmWeb.Common.AppIdentityType.User)
                identity.AddClaim(new Claim($"SwitcherData:UserID", user.Account.AccountID.ToString()));
            return Task.CompletedTask;
        }

        public async Task CreateIdentityExpirationClaim(CookieValidateIdentityContext context)
        {
            // validate security stamp for 'sign out everywhere'
            // here I want to verify the security stamp in every 100 seconds.
            // but I choose not to regenerate the identity cookie, so I passed in NULL 
            var stampValidator = SecurityStampValidator.OnValidateIdentity<TManager, TUser>(TimeSpan.FromSeconds(100), null);
            await stampValidator(context);

            // here we get the cookie expiry time
            var expiration = context.Properties.ExpiresUtc?.LocalDateTime ?? new DateTime();

            // add the expiry time back to cookie as one of the claims
            // to ensure that the claim has latest value, we must keep only one claim
            // otherwise we will be having multiple claims with same type but different values
            var claimType = "Expiration";
            var identity = context.Identity;
            if (identity.HasClaim(c => c.Type == claimType))
            {
                var existingClaim = identity.FindFirst(claimType);
                identity.RemoveClaim(existingClaim);
            }
            var newClaim = new Claim(claimType, expiration.Ticks.ToString());
            context.Identity.AddClaim(newClaim);
        }

        public void Dispose()
        {
            
        }
    }
}