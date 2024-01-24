using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class LinkBuilder : ControlBuilder<LinkBuilder>
    {
        public LinkBuilder() : base("a")
        {

        }

        public LinkBuilder Href(string url)
        {
            this.Attributes["href"] = url;
            return this;
        }

        public LinkBuilder Target(string target)
        {
            this.Attributes["target"] = target;
            return this;
        }
    }
}