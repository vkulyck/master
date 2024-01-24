using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.Utility
{
    public class ExplicitViewAttribute : ActionFilterAttribute
    {
        private readonly string _viewName;
        public ExplicitViewAttribute(string viewName)
        {
            _viewName = viewName;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResultBase;
            if (result != null)
            {
                result.ViewName = _viewName;
            }
        }
    }
}