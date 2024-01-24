using GmWeb.Logic.Data.Models.Profile;
using System.Collections.Generic;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class FilterResultData
    {
        public int MatchCount => this.Clients.Count;
        public double? MatchPercentage => this.TotalCount == 0 ? default(double?) : (double)this.MatchCount / this.TotalCount;
        public int TotalCount { get; set; } = 0;
        public List<ExtendedClient> Clients { get; set; } = new List<ExtendedClient>();
    }
}
