using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders.Specialized
{
    public class StatusMessageBuilder : HtmlControlBuilder<StatusMessageBuilder>
    {
        public DivBuilder HeaderContainer { get; private set; }
        public DivBuilder ErrorContainer { get; private set; }
        public StatusMessageBuilder(HtmlHelper html) : base(html, "div")
        {
            this.AddCssClass("alert", $"alert-{this.Html.ViewBag.StatusType}");
            this.HeaderContainer = this.CreateChild<DivBuilder>()
                .AddCssClass("row", "text-center")
                .Style("font-size: 15px; font-weight: bold;")
                .Content(this.Html.ViewBag.StatusMessage)
            ;
            if (!this.Html.ViewData.ModelState.IsValid)
            {
                this.ErrorContainer = this.CreateChild<DivBuilder>()
                    .AddCssClass("row")
                ;
                this.ErrorContainer.CreateChild(this.Html.ValidationSummary());
            }
        }

        public override string ToHtmlString()
        {
            if (string.IsNullOrWhiteSpace(this.Html.ViewBag.StatusMessage))
                return string.Empty;
            string s = base.ToHtmlString();
            return s;
        }
    }
}