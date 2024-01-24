using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class ResultSet
    {
        public string Name { get; set; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();
        public List<string> Predicates { get; set; }
        [NotMapped]
        [JsonIgnore]
        public FilterResultData Data { get; set; } = new FilterResultData();
    }
}
