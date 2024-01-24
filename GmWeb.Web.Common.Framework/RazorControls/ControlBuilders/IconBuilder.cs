using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class IconBuilder : ControlBuilder<IconBuilder>
    {
        public override bool AllowSelfClosing => false;
        public IconBuilder() : base("i") 
        { 
        }

        public override string ToString()
        {
            return "Icon Builder:" + this.ClassTag;
        }
    }
}