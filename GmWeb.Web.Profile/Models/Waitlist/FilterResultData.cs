using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class FilterResultData : EditableFlowModelBase
    {
        public int MatchCount { get; set; } = 0;
        public double? MatchPercentage { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
        public List<Client> Clients { get; set; } = new List<Client>();
    }
}