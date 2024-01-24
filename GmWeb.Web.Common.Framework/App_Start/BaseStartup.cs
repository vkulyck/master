using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Redis;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.Extensions.Configuration;
using CacheSettings = GmWeb.Logic.Utility.Redis.CacheSettings;
using ConfigAccessor = GmWeb.Logic.Utility.Config.ConfigAccessor;
using EmailClient = GmWeb.Logic.Utility.Email.EmailClient;
using EmailSettings = GmWeb.Logic.Utility.Email.EmailSettings;
using SmsSettings = GmWeb.Logic.Utility.Phone.SmsSettings;

namespace GmWeb.Web.Common
{
    public class BaseStartup
    {
        public void Configuration(IAppBuilder app)
        {
            this.Configure(app);
        }

        public virtual void ConfigureInstances<TUser, TContext, TUserStore, TManager>(IAppBuilder app)
            where TUser : GmIdentity, new()
            where TContext : IdentityDbContext<TUser>
            where TUserStore : IUserStore<TUser>
            where TManager : GmManager<TUser>
        {
            app.CreatePerOwinContext<ConfigAccessor>(() => new ConfigAccessor());
            app.CreatePerOwinContext(() => new AccountCache());
            app.CreatePerOwinContext(GmIdentityContext.Create);
            app.CreatePerOwinContext<TContext>((opts, context) => context.Get<GmIdentityContext>() as TContext);
            
            app.CreatePerOwinContext<GmManager>(GmManager.Create);
            app.CreatePerOwinContext<GmManager<TUser>>((opts, context) => context.Get<GmManager>() as GmManager<TUser>);
            app.CreatePerOwinContext<GmRoleManager>(GmRoleManager.Create);
            app.CreatePerOwinContext<RoleManager<IdentityRole>>((opts, context) => context.Get<GmRoleManager>());
            app.CreatePerOwinContext<TManager>((opts, context) => context.Get<GmManager>() as TManager);
            app.CreatePerOwinContext<UserManager<TUser>>((opts, context) => context.Get<GmManager>() as UserManager<TUser>);
            app.CreatePerOwinContext<ITwoFactorManager<TUser>>((opts, context) => context.Get<GmManager>()as ITwoFactorManager<TUser>);
            app.CreatePerOwinContext<GmSignInManager>((opts, context) => GmSignInManager.Create(opts, context));
            app.CreatePerOwinContext<GmSignInManager<TUser>>((opts, context) => context.Get<GmSignInManager>() as GmSignInManager<TUser>);
            app.CreatePerOwinContext<GmSignInManager<TUser,TManager>>((opts, context) => context.Get<GmSignInManager>() as GmSignInManager<TUser,TManager>);
            app.CreatePerOwinContext<SignInManager<TUser,string >>((opts, context) => context.Get<GmSignInManager>() as SignInManager<TUser, string>);
            app.CreatePerOwinContext<ITokenCache>((opts, context) =>
            {
                var config = context.Get<ConfigAccessor>();
                var settings = config.GetSection("Redis").Get<CacheSettings>();
                var cache = new RedisCache(settings);
                return cache;
            });
            app.CreatePerOwinContext<EmailService>((opts, context) =>
            {
                var config = context.Get<ConfigAccessor>();
                var settings = config.GetSection("Email").Get<EmailSettings>();
                var service = new EmailService(settings);
                return service;
            });
            app.CreatePerOwinContext<EmailClient>((opts, context) =>
            {
                var config = context.Get<ConfigAccessor>();
                var settings = config.GetSection("Email").Get<EmailSettings>();
                var service = new EmailClient(settings);
                return service;
            });
            app.CreatePerOwinContext<SmsService>((opts, context) =>
            {
                var config = context.Get<ConfigAccessor>();
                var settings = config.GetSection("Sms").Get<SmsSettings>();
                var service = new SmsService(settings);
                return service;
            });
            app.CreatePerOwinContext<IDisposableMapper>(() => new IdentityMappingFactory<TUser>());
            app.CreatePerOwinContext<IGmClaimManager<TUser, TManager>>(() => new GmClaimManager<TUser, TManager>());

            // Enable the application to use a cookie to store information for the signed in user
            var cookieAuthOptions = new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromHours(24),
                CookieSecure = CookieSecureOption.SameAsRequest,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = async (context) => await context
                        .OwinContext
                        .Get<IGmClaimManager<TUser, TManager>>()
                        .ValidateAuthenticationCookie(context)
                    ,
                    OnApplyRedirect = this.RedirectViaLogin
                }
            };
#if AUTOLOGIN
            // We need to disable cookie domain rewriting so that authentication requests stay within the app we're debugging.
            cookieAuthOptions.CookieDomain = null;
            cookieAuthOptions.LoginPath = new PathString("/Dev/AutoLogin");
            cookieAuthOptions.LogoutPath = new PathString("/Dev/AutoLogout");
#else
            cookieAuthOptions.CookieDomain = $".{IdentitySettings.CommonParentDomain}";
#endif
            app.UseCookieAuthentication(cookieAuthOptions);
        }

        protected bool hostMatchesCookieDomain(string requestHost, string cookieDomain)
        {
            if (string.IsNullOrWhiteSpace(cookieDomain))
                return false;
            cookieDomain = Regex.Replace(cookieDomain.Trim(), @"^[\s\.]*|(:\d+)?$", string.Empty);
            requestHost = Regex.Replace(requestHost.Trim(), @"^[\s\.]*|(:\d+)?$", string.Empty);
            do
            {
                if (requestHost == cookieDomain)
                    return true;
                requestHost = Regex.Replace(requestHost, @"^[^\.]+\.", string.Empty);
            } while (requestHost.Contains("."));
            return false;
        }

        protected bool controllerActionExists(string controllerName, string actionName)
        {
            var controllerType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(Controller).IsAssignableFrom(x))
                .Where(x => x.Name.Equals($"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault()
            ;
            if (controllerType == null)
                return false;
            var descriptor = new ReflectedControllerDescriptor(controllerType);
            var actions = descriptor.GetCanonicalActions();
            var match = actions.Where(x => x.ActionName == actionName).ToList();
            return match.Count > 0;
        }

        // Via: https://stackoverflow.com/questions/21275399/login-page-on-different-domain
        protected string replaceLoginDomain(CookieApplyRedirectContext context, string replacementDomain)
        {
            if (string.IsNullOrWhiteSpace(context.Options.CookieDomain))
                return null;
            Uri absoluteUri;
            if (Uri.TryCreate(context.RedirectUri, UriKind.Absolute, out absoluteUri))
            {
                if (!this.hostMatchesCookieDomain(absoluteUri.Host, context.Options.CookieDomain))
                    return null;
                var path = PathString.FromUriComponent(absoluteUri);
                if (path == context.OwinContext.Request.PathBase + context.Options.LoginPath)
                {
                    string scheme = IdentitySettings.ExternalScheme;
                    string domain = replacementDomain;
                    string ctlAction = context.Options.LoginPath.Value;
                    var returnUrl = new QueryString(context.Options.ReturnUrlParameter, context.Request.Uri.AbsoluteUri);
                    var redirectUrl = $"{scheme}://{domain}{ctlAction}{returnUrl}";
                    return redirectUrl;
                }
            }
            return null;
        }
        protected virtual void RedirectViaLogin(CookieApplyRedirectContext context)
        {
            // If we're debugging with AutoLogin enabled then we skip the login domain replacement
#if !AUTOLOGIN
            string uri = null;
            if(!IdentitySettings.IsLocalhost)
                uri = this.replaceLoginDomain(context, IdentitySettings.AccountControllerDomain);
            if (string.IsNullOrWhiteSpace(uri))
            {
                context.Options.CookieDomain = null;
            }
            else
            {
                context.RedirectUri = uri;
            }
#endif
            context.Response.Redirect(context.RedirectUri);
        }

        public virtual void Configure(IAppBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.Mappings.ContainsKey(".woff2"))
            {
                provider.Mappings.Add(".woff2", "application/font-woff2");
            }
            var options = new StaticFileOptions
            {
                FileSystem = new EmbeddedVirtualFileSystem(),
                ContentTypeProvider = provider
            };
            app.UseStaticFiles(options);

            EmbeddedBundleConfig.RegisterBundles(BundleTable.Bundles);

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            this.ConfigureInstances<GmIdentity, GmIdentityContext, GmStore, GmManager>(app);
        }
    }
}
