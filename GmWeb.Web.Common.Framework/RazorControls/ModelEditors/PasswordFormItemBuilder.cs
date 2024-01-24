using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.RazorControls.ControlBuilders;
using GmWeb.Logic.Utility.Extensions;
using Kendo.Mvc.UI;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    /// <summary>
    /// Builds a labeled editor for a single property of the parent page's view model.
    /// </summary>
    /// <typeparam name="TModel">The type of the page's view model.</typeparam>
    /// <typeparam name="TProperty">The type of the edited view model property.</typeparam>
    public class PasswordFormItemBuilder<TModel,TProperty> : ModelFormItemBuilder<TModel, TProperty, ModelFormItemOptions<TModel, TProperty>, PasswordFormItemBuilder<TModel, TProperty>>
        where TModel : class, new()
    {
        // When enabled, Autofill allows the password form item to be populated with the model's password.
        // The current password value is derived from the view data and selector expression provided to
        // the form item's constructor. 
        public bool IsAutofillEnabled { get; private set; }
        // Enables or disables the IsAutofillEnabled property
        public PasswordFormItemBuilder<TModel, TProperty> Autofill(bool value = true)
        {
            this.IsAutofillEnabled = value;
            return this;
        }

        public PasswordFormItemBuilder(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector,
            Action<ModelFormItemOptions<TModel, TProperty>> configure = null
        ) : base(html, fieldSelector, configure) { }

        protected override MvcHtmlString CreateEditor(Expression<Func<TModel, TProperty>> selector, IDictionary<string, object> attributes)
        {
            if (this.IsAutofillEnabled)
            {
                var password = ModelMetadata.FromLambdaExpression(selector, this.Html.ViewData).Model?.ToString();
                if (!string.IsNullOrWhiteSpace(password))
                    attributes["value"] = password;
            }
            return base.CreateEditor(selector, attributes);
        }
    }
}