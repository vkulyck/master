using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using GmWeb.Common;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Identity
{
    public class RouteConfig : GmWeb.Web.Common.App_Start.BaseRouteConfig
    {
        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}
