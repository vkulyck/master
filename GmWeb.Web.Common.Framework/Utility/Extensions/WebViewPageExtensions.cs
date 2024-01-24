using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GmWeb.Web.Common.Utility;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Interfaces;
using System.Linq.Expressions;
using System.Reflection;
using GmWeb.Web.Common.Models.Shared;
using GmWeb.Web.Common.RazorControls.ModelEditors;


namespace GmWeb.Web.Common.Utility
{
    public static class WebViewPageExtensions
    {
        public static string Controller(this WebViewPage page) => page.ViewContext.RouteData.Values["controller"].ToString();
        public static string Action(this WebViewPage page) => page.ViewContext.RouteData.Values["action"].ToString();
        public static IHtmlString LoginLink(this WebViewPage page)
        {
            var routeValues = new
            {
                ReturnUrl = page.Request.RawUrl
            };
            var attrs = new { };
            return page.Html.ActionLink("Sign In", "Login", page.Controller(), routeValues, attrs);
        }
        public static IHtmlString LogoutLink(this WebViewPage page)
        {
            var routeValues = new
            {
                ReturnUrl = GmWeb.Web.Common.Controllers.BaseController.DefaultUrl
            };
            var attrs = new { };
            return page.Html.ActionLink("Sign Out", "Logout", page.Controller(), routeValues, attrs);
        }

        public static string AlertLayout(this WebViewPage page)
            => page.FindViewPath("_AlertLayout");
        public static string SimplifiedLayout(this WebViewPage page)
            => page.FindViewPath("_CenterFormLayout");

        public static string FindViewPath(this WebViewPage page, string viewName, string masterName = null)
        {
            var controllerContext = page.ViewContext.Controller.ControllerContext;
            var viewResult = ViewEngines.Engines.FindView(controllerContext, viewName, masterName ?? string.Empty);
            if (viewResult == null)
                throw new Exception(string.Format("The specified view {0} could not be found.", viewName));

            var view = viewResult.View as RazorView;
            if (viewResult == null)
                throw new Exception(string.Format("The specified view {0} must be a Razor view.", viewName));

            return view.ViewPath;
        }
    }
}