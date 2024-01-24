using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class SpanBuilder : ControlBuilder<SpanBuilder>
    {
        public override bool AllowSelfClosing => false;
        public SpanBuilder() : base("span") { }
        public override SpanBuilder DataBind(string field)
        {
            this.Attributes["data-bind"] = $"html: {field}";
            return this;
        }
    }
}