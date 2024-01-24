using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI.Fluent;

namespace GmWeb.Web.Common.RazorControls
{
    public partial class GmKendoFactory : WidgetFactory
    {
        #region Manually Deferred Builders
        public override DiagramBuilder<object, object> Diagram() => base.Diagram().Deferred();
        public new DiagramBuilder<T, U> Diagram<T, U>() where T : class where U : class => base.Diagram<T, U>().Deferred();
        public override ChartBuilder<object> Chart() => base.Chart().Deferred();

        #endregion

        public DefaultControlFactory Defaults { get; private set; }
        public GmKendoFactory(HtmlHelper helper) : base(helper)
        {
            this.Defaults = new DefaultControlFactory(this);
        }

        public class DefaultControlFactory
        {
            public GmKendoFactory Parent { get; private set; }
            public DefaultControlFactory(GmKendoFactory parent)
            {
                this.Parent = parent;
            }
            public ButtonBuilder Button()
            {
                var builder = this.Parent.Button();
                builder.HtmlAttributes(new { @class = "btn btn-primary", style = "font-weight: bold; margin: 5px;" });
                return builder;
            }

            public WindowBuilder ModalWindow()
            {
                var builder = this.Parent.Window()
                    .Visible(false)
                    .Modal(true)
                    .Draggable(true)
                    .Resizable()
                    .Height(1000)
                    .Width(600)
                ;
                return builder;
            }
        }
    }
}
