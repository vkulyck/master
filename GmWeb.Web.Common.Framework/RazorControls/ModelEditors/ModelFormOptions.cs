using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Common;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.RazorControls; 

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public class ModelFormOptions<TModel> : ModelFormOptions<TModel, ModelFormOptions<TModel>> 
        where TModel : class, new()
    { }
    public class ModelFormOptions<TModel,TOptions> : CompositeEditorOptions<TModel,TOptions>
        where TModel : class, new()
        where TOptions : CompositeEditorOptions<TModel,TOptions>
    {
        public string Title { get; set; } = string.Empty;
        public string SubmitLabel { get; set; } = "Submit";
        public string Action { get; set; }
        public FormMethod Method { get; set; } = FormMethod.Post;
        public RouteValueDictionary RouteValues { get; set; }

        public void RouteQuery()
        {
            var routeValues = this.Html.ViewContext.RequestContext.RouteData.Values.ToDictionary(x => x.Key, x => x.Value?.ToString());
            if(string.IsNullOrWhiteSpace(this.Action) && routeValues.TryGetValue("action", out string routeAction))
            {
                this.Action = routeAction;
            }
            var queryString = this.Html.ViewContext.RequestContext.HttpContext.Request.QueryString;
            var queryParams =  queryString.Keys
                .Cast<string>()
                .ToDictionary(k => k, v => queryString[v])
            ;
            var merged = routeValues.MergeLeft(queryParams).ToDictionary(x => x.Key, x => (object)x.Value);
            var rvMerged = new RouteValueDictionary(merged);
            this.Route(merged);
        }

        public void Route(object routeValues)
        {
            this.RouteValues = new RouteValueDictionary(routeValues);
        }
        public void Route(IDictionary<string,object> routeValues)
        {
            this.RouteValues = new RouteValueDictionary(routeValues);
        }
        public void Route(RouteValueDictionary routeValues)
        {
            this.RouteValues = routeValues;
        }
    }
}
