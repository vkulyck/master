using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GmWeb.Web.Common.Controllers;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Demographics
{
    public class RouteConfig : IRouteConfig
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = BaseController.DefaultController, action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
