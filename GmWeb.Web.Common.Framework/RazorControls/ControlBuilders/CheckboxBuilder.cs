using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class CheckboxBuilder : ControlBuilder<CheckboxBuilder>
    {
        protected LabelBuilder ChildLabel { get; private set; }
        protected InputBuilder ChildInput { get; private set; }
        public CheckboxBuilder() : base("span")
        {
            this.ChildInput = this.CreateChild<InputBuilder>().Type(InputType.Checkbox).HtmlAttributes(new { style = "margin: 5px;" });
            this.ChildLabel = this.CreateChild<LabelBuilder>().HtmlAttributes(new { style = "font-weight: bold; margin: 5px;" });
        }

        public override CheckboxBuilder Name(string name)
        {
            this.ChildInput.Attributes["name"] = name;
            return this;
        }
        public CheckboxBuilder Id(string id)
        {
            this.ChildInput.GenerateId(id);
            return this;
        }

        public CheckboxBuilder OnChange(string handler)
        {
            this.ChildInput.Attributes["onchange"] = $"{handler}(this);";
            return this;
        }

        public CheckboxBuilder Value(string value)
        {
            this.ChildInput.Value(value);
            return this;
        }

        public CheckboxBuilder Label(string label)
        {
            this.ChildLabel.Content(label);
            return this;
        }

        public override CheckboxBuilder DataBind(string field)
        {
            this.ChildInput.DataBind(field);
            return this;
        }
    }
}