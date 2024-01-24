using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class FlowLink : EditableFlowModelBase
    {
        public override string EditorTitle => "Flow Link";
        public bool IsLink { get; set; } = true;
        public Predicate Predicate { get; set; } = new Predicate();
        public string ConnectionGuid { get; set; }
        public string Name { get; set; }
        public string SourceStepGuid { get; set; }
        public string TargetStepGuid { get; set; }
    }
}