using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class ButtonBuilder : ControlBuilder<ButtonBuilder>
    {
        public ButtonBuilder() : base("input") 
        {
            this.Attributes["type"] = "button";
            this.Attributes["class"] = "btn btn-primary";
        }

        public ButtonBuilder OnClick(string handler)
        {
            this.Attributes["onclick"] = $"{handler}(this);";
            return this;
        }
    }
}