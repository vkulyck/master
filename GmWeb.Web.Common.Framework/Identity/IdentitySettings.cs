using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Configuration;
using GmWeb.Common;

namespace GmWeb.Web.Common.Identity
{
    public class IdentitySettings
    {
        public static string CommonParentDomain => ConfigurationManager.AppSettings["CommonParentDomain"];
        public static string AccountControllerSubdomain => ConfigurationManager.AppSettings["AccountControllerSubdomain"];
        public static bool IsLocalhost => CommonParentDomain.ToLower().StartsWith("localhost");
        public static string AccountControllerDomain
        {
            get
            {
                if (IsLocalhost)
                    return CommonParentDomain;
                return $"{AccountControllerSubdomain}.{CommonParentDomain}";
            }
        }
        public static string ExternalScheme => ConfigurationManager.AppSettings["ExternalScheme"];
        public static bool EnableRegistration => bool.TryParse(ConfigurationManager.AppSettings["EnableRegistration"], out bool result) ? result : false;
        public static bool EnableLegacyMigration => bool.TryParse(ConfigurationManager.AppSettings["EnableLegacyMigration"], out bool result) ? result : false;
        public static string LoginUrl => $"{ExternalScheme}://{AccountControllerDomain}/Account/Login";
        public static string LogoffUrl => $"{ExternalScheme}://{AccountControllerDomain}/Account/Logoff";
        public static string RegisterUrl => $"{ExternalScheme}://{AccountControllerDomain}/Account/RequestRegistration";
        private HttpContext Context { get; set; }
        private UrlHelper Url { get; set; }
        private string BaseURI
        {
            get
            {
                return $"{ExternalScheme}://identity.{CommonParentDomain}";
            }
        }

        private string AccountUrl(string action, params KeyValuePair<string,object>[] routeValues) 
            => AccountUrl(action, AppIdentityConfig.ConfiguredIdentityType, routeValues.ToDictionary(x => x.Key, x => x.Value));
        private string AccountUrl(string action, Dictionary<string,object> routeValues) 
            => AccountUrl(action, AppIdentityConfig.ConfiguredIdentityType, routeValues);
        private string AccountUrl(string action, AppIdentityType accountType, Dictionary<string, object> routeValues)
        {
            routeValues["controller"] = "Account";
            routeValues["action"] = action;
            routeValues["accountType"] = accountType;
            var url = this.Url.RouteUrl(routeValues);
            url = $"{this.BaseURI}/Account/{action}?ReturnUrl={routeValues["ReturnUrl"]}";
            return url;
        }

        private string ApplicationUrl
        {
            get
            {
                var authority = this.Context.Request.Url.Authority;
                var path = this.Context.Request.ApplicationPath;
                string uri;
                if (string.IsNullOrWhiteSpace(path))
                    uri = $"{ExternalScheme}://{authority}";
                else
                    uri = $"{ExternalScheme}://{authority}{path}";
                return uri;
            }
        }

        public IdentitySettings(HttpContext context)
        {
            this.Context = context;
            this.Url = new UrlHelper(this.Context.Request.RequestContext);
        }

        public void RedirectAction(string action, string returnUrl)
        {
            // TODO: Fix for prod
            returnUrl = $"{this.ApplicationUrl}/Logon.aspx";
            var targetUrl = this.AccountUrl(action, new Dictionary<string,object> { { "ReturnUrl", returnUrl } });
            this.Context.Response.Redirect(targetUrl, false);
            this.Context.ApplicationInstance.CompleteRequest();
        }
        public void RedirectAction(string action) => RedirectAction(action, this.Context.Request.Url.ToString());
        public void RedirectLogin(string returnUrl = null) => RedirectAction("Login", returnUrl ?? this.ApplicationUrl);
        public void RedirectLogoff(string returnUrl = null) => RedirectAction("Logoff", returnUrl ?? this.ApplicationUrl);
        public void RedirectRegister(string returnUrl = null) => RedirectAction("RequestRegistration", returnUrl ?? this.ApplicationUrl);
        public void RedirectPasswordReset(string returnUrl = null) => RedirectAction("RequestPasswordReset", returnUrl ?? this.ApplicationUrl);

    }
}
