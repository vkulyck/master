using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class ScriptBuilder : ControlBuilder<ScriptBuilder>
    {
        public ScriptBuilder() : base("script")
        {
            this.Attributes["defer"] = "defer";
        }
        public ScriptBuilder OnLoad(string script)
        {
            this.InnerHtml = $"" +
                $"defer(function() {{ " +
                $"  {script} " +
                $"}});";
            return this;
        }
    }
}