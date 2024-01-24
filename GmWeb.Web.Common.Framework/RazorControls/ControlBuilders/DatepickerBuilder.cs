using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class DatepickerBuilder : ControlBuilder<DatepickerBuilder>
    {
        protected LabelBuilder ChildLabel { get; private set; }
        protected InputBuilder ChildInput { get; private set; }
        public DatepickerBuilder() : base("span")
        {
            this.ChildInput = this.CreateChild<InputBuilder>().Type(InputType.Datepicker).HtmlAttributes(new { style = "margin: 5px;" });
            this.ChildLabel = this.CreateChild<LabelBuilder>().HtmlAttributes(new { style = "font-weight: bold; margin: 5px;" });
        }

        public override DatepickerBuilder Name(string name)
        {
            this.ChildInput.Attributes["name"] = name;
            return this;
        }
        public DatepickerBuilder Id(string id)
        {
            this.ChildInput.GenerateId(id);
            return this;
        }

        public DatepickerBuilder Value(string value)
        {
            this.ChildInput.Value(value);
            return this;
        }

        public DatepickerBuilder Label(string label)
        {
            this.ChildLabel.Content(label);
            return this;
        }

        public override DatepickerBuilder DataBind(string field)
        {
            this.ChildInput.DataBind(field);
            return this;
        }
    }
}