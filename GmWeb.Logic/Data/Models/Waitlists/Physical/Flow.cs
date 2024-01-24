using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Data.Context.Profile;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    [Table("tblFlows", Schema = "wtl")]
    public class Flow : ContextualDataModel<ProfileContext>
    {
        public int FlowID { get; set; }
        [NotMapped]
        public int WaitlistFlowID { get => this.FlowID; set => this.FlowID = value; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();
        public string Name { get; set; }
        [NotMapped]
        [JsonIgnore]
        public FlowStep InitialStep => this.Steps.SingleOrDefault(x => x.StepType == StepType.Initial);
        [NotMapped]
        [JsonIgnore]
        public FlowStep TerminalStep => this.Steps.SingleOrDefault(x => x.StepType == StepType.Terminal);
        [InverseProperty("Flow")]
        public virtual ICollection<FlowStep> Steps { get; set; }
        [JsonColumn]
        public IList<CategoryData> Data { get; set; }
        [JsonColumn]
        public IList<ResultSet> Results { get; set; }

        public void SetCategory(int id)
        {
            foreach (var d in this.Data)
                d.DataSource.RowID = id;
        }
    }
}
