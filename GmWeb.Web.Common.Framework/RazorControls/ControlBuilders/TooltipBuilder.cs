using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Utility.Extensions;
using Kendo.Mvc.UI;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class TooltipBuilder : ControlBuilder<TooltipBuilder>
    {
        public DivBuilder TooltipTemplate { get; private set; }
        public DivBuilder ArrowContainer { get; private set; }
        public DivBuilder TooltipContainer { get; private set; }
        public ScriptBuilder TooltipInitializer { get; private set; }
        public bool IsManualTrigger { get; private set; } = false;

        public string TooltipContent { get; private set; }
        public string AnchorTargetSelector { get; private set; }
        public TooltipBuilder() : base("div") 
        {
            this.TooltipTemplate = new DivBuilder()
                .AddCssClass("tooltip")
                .HtmlAttributes(new { role = "tooltip" })
            ;
            this.ArrowContainer = this.TooltipTemplate.CreateChild<DivBuilder>().AddCssClass("arrow");
            this.TooltipContainer = this.TooltipTemplate.CreateChild<DivBuilder>()
                .AddCssClass("tooltip-content")
            ;
        }

        public TooltipBuilder Anchor(string targetSelector)
        {
            this.AnchorTargetSelector = targetSelector;
            return this;
        }

        public override TooltipBuilder Content(string content)
        {
            this.TooltipContent = content;
            this.TooltipContainer.Content(this.TooltipContent);
            return this;
        }

        public TooltipBuilder Content(IHtmlString content)
        {
            this.TooltipContent = content.ToHtmlString();
            this.TooltipContainer.Content(this.TooltipContent);
            return this;
        }

        public TooltipBuilder ManualTrigger(bool isManualTrigger)
        {
            this.IsManualTrigger = isManualTrigger;
            return this;
        }

        protected TooltipBuilder GenerateLoadScript()
        {
            string manualConfig = this.IsManualTrigger ? "trigger: 'manual'," : "";
            var source =
                $@"
                    var template = `{this.TooltipTemplate.ToHtmlString()}`;
                    console.log('creating tooltip at anchor:', '{this.AnchorTargetSelector}');
                    console.log('tooltip template:', template);
                    $('{this.AnchorTargetSelector}').tooltip({{
                        placement: 'right',
                        {manualConfig}
                        template: template
                    }});
                "
            ;
            this.TooltipInitializer = this.CreateChild<ScriptBuilder>().OnLoad(source);
            return this;
        }

        public override string ToHtmlString()
        {
            if (string.IsNullOrWhiteSpace(this.AnchorTargetSelector))
                throw new ArgumentException($"Tooltip is missing an anchor point.");
            this.GenerateLoadScript();
            return base.ToHtmlString();
        }
    }
}