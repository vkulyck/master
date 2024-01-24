using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.RazorControls.ControlBuilders;
using GmWeb.Web.Common.RazorControls.ModelEditors;
using GmWeb.Web.Common.RazorControls.ControlBuilders.Specialized;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Common.RazorControls
{
    public partial class GmFactory
    {
        public DefaultControlFactory Defaults { get; private set; }
        protected HtmlHelper Html { get; private set; }
        public GmFactory(HtmlHelper helper)
        {
            this.Html = helper;
            this.Defaults = new DefaultControlFactory(this);
        }
        public ButtonBuilder Button() => new ButtonBuilder();
        public ButtonLinkBuilder ButtonLink() => new ButtonLinkBuilder();
        public RadioButtonBuilder RadioButton() => new RadioButtonBuilder();
        public CheckboxBuilder Checkbox() => new CheckboxBuilder();
        public LabelBuilder Label() => new LabelBuilder();
        public SpanBuilder Span() => new SpanBuilder();
        public TextareaBuilder Textarea() => new TextareaBuilder();
        public ScriptBuilder Script() => new ScriptBuilder();
        public BoundContainerBuilder<TextareaBuilder> ContainedTextarea() => new BoundContainerBuilder<TextareaBuilder>();
        public TextboxBuilder Textbox() => new TextboxBuilder();
        public EnumDropdownBuilder<T> EnumDropdown<T>(EnumValueField Field) where T : struct, IConvertible => new EnumDropdownBuilder<T>(Field);
        public DropdownBuilder<T> Dropdown<T>() => new DropdownBuilder<T>();
        public DatepickerBuilder Datepicker() => new DatepickerBuilder();
        public BrandedFooterBuilder BrandedFooter() => new BrandedFooterBuilder();
        public BrandedLogoBuilder BrandedLogo() => new BrandedLogoBuilder();
        public StatusMessageBuilder StatusMessage() => new StatusMessageBuilder(this.Html);
    }
    public partial class GmModelFactory<TModel> : GmFactory
        where TModel : class, new()
    {
        protected new HtmlHelper<TModel> Html { get; private set; }
        public GmModelFactory(HtmlHelper<TModel> helper) : base(helper)
        {
            this.Html = helper;
        }

        #region Model-based Form Items

        public ModelFormItemBuilder<TModel, TProperty> FormItem<TProperty>(Expression<Func<TModel, TProperty>> fieldSelector)
            => new ModelFormItemBuilder<TModel, TProperty>(this.Html, fieldSelector);
        public ModelFormItemBuilder<TModel, TProperty> FormItem<TProperty>(Expression<Func<TModel, TProperty>> fieldSelector, Action<ModelFormItemOptions<TModel,TProperty>> configure)
            => new ModelFormItemBuilder<TModel, TProperty>(this.Html, fieldSelector, configure);
        public PasswordFormItemBuilder<TModel, TProperty> PasswordFormItem<TProperty>(Expression<Func<TModel, TProperty>> fieldSelector)
            => new PasswordFormItemBuilder<TModel, TProperty>(this.Html, fieldSelector, cfg => 
            { 
                cfg.EditorSelector = this.Html.PasswordFor; 
            });
        public PasswordFormItemBuilder<TModel, TProperty> PasswordFormItem<TProperty>(Expression<Func<TModel, TProperty>> fieldSelector, Action<ModelFormItemOptions<TModel, TProperty>> configure)
            => new PasswordFormItemBuilder<TModel, TProperty>(this.Html, fieldSelector, cfg =>
            {
                configure(cfg);
                if (cfg.EditorSelector == null)
                    cfg.EditorSelector = this.Html.PasswordFor;
            });

        #endregion
        public ModelFormBuilder<TModel> BeginForm(Action<ModelFormOptions<TModel>> configure)
            => new ModelFormBuilder<TModel>(this.Html, configure).BeginForm();
        public MeteredPasswordEditor<TModel> BeginPasswordMeter(Expression<Func<TModel, string>> passwordSelector)
            => new MeteredPasswordEditor<TModel>(this.Html, passwordSelector).BeginForm();
        public MeteredPasswordEditor<TModel> BeginPasswordMeter(Expression<Func<TModel, string>> passwordSelector, Action<MeteredPasswordOptions<TModel>> configure)
            => new MeteredPasswordEditor<TModel>(this.Html, passwordSelector, configure).BeginForm();

        #region Specialized Form Items
        public NumericEditorBuilder<TModel, TProperty> NumericTextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> Selector,
            object HtmlAttributes
        ) => new NumericEditorBuilder<TModel, TProperty>(this.Html, Selector, HtmlAttributes);
        public NumericEditorBuilder<TModel, TProperty> FloatTextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> Selector,
            object HtmlAttributes
        ) => new NumericEditorBuilder<TModel, TProperty>(this.Html, Selector, HtmlAttributes);
        #endregion

    }

    public class DefaultControlFactory
    {
        public GmFactory Parent { get; private set; }
        public DefaultControlFactory(GmFactory parent)
        {
            this.Parent = parent;
        }
        public ButtonBuilder Button()
        {
            var builder = this.Parent.Button();
            builder.HtmlAttributes(new { @class = "btn btn-primary", style = "font-weight: bold; margin: 5px;" });
            return builder;
        }

        public ButtonLinkBuilder ButtonLink()
        {
            var builder = this.Parent.ButtonLink();
            builder.HtmlAttributes(new { @class = "btn btn-primary", style = "font-weight: bold; margin: 5px;" });
            return builder;
        }

        public BoundContainerBuilder<TextareaBuilder> AutosizeTextarea()
        {
            var container = this.Parent.ContainedTextarea();

            var areaBuilder = container.CreateChild<TextareaBuilder>();
            areaBuilder.AddCssClass("autosize");
            areaBuilder.Style("width: 100%; text-align: right; padding-right: 5px;");
            areaBuilder.Attributes["rows"] = "1";
            areaBuilder.Attributes["data-min-rows"] = "1";
            var guid = areaBuilder.Attributes["guid"] = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();

            var scriptBuilder = container.CreateChild<ScriptBuilder>();
            scriptBuilder.OnLoad($@"
                    var area = $('textarea[guid=""{guid}""]');
                    ConfigureAutosizeTextarea(area);
                ");

            return container;
        }
    }
}
