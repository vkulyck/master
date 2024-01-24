using Kendo.Mvc.Infrastructure;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders.Specialized
{
    public class BrandedLogoBuilder : ControlBuilder<BrandedLogoBuilder>
    {
        public ControlBuilder TextContainer { get; private set; }
        public LinkBuilder BrandLink { get; private set; }
        public TextBuilder Text { get; private set; }
        public BrandedLogoBuilder() : base("div")
        {
            var imageContainer = this.CreateChild<DivBuilder>().AddCssClass("dropdown", "profile-element");
            var span = imageContainer.CreateChild<SpanBuilder>().AddCssClass("clear");
            var subSpan = span.CreateChild<SpanBuilder>().AddCssClass("block", "m-t-xs").Style("text-align: center;");
            var logoPath = this.Url.ImagePath("goodmojo-logo.png");
            var img = subSpan.CreateChild<ImageBuilder>().Source(logoPath).Style("width: 62px; height: 100px;");
        }
    }
}