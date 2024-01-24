using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Web.Common.RazorControls.ControlBuilders;
using GmWeb.Logic.Utility.Extensions;
using System.Security.Policy;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public class ModelFormBuilder<TModel> : ModelFormBuilder<TModel, ModelFormOptions<TModel>, ModelFormBuilder<TModel>>
        where TModel : class, new()
    {
        public ModelFormBuilder(HtmlHelper<TModel> html, Action<ModelFormOptions<TModel>> config)
            : base(html, config)
        { }
    }
    /// <summary>
    /// Builds a labeled editor for a single property of the parent page's view model.
    /// </summary>
    /// <typeparam name="TModel">The type of the page's view model.</typeparam>
    /// <typeparam name="TProperty">The type of the edited view model property.</typeparam>
    public class ModelFormBuilder<TModel,TOptions,TBuilder> : CompositeEditorBuilder<TModel, TOptions, TBuilder>
        where TModel : class, new()
        where TOptions : ModelFormOptions<TModel,TOptions>, new()
        where TBuilder : ModelFormBuilder<TModel,TOptions,TBuilder>
    {
        public FormBuilder FormElement { get; private set; }
        public HeaderBuilder DescriptionHeader { get; private set; }
        public string SubmitAction => this.Options.Action;
        public string SubmitUri => this.Url.Action(this.SubmitAction, this.Options.RouteValues);
        public FormMethod SubmitMethod => this.Options.Method;
        public object HtmlAttributes => this.Options.HtmlAttributes;
        public string Title => this.Options.Title;
        public string Description => this.Options.Description;

        public ModelFormBuilder(HtmlHelper<TModel> html, Action<TOptions> config)
            : base(html, config)
        {
            this.Configure();
        }

        protected void BuildFormElement()
        {
            this.Container = new DivBuilder();
            this.FormElement = this.Container.CreateChild<FormBuilder>();
            this.FormElement.AppendChild(this.Html.AntiForgeryToken());
            if (!string.IsNullOrWhiteSpace(this.Options.Description))
            {
                this.DescriptionHeader = new HeaderBuilder(HeadingLevel.H4).Content(this.Options.Description);
                this.FormElement.AppendChild(this.DescriptionHeader);
            }
            this.FormElement.AppendChild(new BreakBuilder());
            if(this.Options.GenerateValidationSummary == true)
                this.FormElement.AppendChild(this.Html.ValidationSummary());
        }
        protected void Configure()
        {
            this.BuildFormElement();
            object formAttributes = new
            {
                @class = "form-horizontal",
                role = "form",
                action = this.SubmitUri,
                method = this.SubmitMethod
            };
            if (this.HtmlAttributes != null)
                formAttributes = formAttributes.MergeAttributes(this.HtmlAttributes);
            var attrMap = formAttributes.ToAttributeDictionary();
            foreach (var kvp in attrMap)
                this.FormElement.Attributes[kvp.Key] = kvp.Value?.ToString();
        }
        protected override string BuildInnerEditorStart()
        {
            var html = 
                this.FormElement.BuildStartHtml() 
                + this.FormElement.BuildInnerHtml()
            ;
            return html;
        }
        protected override string BuildInnerEditorEnd()
        {
            //var container = new DivBuilder().HtmlAttributes(new { @class = "form-group" });
            //var subcontainer = container.CreateChild<DivBuilder>().HtmlAttributes(new { @class = "col-md-offset-2 col-md-8" });
            var button = new ButtonBuilder();
            button
                .HtmlAttributes(new { value = this.Options.SubmitLabel, type = "submit", @class = "full-width m-b" });
            var html =
                button.ToHtmlString() 
                + this.FormElement.BuildEndHtml()
            ;
            return html;
        }
    }
}
