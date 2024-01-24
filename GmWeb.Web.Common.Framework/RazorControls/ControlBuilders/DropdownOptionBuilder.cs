using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class DropdownOptionBuilder : ControlBuilder<DropdownOptionBuilder>
    {
        public DropdownOptionBuilder() : base("option") { }

        public DropdownOptionBuilder Value(string value)
        {
            this.Attributes["value"] = value;
            return this;
        }

        public DropdownOptionBuilder Display(string display)
        {
            this.Content(display);
            return this;
        }
    }
}