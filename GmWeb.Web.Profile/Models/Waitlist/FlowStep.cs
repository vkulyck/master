using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Data.Models.Waitlists;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class FlowStep : EditableFlowModelBase
    {
        public override string EditorTitle => "Flow Step";
        public bool IsStep { get; set; } = true;
        public int? WaitlistFlowID { get; set; }
        public int? WaitlistFlowStepID { get; set; }
        public StepType StepType { get; set; }
        public bool IsInitial => this.StepType == StepType.Initial;
        public bool IsTerminal => this.StepType == StepType.Terminal;
        public string Name { get; set; }
        public string ShapeGuid { get; set; }
        public ShapeLayout Layout { get; set; } = new ShapeLayout();
        public List<ClientQuery> Queries { get; set; } = new List<ClientQuery>();
        public List<FlowLink> Links { get; set; } = new List<FlowLink>();
    }
}