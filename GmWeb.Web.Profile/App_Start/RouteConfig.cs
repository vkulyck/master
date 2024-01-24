using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using GmWeb.Web.Common.Utility;
using GmWeb.Common;
using GmWeb.Web.Common.Config;

namespace GmWeb.Web.Profile
{
    public class RouteConfig : GmWeb.Web.Common.App_Start.BaseRouteConfig
    {
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            //EmbeddedRouteConfig.RegisterRoutes(routes);
            base.RegisterRoutes(routes);
            routes.MapRoute(
                name: "StandardRequests",
                url: "{controller}/{action}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index"
                }
            );
        }
    }
}
