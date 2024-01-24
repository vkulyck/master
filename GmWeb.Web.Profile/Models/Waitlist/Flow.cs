using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class Flow : EditableFlowModelBase
    {
        public int? WaitlistFlowID { get; set; }
        public string Name { get; set; } = "New Workflow";
        public override string EditorTitle => "Flow";
        public List<CategoryData> Data { get; set; } = new List<CategoryData>();
        public List<ResultSet> Results { get; set; } = new List<ResultSet>();
        public List<FlowStep> Steps { get; set; } = new List<FlowStep>();
    }
}