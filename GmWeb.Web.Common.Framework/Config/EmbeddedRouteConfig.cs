using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Common.Config
{
    public class EmbeddedRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.RouteExistingFiles = true;
            routes.IgnoreRoute("Content/{*staticfile}");
            routes.IgnoreRoute("Scripts/{*staticfile}");
        }
    }
}
