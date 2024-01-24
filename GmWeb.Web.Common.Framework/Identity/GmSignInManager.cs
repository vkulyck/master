using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using GmWeb.Web.Common.Identity;
using System.Web.Security;

namespace GmWeb.Web.Common.Identity
{
    public class GmSignInManager : GmSignInManager<GmIdentity>
    {
        public GmSignInManager(UserManager<GmIdentity> userManager, IOwinContext context)
            : base(userManager, context)
        {
        }
        public static GmSignInManager Create(IdentityFactoryOptions<GmSignInManager> options, IOwinContext context)
        {
            var manager = context.Get<UserManager<GmIdentity>>();
            return new GmSignInManager(manager, context);
        }
    }
    public class GmSignInManager<TUser> : GmSignInManager<TUser,GmManager<TUser>>
        where TUser : GmIdentity, new()
    {
        public GmSignInManager(UserManager<TUser> userManager, IOwinContext context)
            : base(userManager, context)
        {
        }
        public static GmSignInManager<TUser> Create(IdentityFactoryOptions<GmSignInManager<TUser>> options, IOwinContext context)
        {
            var manager = context.Get<UserManager<TUser>>();
            return new GmSignInManager<TUser>(manager, context);
        }
    }
    public class GmSignInManager<TUser,TManager> : SignInManager<TUser, string>
        where TUser : GmIdentity
        where TManager : UserManager<TUser>
    {
        public IOwinContext Context { get; private set; }
        public GmSignInManager(UserManager<TUser> userManager, IOwinContext context)
            : base(userManager, context.Authentication)
        {
            this.Context = context;
        }

        public static GmSignInManager<TUser,TManager> Create(IdentityFactoryOptions<SignInManager<TUser, string>> options, IOwinContext context)
        {
            var manager = context.Get<UserManager<TUser>>();
            return new GmSignInManager<TUser, TManager>(manager, context);
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var result = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            if(result == SignInStatus.Success)
                FormsAuthentication.SetAuthCookie(userName, isPersistent);
            return result;
        }

        public override async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var result = await base.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
            if (result == SignInStatus.Success)
            {
                var userName = this.AuthenticationManager
                    .AuthenticationResponseGrant
                    .Identity
                    .Name
                ;
                FormsAuthentication.SetAuthCookie(userName, isPersistent);
            }
            return result;
        }

        public async Task RefreshSignInAsync(TUser model, bool isPersistent = false, string returnUrl = null)
        {
            FormsAuthentication.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorCookie, DefaultAuthenticationTypes.ExternalCookie);
            var identity = await model.GenerateUserIdentityAsync(this.UserManager);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
            FormsAuthentication.SetAuthCookie(model.UserName, isPersistent);
        }
        public async Task RefreshSignInAsync(string email, bool isPersistent = false, string returnUrl = null)
        {
            var user = await this.UserManager.FindByEmailAsync(email);
            await this.RefreshSignInAsync(user, isPersistent, returnUrl);
        }

        public async Task SignOutAsync()
        {
            FormsAuthentication.SignOut();
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            await Task.CompletedTask;
        }
    }
}