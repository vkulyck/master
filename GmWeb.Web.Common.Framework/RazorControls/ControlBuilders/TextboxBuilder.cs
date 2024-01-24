using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class TextboxBuilder : InputBuilder
    {
        public TextboxBuilder() : base(InputType.Textbox)
        {
            this.Attributes["data-role"] = "textbox";
            this.Attributes["style"] = "text-align: right; float: right;";
        }
    }
}