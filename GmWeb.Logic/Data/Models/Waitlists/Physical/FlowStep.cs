using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Data.Context.Profile;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    [Table("tblFlowSteps", Schema = "wtl")]
    public class FlowStep : ContextualDataModel<ProfileContext>
    {
        protected bool IsLoaded { get; set; } = false;

        public int FlowStepID { get; set; }
        [ForeignKey("Flow")]
        public int? FlowID { get; set; }
        [NotMapped]
        [JsonIgnore]
        public virtual Flow Flow { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();
        [SqlDataType(System.Data.SqlDbType.Int)]
        public StepType StepType { get; set; } = StepType.Standard;
        [JsonColumn]
        public ShapeLayout Layout { get; set; } = new ShapeLayout();

        [JsonColumn]
        /// <summary>
        /// A set of queries that may be answered to direct a <see cref="FlowStep"/> dialog to the next step.
        /// </summary>
        public virtual IList<ClientQuery> Queries { get; set; } = new List<ClientQuery>();

        [JsonColumn]
        /// <summary>
        /// A set of queries that may be answered to direct a <see cref="FlowStep"/> dialog to the next step.
        /// </summary>        
        public virtual IList<FlowLink> Links { get; set; } = new List<FlowLink>();
    }
}
