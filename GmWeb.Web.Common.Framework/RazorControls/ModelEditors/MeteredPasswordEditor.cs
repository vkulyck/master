using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Web.Common.RazorControls.ControlBuilders;
using GmWeb.Logic.Utility.Extensions;
using System.Xml;
using System.IO;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    /// <summary>
    /// Builds a labeled editor for a single property of the parent page's view model.
    /// </summary>
    /// <typeparam name="TModel">The type of the page's view model.</typeparam>
    /// <typeparam name="TProperty">The type of the edited view model property.</typeparam>
    public class MeteredPasswordEditor<TModel> : CompositeEditorBuilder<TModel, MeteredPasswordOptions<TModel>, MeteredPasswordEditor<TModel>>
        where TModel : class, new()
    {
        public override DivBuilder Container { get => base.Container; protected set => base.Container = value; }
        public DivBuilder StrengthErrorDisplayTemplate { get; protected set; }
        public SpanBuilder StrengthProgressDisplay { get; protected set; }
        public ScriptBuilder InitializationScript { get; protected set; }
        public TooltipBuilder ErrorTooltip { get; protected set; }
        public ControlBuilderBase Editor { get; protected set; }
        public string PasswordEditorId { get; protected set; }
        public string PasswordEditorContainerId { get; protected set; }
        public Expression<Func<TModel,string>> PasswordSelector { get; protected set; }

        // Constructors
        public MeteredPasswordEditor(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, string>> passwordSelector
        )
            : this(html, passwordSelector, null)
        {
        }

        public MeteredPasswordEditor(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, string>> passwordSelector,
            Action<MeteredPasswordOptions<TModel>> config
        )
            : base(html, config)
        {
            this.PasswordSelector = passwordSelector;
            this.BuildEditor();
        }

        protected override void BuildEditor()
        {
            base.BuildEditor();
            this.PasswordEditorContainerId = this.Container.Attributes["id"] = $"password-editor-container-{this.Guid}";
            this.Container.Attributes["title"] = "placeholder-title-for-tooltip";
            this.PasswordEditorId = $"password-editor-{this.Guid}";
            this.Editor = this.Html.GmRazor().PasswordFormItem(this.PasswordSelector, cfg =>
            {
                cfg.HtmlAttributes = new { @id = this.PasswordEditorId, title = "placeholder-title-for-tooltip" }.ToAttributeDictionary();
            });

            //if (EditorBuilder.Instance.Options?.GenerateItemLabels == false)
            //{
            //    this.Container.AddCssClass("col-md-12").AddCssClass("gm-form-item-cell");
            //}
            //else
            //{
            //    var label = this.Html.LabelFor(this.PasswordSelector, new { @class = "control-label gm-form-item-label col-md-4" });
            //    this.Container.AppendChild(label);
            //    this.Editor.AddCssClass("col-md-8");
            //    this.Editor.AddCssClass("gm-form-item-cell");
            //}
            this.Container.AppendChild(this.Editor);

            if (string.IsNullOrWhiteSpace(this.Options.StrengthProgressDisplayId))
            {
                this.Options.StrengthProgressDisplayId = $"password-strength-progress-{this.Guid}";
                this.StrengthProgressDisplay = this
                    .PushBottom<SpanBuilder>()
                    .HtmlAttributes(new
                    {
                        @id = this.Options.StrengthProgressDisplayId,
                        @class = "pwstrength-progress-bar"
                    })
                ;
            }
            this.StrengthErrorDisplayTemplate = new DivBuilder()
                .HtmlAttributes(new
                {
                    @id = $"password-errors-{this.Guid}",
                    @class = "font-bold password-strength-errors"
                })
            ;
            this.ErrorTooltip = this
                .Prepend<TooltipBuilder>()
                .Anchor($"#{this.PasswordEditorContainerId}")
                .Content(this.StrengthErrorDisplayTemplate)
                .ManualTrigger(true)
            ;
            this.InitializationScript = this
                .Prepend<ScriptBuilder>()
                .OnLoad($@"
                    var config = {{
                        guid: '{this.Guid}',
                        progress_display_selector: '#{this.Options.StrengthProgressDisplayId}',
                        error_display_selector: '.password-strength-errors',
                        error_tooltip_selector: '#{this.PasswordEditorContainerId}',
                        container_id: '{this.PasswordEditorContainerId}',
                        editor_id: '{this.PasswordEditorId}'
                    }};
                    ConfigurePasswordMeter(config);
                ")
            ;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
