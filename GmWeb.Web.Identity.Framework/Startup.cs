using Microsoft.Owin;
using Owin;
using BaseStartup = GmWeb.Web.Common.BaseStartup;
using GmWeb.Web.Identity.Models;

[assembly: OwinStartupAttribute(typeof(GmWeb.Web.Identity.Startup))]
namespace GmWeb.Web.Identity
{
    public partial class Startup : BaseStartup
    {
        public override void Configure(IAppBuilder app)
        {
            base.Configure(app);
        }

        public override void ConfigureInstances<TUser, TContext, TUserStore, TManager>(IAppBuilder app)
        {
            base.ConfigureInstances<TUser, TContext, TUserStore, TManager>(app);
            app.CreatePerOwinContext<TwoFactorViewModelFactory<TUser>>(TwoFactorViewModelFactory<TUser>.Create);
        }
    }
}
