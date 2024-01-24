using GmWeb.Logic.Data.Models.Waitlists;
using System;
using System.Linq;

namespace GmWeb.Logic.Data.Context.Profile
{
    public partial class Flows
    {
        public FlowPath CreateFlowPath(Predicate<Flow> selector)
        {
            var flow = this.SingleOrDefault(x => selector(x));
            var path = new FlowPath(flow.InitialStep, this.Cache);
            return path;
        }

        public FlowStep GetNextStep(FlowStep current, FlowPath path)
        {
            if (current.StepType == StepType.Terminal)
                return null;
            if (current.Links.Count == 0)
                throw new Exception($"Non-terminal flow step contains no links: {current.Name}");

            FlowLink link = null;
            var cache = new VariableCache(Path: path, EnableDispose: false).Initialize();
            bool? result = null;
            foreach (var l in current.Links)
            {
                var expression = new NCalc.Expression(l.Predicate);
                result = expression.ValidateAndEvaluate(EntityAssignments: cache.Assignments);
                if (result.HasValue && result.Value)
                {
                    link = l;
                    break;
                }
            }
            var step = link.TargetStep;
            path.Transitions.Add((link, step));
            return step;
        }
    }
}
