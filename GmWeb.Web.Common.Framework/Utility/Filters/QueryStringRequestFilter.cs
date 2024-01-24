using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GmWeb.Web.Common.Controllers;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Common.Utility
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class QueryStringRequestFilter : ActionFilterAttribute
    {
        public List<(string Name, Type ParamType)> Params { get; } = new List<(string Name, Type ParamType)>();
        public QueryStringRequestFilter(params string[] QueryParams)
        {
            if (QueryParams.Length % 2 != 0)
                throw new Exception($"Each query param must be followed by its type name.");
            var pairs = QueryParams.ToTuples(2);
            foreach(var pair in pairs)
            {
                var name = pair[0];
                var type = pair[1].GetTypeFromName();
                this.Params.Add((name, type));
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var queryString = filterContext.HttpContext.Request.QueryString;
            foreach(var paramConfig in this.Params)
            {
                var query = queryString[paramConfig.Name];
                if (query == null)
                    continue;
                var converted = paramConfig.ParamType.ConvertValue(query);
                filterContext.Controller.ViewData[paramConfig.Name] = converted;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}