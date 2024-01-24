using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders.Specialized
{
    public class TextBuilder : ControlBuilder<TextBuilder>
    {
        public string Text { get; private set; }
        public TextBuilder(string text) : base("_")
        {
            this.Text = text;
        }

        public override string ToHtmlString()
        {
            return this.Text;
        }
    }
}