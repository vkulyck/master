using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public enum InputType
    {
        Checkbox = 1,
        Radio = 2,
        Textbox = 3,
        Datepicker = 4
    }
    public class InputBuilder : ControlBuilder<InputBuilder>
    {
        public InputType InputType { get; private set; }
        public InputBuilder() : this(InputType.Textbox) { }
        public InputBuilder(InputType type) : base("input") { this.Type(type); }
        public InputBuilder Type(InputType type)
        {
            this.InputType = type;
            this.Attributes["type"] = type.ToString().ToLower();
            return this;
        }

        public InputBuilder Value(string value)
        {
            this.Attributes["value"] = value;
            return this;
        }

        public override InputBuilder DataBind(string field)
        {
            if (this.InputType == InputType.Checkbox || this.InputType == InputType.Radio)
            {
                this.Attributes["data-bind"] = $"checked: {field}";
            }
            else base.DataBind(field);
            return this;
        }
    }
}
