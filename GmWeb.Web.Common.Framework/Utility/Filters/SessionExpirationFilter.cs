using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GmWeb.Web.Common.Controllers;

namespace GmWeb.Web.Common.Utility
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionExpirationFilter : ActionFilterAttribute
    {
        private bool IsLoginRequired(ActionExecutingContext filterContext)
        {
            var actionAllowsAnon = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            var controllerAllowsAnon = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            return !actionAllowsAnon && !controllerAllowsAnon;
        }

        private string BuildLoginRedirectUrl(ActionExecutingContext filterContext)
        {
            UrlHelper helper = new UrlHelper(filterContext.RequestContext);
            var routeValues = new RouteValueDictionary(filterContext.ActionParameters);
            var reqUrl = helper.Action(
                filterContext.ActionDescriptor.ActionName,
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                routeValues
            );
            string loginController = "Account";
            if(routeValues.ContainsKey("controller"))
            {
                var c = routeValues["controller"]?.ToString();
                if (!string.IsNullOrWhiteSpace(c))
                    loginController = c;
            }
            string respUrl;
            if (string.IsNullOrEmpty(reqUrl))
            {
                respUrl = helper.Action("Login", loginController);
            }
            else
            {
                respUrl = helper.Action("Login", loginController, new { ReturnUrl = reqUrl });
            }
            return respUrl;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool loginRequired = IsLoginRequired(filterContext);
            if (filterContext.HttpContext.Session.IsNewSession && loginRequired)
            {
                var respUrl = BuildLoginRedirectUrl(filterContext);
                filterContext.Result = new RedirectResult(respUrl);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}