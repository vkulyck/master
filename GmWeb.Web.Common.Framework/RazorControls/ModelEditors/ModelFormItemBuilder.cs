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
    public class ModelFormItemBuilder<TModel, TProperty> : ModelFormItemBuilder<TModel, TProperty, ModelFormItemOptions<TModel, TProperty>, ModelFormItemBuilder<TModel, TProperty>>
        where TModel : class, new()
    {
        public ModelFormItemBuilder(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector,
            Action<ModelFormItemOptions<TModel, TProperty>> configure = null
        ) : base(html, fieldSelector, configure) { }
    }
    /// <summary>
    /// Builds a labeled editor for a single property of the parent page's view model.
    /// </summary>
    /// <typeparam name="TModel">The type of the page's view model.</typeparam>
    /// <typeparam name="TProperty">The type of the edited view model property.</typeparam>
    public abstract class ModelFormItemBuilder<TModel,TProperty,TOptions, TBuilder> : HtmlControlBuilder<TBuilder>
        where TModel : class, new()
        where TOptions : ModelFormItemOptions<TModel, TProperty, TOptions>, new()
        where TBuilder : ModelFormItemBuilder<TModel, TProperty, TOptions, TBuilder>
    {
        public new HtmlHelper<TModel> Html { get; private set; }
        public Expression<Func<TModel, TProperty>> FieldSelector { get; private set; }
        public IDictionary<string, object> EditorAttributes { get; private set; } = new Dictionary<string, object>();
        public IDictionary<string, object> BuilderAttributes { get; private set; } = new Dictionary<string, object>();
        public TOptions Options { get; private set; }
        public DivBuilder EditorContainer { get; private set; }
        public TooltipBuilder Tooltip{ get; private set; }
        public string SelectorProperty { get; private set; }
        public string DescriptionTooltipId => $"{this.SelectorProperty}-tooltip-{this.Options.Guid}";

        public ModelFormItemBuilder(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector,
            Action<TOptions> configure = null
        ) : base(html, "div")
        {
            this.Html = html;
            this.FieldSelector = fieldSelector;
            this.Configure(configure);
        }

        protected void ResetConfiguration() => this.ResetChildren();

        public ModelFormItemBuilder<TModel,TProperty,TOptions,TBuilder> Configure(Action<TOptions> configure)
        {
            this.ResetConfiguration();
            var options = new TOptions().WithHtml(this.Html);
            this.Options = options;
            if(configure != null)
                configure(options);
            return this;
        }

        protected void Build()
        {
            this.AssignAttributes();
            this.SelectorProperty = this.FieldSelector.GetSelectorProperty().Name;
            this.AddCssClass("form-group");
            var dict = this.Options.HtmlAttributes;
            if (EditorBuilder.Instance.Options?.GenerateItemLabels == false)
            {
                this.EditorContainer = this.CreateChild<DivBuilder>();
                this.EditorContainer.AddCssClass("col-md-12").AddCssClass("gm-form-item-cell");
            }
            else
            {
                var label = this.buildLabel();
                this.InsertChild(label);
                this.EditorContainer = this.CreateChild<DivBuilder>();
                this.EditorContainer.AddCssClass("col-md-8").AddCssClass("gm-form-item-cell");
            }
            MvcHtmlString editor = this.CreateEditor(this.FieldSelector, this.EditorAttributes);
            this.EditorContainer.InsertChild(editor);
        }

        protected virtual MvcHtmlString CreateEditor(Expression<Func<TModel,TProperty>> selector, IDictionary<string,object> attributes)
        {
            MvcHtmlString editor;
            if (this.Options.EditorSelector == null)
                editor = this.Html.CreateDefaultEditor(selector, attributes);
            else
                editor = this.Options.EditorSelector(selector, attributes);
            return editor;
        }

        protected virtual void AssignAttributes()
        {
            var optionAttributes = this.Options.HtmlAttributes.ToDictionary();
            var editorAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "class", "form-control gm-form-item-editor" }
            };
            foreach (var editorKey in new string[] { "id", "value" })
            {
                if (optionAttributes.ContainsKey(editorKey))
                    editorAttributes[editorKey] = optionAttributes[editorKey];
                optionAttributes.Remove(editorKey);
            }
            this.EditorAttributes = editorAttributes;
            this.BuilderAttributes = optionAttributes;
            this.HtmlAttributes(this.BuilderAttributes);
        }

        protected DivBuilder buildLabel()
        {
            var outerContainer = new DivBuilder().AddCssClass("col-md-4").AddCssClass("gm-form-item-cell");
            var outerRowContainer = outerContainer.CreateChild<DivBuilder>();
            var label = this.Html.LabelFor(this.FieldSelector, new { @class = "control-label gm-form-item-label" });
            if (string.IsNullOrWhiteSpace(this.Options.Description))
            {
                outerRowContainer.AppendChild(label);    
            }
            else
            {
                this.Tooltip = outerRowContainer.CreateChild<TooltipBuilder>()
                    .Content(this.Options.Description)
                    .Anchor($"#{this.DescriptionTooltipId}")
                ;
                var labelContainer = outerRowContainer.CreateChild<DivBuilder>();
                labelContainer.AppendChild(label);
                outerRowContainer.AddCssClass("row").Style("display: flex; align-items: center;");
                labelContainer.AddCssClass("col-md-9").Style("padding-right:0px;");
                var descTooltipContainer = outerRowContainer.CreateChild<DivBuilder>()
                    .AddCssClass("col-md-3")
                    .Style("padding-left: 6px; padding-top: 6px;")
                ;
                var descTooltipLink = descTooltipContainer.CreateChild<LinkBuilder>()
                    .HtmlAttributes(new Dictionary<string,object>
                    {
                        { "id", this.DescriptionTooltipId },
                        { "data-toggle", "tooltip" },
                        {"title", "Password Requirements" },
                        {"href", "#" }
                    })
                ;
                descTooltipLink.CreateChild<IconBuilder>()
                    .AddCssClass("fas fa-info-circle")
                ;
            }
            return outerContainer;
        }

        public override string ToHtmlString()
        {
            this.Build();
            return base.ToHtmlString();
        }
    }
}