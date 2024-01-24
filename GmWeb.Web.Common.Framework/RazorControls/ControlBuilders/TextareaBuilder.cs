using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class TextareaBuilder : ControlBuilder<TextareaBuilder>
    {
        public TextareaBuilder() : base("textarea")
        {
            this.Attributes["data-role"] = "textbox";
        }
    }
}