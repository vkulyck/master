using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Common;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.RazorControls;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public class EditorOptions
    {
        public virtual bool GenerateItemLabels { get; set; }
    }
    public class CompositeEditorOptions<TModel> : CompositeEditorOptions<TModel, CompositeEditorOptions<TModel>>
        where TModel : class, new()
    { }
    public class CompositeEditorOptions<TModel,TOptions> : EditorOptions
        where TModel : class, new()
        where TOptions : CompositeEditorOptions<TModel,TOptions>
    {
        public HtmlHelper<TModel> Html { get; set; }
        public string Guid { get; } = ModelExtensions.GenerateGuid();
        public IDictionary<string,object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
        public string Description { get; set; } = null;
        public bool GenerateValidationSummary { get; set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public TOptions WithHtml(HtmlHelper<TModel> html)
        {
            this.Html = html;
            this.IsInitialized = true;
            return (TOptions)this;
        }
    }
}