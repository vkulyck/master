using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using GmWeb.Common;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Common.App_Start
{
    public class BaseRouteConfig : IRouteConfig
    {
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            Config.EmbeddedRouteConfig.RegisterRoutes(routes);
            routes.MapRoute(
                name: "StandardRequests",
                url: "{controller}/{action}",
                defaults: new
                {
                    controller = "Account",
                    action = "Index",
                    accountType = AppIdentityType.User
                }
            );
            routes.MapRoute(
                name: "BaseRequests",
                url: "{accountType}/{controller}/{action}",
                defaults: new
                {
                    controller = "Account",
                    action = "Index",
                    accountType = AppIdentityType.User
                }
            );
        }
    }
}
