using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Data.Models.Shared;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class CategoryData : EditableFlowModelBase
    {
        public string VariableName { get; set; } = "New Variable";
        public override string EditorTitle => "Category Data";
        public override string EditorNameField => "VariableName";
        public int DataSourceID { get; set; }
        public int? CategoryID { get; set;  }
        public DynamicFieldValue ConfiguredValue { get; set; }
        public object ConvertedValue => this.ConfiguredValue.ConvertedValue;
        public string FormattedValue => this.ConfiguredValue.FormattedValue;
        public string LookupTableName { get; set; }
    }
}