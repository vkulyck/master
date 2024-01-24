using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class ImageBuilder : ControlBuilder<ImageBuilder>
    {
        public ImageBuilder() : base("img") { }

        public ImageBuilder Source(string uri)
        {
            this.Attributes["src"] = uri;
            return this;
        }
    }
}