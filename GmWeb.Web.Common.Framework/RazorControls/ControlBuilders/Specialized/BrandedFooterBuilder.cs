using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders.Specialized
{
    public class BrandedFooterBuilder : ControlBuilder<BrandedFooterBuilder>
    {
        public ControlBuilder TextContainer { get; private set; }
        public LinkBuilder BrandLink { get; private set; }
        public TextBuilder Text { get; private set; }
        public BrandedFooterBuilder() : base("p")
        {
            this.AddCssClass("m-t");
            this.TextContainer = this.CreateChild("small");
            this.Text = this.TextContainer.CreateText($"© {DateTime.Now.Year.ToString()} Powered by ");
            this.BrandLink = this.TextContainer
                .CreateChild<LinkBuilder>()
                .Href("https://goodmojo.us")
                .Target("_self")
                .Content("GoodMojo Corp.")
            ;
        }

        public override string ToHtmlString()
        {
            string s = base.ToHtmlString();
            return s;
        }
    }
}