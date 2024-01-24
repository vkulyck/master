using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Helpers;

namespace GmWeb.Web.Common.Utility
{
    /// <summary>
    /// This extends the anti-forgery functionality built into ASP.NET MVC with the following additional features:
    /// 1. Apply anti-forgery validation per controller instead of per method
    /// 2. Allow anti-forgery validation on AJAX posts
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidatePostForgeryAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            //  Only validate POSTs
            if (request.HttpMethod == WebRequestMethods.Http.Post)
            {
                //  Ajax POSTs and normal form posts have to be treated differently when it comes
                //  to validating the AntiForgeryToken
                if (request.IsAjaxRequest())
                {
                    var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];

                    var cookieValue = antiForgeryCookie != null
                        ? antiForgeryCookie.Value
                        : null;

                    AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
                }
                else
                {
                    new ValidateAntiForgeryTokenAttribute()
                        .OnAuthorization(filterContext);
                }
            }
        }
    }
}
