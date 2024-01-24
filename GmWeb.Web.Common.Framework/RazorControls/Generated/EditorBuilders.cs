

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GmWeb.Web.Common.RazorControls
{
    public partial class GmModelFactory<TModel>
    {
        public MvcHtmlString CheckBoxFor(
            Expression<Func<TModel, bool>> selector
        ) => this.Html.CheckBoxFor(selector);
        public MvcHtmlString CheckBoxFor(
            Expression<Func<TModel, bool>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.Html.CheckBoxFor(selector, htmlAttributes);
        public MvcHtmlString TextBoxFor(
            Expression<Func<TModel, string>> selector
        ) => this.Html.TextBoxFor(selector);
        public MvcHtmlString TextBoxFor(
            Expression<Func<TModel, string>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.Html.TextBoxFor(selector, htmlAttributes);
        public MvcHtmlString TextAreaFor(
            Expression<Func<TModel, string>> selector
        ) => this.Html.TextAreaFor(selector);
        public MvcHtmlString TextAreaFor(
            Expression<Func<TModel, string>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.Html.TextAreaFor(selector, htmlAttributes);
        public MvcHtmlString PasswordBoxFor(
            Expression<Func<TModel, string>> selector
        ) => this.Html.PasswordFor(selector);
        public MvcHtmlString PasswordBoxFor(
            Expression<Func<TModel, string>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.Html.PasswordFor(selector, htmlAttributes);
        public MvcHtmlString NumericTextBoxFor(
            Expression<Func<TModel, int>> selector
        ) => this.NumericTextBoxFor(selector);
        public MvcHtmlString NumericTextBoxFor(
            Expression<Func<TModel, int>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.NumericTextBoxFor(selector, htmlAttributes);
        public MvcHtmlString FloatTextBoxFor(
            Expression<Func<TModel, float>> selector
        ) => this.FloatTextBoxFor(selector);
        public MvcHtmlString FloatTextBoxFor(
            Expression<Func<TModel, float>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.FloatTextBoxFor(selector, htmlAttributes);
        public MvcHtmlString FloatTextBoxFor(
            Expression<Func<TModel, double>> selector
        ) => this.FloatTextBoxFor(selector);
        public MvcHtmlString FloatTextBoxFor(
            Expression<Func<TModel, double>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.FloatTextBoxFor(selector, htmlAttributes);
        public MvcHtmlString FloatTextBoxFor(
            Expression<Func<TModel, decimal>> selector
        ) => this.FloatTextBoxFor(selector);
        public MvcHtmlString FloatTextBoxFor(
            Expression<Func<TModel, decimal>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.FloatTextBoxFor(selector, htmlAttributes);
        public MvcHtmlString CalendarClockFor(
            Expression<Func<TModel, DateTime>> selector
        ) => this.CalendarClockFor(selector);
        public MvcHtmlString CalendarClockFor(
            Expression<Func<TModel, DateTime>> selector,
            IDictionary<string,object> htmlAttributes
        ) => this.CalendarClockFor(selector, htmlAttributes);
    }
}
