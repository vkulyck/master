using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Web.Common;
using GmWeb.Web.Identity.Models;

namespace GmWeb.Web.Identity.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString RenderTwoFactorPartial<TModel>(this HtmlHelper<TModel> html)
            where TModel : TwoFactorConfigurationViewModel, new()
        => html.RenderTwoFactorPartial(html.ViewData.Model);

        public static MvcHtmlString RenderTwoFactorPartial<TModel>(this HtmlHelper<TModel> html, TModel model)
            where TModel : TwoFactorConfigurationViewModel, new()
        {
            var candidates = new List<string>() { model.InformationType.ToString(), "Default" };
            var searchHints = new List<string>();
            foreach (var candidate in candidates)
            {
                string path = candidate.ToString();
                var parents = new List<string> { "", "_", "EditorTemplates/", "Account/" };
                foreach (var dir in parents)
                {
                    path = $"{dir}{path}";
                    searchHints.Add(path);
                }
            }
            ViewEngineResult result = null;
            foreach (var hint in searchHints)
            {
                var controllerContext = html.ViewContext.Controller.ControllerContext;
                result = ViewEngines.Engines.FindPartialView(controllerContext, hint);
                if (result?.View != null)
                    break;
                result = ViewEngines.Engines.FindView(controllerContext, hint, null);
                if (result?.View != null)
                    break;
            }
            if (result?.View == null)
                return MvcHtmlString.Empty;
            var castResult = result.View as BuildManagerCompiledView;
            var resultPath = castResult.ViewPath;
            return html.Partial(resultPath);
        }
    }
}