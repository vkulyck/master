using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GmWeb.Web.Profile.Models.Shared;
using Newtonsoft.Json;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class ResultSet : EditableFlowModelBase
    {
        public string Name { get; set; } = "New Result Set";
        public override string EditorTitle => "Result Set";
        public override bool IsNameMultiline => true;
        public List<Predicate> Predicates { get; set; } = new List<Predicate>();
        [JsonIgnore]
        public IEnumerable<string> Formulas => this.Predicates.Select(x => x.Formula);
        public FilterResultData Data { get; set; } = new FilterResultData();
    }
}