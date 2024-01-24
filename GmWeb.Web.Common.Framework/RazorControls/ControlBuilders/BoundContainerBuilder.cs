using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class BoundContainerBuilder<ControlType> : ControlBuilder<BoundContainerBuilder<ControlType>> where ControlType : ControlBuilderBase
    {
        public BoundContainerBuilder() : base("div")
        {
        }

        public override BoundContainerBuilder<ControlType> DataBind(string field)
        {
            var area = this.Children.OfType<ControlType>().Single();
            area.Attributes["data-bind"] = $"value: {field}";
            return this;
        }
    }
}
