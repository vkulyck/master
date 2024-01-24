using System;
using System.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using BaseStartup = GmWeb.Web.Common.BaseStartup;
using GmWeb.Logic.Interfaces;
using GmWeb.Web.Profile.Utility;

[assembly: OwinStartupAttribute(typeof(GmWeb.Web.Profile.Startup))]
namespace GmWeb.Web.Profile
{
    public partial class Startup : BaseStartup
    {
        public override void Configure(IAppBuilder app)
        {
            base.Configure(app);
            app.CreatePerOwinContext<IDisposableMapper>(() => new ClientServicesMappingFactory());
        }
    }
}
