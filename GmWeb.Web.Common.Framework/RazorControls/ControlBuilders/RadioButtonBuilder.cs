using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class RadioButtonBuilder : ControlBuilder<RadioButtonBuilder>
    {
        protected LabelBuilder ChildLabel { get; private set; }
        protected InputBuilder ChildInput { get; private set; }
        public RadioButtonBuilder() : base("span")
        {
            this.ChildInput = this.CreateChild<InputBuilder>().Type(InputType.Radio).HtmlAttributes(new { style = "margin: 5px;" });
            this.ChildLabel = this.CreateChild<LabelBuilder>().HtmlAttributes(new { style = "font-weight: bold; margin: 5px;" });
        }

        public override RadioButtonBuilder Name(string name)
        {
            this.ChildInput.Attributes["name"] = name;
            return this;
        }
        public RadioButtonBuilder Id(string id)
        {
            this.ChildInput.GenerateId(id);
            return this;
        }

        public RadioButtonBuilder OnChange(string handler)
        {
            this.ChildInput.Attributes["onchange"] = $"{handler}(this);";
            return this;
        }

        public RadioButtonBuilder Value(string value)
        {
            this.ChildInput.Value(value);
            return this;
        }

        public RadioButtonBuilder Label(string label)
        {
            this.ChildLabel.Content(label);
            return this;
        }

        public override RadioButtonBuilder Content(string content)
        {
            return this;
        }
    }
}