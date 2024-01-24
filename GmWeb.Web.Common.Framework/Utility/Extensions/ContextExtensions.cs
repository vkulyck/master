using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.Utility
{
    public static class ContextExtensions
    {
        public static string GetControllerName(this ControllerContext context)
        {
            return context.RouteData.Values["controller"].ToString();
        }

        public static void SetControllerName(this ControllerContext context, string name)
        {
            context.RouteData.Values["controller"] = name;
        }
    }
}