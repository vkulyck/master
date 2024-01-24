using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text.RegularExpressions;
using System.Reflection;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public class NumericEditorBuilder<TModel,TProperty> : EditorBuilder<TModel, TProperty>
    {
        public NumericEditorBuilder(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> propertySelector
        ) : this(html, propertySelector, null) { }
        public NumericEditorBuilder(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> propertySelector,
            object htmlAttributes
        ) : base(html, html.TextBoxFor, propertySelector, htmlAttributes)
        {
            var numericAttributes = new { @type = "number" };
            this.HtmlAttributes["type"] = "number";
        }
    }
}