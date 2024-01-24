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

namespace GmWeb.Web.Common.RazorControls
{
    public abstract class EditorBuilder<TModel, TProperty> : IHtmlString
    {
        public HtmlHelper<TModel> Html { get; private set; }
        public Func<Expression<Func<TModel, TProperty>>, IDictionary<string,object>, MvcHtmlString> EditorSelector { get; private set; }
        public Expression<Func<TModel, TProperty>> PropertySelector { get; private set; }
        public IDictionary<string, object> HtmlAttributes { get; protected set; }
        public EditorBuilder(
            HtmlHelper<TModel> html,
            Func<Expression<Func<TModel, TProperty>>, IDictionary<string, object>, MvcHtmlString> editorSelector,
            Expression<Func<TModel, TProperty>> propertySelector
        ) : this(html, editorSelector, propertySelector, null) { }
        public EditorBuilder(
            HtmlHelper<TModel> html, 
            Func<Expression<Func<TModel,TProperty>>, IDictionary<string,object>, MvcHtmlString> editorSelector, 
            Expression<Func<TModel,TProperty>> propertySelector,
            object htmlAttributes
        )
        {
            this.Html = html;
            this.EditorSelector = editorSelector;
            this.PropertySelector = propertySelector;
            if (htmlAttributes == null)
                this.HtmlAttributes = new Dictionary<string, object>();
            else
                this.HtmlAttributes = htmlAttributes.ToAttributeDictionary();
        }

        public string ToHtmlString()
        {
            return this.EditorSelector(this.PropertySelector, this.HtmlAttributes).ToHtmlString();
        }
    }
}
