using GmWeb.Logic.Data.Context.Profile;
using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class FlowPath
    {
        public Flow Flow => this.Start?.Flow;
        public FlowStep Start { get; private set; }
        public ProfileCache Cache { get; private set; }
        public List<(FlowLink Link, FlowStep Step)> Transitions { get; set; } = new List<(FlowLink Link, FlowStep Step)>();

        public FlowPath(FlowStep start, ProfileCache cache)
        {
            this.Start = start;
            this.Cache = cache;
        }

        public IEnumerable<FlowStep> Steps
        {
            get
            {
                yield return this.Start;
                foreach (var step in this.Transitions.Select(x => x.Step))
                {
                    yield return step;
                }
            }
        }

        public IEnumerable<FlowLink> Links => this.Transitions.Select(x => x.Link);
    }
}
