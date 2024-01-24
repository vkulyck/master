using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public enum EnumValueField { ID, Name };
    public class EnumDropdownBuilder<EnumType> : ControlBuilder<EnumDropdownBuilder<EnumType>> where EnumType : struct, IConvertible
    {
        public EnumDropdownBuilder(EnumValueField Field) : base("select")
        {
            var values = EnumExtensions.GetEnumViewModels<EnumType>();
            foreach(var value in values)
            {
                var option = this.CreateChild<DropdownOptionBuilder>();
                if (Field == EnumValueField.ID)
                    option.Value(value.ID.ToString());
                else if (Field == EnumValueField.Name)
                    option.Value(value.Name);
                option.Display(value.ShortName);
            }
        }
    }
}