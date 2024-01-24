using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Hosting;
using System.Web.Helpers;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Embedded.Config;
using System.Reflection;
using GmWeb.Web.Common.Config;

namespace GmWeb.Web.Common.App_Start
{
    public abstract class BaseMvcApplication : System.Web.HttpApplication
    {
        public static readonly DirectoryInfo SourceAssemblyDirectory = Directory.GetParent(new Uri(SourceAssembly.GetName().CodeBase).LocalPath);
        public static Assembly SourceAssembly => Assembly.GetAssembly(typeof(BaseMvcApplication));
        public static string SourceAssemblyName => SourceAssembly.GetName().Name;
        protected abstract void Application_Start();
    }
    public abstract class BaseMvcApplication<TFilterConfig,TRouteConfig,TBundleConfig> : BaseMvcApplication where TFilterConfig : IFilterConfig, new() where TRouteConfig : IRouteConfig, new() where TBundleConfig : IBundleConfig,  new()
    {
        public TFilterConfig FilterConfig { get; } = new TFilterConfig();
        public TRouteConfig RouteConfig { get; } = new TRouteConfig();
        public TBundleConfig BundleConfig { get; } = new TBundleConfig();
        protected override void Application_Start()
        {
            this.InitializeDatabase();
            AreaRegistration.RegisterAllAreas();
            this.FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            this.BundleConfig.RegisterBundles(BundleTable.Bundles);
            this.RouteConfig.RegisterRoutes(RouteTable.Routes);
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedVirtualPathProvider());
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CoreViewEngine());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        protected virtual void InitializeDatabase() { }
    }
}
