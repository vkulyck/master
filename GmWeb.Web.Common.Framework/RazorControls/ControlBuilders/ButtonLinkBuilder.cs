using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class ButtonLinkBuilder : ControlBuilder<ButtonLinkBuilder>
    {
        public ButtonLinkBuilder() : base("a") { }

        public override ButtonLinkBuilder Enable(bool enable)
        {
            base.Enable(enable);
            if (this.Enabled)
                this.RemoveCssClass("disabled");
            else
                this.AddCssClass("disabled");
            return this;
        }

        public ButtonLinkBuilder Uri(string uri)
        {
            this.Attributes["href"] = uri;
            return this;
        }
    }
}