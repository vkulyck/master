using GmWeb.Logic.Data.Context.Profile;
using GmWeb.Logic.Utility.Extensions;
using Newtonsoft.Json;
using System.Linq;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class FlowLink : ContextualDataModel<ProfileContext>
    {
        public string Guid { get; set; } = ModelExtensions.GenerateGuid();
        public string Name { get; set; }
        public string Predicate { get; set; }
        public string SourceStepGuid { get; set; }
        [JsonIgnore]
        public FlowStep SourceStep { get; private set; }
        public string TargetStepGuid { get; set; }
        [JsonIgnore]
        public FlowStep TargetStep { get; private set; }

        public override void PopulateReferences(ProfileContext cache)
        {
            base.PopulateReferences(cache);
            this.SourceStep = cache.FlowSteps.SingleOrDefault(x => x.Guid == this.SourceStepGuid);
            this.TargetStep = cache.FlowSteps.SingleOrDefault(x => x.Guid == this.TargetStepGuid);
        }
    }
}
