using Microsoft.Owin;
using Owin;
using BaseStartup = GmWeb.Web.Common.BaseStartup;

[assembly: OwinStartupAttribute(typeof(GmWeb.Web.Demographics.Startup))]
namespace GmWeb.Web.Demographics
{
    public partial class Startup : BaseStartup
    {
        public override void Configure(IAppBuilder app)
        {
            base.Configuration(app);
            ConfigureAuth(app);
        }
    }
}
