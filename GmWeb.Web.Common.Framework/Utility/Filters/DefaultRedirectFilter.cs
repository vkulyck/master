using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GmWeb.Web.Common.Controllers;
using System.Reflection;

namespace GmWeb.Web.Common.Utility
{
    
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DefaultRedirectFilter : ActionFilterAttribute
    {
        protected bool CheckSetDefaultArg(RouteValueDictionary actionParams, ParameterInfo arg)
        {
            if (!arg.HasDefaultValue)
                return false;
            var paramValue = actionParams[arg.Name];
            if (paramValue == null)
            {
                paramValue = arg.DefaultValue;
                return true;
            }
            else
            {
                Type paramType = Nullable.GetUnderlyingType(arg.ParameterType);
                if (paramType != null && paramType.IsEnum)
                {
                    if (paramValue is int)
                    {
                        var enumObject = Enum.ToObject(paramType, paramValue);
                        actionParams[arg.Name] = enumObject;
                        return true;
                    }
                }
            }
            return false;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionParams = new RouteValueDictionary(filterContext.ActionParameters);
            var method = (filterContext.ActionDescriptor as ReflectedActionDescriptor)?.MethodInfo;
            bool redirect = false;
            if (method != null)
            {
                var args = method.GetParameters();
                foreach (var arg in args)
                    if (CheckSetDefaultArg(actionParams, arg))
                        redirect = true;
            }
            if (redirect)
            {
                var  helper = new UrlHelper(filterContext.RequestContext);
                var respUrl = helper.Action(
                    filterContext.ActionDescriptor.ActionName,
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    actionParams
                );
                filterContext.Result = new RedirectResult(respUrl);
            }
            else base.OnActionExecuting(filterContext);
        }
    }
}