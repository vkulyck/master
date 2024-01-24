using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public class ModelFormItemOptions<TModel, TProperty> : ModelFormItemOptions<TModel, TProperty, ModelFormItemOptions<TModel,TProperty>>
        where TModel : class, new()
    { }
    public class ModelFormItemOptions<TModel, TProperty, TOptions> : CompositeEditorOptions<TModel, TOptions>
        where TModel : class, new()
        where TOptions : ModelFormItemOptions<TModel, TProperty, TOptions>
    {
        public Func<Expression<Func<TModel, TProperty>>, IDictionary<string, object>, MvcHtmlString> EditorSelector { get; set; }
    }
}