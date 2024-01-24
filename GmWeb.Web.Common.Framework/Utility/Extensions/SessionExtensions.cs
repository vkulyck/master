using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using GmWeb.Web.Common.Controllers;

namespace GmWeb.Web.Common.Utility
{
    public static class SessionExtensions
    {
        public static bool TrySessionUpdate(this BaseController controller, string newSessionId)
        {
            return controller.TrySessionUpdate(newSessionId, null);
        }
        public static bool TrySessionUpdate(this BaseController controller, HttpCookie cookie)
        {
            var sessionID = cookie.Value;
            return controller.TrySessionUpdate(sessionID, null);
        }
        public static bool TrySessionUpdate(this BaseController controller, HttpCookie cookie, string redirectUrl)
        {
            var sessionID = cookie.Value;
            return controller.TrySessionUpdate(sessionID, redirectUrl);
        }
        public static bool TrySessionUpdate(this BaseController controller, string newSessionId, string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
                redirectUrl = $"~/{BaseController.DefaultController}/";
            if (controller.HttpContext.SetSessionId(newSessionId))
            {
                controller.Response.Redirect(redirectUrl);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string CreateSessionId(this HttpContext httpContext)
        {
            var manager = new SessionIDManager();

            string newSessionId = manager.CreateSessionID(httpContext);

            return newSessionId;
        }

        public static bool SetSessionId(this HttpContextBase httpContextBase, string newSessionId)
        {
            HttpContext httpContext = httpContextBase.ApplicationInstance.Context;
            return SetSessionId(httpContext, newSessionId);
        }

        public static bool SetSessionId(this HttpContext httpContext, string newSessionId)
        {
            if (string.IsNullOrEmpty(newSessionId) || httpContext.Session.SessionID == newSessionId)
                return false;
            var manager = new SessionIDManager();
            manager.SaveSessionID(httpContext, newSessionId, out bool _1, out bool _2);
            httpContext.Session["__PLACEHOLDER__"] = "__PLACEHOLDER__";
            return true;
        }
    }
}