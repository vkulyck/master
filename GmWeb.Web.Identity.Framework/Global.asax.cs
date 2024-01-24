using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Identity
{
    public class MvcApplication : GmWeb.Web.Common.App_Start.BaseMvcApplication<FilterConfig, RouteConfig, BundleConfig>
    {
        protected override void Application_Start()
        {
            base.Application_Start();
        }

        protected override void InitializeDatabase()
        {
        }

        public override void Init()
        {
            base.Init();
            this.AcquireRequestState += showRouteValues;
        }

        protected void showRouteValues(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null)
                return;
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
        }
        void Session_Start(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Add("__MyAppSession", string.Empty);
        }
    }
}
